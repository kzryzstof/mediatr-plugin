using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Conversions;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using NoSuchCompany.ReSharper.MediatrPlugin.Actions;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Search;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Tree;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services
{
    internal sealed class MediatR : IMediatR
    {
        private readonly IHandlrCreator _handlrCreator;

        public MediatR(): this(new HandlrCreator()) { }

        private MediatR(IHandlrCreator handlrCreator)
        {
            _handlrCreator = handlrCreator;
        }

        private const string MediatrModuleName = "MediatR";
        
        public ITypeElement? FindHandler(IIdentifier identifier)
        {
            Guard.ThrowIfIsNull(identifier, nameof(identifier));

            IPsiServices psiServices = identifier.GetPsiServices();

            IType mediatrRequestHandlerType = CSharpTypeFactory.CreateType("MediatR.IRequestHandler<in TRequest, TResponse>", identifier);
            var mediatrRequestHandlerDeclaredType = (IDeclaredType) mediatrRequestHandlerType;
            IResolveResult resolveResult = mediatrRequestHandlerDeclaredType.Resolve();

            var inheritorsConsumer = new InheritorsConsumer();

            ITypeElement handlerTypeElement = mediatrRequestHandlerType.GetTypeElement()!;
                
            psiServices
                .Finder
                .FindInheritors
                (
                    handlerTypeElement, 
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

            Logger.Instance.Log(LoggingLevel.VERBOSE, $"Possible inheritors found: {string.Join(",", results.Select(i => i.GetClrName().FullName))}");

            ITypeElement? inheritorTypeElement = SelectInheritor(results, identifier);

            return inheritorTypeElement;
        }

        public bool IsRequest(IIdentifier identifier)
        {
            Guard.ThrowIfIsNull(identifier, nameof(identifier));
                
            ICSharpTypeConversionRule typeConversionRule = identifier.GetPsiModule().GetTypeConversionRule();
            
            IDeclaredType declaredType = identifier.ToDeclaredType();

            IPsiModule? mediatrModule = identifier.GetPsiServices().Modules.GetModules().FirstOrDefault(module => string.Equals(module.Name, MediatrModuleName, StringComparison.OrdinalIgnoreCase));

            if (mediatrModule == null)
            {
                Logger.Instance.Log(LoggingLevel.VERBOSE, $"> Unable to find a module starting with the name '{MediatrModuleName}'");
                return false;
            }

            IType mediatrBaseRequestType = TypeFactory.CreateTypeByCLRName("MediatR.IBaseRequest", mediatrModule);

            if (!mediatrBaseRequestType.IsResolved)
            {
                //  This happens if there are errors in the solution and ReSharper does not know about the MediatR module yet.
                Logger.Instance.Log(LoggingLevel.VERBOSE, $"> The mediatR IBaseRequest type is not resolved");
                return false;
            }
            
            if (declaredType.IsSubtypeOf(mediatrBaseRequestType, typeConversionRule))
            {
                Logger.Instance.Log(LoggingLevel.VERBOSE, $"> The declared type '{declaredType.GetClrName().FullName}' is considered a subtype of '{mediatrBaseRequestType.GetScalarType()!.GetClrName().FullName}'");
                return true;
            }

            Logger.Instance.Log(LoggingLevel.VERBOSE, $"> The declared type '{declaredType.GetClrName().FullName}' is not considered a subtype of '{mediatrBaseRequestType.GetScalarType()!.GetClrName().FullName}'");
            return false;
        }

        public IClassLikeDeclaration CreateHandlrFor(IIdentifier identifier)
        {
            return _handlrCreator.CreateHandlrFor(identifier);
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
                    .Where(scalarTypeName => scalarTypeName.TypeArgumentList is not null)
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