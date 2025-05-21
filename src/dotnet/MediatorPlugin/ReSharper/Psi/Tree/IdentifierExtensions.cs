using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Diagnostics;

namespace ReSharper.MediatorPlugin.ReSharper.Psi.Tree;

internal static class IdentifierExtensions
{
    public static IDeclaredType ToDeclaredType(this IIdentifier identifier)
    {
        Guard.ThrowIfIsNull(identifier, nameof(identifier));

        IType type = CSharpTypeFactory.CreateType(identifier.Name, identifier);

        return (IDeclaredType) type;
    }
}