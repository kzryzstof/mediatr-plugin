using DotNetCore6_Domain.Entities;
using MediatR;

namespace DotNetCore6_Domain.Requests;

public interface ISharedCommandHandler<in TRequest, TResponse> : IRequestHandler<TRequest, Dto<TResponse>> where TRequest : IRequest<Dto<TResponse>>
{
    
}

public class XCommandRequest : IRequest<Dto<XCommandResponse>> {}

public class XCommandResponse {}

