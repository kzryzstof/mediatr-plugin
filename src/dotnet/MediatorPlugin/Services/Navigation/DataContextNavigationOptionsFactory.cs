using JetBrains.Application.DataContext;
using JetBrains.ReSharper.Feature.Services.Navigation;

namespace ReSharper.MediatorPlugin.Services.Navigation;

internal sealed class DataContextNavigationOptionsFactory : INavigationOptionsFactory
{
    private readonly IDataContext _dataContext;

    public DataContextNavigationOptionsFactory
    (
        IDataContext dataContext
    )
    {
        _dataContext = dataContext;
    }

    public NavigationOptions Get
    (
        string title
    )
    {
        return NavigationOptions.FromDataContext
        (
            _dataContext,
            title
        );
    }
}