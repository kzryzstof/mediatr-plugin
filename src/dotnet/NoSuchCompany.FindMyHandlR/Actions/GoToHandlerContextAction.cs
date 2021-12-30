// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 30/12/2021 @ 10:26
// Last author: Christophe Commeyne
// ==========================================================================

using System;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Tree;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Actions
{
    #region Class

    [ContextAction
    (
        Name = "Go to HandlR",
        Description = "Looks for the MediatR handler matching the selected request",
        Group = "C#",
        Disabled = false,
        Priority = 1
    )]
    public sealed class GoToHandlerContextAction : ContextActionBase
    {
        #region Constants

        private readonly IHandlrNavigator _handlrNavigator;

        private readonly IIdentifier _mediatrRequestIdentifier;

        #endregion

        #region Properties

        public override string Text => "Go to HandlR";

        #endregion

        #region Constructors

        /// <param name="dataProvider"></param>
        /// <param name="handlrNavigator"></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if:
        /// - The <paramref name="dataProvider" /> instance is null.
        /// - The <paramref name="handlrNavigator" /> instance is null.
        /// </exception>
        internal GoToHandlerContextAction(LanguageIndependentContextActionDataProvider dataProvider, IHandlrNavigator handlrNavigator)
        {
            Guard.ThrowIfIsNull(dataProvider, nameof(dataProvider));
            Guard.ThrowIfIsNull(handlrNavigator, nameof(handlrNavigator));

            _handlrNavigator = handlrNavigator;
            _mediatrRequestIdentifier = GetSelectedMediatrRequest(dataProvider);
        }

        /// <param name="dataProvider"></param>
        public GoToHandlerContextAction(LanguageIndependentContextActionDataProvider dataProvider)
            : this(dataProvider, new HandlrNavigator(new MediatR()))
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
            _handlrNavigator.Navigate(_mediatrRequestIdentifier);

            return DefaultActions.Empty;
        }

        #endregion

        #region Private Methods

        private IIdentifier GetSelectedMediatrRequest(IContextActionDataProvider dataProvider)
        {
            if (dataProvider.GetSelectedElement<ITreeNode>() is not IIdentifier mediatrRequestIdentifier)
            {
                Logger.Instance.Log(LoggingLevel.VERBOSE, $"The selected tree node is not an instance of {nameof(IIdentifier)}.");
                return new NullIdentifier(dataProvider.PsiModule);
            }

            if (!_handlrNavigator.IsRequest(mediatrRequestIdentifier))
            {
                Logger.Instance.Log(LoggingLevel.VERBOSE, "The selected tree node is not mediatR request.");
                return new NullIdentifier(dataProvider.PsiModule);
            }

            return mediatrRequestIdentifier;
        }

        #endregion
    }

    #endregion
}