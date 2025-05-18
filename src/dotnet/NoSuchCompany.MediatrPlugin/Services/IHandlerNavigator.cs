using JetBrains.ReSharper.Psi.Tree;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services
{
    public interface IHandlerNavigator
    {
        bool IsRequest(IIdentifier identifier);

        void Navigate(IIdentifier request);
    }
}