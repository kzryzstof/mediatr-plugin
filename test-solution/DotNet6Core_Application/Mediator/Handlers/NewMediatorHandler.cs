using DotNetCore6_Domain.Mediator;
using DotNetCore6_Domain.Mediator.Requests;
using Mediator;

namespace DotNet6Core_Application.Mediator.Handlers;

public class NewMediatorHandler : IRequestHandler<NewMediatorRequest>
{
    public ValueTask<Unit> Handle(NewMediatorRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}