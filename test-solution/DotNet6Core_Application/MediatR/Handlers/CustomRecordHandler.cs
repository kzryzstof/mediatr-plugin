using DotNetCore6_Domain.MediatR.Requests;
using MediatR;

namespace DotNet6Core_Application.MediatR.Handlers;

public record CustomRecordHandler : IRequestHandler<CustomRecordRequest>
{
    public Task Handle(CustomRecordRequest recordRequest, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}