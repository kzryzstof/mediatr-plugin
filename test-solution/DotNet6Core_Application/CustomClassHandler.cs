using DotNetCore6_Domain.Requests;
using MediatR;

namespace DotNet6Core_Application;

public class CustomClassHandler : IRequestHandler<CustomClassRequest>
{
    public Task<Unit> Handle(CustomClassRequest classRequest, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}