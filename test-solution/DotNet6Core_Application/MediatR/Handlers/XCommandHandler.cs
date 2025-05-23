using DotNetCore6_Domain.Entities;
using DotNetCore6_Domain.MediatR.Handlers;
using DotNetCore6_Domain.MediatR.Requests;

namespace DotNet6Core_Application.MediatR.Handlers;

public class XCommandHandler : ISharedCommandHandler<XCommandRequest, XCommandResponse>
{
    public Task<Dto<XCommandResponse>> Handle
    (
        XCommandRequest request,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}