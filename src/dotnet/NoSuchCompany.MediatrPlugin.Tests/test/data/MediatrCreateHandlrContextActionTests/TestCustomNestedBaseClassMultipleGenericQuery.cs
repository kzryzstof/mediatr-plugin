using MediatR;
using System.Collections.Generic;

namespace ClassLibrary1;

public class SomeDto {}
public class OtherDto {}

public abstract class BaseQuery<T> : IRequest<T>
{
}

public abstract class GetItemsQuery<T> : BaseQuery<IDictionary<SomeDto, T>>
{
}

public class Some{caret}Query : GetItemsQuery<OtherDto>
{
}