using JetBrains.Application.UI.Controls.JetPopupMenu;
using JetBrains.ReSharper.Feature.Services.Occurrences;

namespace ReSharper.MediatorPlugin.Actions;

public sealed class MediatorOccurrencePresenter : IOccurrencePresenter
{
    public bool IsApplicable
    (
        IOccurrence occurrence
    )
    {
        throw new System.NotImplementedException();
    }
    
    public bool Present
    (
        IMenuItemDescriptor descriptor,
        IOccurrence occurrence,
        OccurrencePresentationOptions occurrencePresentationOptions
    )
    {
        throw new System.NotImplementedException();
    }
}