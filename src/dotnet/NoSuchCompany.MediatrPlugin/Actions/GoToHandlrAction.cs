// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 30/12/2021 @ 10:23
// ==========================================================================

using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Actions
{
    [Action("GoToHandlrAction", "Go to the HandlR")]
    public class GoToHandlrAction : IActionWithExecuteRequirement, IExecutableAction
    {
        private readonly IHandlrNavigator _handlrNavigator;

        public GoToHandlrAction()
        {
            _handlrNavigator = new HandlrNavigator(new MediatR());
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            Guard.ThrowIfIsNull(context, nameof(context));

            var selectedTreeNode = context.GetSelectedTreeNode<ITreeNode>();

            if (selectedTreeNode is not IIdentifier selectedIdentifier)
            {
                Logger.Instance.Log(LoggingLevel.WARN, $"Selected element is not an instance {nameof(IIdentifier)}");
                return;
            }

            _handlrNavigator.Navigate(selectedIdentifier);
        }

        public IActionRequirement GetRequirement(IDataContext dataContext)
        {
            return CommitAllDocumentsRequirement.TryGetInstance(dataContext);
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            Guard.ThrowIfIsNull(context, nameof(context));

            return IsMediatrRequestSelected(context);
        }

        private bool IsMediatrRequestSelected(IDataContext context)
        {
            var selectedTreeNode = context.GetSelectedTreeNode<ITreeNode>();

            if (selectedTreeNode is not IIdentifier selectedIdentifier || !_handlrNavigator.IsRequest(selectedIdentifier))
            {
                Logger.Instance.Log(LoggingLevel.WARN, "Selected element is not an MediatR request");
                return false;
            }

            return true;
        }
    }
}