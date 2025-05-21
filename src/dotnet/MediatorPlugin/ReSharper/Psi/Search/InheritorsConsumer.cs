using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Search;

namespace ReSharper.MediatorPlugin.ReSharper.Psi.Search;

internal sealed class InheritorsConsumer : IFindResultConsumer<ITypeElement>
{
    private const int MaxInheritors = 50;

    private readonly HashSet<ITypeElement> _elements = new();

    public IEnumerable<ITypeElement> FoundElements => _elements;

    public ITypeElement Build
    (
        FindResult result
    )
    {
        if (result is FindResultInheritedElement inheritedElement)
            return (ITypeElement) inheritedElement.DeclaredElement;

        return null;
    }

    public FindExecution Merge
    (
        ITypeElement data
    )
    {
        _elements.Add(data);
        return _elements.Count < MaxInheritors ? FindExecution.Continue : FindExecution.Stop;
    }
}