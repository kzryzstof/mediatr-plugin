using DotNetCore6_Domain.Entities;
using Mediator;

namespace DotNetCore6_Domain.Mediator.Requests;

public class NewMediatorRequest : IRequest<Dto<string>>, IRequest<Unit>
{
    
}