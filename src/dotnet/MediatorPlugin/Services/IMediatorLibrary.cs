using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.MediatorPlugin.Services;

internal interface IMediatorLibrary
{
    IEnumerable<ITypeElement> FindHandlers
    (
        IIdentifier identifier
    );

    bool IsSupported
    (
        IIdentifier identifier
    );
        
    IClassLikeDeclaration CreateHandlrFor
    (
        IIdentifier identifier
    );
}