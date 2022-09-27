using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Menu;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ActionExtensions;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services;

namespace NoSuchCompany.ReSharper.MediatrPlugin.Actions
{
	[ActionWithPsiContext("GoToHandlrAction", "Go to HandlR", Kind = CompilationContextKind.Global, IdeaShortcuts = new [] {"Shift+F10"}, VsShortcuts = new [] {"Shift+F10"})]
	public class GoToHandlrAction : IActionWithExecuteRequirement, IExecutableAction, IInsertLast<NavigateMenu>
	{
		private readonly IHandlrNavigator _handlrNavigator;

		public GoToHandlrAction()
		{
			_handlrNavigator = new HandlrNavigator(new MediatR());
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
	}
}