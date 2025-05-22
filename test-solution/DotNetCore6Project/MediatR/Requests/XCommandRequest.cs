using DotNetCore6_Domain.Entities;
using MediatR;

namespace DotNetCore6_Domain.MediatR.Requests;

public class XCommandRequest : IRequest<Dto<XCommandResponse>> {}

public class XCommandResponse {}

