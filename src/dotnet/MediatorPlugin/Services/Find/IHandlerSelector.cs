using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Services.Navigation;

namespace ReSharper.MediatorPlugin.Services.Find;

internal interface IHandlerSelector
{
    bool IsMediatorRequestSupported
    (
        IIdentifier identifier
    );

    void NavigateToHandler
    (
        ISolution solution,
        ITreeNode selectedTreeNode,
        INavigationOptionsFactory navigationOptionsFactory
    );
}