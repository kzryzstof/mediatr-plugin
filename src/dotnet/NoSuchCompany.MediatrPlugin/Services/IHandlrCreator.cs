using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace NoSuchCompany.ReSharper.MediatrPlugin.Actions
{
    public interface IHandlrCreator
    {
        IClassDeclaration CreateHandlrFor(IIdentifier identifier);
    }
}