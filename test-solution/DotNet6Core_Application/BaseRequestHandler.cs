using DotNetCore6_Domain.Requests;
using MediatR;

namespace DotNet6Core_Application;

public abstract class BaseRequestHandler : IRequestHandler<OtherRequest>
{
    public Task<Unit> Handle(OtherRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}