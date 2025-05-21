using DotNetCore6_Domain.Notifications;
using MediatR;

namespace DotNet6Core_Application.Notifcations;

internal sealed class FirstCustomNotificationHandler : INotificationHandler<CustomNotification>
{
    public Task Handle(CustomNotification notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}