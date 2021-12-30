// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 28/12/2021 @ 09:14
// Last author: Christophe Commeyne
// ==========================================================================

using System;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.Navigation.NavigationExtensions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Tree;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Actions
{
    #region Class

    [ContextAction
    (
        Name = "Find my HandlR",
        Description = "Looks for the MediatR handler matching the selected request",
        Group = "C#",
        Disabled = false,
        Priority = 1
    )]
    public sealed class GoToHandlerContextAction : ContextActionBase
    {
        #region Constants

        private readonly IMediatR _mediatR;

        private readonly IIdentifier _mediatrRequestIdentifier;

        #endregion

        #region Properties

        public override string Text => "Find my HandlR!";

        #endregion

        #region Constructors

        /// <param name="dataProvider"></param>
        /// <param name="mediatR"></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if:
        /// - The <paramref name="dataProvider" /> instance is null.
        /// - The <paramref name="mediatR" /> instance is null.
        /// </exception>
        internal GoToHandlerContextAction(LanguageIndependentContextActionDataProvider dataProvider, IMediatR mediatR)
        {
            Guard.ThrowIfIsNull(dataProvider, nameof(dataProvider));
            Guard.ThrowIfIsNull(mediatR, nameof(mediatR));

            _mediatR = mediatR;
            _mediatrRequestIdentifier = GetSelectedMediatrRequest(dataProvider);
        }

        /// <param name="dataProvider"></param>
        public GoToHandlerContextAction(LanguageIndependentContextActionDataProvider dataProvider)
            : this(dataProvider, new MediatR())
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds out whether the selected type implements MediatR's 'IBaseRequest'.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public override bool IsAvailable(IUserDataHolder _)
        {
            return _mediatrRequestIdentifier is not NullIdentifier;
        }

        #endregion

        #region Protected Methods

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            try
            {
                Logger.Instance.Log(LoggingLevel.WARN, $"Looking for a possible MediatR handler that is using the type '{_mediatrRequestIdentifier.Name}'");

                //  Finds the MediatR handler for the selected request.
                (bool handlerTypeFound, ITypeElement? mediatrHandlerTypeElement) = FindHandler(solution);

                if (!handlerTypeFound)
                    return DefaultActions.Empty;

                //  Finds the C# file where the MediatR handler is stored.
                (bool fileFound, ICSharpFile? csharpFile) = FindCSharpFile(mediatrHandlerTypeElement!);

                if (!fileFound)
                    return DefaultActions.Empty;

                //  Finds the tree node of the handler in that file.
                (bool nodeFound, ITreeNode? treeNode) = FindTreeNode(mediatrHandlerTypeElement!, csharpFile!);

                if (!nodeFound)
                    return DefaultActions.Empty;

                //  Go to the file!
                NavigateToFile(treeNode!);
            }
            catch (Exception unhandledException)
            {
                Logger.Instance.Log(LoggingLevel.ERROR, unhandledException.ToString());
            }

            return DefaultActions.Empty;
        }

        #endregion

        #region Private Methods

        private (bool fileFound, ICSharpFile? csharpFile) FindCSharpFile(IDeclaredElement typeElement)
        {
            (bool fileFound, ICSharpFile? csharpFile) = typeElement.FindCSharpFile();

            if (!fileFound)
                Logger.Instance.Log(LoggingLevel.WARN, "The C# source file of the MediatR handler could not be found.");
            else
                Logger.Instance.Log(LoggingLevel.WARN, $"The C# source file of the MediatR handler has been found: '{csharpFile!.GetSourceFile()!.DisplayName}'");

            return (fileFound, csharpFile);
        }

        private (bool handlerFound, ITypeElement? typeElement) FindHandler(ISolution solution)
        {
            ITypeElement? mediatrHandlerTypeElement = _mediatR.FindHandler(solution, _mediatrRequestIdentifier);

            if (mediatrHandlerTypeElement is null)
                Logger.Instance.Log(LoggingLevel.WARN, $"No MediatR handler using the type '{_mediatrRequestIdentifier.Name}' has been found.");
            else
                Logger.Instance.Log(LoggingLevel.WARN, $"A MediatR handler using the type '{_mediatrRequestIdentifier.Name}' has been found: '{mediatrHandlerTypeElement.GetClrName().FullName}'");

            return (mediatrHandlerTypeElement is not null, mediatrHandlerTypeElement);
        }

        private (bool nodeFound, ITreeNode? treeNode) FindTreeNode(ITypeElement typeElement, ICSharpFile csharpFile)
        {
            ITreeNode? treeNode = csharpFile.GetTreeNode(typeElement.GetFullname());

            if (treeNode is null)
                Logger.Instance.Log(LoggingLevel.WARN, $"The tree node for the type '{typeElement.ShortName}' could not be found in the file '{csharpFile.GetSourceFile()!.DisplayName}'.");
            else
                Logger.Instance.Log(LoggingLevel.WARN, $"The tree node for the type '{typeElement.ShortName}' has been found in the file '{csharpFile.GetSourceFile()!.DisplayName}'.");

            return (treeNode is not null, treeNode);
        }

        private IIdentifier GetSelectedMediatrRequest(IContextActionDataProvider dataProvider)
        {
            if (dataProvider.GetSelectedElement<ITreeNode>() is not IIdentifier mediatrRequestIdentifier)
            {
                Logger.Instance.Log(LoggingLevel.VERBOSE, $"The selected tree node is not an instance of {nameof(IIdentifier)}.");
                return new NullIdentifier(dataProvider.PsiModule);
            }

            if (!_mediatR.IsRequest(mediatrRequestIdentifier))
            {
                Logger.Instance.Log(LoggingLevel.VERBOSE, "The selected tree node is not mediatR request.");
                return new NullIdentifier(dataProvider.PsiModule);
            }

            return mediatrRequestIdentifier;
        }

        private void NavigateToFile(ITreeNode treeNode)
        {
            treeNode.NavigateToTreeNode(true);
        }

        #endregion
    }

    #endregion
}