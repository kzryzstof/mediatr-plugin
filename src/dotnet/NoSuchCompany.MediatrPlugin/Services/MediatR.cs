// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 27/12/2021 @ 20:16
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Search;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Tree;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services
{
    internal sealed class MediatR : IMediatR
    {
        public ITypeElement? FindHandler(IIdentifier identifier)
        {
            Guard.ThrowIfIsNull(identifier, nameof(identifier));

            IPsiServices psiServices = identifier.GetPsiServices();

            IType mediatrRequestHandlerType = CSharpTypeFactory.CreateType("MediatR.IRequestHandler<in TRequest, TResponse>", identifier);
            var mediatrRequestHandlerDeclaredType = (IDeclaredType) mediatrRequestHandlerType;
            IResolveResult resolveResult = mediatrRequestHandlerDeclaredType.Resolve();

            var inheritorsConsumer = new InheritorsConsumer();

            psiServices
                .Finder
                .FindInheritors
                (
                    mediatrRequestHandlerType.GetTypeElement(), 
                    inheritorsConsumer,
                    new ProgressIndicator(Lifetime.Eternal)
                );

            //  I do not know why but FindInheritors and GetPossibleInheritors do not return all the handlers by themselves.
            //  I have to union both results of each to get all the actual IRequestHandler implementations.
            IEnumerable<ITypeElement> results = inheritorsConsumer
                .FoundElements
                .Union
                (psiServices
                    .Symbols
                    .GetPossibleInheritors(resolveResult.DeclaredElement.ShortName)
                );

            Logger.Instance.Log(LoggingLevel.WARN, $"Possible inheritors found: {string.Join(",", results.Select(i => i.GetClrName().FullName))}");

            ITypeElement? inheritorTypeElement = SelectInheritor(results, identifier);

            return inheritorTypeElement;
        }

        public bool IsRequest(IIdentifier identifier)
        {
            Guard.ThrowIfIsNull(identifier, nameof(identifier));

            IDeclaredType declaredType = identifier.ToDeclaredType();

            IType mediatrBaseRequestType = CSharpTypeFactory.CreateType("MediatR.IBaseRequest", declaredType.Module);

            if (declaredType.IsSubtypeOf((IDeclaredType) mediatrBaseRequestType))
            {
                Logger.Instance.Log(LoggingLevel.VERBOSE, $"> The declared type '{declaredType.GetClrName().FullName}' is considered a subtype of '{mediatrBaseRequestType.GetScalarType()!.GetClrName().FullName}'");
                return true;
            }

            Logger.Instance.Log(LoggingLevel.VERBOSE, $"> The declared type '{declaredType.GetClrName().FullName}' is not considered a subtype of '{mediatrBaseRequestType.GetScalarType()!.GetClrName().FullName}'");
            return false;
        }

        private ITypeElement? SelectInheritor(IEnumerable<ITypeElement> inheritors, IIdentifier selectedIdentifier)
        {
            IDeclaredType selectedDeclaredType = selectedIdentifier.ToDeclaredType();
            string? selectedFullTypeName = selectedDeclaredType.GetClrName().FullName;
            
            Logger.Instance.Log(LoggingLevel.VERBOSE, $"Selected type: '{selectedFullTypeName}'");

            foreach (ITypeElement? inheritor in inheritors)
            {
                Logger.Instance.Log(LoggingLevel.VERBOSE, $"Inheritor: '{inheritor.GetClrName().FullName}'");

                if (inheritor is not IClass classInheritor)
                {
                    Logger.Instance.Log(LoggingLevel.VERBOSE, $"Skipped: '{inheritor.GetClrName().FullName}' is not an IClass instance.");
                    continue;
                }

                if (classInheritor.IsAbstract)
                {
                    Logger.Instance.Log(LoggingLevel.VERBOSE, $"Skipped: '{inheritor.GetClrName().FullName}' is abstract.");
                    continue;
                }

                bool supportRequest = classInheritor
                    .GetDeclarations()
                    .Cast<IClassDeclaration>()
                    .SelectMany(classDeclaration => classDeclaration.InheritedTypeUsagesEnumerable)
                    .Cast<IUserTypeUsage>()
                    .Select(userTypeUsage => userTypeUsage.ScalarTypeName)
                    .Select(scalarTypeName => scalarTypeName.TypeArgumentList)
                    .Select(typeArgumentList => typeArgumentList.Children())
                    .SelectMany(treeNode => treeNode)
                    .Where(child => child is IUserTypeUsage)
                    .Cast<IUserTypeUsage>()
                    .Any(userTypeUsage =>
                    {
                        IDeclaredType declaredType = userTypeUsage.ScalarTypeName.NameIdentifier.ToDeclaredType();
                        string? fullName = declaredType.GetClrName().FullName;
                        
                        Logger.Instance.Log(LoggingLevel.VERBOSE, $"--≥ '{fullName}'");
                        return string.Equals(fullName, selectedFullTypeName, StringComparison.CurrentCultureIgnoreCase);
                    });

                if (!supportRequest)
                {
                    Logger.Instance.Log(LoggingLevel.VERBOSE, $"Skipped: '{inheritor.GetClrName().FullName}' does not use '{selectedIdentifier.Name}'.");
                    continue;
                }

                Logger.Instance.Log(LoggingLevel.VERBOSE, $"Found: '{inheritor.GetClrName().FullName}'.");

                return inheritor;
            }

            return null;
        }
    }
}