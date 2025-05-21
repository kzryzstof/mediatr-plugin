using System.Collections.Generic;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Diagnostics;
using ReSharper.MediatorPlugin.Services.Navigation;

namespace ReSharper.MediatorPlugin.Services.Find;

internal sealed class HandlerSelector
{
    private readonly IHandlerNavigator _handlerNavigator;
    
    public HandlerSelector()
    {
        _handlerNavigator = new HandlerNavigator(new MediatR.MediatR());
    }
    
    public void Navigate
    (
        ISolution solution,
        ITreeNode selectedTreeNode,
        INavigationOptionsFactory navigationOptionsFactory
    )
    {
        if (selectedTreeNode is not IIdentifier selectedIdentifier)
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE, $"Selected element is not an instance {nameof(IIdentifier)}");
            return;
        }
                    
        var potentialNavigationPoints = new List<INavigationPoint>();

        IEnumerable<IDeclaredElement> result = _handlerNavigator.GetHandlers
        (
            selectedIdentifier
        );
                    
        foreach (IDeclaredElement? handler in result)
            potentialNavigationPoints.Add(new DeclaredElementNavigationPoint(handler));
                    
        // Get required components from the data context
        NavigationOptions options = navigationOptionsFactory.Get("Which handler do you want to navigate to?");
        NavigationManager.GetInstance(solution).Navigate(potentialNavigationPoints, options);
    }
}