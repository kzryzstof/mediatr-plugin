using DotNetCore6_Domain.MediatR.Requests;
using MediatR;

namespace DotNet6Core_Application.MediatR.Handlers;

public class OutestClassHandler
{
    public class OuterClassHandler
    {
        public class InnerClassHandler : IRequestHandler<InnerClassRequest>
        {
            public Task Handle(InnerClassRequest request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}