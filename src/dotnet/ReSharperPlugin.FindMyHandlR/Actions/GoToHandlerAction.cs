// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 27/12/2021 @ 15:29
// Last author: Christophe Commeyne
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.Navigation.NavigationExtensions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;
using JetBrains.Util;
using JetBrains.Util.DataStructures;

namespace ReSharperPlugin.SandboxPlugin.Actions;

#region Class

[ContextAction
(
    Name = "Find my HandlR",
    Description = "Looks for the MediatR handler matching the selected request",
    Group = "C#",
    Disabled = false,
    Priority = 1
)]
public sealed class GoToHandlerAction : ContextActionBase
{
    #region Constants

    private readonly IType? _mediatrBaseRequestType;

    private readonly IDeclaredElement? _mediatrRequestHandlerDeclaredElement;

    private readonly IDeclaredType? _selectedDeclaredType;

    private readonly IIdentifier? _selectedIdentifier;

    private static readonly ILog Logger = Log.GetLog<GoToHandlerAction>();

    private static readonly Action<ITextControl> NoAction = _ => { };

    #endregion

    #region Properties

    public override string Text => "Find my HandlR!";

    #endregion

    #region Constructors

    /// <param name="dataProvider"></param>
    public GoToHandlerAction(LanguageIndependentContextActionDataProvider dataProvider)
    {
        if (dataProvider.GetSelectedElement<ITreeNode>() is not IIdentifier selectedIdentifier)
        {
            Logger.Log(LoggingLevel.VERBOSE, $"The selected tree node is not an instance of {nameof(IIdentifier)}.");
            return;
        }

        _selectedIdentifier = selectedIdentifier;
        _selectedDeclaredType = TryGetSelectedDeclaredType(selectedIdentifier);

        //  Gets the MediatR types: the IBaseRequest and the IRequestHandler.
        _mediatrBaseRequestType = GetMediatrBaseRequestType();
        _mediatrRequestHandlerDeclaredElement = GetMediatrRequestHandlerDeclaredElement();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Finds out whether the selected type implements MediatR's 'IBaseRequest'.
    /// </summary>
    /// <param name="cache"></param>
    /// <returns></returns>
    public override bool IsAvailable(IUserDataHolder cache)
    {
        if (_selectedDeclaredType is null)
            return false;

        if (_selectedDeclaredType.IsSubtypeOf((IDeclaredType)_mediatrBaseRequestType!))
        {
            Logger.Log(LoggingLevel.VERBOSE, $"> The declared type '{_selectedDeclaredType.GetClrName().FullName}' is considered a subtype of '{_mediatrBaseRequestType!.GetScalarType()!.GetClrName().FullName}'");
            return true;
        }

        Logger.Log(LoggingLevel.VERBOSE, $"> The declared type '{_selectedDeclaredType.GetClrName().FullName}' is not considered a subtype of '{_mediatrBaseRequestType!.GetScalarType()!.GetClrName().FullName}'");
        return false;
    }

    #endregion

    #region Protected Methods

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        if (_selectedIdentifier is null)
        {
            Logger.Log(LoggingLevel.WARN, $"???");
            return NoAction;
        }

        try
        {
            Logger.Log(LoggingLevel.WARN, $"Looking for a possible MediatR handler that is using the type '{_selectedIdentifier.Name}'");

            ITypeElement? mediatrHandlerTypeElement = FindMediatrHandlerWithSpecifiedRequestType(solution);

            if (mediatrHandlerTypeElement is null)
            {
                Logger.Log(LoggingLevel.WARN, $"No MediatR handler using the type '{_selectedIdentifier.Name}' has been found.");
                return NoAction;
            }

            Logger.Log(LoggingLevel.WARN, $"A MediatR handler using the type '{_selectedIdentifier.Name}' has been found: '{mediatrHandlerTypeElement.GetClrName().FullName}'");

            (bool found, ICSharpFile? csharpFile) = RetrieveCSharpFile(mediatrHandlerTypeElement);

            if (!found)
            {
                Logger.Log(LoggingLevel.WARN, "The C# source file of the MediatR handler could not be found.");
                return NoAction;
            }

            Logger.Log(LoggingLevel.WARN, $"The C# source file of the MediatR handler has been found: {csharpFile.GetSourceFile().DisplayName}");

            ITreeNode? treeNode = GetTypeTreeNodeByFqn(csharpFile, $"{mediatrHandlerTypeElement.GetContainingNamespace().QualifiedName}.{mediatrHandlerTypeElement.ShortName}");

            treeNode.NavigateToTreeNode(true);
        }
        catch (Exception unhandledException)
        {
            Logger.Log(LoggingLevel.ERROR, unhandledException.ToString());
        }

        return NoAction;
    }

    #endregion

    private sealed class InheritorsConsumer : IFindResultConsumer<ITypeElement>
    {
        private const int MaxInheritors = 50;

        private readonly HashSet<ITypeElement> _elements = new ();

        public IEnumerable<ITypeElement> FoundElements => _elements;

        public ITypeElement Build(FindResult result)
        {
            if (result is FindResultInheritedElement inheritedElement)
                return (ITypeElement) inheritedElement.DeclaredElement;
            
            return null;
        }

        public FindExecution Merge(ITypeElement data)
        {
            _elements.Add(data);
            return _elements.Count < MaxInheritors ? FindExecution.Continue : FindExecution.Stop;
        }
    }
    
    #region Private Methods

