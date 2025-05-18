using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services
{
    public interface IMediator
    {
        ITypeElement? FindHandler(IIdentifier identifier);

        bool IsRequest(IIdentifier identifier);
        
        IClassLikeDeclaration CreateHandlrFor(IIdentifier identifier);
    }
}