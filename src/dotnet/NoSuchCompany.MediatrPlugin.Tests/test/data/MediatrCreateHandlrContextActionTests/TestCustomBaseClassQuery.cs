using MediatR;
namespace ClassLibrary1;

public class SomeDto {}

public abstract class BaseQuery<T> : IRequest<T>
{
};

public class Some{caret}Query : BaseQuery<SomeDto>
{
}