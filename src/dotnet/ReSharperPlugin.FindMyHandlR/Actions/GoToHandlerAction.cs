using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.Navigation.NavigationExtensions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Paths;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using JetBrains.Util.DataStructures;

namespace ReSharperPlugin.SandboxPlugin.Actions
{
    #region Classes

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

        private readonly IDeclaredElement? _mediatrAsyncRequestHandlerDeclaredElement;

        private readonly IDeclaredElement? _mediatrRequestHandlerDeclaredElement;

        private readonly IType _mediatrRequestType;

        private readonly IType _mediatrRequestWithResponseType;

        private readonly IDeclaredType? _selectedDeclaredType;

        private readonly IIdentifier? _selectedTreeNode;

        #endregion

        #region Fields

        private static readonly ILog GoToHandlerActionLog = Log.GetLog<GoToHandlerAction>();

        #endregion

        #region Properties

        public override string Text => "Find my HandlR!";

        #endregion

        #region Constructors

        public GoToHandlerAction(LanguageIndependentContextActionDataProvider dataProvider)
        {
            GoToHandlerActionLog.Log(LoggingLevel.WARN, "GoToHandlerActionCalled");

            try
            {
                _selectedTreeNode = dataProvider.GetSelectedElement<ITreeNode>() as IIdentifier;

                if (_selectedTreeNode is null)
                    return;

                _selectedDeclaredType = TryGetSelectedType(_selectedTreeNode);

                _mediatrRequestType = CSharpTypeFactory.CreateType("MediatR.IRequest", _selectedTreeNode);
                _mediatrRequestWithResponseType = CSharpTypeFactory.CreateType("MediatR.IRequest<out TResponse>", _selectedTreeNode);

                IType mediatrRequestHandlerType = CSharpTypeFactory.CreateType("MediatR.IRequestHandler<in TRequest, TResponse>", _selectedTreeNode);
                var mediatrRequestHandlerDeclaredType = mediatrRequestHandlerType as IDeclaredType;
                IResolveResult? resolveResult = mediatrRequestHandlerDeclaredType?.Resolve();
                _mediatrRequestHandlerDeclaredElement = resolveResult?.DeclaredElement;

                IType mediatrAsyncRequestHandlerType = CSharpTypeFactory.CreateType("MediatR.AsyncRequestHandler<in TRequest>", _selectedTreeNode);
                var mediatrAsyncRequestHandlerDeclaredType = mediatrAsyncRequestHandlerType as IDeclaredType;
                IResolveResult? asyncRequestResolveResult = mediatrAsyncRequestHandlerDeclaredType?.Resolve();
                _mediatrAsyncRequestHandlerDeclaredElement = asyncRequestResolveResult?.DeclaredElement;
            }
            catch (Exception unhandledException)
            {
                GoToHandlerActionLog.Log(LoggingLevel.ERROR, unhandledException.ToString());
            }
        }

        #endregion

        #region Public Methods

        public static ICSharpFile GetCSharpFile(IProject project, string filename)
        {
            VirtualFileSystemPath fileSystem = VirtualFileSystemPath.Parse(filename, LocalInteractionContext.Instance);
            IPsiSourceFile? file = project.GetPsiSourceFileInProject(fileSystem);
            return file?.GetPsiFiles<CSharpLanguage>().SafeOfType<ICSharpFile>().SingleOrDefault();
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            return true;
            /*
            return _selectedDeclaredType is not null
                && 
                (
                    _selectedDeclaredType.IsSubtypeOf(_mediatrRequestType)
                    || _selectedDeclaredType.IsSubtypeOf(_mediatrRequestWithResponseType)
                );
            */
        }

        #endregion

        #region Protected Methods

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            try
            {
                IPsiServices psiServices = solution.GetPsiServices();

                IReadOnlyCollection<ITypeElement> possibleInheritors = psiServices
                    .Symbols
                    .GetPossibleInheritors(_mediatrRequestHandlerDeclaredElement?.ShortName)
                    .ToList()
                    .Union(psiServices.Symbols.GetPossibleInheritors(_mediatrAsyncRequestHandlerDeclaredElement?.ShortName))
                    .ToList();

                ITypeElement? typeElement = SelectInheritor(possibleInheritors);

                HybridCollection<IPsiSourceFile> sourceFiles = typeElement.GetSourceFiles();

                var sourceFile = sourceFiles.First() as IPsiProjectFile;
                IProject? project = sourceFile.GetProject();
                ICSharpFile? file = GetCSharpFile(project, sourceFile.Location.FullPath);
                ITreeNode node = GetTypeTreeNodeByFqn(file, $"{typeElement.GetContainingNamespace().QualifiedName}.{typeElement.ShortName}");
                node.NavigateToTreeNode(true);
            }
            catch (Exception unhandledException)
            {
                GoToHandlerActionLog.Log(LoggingLevel.ERROR, unhandledException.ToString());
            }
            
            return null;
        }

        #endregion

        #region Private Methods

        private static string GetLongNameFromFqn(string fqn)
        {
            int pos = fqn.LastIndexOf(".", StringComparison.Ordinal) + 1;
            return pos > 0 ? fqn.Substring(0, pos - 1) : fqn;
        }

        private static string GetShortNameFromFqn(string fqn)
        {
            int pos = fqn.LastIndexOf(".", StringComparison.Ordinal) + 1;
            return pos > 0 ? fqn.Substring(pos) : fqn;
        }

        private static ITreeNode GetTypeTreeNodeByFqn(ICSharpFile file, string typeName)
        {
            string namespaceName = GetLongNameFromFqn(typeName);
            string shortName = GetShortNameFromFqn(typeName);

            TreeNodeEnumerable<ICSharpNamespaceDeclaration> namespaceDecls = file.NamespaceDeclarationsEnumerable;
            ICSharpNamespaceDeclaration? namespaceDecl = (from decl in namespaceDecls
                where decl.DeclaredName == namespaceName
                select decl).FirstOrDefault();

            if (namespaceDecl == null)
                return null;

            TreeNodeEnumerable<ICSharpTypeDeclaration> typeDecls = namespaceDecl.TypeDeclarationsEnumerable;

            List<ICSharpTypeDeclaration> resultList = (from node in typeDecls
                where node.DeclaredName == shortName
                select node).ToList();

            return resultList.FirstOrDefault();
        }

        private static IDeclaredType? TryGetSelectedType(ITreeNode? selectedTreeNode)
        {
            if (selectedTreeNode is null)
                return null;

            var selectedIdentifier = selectedTreeNode as IIdentifier;

            if (selectedIdentifier is null)
                return null;

            IType type = CSharpTypeFactory.CreateType(selectedIdentifier.Name, selectedIdentifier);

            return type as IDeclaredType;

            // IResolveResult? result = declaredType?.Resolve();
            //
            // return result?.DeclaredElement;
        }

        private ITypeElement? SelectInheritor(IEnumerable<ITypeElement> inheritors)
        {
            foreach (ITypeElement? inheritor in inheritors)
            {
                if (inheritor is not IClass classInheritor)
                    continue;

                if (classInheritor.IsAbstract)
                    continue;

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
                    .Any(userTypeUsage => string.Equals(userTypeUsage.ScalarTypeName.ShortName, _selectedTreeNode.Name, StringComparison.CurrentCultureIgnoreCase));

                if (supportRequest)
                    return inheritor;
            }

            return null;
        }

        #endregion
    }

    #endregion
}