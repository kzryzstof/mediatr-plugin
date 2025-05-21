using System;
using JetBrains.Application.Progress;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharper.MediatorPlugin.Diagnostics;
using ReSharper.MediatorPlugin.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Services;
using ReSharper.MediatorPlugin.Services.Find;
using ReSharper.MediatorPlugin.Services.MediatR;
using ReSharper.MediatorPlugin.Services.Navigation;

namespace ReSharper.MediatorPlugin.Actions
{
    [ContextAction
    (
        Name = "Go to HandlR",
        Description = "Looks for the MediatR handler matching the selected request",
        GroupType = typeof(CSharpContextActions),
        Disabled = false,
        Priority = 1
    )]
    public sealed class GoToHandlrContextAction : ContextActionBase
    {
        private readonly IHandlerNavigator _handlerNavigator;
        private readonly HandlerSelector _handlerSelector;
        private readonly LanguageIndependentContextActionDataProvider _dataProvider;
        
        private readonly IIdentifier _mediatrRequestIdentifier;

        public override string Text => "Go to HandlR";

        internal GoToHandlrContextAction
        (
            LanguageIndependentContextActionDataProvider dataProvider,
            IHandlerNavigator handlerNavigator
        )
        {
            Guard.ThrowIfIsNull(dataProvider, nameof(dataProvider));
            Guard.ThrowIfIsNull(handlerNavigator, nameof(handlerNavigator));

            Logger.Instance.Log(LoggingLevel.WARN, "Ctor called.");

            _dataProvider = dataProvider;
            _handlerNavigator = handlerNavigator;
            _handlerSelector = new HandlerSelector();
            _mediatrRequestIdentifier = GetSelectedMediatrRequest(dataProvider);
        }

        public GoToHandlrContextAction
        (
            LanguageIndependentContextActionDataProvider dataProvider
        ) : this(dataProvider, new HandlerNavigator(new MediatR()))
        {
        }
        
        public override bool IsAvailable
        (
            IUserDataHolder dataHolder
        )
        {
            Logger.Instance.Log(LoggingLevel.WARN, "IsAvailable");
            
            return _mediatrRequestIdentifier is not NullIdentifier;
        }

        private PopupWindowContextSource _windowContext;
        
        public override void Execute
        (
            ISolution solution,
            ITextControl textControl
        )
        {
            _windowContext = textControl.PopupWindowContextFactory.ForCaret();
            
            base.Execute(solution, textControl);
        }

        protected override Action<ITextControl> ExecutePsiTransaction
        (
            ISolution solution,
            IProgressIndicator progress
        )
        {
            Logger.Instance.Log
            (
                LoggingLevel.WARN,
                "ExecutePsiTransaction"
            );

            _handlerSelector.Navigate
            (
                solution,
                _mediatrRequestIdentifier,
                new PopupWindowContextSourceNavigationOptionsFactory(_windowContext)  
            );
            
            return DefaultActions.Empty;
        }

        private IIdentifier GetSelectedMediatrRequest
        (
            IContextActionDataProvider dataProvider
        )
        {
            var selectedElement = dataProvider.GetSelectedElement<ITreeNode>();

            if (selectedElement is null)
                return new NullIdentifier(dataProvider.PsiModule);
            
            IIdentifier? selectedIdentifier = selectedElement as IIdentifier ?? selectedElement.NextSibling as IIdentifier;
            
            if (selectedIdentifier is null)
            {
                Logger.Instance.Log
                (
                    LoggingLevel.VERBOSE,
                    $"The selected tree node is not an instance of {nameof(IIdentifier)}."
                );
                
                return new NullIdentifier(dataProvider.PsiModule);
            }
            
            if (!_handlerNavigator.IsRequest(selectedIdentifier))
            {
                Logger.Instance.Log
                (
                    LoggingLevel.VERBOSE,
                    "The selected tree node is not mediatR request."
                );
                
                return new NullIdentifier(dataProvider.PsiModule);
            }

            return selectedIdentifier;
        }
    }
}