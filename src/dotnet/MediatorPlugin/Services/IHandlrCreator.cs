using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.MediatorPlugin.Services
{
    public interface IHandlrCreator
    {
        IClassDeclaration CreateHandlrFor(IIdentifier identifier);
    }
}