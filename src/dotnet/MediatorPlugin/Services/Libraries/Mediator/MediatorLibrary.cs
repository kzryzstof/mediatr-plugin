using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Services.Libraries;

namespace ReSharper.MediatorPlugin.Services.MediatR;

internal sealed class MediatorLibrary : Library
{
    private const string MediatorModuleName = "Mediator";
    
    private const string MediatorBaseRequestFullyQualifiedName = $"{MediatorModuleName}.IBaseRequest";
    private const string MediatorNotificationFullyQualifiedName = $"{MediatorModuleName}.INotification";

    private const string MediatorRequestHandlerFullyQualifiedName = $"{MediatorModuleName}.IRequestHandler`1";
    private const string MediatorNotificationHandlerFullyQualifiedName = $"{MediatorModuleName}.INotificationHandler`1";
    
    public MediatorLibrary() : base
    (
        MediatorModuleName,
        MediatorBaseRequestFullyQualifiedName,
        MediatorNotificationFullyQualifiedName,
        MediatorRequestHandlerFullyQualifiedName,
        MediatorNotificationHandlerFullyQualifiedName
    )
    {
    }

    public override IClassLikeDeclaration CreateHandlrFor
    (
        IIdentifier identifier
    )
    {
        throw new System.NotImplementedException();
    }
}