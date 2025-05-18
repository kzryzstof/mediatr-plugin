using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.MediatorPlugin.Services
{
    public interface IHandlerNavigator
    {
        bool IsRequest(IIdentifier identifier);

        void Navigate(IIdentifier request);
    }
}