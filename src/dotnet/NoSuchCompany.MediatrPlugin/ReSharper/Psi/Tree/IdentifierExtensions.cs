using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Tree
{
    internal static class IdentifierExtensions
    {
        public static IDeclaredType ToDeclaredType(this IIdentifier identifier)
        {
            Guard.ThrowIfIsNull(identifier, nameof(identifier));

            var type = CSharpTypeFactory.CreateType(identifier.Name, identifier);

            return (IDeclaredType) type;
        }
    }
}