    private ITypeElement? FindMediatrHandlerWithSpecifiedRequestType(ISolution solution)
    {
        IPsiServices psiServices = solution.GetPsiServices();
        
        IReadOnlyCollection<ITypeElement> possibleInheritors = psiServices
            .Symbols
            .GetPossibleInheritors(_mediatrRequestHandlerDeclaredElement!.ShortName)
            .ToList();
        
        Logger.Log(LoggingLevel.WARN, $"Possible inheritors found: {string.Join(",", possibleInheritors.Select(i => i.ShortName))}");
        
        var inheritorsConsumer = new InheritorsConsumer();
        
        IType mediatrRequestHandlerType = CSharpTypeFactory.CreateType("MediatR.IRequestHandler<in TRequest>", _selectedIdentifier!);
        
        psiServices
            .Finder
            .FindInheritors(mediatrRequestHandlerType.GetTypeElement(), inheritorsConsumer, new ProgressIndicator(Lifetime.Eternal));
        
        Logger.Log(LoggingLevel.WARN, $"(2) Possible inheritors found: {string.Join(",", inheritorsConsumer.FoundElements.Select(i => i.ShortName))}");

        ITypeElement? inheritorTypeElement = SelectInheritor(inheritorsConsumer.FoundElements);
        
        return inheritorTypeElement;
    }

    private static string GetLongNameFromFqn(string fqn)
    {
        int pos = fqn.LastIndexOf(".", StringComparison.Ordinal) + 1;
        return pos > 0 ? fqn[..(pos - 1)] : fqn;
    }

    private IType GetMediatrBaseRequestType()
    {
        return CSharpTypeFactory.CreateType("MediatR.IBaseRequest", _selectedIdentifier!);
    }

    private IDeclaredElement GetMediatrRequestHandlerDeclaredElement()
    {
        IType mediatrRequestHandlerType = CSharpTypeFactory.CreateType("MediatR.IRequestHandler<in TRequest, TResponse>", _selectedIdentifier!);
        var mediatrRequestHandlerDeclaredType = (IDeclaredType)mediatrRequestHandlerType;
        IResolveResult resolveResult = mediatrRequestHandlerDeclaredType.Resolve();
        return resolveResult.DeclaredElement;
    }

    private static string GetShortNameFromFqn(string fqn)
    {
        int pos = fqn.LastIndexOf(".", StringComparison.Ordinal) + 1;
        return pos > 0 ? fqn[pos..] : fqn;
    }

    private static ITreeNode? GetTypeTreeNodeByFqn(ICSharpTypeAndNamespaceHolderDeclaration file, string typeName)
    {
        string namespaceName = GetLongNameFromFqn(typeName);
        string shortName = GetShortNameFromFqn(typeName);

        TreeNodeEnumerable<ICSharpNamespaceDeclaration> namespaceDecls = file.NamespaceDeclarationsEnumerable;
        ICSharpNamespaceDeclaration? namespaceDecl = (from decl in namespaceDecls
                                                      where decl.DeclaredName == namespaceName
                                                      select decl).FirstOrDefault();

        if (namespaceDecl is null)
            return null;

        TreeNodeEnumerable<ICSharpTypeDeclaration> typeDecls = namespaceDecl.TypeDeclarationsEnumerable;

        List<ICSharpTypeDeclaration> resultList = (from node in typeDecls
                                                   where node.DeclaredName == shortName
                                                   select node).ToList();

        return resultList.FirstOrDefault();
    }

    private static (bool found, ICSharpFile? csharpFile) RetrieveCSharpFile(IDeclaredElement mediatrHandlerDeclaredElement)
    {
        HybridCollection<IPsiSourceFile> sourceFiles = mediatrHandlerDeclaredElement.GetSourceFiles();

        var sourceFile = sourceFiles.First() as IPsiProjectFile;

        if (sourceFile is null)
            return (false, null);

        ICSharpFile? csharpFile = sourceFile?
            .GetPsiFiles<CSharpLanguage>()
            .SafeOfType<ICSharpFile>()
            .SingleOrDefault();

        return (csharpFile is not null, csharpFile);
    }

    private ITypeElement? SelectInheritor(IEnumerable<ITypeElement> inheritors)
    {
        foreach (ITypeElement? inheritor in inheritors)
        {
            Logger.Log(LoggingLevel.WARN, $"Inheritor: '{inheritor.GetClrName().FullName}'");

            if (inheritor is not IClass classInheritor)
            {
                Logger.Log(LoggingLevel.WARN, $"Skipped: '{inheritor.GetClrName().FullName}' is not an IClass instance.");
                continue;
            }

            if (classInheritor.IsAbstract)
            {
                Logger.Log(LoggingLevel.WARN, $"Skipped: '{inheritor.GetClrName().FullName}' is abstract.");
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
                .Any(userTypeUsage => string.Equals(userTypeUsage.ScalarTypeName.ShortName, _selectedIdentifier.Name, StringComparison.CurrentCultureIgnoreCase));

            if (!supportRequest)
            {
                Logger.Log(LoggingLevel.WARN, $"Skipped: '{inheritor.GetClrName().FullName}' does not use '{_selectedIdentifier.Name}'.");
                continue;
            }

            return inheritor;
        }

        return null;
    }

    private static IDeclaredType TryGetSelectedDeclaredType(IIdentifier selectedIdentifier)
    {
        IType type = CSharpTypeFactory.CreateType(selectedIdentifier.Name, selectedIdentifier);

        return (IDeclaredType)type;
    }

    #endregion
}

#endregion