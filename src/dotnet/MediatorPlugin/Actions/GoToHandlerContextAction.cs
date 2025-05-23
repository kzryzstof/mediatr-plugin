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
using ReSharper.MediatorPlugin.Services.Find;
using ReSharper.MediatorPlugin.Services.Navigation;

namespace ReSharper.MediatorPlugin.Actions;

[ContextAction
(
    Name = "Go to Handler",
    Description = "Looks for the Mediator handler matching the selected request",
    GroupType = typeof(CSharpContextActions),
    Disabled = false,
    Priority = 1
)]
public sealed class GoToHandlerContextAction : ContextActionBase
{
    private readonly IHandlerSelector _handlerSelector;
    private readonly IIdentifier _mediatrRequestIdentifier;

    private PopupWindowContextSource? _windowContext;
    
    public override string Text => "Go to Handler";

    public GoToHandlerContextAction
    (
        LanguageIndependentContextActionDataProvider dataProvider
    )
    {
        Guard.ThrowIfIsNull(dataProvider, nameof(dataProvider));

        Logger.Instance.Log(LoggingLevel.WARN, "Ctor called.");

        _handlerSelector = new HandlerSelector();
        _mediatrRequestIdentifier = GetSelectedMediatorRequest(dataProvider);
    }
        
    public override bool IsAvailable
    (
        IUserDataHolder dataHolder
    )
    {
        Logger.Instance.Log(LoggingLevel.WARN, "IsAvailable");
            
        return _mediatrRequestIdentifier is not NullIdentifier;
    }
    
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
        if (_windowContext is null)
        {
            Logger.Instance.Log
            (
                LoggingLevel.WARN,
                "Unable to retrieve the popup window context: navigation not possible"
            );
            return DefaultActions.Empty;
        }

        Logger.Instance.Log
        (
            LoggingLevel.INFO,
            "Navigating to one of the found handlers"
        );

        _handlerSelector.NavigateToHandler
        (
            solution,
            _mediatrRequestIdentifier,
            new PopupWindowContextSourceNavigationOptionsFactory(_windowContext)  
        );
            
        return DefaultActions.Empty;
    }

    private IIdentifier GetSelectedMediatorRequest
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
            
        if (!_handlerSelector.IsMediatorRequestSupported(selectedIdentifier))
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