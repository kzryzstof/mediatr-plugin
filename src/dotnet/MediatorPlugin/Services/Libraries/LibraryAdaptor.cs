using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Services.MediatR;

namespace ReSharper.MediatorPlugin.Services.Libraries;

internal sealed class LibraryAdaptor : ILibraryAdaptor
{
    private readonly List<ILibrary> _mediatorLibrairies;
    
    public LibraryAdaptor()
    {
        _mediatorLibrairies = new List<ILibrary>
        {
            new MediatorLibrary(),
            new MediatRLibrary()
        };
    }

    public IEnumerable<ITypeElement> FindHandlers
    (
        IIdentifier identifier
    )
    {
        return _mediatorLibrairies.SelectMany(library => library.FindHandlers(identifier));
    }

    public bool IsSupported
    (
        IIdentifier identifier
    )
    {
        return _mediatorLibrairies.Any(library => library.IsSupported(identifier));
    }

    public IClassLikeDeclaration CreateHandlrFor
    (
        IIdentifier identifier
    )
    {
        return new MediatRLibrary().CreateHandlrFor(identifier);
    }
}