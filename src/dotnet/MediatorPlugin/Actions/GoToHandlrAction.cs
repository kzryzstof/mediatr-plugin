using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Menu;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Diagnostics;
using ReSharper.MediatorPlugin.Services.Find;
using ReSharper.MediatorPlugin.Services.Navigation;

namespace ReSharper.MediatorPlugin.Actions;

[Action
(
    "GoToHandlrAction",
    "Go to Handler",
    IdeaShortcuts = new [] {"Alt+H"}, 
    VsShortcuts = new [] {"Alt+H"}
)]
public class GoToHandlrAction : IActionWithExecuteRequirement, IExecutableAction, IInsertLast<NavigateMenu>
{
    private readonly IHandlerSelector _handlerSelector;

    public GoToHandlrAction()
    {
        Logger.Instance.Log(LoggingLevel.VERBOSE, "GoToHandlrAction instance has been created");

        _handlerSelector = new HandlerSelector();
    }

    public void Execute
    (
        IDataContext context,
        DelegateExecute nextExecute
    )
    {
        Guard.ThrowIfIsNull(context, nameof(context));

        var solution = context.GetComponent<ISolution>();
        var selectedTreeNode = context.GetSelectedTreeNode<ITreeNode>();

        if (selectedTreeNode is not IIdentifier selectedIdentifier)
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE, $"Selected element is not an instance {nameof(IIdentifier)}");
            return;
        }

        _handlerSelector.NavigateToHandler
        (
            solution,
            selectedTreeNode,
            new DataContextNavigationOptionsFactory(context)
        );
    }

    public IActionRequirement GetRequirement
    (
        IDataContext dataContext
    )
    {
        return CommitAllDocumentsRequirement.TryGetInstance(dataContext);
    }

    public bool Update
    (
        IDataContext context,
        ActionPresentation presentation,
        DelegateUpdate nextUpdate
    )
    {
        Guard.ThrowIfIsNull(context, nameof(context));

        return IsMediatrRequestSelected(context);
    }

    private bool IsMediatrRequestSelected
    (
        IDataContext context
    )
    {
        var selectedTreeNode = context.GetSelectedTreeNode<ITreeNode>();

        if (selectedTreeNode is IIdentifier selectedIdentifier && _handlerSelector.IsMediatorRequestSupported(selectedIdentifier))
            return true;

        Logger.Instance.Log(LoggingLevel.VERBOSE, "Selected element is not a supported Mediator request");
            
        return false;
    }
}