using MediatR;

namespace DotNetCore6_Domain.MediatR.Requests;

public readonly record struct MyRequest(string input) : IRequest<MyResponse>;