using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;

namespace Application
{
    internal sealed class TriggerDeletionHandler : IRequestHandler<TriggerDeletionRequest>
    {
        public Task<Unit> Handle(TriggerDeletionRequest request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}