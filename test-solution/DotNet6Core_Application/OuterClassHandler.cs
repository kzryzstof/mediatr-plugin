using DotNetCore6_Domain.Requests;
using MediatR;

namespace DotNet6Core_Application;

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