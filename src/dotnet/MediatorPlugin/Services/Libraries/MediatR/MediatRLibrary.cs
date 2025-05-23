using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Services.Create;
using ReSharper.MediatorPlugin.Services.Libraries;

namespace ReSharper.MediatorPlugin.Services.MediatR;

internal sealed class MediatRLibrary : Library
{
    private const string MediatrModuleName = "MediatR";
    
    private const string MediatrBaseRequestFullyQualifiedName = $"{MediatrModuleName}.IBaseRequest";
    private const string MediatrNotificationFullyQualifiedName = $"{MediatrModuleName}.INotification";

    private const string MediatrRequestHandlerFullyQualifiedName = $"{MediatrModuleName}.IRequestHandler`1";
    private const string MediatrNotificationHandlerFullyQualifiedName = $"{MediatrModuleName}.INotificationHandler`1";
    
    private readonly IHandlrCreator _handlrCreator;

    public MediatRLibrary() : base
    (
        MediatrModuleName,
        MediatrBaseRequestFullyQualifiedName,
        MediatrNotificationFullyQualifiedName,
        MediatrRequestHandlerFullyQualifiedName,
        MediatrNotificationHandlerFullyQualifiedName
    )
    {
        _handlrCreator = new HandlrCreator();
    }
    
    public override IClassLikeDeclaration CreateHandlrFor
    (
        IIdentifier identifier
    )
    {
        return _handlrCreator.CreateHandlrFor(identifier);
    } 
}