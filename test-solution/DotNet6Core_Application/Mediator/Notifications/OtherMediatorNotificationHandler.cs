using DotNetCore6_Domain.Mediator;
using DotNetCore6_Domain.Mediator.Notifications;
using Mediator;

namespace DotNet6Core_Application.Mediator.Notifications;

public class MediatorNotificationHandler : INotificationHandler<NewMediatorNotification>
{
    public ValueTask Handle(NewMediatorNotification notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}