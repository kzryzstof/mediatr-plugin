using JetBrains.ReSharper.Feature.Services.Navigation;

namespace ReSharper.MediatorPlugin.Services.Navigation;

internal interface INavigationOptionsFactory
{
    NavigationOptions Get
    (
        string title
    );
}