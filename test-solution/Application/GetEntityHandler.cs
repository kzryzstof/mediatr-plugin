using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;

namespace Application
{
    internal sealed class GetEntityHandler : IRequestHandler<GetEntityRequest, GetEntityResponse>
    {
        public Task<GetEntityResponse> Handle(GetEntityRequest request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}