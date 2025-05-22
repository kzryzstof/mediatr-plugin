using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Services.Create;

namespace ReSharper.MediatorPlugin.Services.MediatR;

internal sealed class MediatorLibrary : IMediatorLibrary
{
    private const string MediatrModuleName = "Mediator";
    
    private const string MediatrBaseRequestFullyQualifiedName = $"{MediatrModuleName}.IBaseRequest";
    private const string MediatrNotificationFullyQualifiedName = $"{MediatrModuleName}.INotification";

    private const string MediatrRequestHandlerFullyQualifiedName = $"{MediatrModuleName}.IRequestHandler<in TRequest, TResponse>";
    private const string MediatrNotificationHandlerFullyQualifiedName = $"{MediatrModuleName}.INotificationHandler<in TNotification>";
    
    private readonly IHandlrCreator _handlrCreator;

    public MediatorLibrary(): this(new HandlrCreator()) { }

    private MediatorLibrary
    (
        IHandlrCreator handlrCreator
    )
    {
        _handlrCreator = handlrCreator;
    }
    
    public IEnumerable<ITypeElement> FindHandlers
    (
        IIdentifier identifier
    )
    {
        if (IsMediatrRequest(identifier))
        {
            return identifier.FindHandlers
            (
                MediatrRequestHandlerFullyQualifiedName
            );
        }
        
        if (IsMediatrNotification(identifier))
        {
            return identifier.FindHandlers
            (
                MediatrNotificationHandlerFullyQualifiedName
            );
        }

        return [];
    }
    
    public bool IsSupported
    (
        IIdentifier identifier
    )
    {
        return IsMediatrRequest(identifier)
               || IsMediatrNotification(identifier);
    }

    public IClassLikeDeclaration CreateHandlrFor
    (
        IIdentifier identifier
    )
    {
        return _handlrCreator.CreateHandlrFor(identifier);
    } 
        
    private static bool IsMediatrRequest
    (
        IIdentifier identifier
    )
    {
        return identifier.IsRequestTypeSupported
        (
            MediatrModuleName,
            MediatrBaseRequestFullyQualifiedName
        );
    }

    private static bool IsMediatrNotification
    (
        IIdentifier identifier
    )
    {
        return identifier.IsRequestTypeSupported
        (
            MediatrModuleName,
            MediatrNotificationFullyQualifiedName
        );
    }
}