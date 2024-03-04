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
    [ContextAction
    (
        Name = "Go to HandlR",
        Description = "Looks for the MediatR handler matching the selected request",
        Disabled = false,
        Priority = 1
    )]
    public sealed class GoToHandlrContextAction : ContextActionBase
    {
        private readonly IHandlrNavigator _handlrNavigator;

        private readonly IIdentifier _mediatrRequestIdentifier;

        public override string Text => "Go to HandlR";

        /// <param name="dataProvider"></param>
        /// <param name="handlrNavigator"></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if:
        /// - The <paramref name="dataProvider" /> instance is null.
        /// - The <paramref name="handlrNavigator" /> instance is null.
        /// </exception>
        internal GoToHandlrContextAction(LanguageIndependentContextActionDataProvider dataProvider,
            IHandlrNavigator handlrNavigator)
        {
            Guard.ThrowIfIsNull(dataProvider, nameof(dataProvider));
            Guard.ThrowIfIsNull(handlrNavigator, nameof(handlrNavigator));

            Logger.Instance.Log(LoggingLevel.WARN, "Ctor called.");

            _handlrNavigator = handlrNavigator;
            _mediatrRequestIdentifier = GetSelectedMediatrRequest(dataProvider);
        }

        /// <param name="dataProvider"></param>
        public GoToHandlrContextAction(LanguageIndependentContextActionDataProvider dataProvider)
            : this(dataProvider, new HandlrNavigator(new MediatR()))
        {
        }

        /// <summary>
        /// Finds out whether the selected type implements MediatR's 'IBaseRequest'.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public override bool IsAvailable(IUserDataHolder _)
        {
            Logger.Instance.Log(LoggingLevel.WARN, "IsAvailable");

            return _mediatrRequestIdentifier is not NullIdentifier;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            Logger.Instance.Log(LoggingLevel.WARN, "ExecutePsiTransaction");

            _handlrNavigator.Navigate(_mediatrRequestIdentifier);

            return DefaultActions.Empty;
        }

        private IIdentifier GetSelectedMediatrRequest(IContextActionDataProvider dataProvider)
        {
            if (dataProvider.GetSelectedElement<ITreeNode>() is not IIdentifier mediatrRequestIdentifier)
            {
                Logger.Instance.Log(LoggingLevel.VERBOSE,
                    $"The selected tree node is not an instance of {nameof(IIdentifier)}.");
                return new NullIdentifier(dataProvider.PsiModule);
            }

            if (!_handlrNavigator.IsRequest(mediatrRequestIdentifier))
            {
                Logger.Instance.Log(LoggingLevel.VERBOSE, "The selected tree node is not mediatR request.");
                return new NullIdentifier(dataProvider.PsiModule);
            }

            return mediatrRequestIdentifier;
        }
    }
}