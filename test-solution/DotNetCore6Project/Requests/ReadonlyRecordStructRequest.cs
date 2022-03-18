using MediatR;

namespace DotNetCore6_Domain.Requests;

public readonly record struct MyRequest(string input) : IRequest<MyResponse>;