using DotNetCore6_Domain.Notifications;
using MediatR;

namespace DotNet6Core_Application.Notifcations;

internal sealed class SecondCustomNotificationHandler : INotificationHandler<CustomNotification>
{
    public Task Handle(CustomNotification notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}