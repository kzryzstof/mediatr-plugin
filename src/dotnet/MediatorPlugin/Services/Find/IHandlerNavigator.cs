using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.MediatorPlugin.Services.Find;

internal interface IHandlerNavigator
{
    bool IsRequest(IIdentifier identifier);

    IEnumerable<IDeclaredElement> GetHandlers
    (
        IIdentifier selectedIdentifier
    );
}