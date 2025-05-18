using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Menu;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services;

namespace NoSuchCompany.ReSharper.MediatrPlugin.Actions
{
    [Action
    (
        "GoToHandlrAction",
        "Go to HandlR",
        IdeaShortcuts = new [] {"Alt+H"}, 
        VsShortcuts = new [] {"Alt+H"}
    )]
    public class GoToHandlrAction : IActionWithExecuteRequirement, IExecutableAction, IInsertLast<NavigateMenu>
    {
        private readonly IHandlerNavigator _handlerNavigator;

        public GoToHandlrAction()
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE, "GoToHandlrAction instance has been created");

            _handlerNavigator = new HandlerNavigator(new MediatR());
        }

        public void Execute
        (
            IDataContext context,
            DelegateExecute nextExecute
        )
        {
            Guard.ThrowIfIsNull(context, nameof(context));

            var selectedTreeNode = context.GetSelectedTreeNode<ITreeNode>();

            if (selectedTreeNode is not IIdentifier selectedIdentifier)
            {
                Logger.Instance.Log(LoggingLevel.VERBOSE, $"Selected element is not an instance {nameof(IIdentifier)}");
                return;
            }

            _handlerNavigator.Navigate(selectedIdentifier);
        }

        public IActionRequirement GetRequirement(IDataContext dataContext)
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

            if (selectedTreeNode is IIdentifier selectedIdentifier && _handlerNavigator.IsRequest(selectedIdentifier))
                return true;

            Logger.Instance.Log(LoggingLevel.VERBOSE, "Selected element is not an MediatR request");
            
            return false;
        }
    }
}