using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;

namespace Application
{
    internal sealed class StartStreamingHandler : AsyncRequestHandler<StartStreamingRequest>
    {
        protected override Task Handle(StartStreamingRequest request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}