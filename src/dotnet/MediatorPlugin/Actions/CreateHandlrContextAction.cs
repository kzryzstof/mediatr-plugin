using System;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Scope;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharper.MediatorPlugin.Diagnostics;
using ReSharper.MediatorPlugin.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Services;
using ReSharper.MediatorPlugin.Services.MediatR;

namespace ReSharper.MediatorPlugin.Actions
{
    [ContextAction
    (
        Name = "Create HandlR",
        Description = "Creates MediatR handler matching the selected request",
        Group = "C#",
        Disabled = false,
        Priority = 2
    )]
    public sealed class CreateHandlrContextAction : ContextActionBase
    {
        private readonly LanguageIndependentContextActionDataProvider _dataProvider;
        private readonly IMediator _mediatR;

        private readonly IIdentifier _mediatrRequestIdentifier;

        public override string Text => "Create HandlR";

        internal CreateHandlrContextAction
        (
            LanguageIndependentContextActionDataProvider dataProvider,
            IMediator mediatR
        )
        {
            Guard.ThrowIfIsNull(dataProvider, nameof(dataProvider));
            Guard.ThrowIfIsNull(mediatR, nameof(mediatR));

            Logger.Instance.Log(LoggingLevel.VERBOSE, "Ctor called.");

            _mediatR = mediatR;
            _dataProvider = dataProvider;
            _mediatrRequestIdentifier = GetSelectedMediatrRequest(dataProvider);
        }

        public CreateHandlrContextAction(LanguageIndependentContextActionDataProvider dataProvider)
            : this(dataProvider, new MediatR())
        {
        }

        /// <summary>
        /// Finds out whether the selected type implements MediatR's 'IBaseRequest'.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public override bool IsAvailable(IUserDataHolder _)
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE, "IsAvailable");

            return _mediatrRequestIdentifier is not NullIdentifier;
        }

        protected override Action<ITextControl> ExecutePsiTransaction
        (
            ISolution solution,
            IProgressIndicator progress
        )
        {
            if (_dataProvider.GetSelectedTreeNode<ITreeNode>() is not IIdentifier selectedIdentifier)
                return null;
            
            IClassLikeDeclaration requestTypeDeclaration = _dataProvider.GetSelectedElement<IClassLikeDeclaration>().NotNull();

            IClassLikeDeclaration classDeclaration = _mediatR.CreateHandlrFor(selectedIdentifier);

            using (WriteLockCookie.Create())
            {
                // todo: provide some customization where handler class should be placed at
                IClassLikeDeclaration handlerTypeDeclarationNode = ModificationUtil.AddChildAfter(requestTypeDeclaration, classDeclaration);

                return textControl =>
                {
                    // todo: add customization here as well
                    IMethodDeclaration? methodDeclaration = handlerTypeDeclarationNode.MethodDeclarations.First();
                    ICSharpStatement? statement = methodDeclaration.Body.Statements.FirstOrDefault(); // select 'not implemented' part
                    DocumentRange range = statement?.GetDocumentRange() ?? DocumentRange.InvalidRange;
                    textControl.Selection.SetRange(range);
                };
            }
        }

        private IIdentifier GetSelectedMediatrRequest(IContextActionDataProvider dataProvider)
        {
            if (dataProvider.GetSelectedElement<ITreeNode>() is not IIdentifier someIdentifier)
            {
                Logger.Instance.Log
                (
                    LoggingLevel.VERBOSE,
                    $"The selected tree node is not an instance of {nameof(IIdentifier)}."
                );
                return new NullIdentifier(dataProvider.PsiModule);
            }

            if (!_mediatR.IsRequest(someIdentifier))
            {
                Logger.Instance.Log(LoggingLevel.VERBOSE, "The selected tree node is not mediatR request.");
                return new NullIdentifier(dataProvider.PsiModule);
            }

            return someIdentifier;
        }
    }
}