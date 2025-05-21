using JetBrains.Application.DataContext;
using JetBrains.ReSharper.Feature.Services.Navigation;

namespace ReSharper.MediatorPlugin.Services.Navigation;

internal sealed class DataContextNavigationOptionsFactory
(
    IDataContext dataContext
) : INavigationOptionsFactory
{
    public NavigationOptions Get
    (
        string title
    )
    {
        return NavigationOptions.FromDataContext
        (
            dataContext,
            title
        );
    }
}