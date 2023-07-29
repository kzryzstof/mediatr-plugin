using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace NoSuchCompany.ReSharper.MediatrPlugin.Actions;

public interface IHandlrCreator
{
    IClassDeclaration CreateHandlrFor(IClassLikeDeclaration request);
}