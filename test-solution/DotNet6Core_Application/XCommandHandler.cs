using DotNetCore6_Domain.Entities;
using DotNetCore6_Domain.Requests;

namespace DotNet6Core_Application;

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