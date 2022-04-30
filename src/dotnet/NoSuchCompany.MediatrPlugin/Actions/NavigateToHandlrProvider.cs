using System.Collections.Generic;
using JetBrains.Application.DataContext;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Psi.Tree;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services;

namespace NoSuchCompany.ReSharper.MediatrPlugin.Actions
{
    [ContextNavigationProvider]
    public class NavigateToCtorProvider : INavigateFromHereProvider
    {
        private static readonly string NavigateToHandlrActionId = "NavigateToHandlR";
        private readonly IHandlrNavigator _handlrNavigator;

        public NavigateToCtorProvider()
        {
            _handlrNavigator = new HandlrNavigator(new MediatR());
        }

        public IEnumerable<ContextNavigation> CreateWorkflow(IDataContext dataContext)
        {
            if (false == IsMediatrRequestSelected(dataContext))
            {
                Logger.Instance.Log(LoggingLevel.WARN, $"Selected element is not an instance {nameof(IIdentifier)}");
                yield break;
            }

            yield return new ContextNavigation("Go to handlR...", NavigateToHandlrActionId, NavigationActionGroup.Other, () =>
            {
                var selectedTreeNode = dataContext.GetSelectedTreeNode<ITreeNode>();
                
                if (selectedTreeNode is not IIdentifier selectedIdentifier)
                    return;

                _handlrNavigator.Navigate(selectedIdentifier);
            });
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
