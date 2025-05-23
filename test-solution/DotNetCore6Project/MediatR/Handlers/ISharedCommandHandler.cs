using DotNetCore6_Domain.Entities;
using MediatR;

namespace DotNetCore6_Domain.MediatR.Handlers;

public interface ISharedCommandHandler<in TRequest, TResponse> : IRequestHandler<TRequest, Dto<TResponse>>
    where TRequest : IRequest<Dto<TResponse>>;
