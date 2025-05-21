using JetBrains.Application.UI.PopupLayout;
using JetBrains.ReSharper.Feature.Services.Navigation;

namespace ReSharper.MediatorPlugin.Services.Navigation;

internal sealed class PopupWindowContextSourceNavigationOptionsFactory
(
    PopupWindowContextSource windowContext
) : INavigationOptionsFactory
{
    public NavigationOptions Get
    (
        string title
    )
    {
        return NavigationOptions.FromWindowContext
        (
            windowContext,
            title
        );
    }
}