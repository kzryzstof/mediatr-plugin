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

            var psiServices = identifier.GetPsiServices();

            var mediatrRequestHandlerType = CSharpTypeFactory.CreateType("MediatR.IRequestHandler<in TRequest, TResponse>", identifier);
            var mediatrRequestHandlerDeclaredType = (IDeclaredType) mediatrRequestHandlerType;
            var resolveResult = mediatrRequestHandlerDeclaredType.Resolve();

            var inheritorsConsumer = new InheritorsConsumer();

            psiServices
                .Finder
                .FindInheritors(mediatrRequestHandlerType.GetTypeElement(), inheritorsConsumer,
                    new ProgressIndicator(Lifetime.Eternal));

            //  I do not know why but FindInheritors and GetPossibleInheritors do not return all the handlers by themselves.
            //  I have to union both results of each to get all the actual IRequestHandler implementations.
            IEnumerable<ITypeElement> results = inheritorsConsumer
                .FoundElements
                .Union
                (psiServices
                    .Symbols
                    .GetPossibleInheritors(resolveResult.DeclaredElement.ShortName)
                );

            Logger.Instance.Log(LoggingLevel.WARN,
                $"Possible inheritors found: {string.Join(",", results.Select(i => i.GetClrName().FullName))}");

            var inheritorTypeElement = SelectInheritor(results, identifier);

            return inheritorTypeElement;
        }

        public bool IsRequest(IIdentifier identifier)
        {
            Guard.ThrowIfIsNull(identifier, nameof(identifier));

            var declaredType = identifier.ToDeclaredType();

            var mediatrBaseRequestType = CSharpTypeFactory.CreateType("MediatR.IBaseRequest", declaredType.Module);

            if (declaredType.IsSubtypeOf((IDeclaredType) mediatrBaseRequestType))
            {
                Logger.Instance.Log(LoggingLevel.VERBOSE,
                    $"> The declared type '{declaredType.GetClrName().FullName}' is considered a subtype of '{mediatrBaseRequestType.GetScalarType()!.GetClrName().FullName}'");
                return true;
            }

            Logger.Instance.Log(LoggingLevel.VERBOSE,
                $"> The declared type '{declaredType.GetClrName().FullName}' is not considered a subtype of '{mediatrBaseRequestType.GetScalarType()!.GetClrName().FullName}'");
            return false;
        }

        private ITypeElement? SelectInheritor(IEnumerable<ITypeElement> inheritors, IIdentifier selectedIdentifier)
        {
            foreach (var inheritor in inheritors)
            {
                Logger.Instance.Log(LoggingLevel.WARN, $"Inheritor: '{inheritor.GetClrName().FullName}'");

                if (inheritor is not IClass classInheritor)
                {
                    Logger.Instance.Log(LoggingLevel.WARN,
                        $"Skipped: '{inheritor.GetClrName().FullName}' is not an IClass instance.");
                    continue;
                }

                if (classInheritor.IsAbstract)
                {
                    Logger.Instance.Log(LoggingLevel.WARN, $"Skipped: '{inheritor.GetClrName().FullName}' is abstract.");
                    continue;
                }

                var supportRequest = classInheritor
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
                    .Any(userTypeUsage => string.Equals(userTypeUsage.ScalarTypeName.ShortName, selectedIdentifier.Name,
                        StringComparison.CurrentCultureIgnoreCase));

                if (!supportRequest)
                {
                    Logger.Instance.Log(LoggingLevel.WARN,
                        $"Skipped: '{inheritor.GetClrName().FullName}' does not use '{selectedIdentifier.Name}'.");
                    continue;
                }

                return inheritor;
            }

            return null;
        }
    }
}