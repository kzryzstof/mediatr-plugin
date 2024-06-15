using DotNetCore6_Domain.Requests;
using MediatR;

namespace DotNet6Core_Application;

public abstract class BaseRequestHandler : IRequestHandler<OtherRequest>
{
    public Task Handle(OtherRequest request, CancellationToken cancellationToken)
    {
        NewRequest newRequest = new NewRequest();
        
        throw new NotImplementedException();
    }
}