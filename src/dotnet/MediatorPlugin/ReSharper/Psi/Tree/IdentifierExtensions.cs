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
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReSharper.MediatorPlugin.Diagnostics;
using ReSharper.MediatorPlugin.ReSharper.Psi.Search;
using Xunit;

namespace ReSharper.MediatorPlugin.ReSharper.Psi.Tree;

internal static class IdentifierExtensions
{
    private static readonly List<ITypeElement> EmptyTypeElements = new List<ITypeElement>();
    
    public static IDeclaredType ToDeclaredType
    (
        this IIdentifier identifier
    )
    {
        Guard.ThrowIfIsNull(identifier, nameof(identifier));

        IType type = CSharpTypeFactory.CreateType(identifier.Name, identifier);

        return (IDeclaredType) type;
    }
    
    public static IEnumerable<ITypeElement> FindHandlers
    (
        this IIdentifier identifier,
        string requestHandlerType
    )
    {
        IPsiServices psiServices = identifier.GetPsiServices();

        //  Need to get the PSI 
        var psiModules = psiServices.GetComponent<IPsiModules>();
        IPsiModule? mediatrPsiModule = psiModules.GetModules().FirstOrDefault(psiModule => psiModule.Name == "MediatR");
        ITypeElement? typeElement = TypeFactory.CreateTypeByCLRName(requestHandlerType, mediatrPsiModule).GetTypeElement();

        if (identifier.Parent is not IDeclaration declaration)
            return EmptyTypeElements;
        
        var inheritorsConsumer = new InheritorsConsumer();

        psiServices
            .SingleThreadedFinder
            .FindInheritors
            (
                typeElement, 
                inheritorsConsumer,
                new ProgressIndicator(Lifetime.Eternal)
            );

        //  I do not know why but FindInheritors and GetPossibleInheritors do not return all the handlers by themselves.
        //  I have to union both results of each to get all the actual IRequestHandler implementations.
        List<ITypeElement> results = inheritorsConsumer
            .FoundElements
            .Union
            (
                psiServices
                    .Symbols
                    .GetPossibleInheritors(declaration.DeclaredElement?.ShortName ?? "not found")
            )
            .ToList();

        Logger.Instance.Log
        (
            LoggingLevel.VERBOSE,
            $"Possible inheritors found: {string.Join(",", results.Select(i => i.GetClrName().FullName))}"
        );

        return SelectInheritors(results, identifier);
    }
    
    public static bool IsRequestTypeSupported
    (
        this IIdentifier identifier,
        string moduleName,
        string requestType
    )
    {
        ICSharpTypeConversionRule typeConversionRule = identifier
            .GetPsiModule()
            .GetTypeConversionRule();
            
        IDeclaredType declaredType = identifier.ToDeclaredType();

        IPsiModule? mediatrModule = identifier
            .GetPsiServices()
            .Modules
            .GetModules()
            .FirstOrDefault
            (
                module => string.Equals(module.Name, moduleName, StringComparison.OrdinalIgnoreCase)
            );

        if (mediatrModule == null)
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE, $"> Unable to find a module starting with the name '{ moduleName}'");
            return false;
        }

        IType mediatrBaseRequestType = TypeFactory.CreateTypeByCLRName(requestType, mediatrModule);

        if (!mediatrBaseRequestType.IsResolved)
        {
            //  This happens if there are errors in the solution and ReSharper does not know about the MediatR module yet.
            Logger.Instance.Log(LoggingLevel.VERBOSE, "> The mediatR IBaseRequest type is not resolved");
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
    
    private static IEnumerable<ITypeElement> SelectInheritors
    (
        IEnumerable<ITypeElement> inheritors,
        IIdentifier selectedIdentifier
    )
    {
        IDeclaredType selectedDeclaredType = selectedIdentifier.ToDeclaredType();
        string selectedFullTypeName = selectedDeclaredType.GetClrName().FullName;
            
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
                .SelectMany(GetInheritedTypeUsageName)
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

            yield return inheritor;
        }
    }
    
    private static IEnumerable<IReferenceName> GetInheritedTypeUsageName
    (
        IDeclaration declaration
    )
    {
        IEnumerable<ITypeUsage> typeUsages;
            
        if (declaration is IClassDeclaration classDeclaration)
            typeUsages = classDeclaration.InheritedTypeUsagesEnumerable;
        else if (declaration is IRecordDeclaration recordDeclaration)
            typeUsages = recordDeclaration.InheritedTypeUsagesEnumerable;
        else
            typeUsages = new List<ITypeUsage>();
            
        return typeUsages
            .Where
            (
                typeUsage => typeUsage is IUserTypeUsage
            )
            .Cast<IUserTypeUsage>()
            .Select
            (
                userTypeUsager => userTypeUsager.ScalarTypeName
            );
    }
}