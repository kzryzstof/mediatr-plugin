using DotNetCore6_Domain.Requests;
using MediatR;

namespace DotNet6Core_Application;

public class ReadonlyRecordStructHandler : IRequestHandler<MyRequest, MyResponse>
{
    public Task<MyResponse> Handle(MyRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}