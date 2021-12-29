// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 27/12/2021 @ 20:16
// Last author: Christophe Commeyne
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
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
    #region Class

    internal sealed class MediatR : IMediatR
    {
        #region Public Methods

        public ITypeElement? FindHandler(ISolution solution, IIdentifier identifier)
        {
            Guard.ThrowIfIsNull(solution, nameof(solution));
            Guard.ThrowIfIsNull(identifier, nameof(identifier));
        
            IType mediatrRequestHandlerType = CSharpTypeFactory.CreateType("MediatR.IRequestHandler<in TRequest, TResponse>", identifier);
            var mediatrRequestHandlerDeclaredType = (IDeclaredType)mediatrRequestHandlerType;
            IResolveResult resolveResult = mediatrRequestHandlerDeclaredType.Resolve();

            IPsiServices psiServices = solution.GetPsiServices();

            var inheritorsConsumer = new InheritorsConsumer();

            psiServices
                .Finder
                .FindInheritors(mediatrRequestHandlerType.GetTypeElement(), inheritorsConsumer, new ProgressIndicator(Lifetime.Eternal));

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

            if (declaredType.IsSubtypeOf((IDeclaredType)mediatrBaseRequestType))
            {
                Logger.Instance.Log(LoggingLevel.VERBOSE, $"> The declared type '{declaredType.GetClrName().FullName}' is considered a subtype of '{mediatrBaseRequestType.GetScalarType()!.GetClrName().FullName}'");
                return true;
            }

            Logger.Instance.Log(LoggingLevel.VERBOSE, $"> The declared type '{declaredType.GetClrName().FullName}' is not considered a subtype of '{mediatrBaseRequestType.GetScalarType()!.GetClrName().FullName}'");
            return false;
        }

        #endregion

        #region Private Methods

        private ITypeElement? SelectInheritor(IEnumerable<ITypeElement> inheritors, IIdentifier selectedIdentifier)
        {
            foreach (ITypeElement? inheritor in inheritors)
            {
                Logger.Instance.Log(LoggingLevel.WARN, $"Inheritor: '{inheritor.GetClrName().FullName}'");

                if (inheritor is not IClass classInheritor)
                {
                    Logger.Instance.Log(LoggingLevel.WARN, $"Skipped: '{inheritor.GetClrName().FullName}' is not an IClass instance.");
                    continue;
                }

                if (classInheritor.IsAbstract)
                {
                    Logger.Instance.Log(LoggingLevel.WARN, $"Skipped: '{inheritor.GetClrName().FullName}' is abstract.");
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
                    .Any(userTypeUsage => string.Equals(userTypeUsage.ScalarTypeName.ShortName, selectedIdentifier.Name, StringComparison.CurrentCultureIgnoreCase));

                if (!supportRequest)
                {
                    Logger.Instance.Log(LoggingLevel.WARN, $"Skipped: '{inheritor.GetClrName().FullName}' does not use '{selectedIdentifier.Name}'.");
                    continue;
                }

                return inheritor;
            }

            return null;
        }

        #endregion
    }

    #endregion
}