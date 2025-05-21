using JetBrains.Application.UI.PopupLayout;
using JetBrains.ReSharper.Feature.Services.Navigation;

namespace ReSharper.MediatorPlugin.Services.Navigation;

internal sealed class PopupWindowContextSourceNavigationOptionsFactory : INavigationOptionsFactory
{
    private readonly PopupWindowContextSource _windowContext;

    public PopupWindowContextSourceNavigationOptionsFactory
    (
        PopupWindowContextSource windowContext
    )
    {
        _windowContext = windowContext;
    }

    public NavigationOptions Get
    (
        string title
    )
    {
        return NavigationOptions.FromWindowContext
        (
            _windowContext,
            title
        );
    }
}