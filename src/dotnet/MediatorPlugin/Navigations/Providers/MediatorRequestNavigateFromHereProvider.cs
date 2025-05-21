using System.Collections.Generic;
using JetBrains.Application.DataContext;
using JetBrains.Application.Parts;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Diagnostics;
using ReSharper.MediatorPlugin.Services.Find;
using ReSharper.MediatorPlugin.Services.Navigation;

namespace ReSharper.MediatorPlugin.Navigations.Providers;

[ContextNavigationProvider(Instantiation.DemandAnyThreadSafe)]
public sealed class MediatorRequestNavigateFromHereProvider : INavigateFromHereProvider
{
    private readonly HandlerSelector _handlerSelector = new();

    public IEnumerable<ContextNavigation> CreateWorkflow
    (
        IDataContext dataContext
    )
    {
        return new List<ContextNavigation>
        {
            new ContextNavigation
            (
                "Go to mediator handler",
                "",
                NavigationActionGroup.Other,
                () =>
                {
                    var solution = dataContext.GetComponent<ISolution>();
                    var selectedTreeNode = dataContext.GetSelectedTreeNode<ITreeNode>();

                    if (selectedTreeNode is not IIdentifier selectedIdentifier)
                    {
                        Logger.Instance.Log(LoggingLevel.VERBOSE, $"Selected element is not an instance {nameof(IIdentifier)}");
                        return;
                    }
                    
                    _handlerSelector.Navigate
                    (
                        solution,
                        selectedTreeNode,
                        new DataContextNavigationOptionsFactory(dataContext)
                    ); 
                }
            )
        };
    }
}