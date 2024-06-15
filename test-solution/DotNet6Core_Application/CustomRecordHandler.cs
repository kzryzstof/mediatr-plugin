using DotNetCore6_Domain.Requests;
using MediatR;

namespace DotNet6Core_Application;

public class CustomRecordHandler : IRequestHandler<CustomRecordRequest>
{
    public Task Handle(CustomRecordRequest recordRequest, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}