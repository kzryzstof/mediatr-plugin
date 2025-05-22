using DotNetCore6_Domain.MediatR.Requests;
using MediatR;

namespace DotNet6Core_Application.MediatR.Handlers;

public class CustomClassHandler : IRequestHandler<CustomClassRequest>
{
    public Task Handle(CustomClassRequest classRequest, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}