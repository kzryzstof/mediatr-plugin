using DotNetCore6_Domain.MediatR.Requests;
using MediatR;

namespace DotNet6Core_Application.MediatR.Handlers;

public abstract class BaseRequestHandler : IRequestHandler<OtherRequest>
{
    public Task Handle(OtherRequest request, CancellationToken cancellationToken)
    {
        NewRequest newRequest = new NewRequest();
        
        throw new NotImplementedException();
    }
}