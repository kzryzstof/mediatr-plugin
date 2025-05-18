using MediatR;
using System.Collections.Generic;

namespace ClassLibrary1;

public class SomeDto {}

public abstract class BaseQuery<T> : IRequest<T>
{
}

public abstract class GetItemsQuery<T> : BaseQuery<IReadOnlyCollection<T>>
{
}

public class Some{caret}Query : GetItemsQuery<SomeDto>
{
}