using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.ReSharper.Psi.Tree;

namespace ReSharper.MediatorPlugin.Services.Libraries;

internal abstract class Library : ILibrary
{
    private static readonly List<ITypeElement> EmptyTypeElements = new List<ITypeElement>();
    
    private readonly string _moduleName;
    
    private readonly string _requestClrName;
    private readonly string _notificationClrName;

    private readonly string _requestHandlerClrName;
    private readonly string _notificationHandlerClrName;

    private ITypeElement? _requestTypeElement;
    private ITypeElement? _notificationTypeElement;

    protected Library
    (
        string moduleName,
        string requestClrName,
        string notificationClrName,
        string requestHandlerClrName,
        string notificationHandlerClrName
    )
    {
        _moduleName = moduleName;

        _requestClrName = requestClrName;
        _notificationClrName = notificationClrName;
        
        _requestHandlerClrName = requestHandlerClrName;
        _notificationHandlerClrName = notificationHandlerClrName;
    }

    public IEnumerable<ITypeElement> FindHandlers
    (
        IIdentifier identifier
    )
    {
        InitializeTypeElementsIfNeeded(identifier);
        
        if (IsRequest(identifier))
        {
            return identifier.FindHandlers
            (
                _requestTypeElement
            );
        }
        
        if (IsNotification(identifier))
        {
            return identifier.FindHandlers
            (
                _notificationTypeElement
            );
        }

        return EmptyTypeElements;
    }
    
    public bool IsSupported
    (
        IIdentifier identifier
    )
    {
        return IsRequest(identifier) || IsNotification(identifier);
    }

    public abstract IClassLikeDeclaration CreateHandlrFor
    (
        IIdentifier identifier
    );
    
    private void InitializeTypeElementsIfNeeded
    (
        IIdentifier identifier
    )
    {
        if (_requestTypeElement is not null || _notificationTypeElement is not null)
            return;
        
        //  Need to get the PSI 
        IPsiServices psiServices = identifier.GetPsiServices();
        
        var psiModules = psiServices.GetComponent<IPsiModules>();
        IAssemblyPsiModule? mediatrPsiModule = psiModules.GetAssemblyModules().FirstOrDefault(psiModule => psiModule.Name == _moduleName);

        if (mediatrPsiModule is null)
            return;
        
        ISymbolScope symbolScope = psiServices.Symbols.GetSymbolScope(mediatrPsiModule, true, false);
        _requestTypeElement = symbolScope.GetTypeElementByCLRName(_requestHandlerClrName);
        _notificationTypeElement = symbolScope.GetTypeElementByCLRName(_notificationHandlerClrName);
    }
    
    private bool IsRequest
    (
        IIdentifier identifier
    )
    {
        return identifier.IsRequestTypeSupported
        (
            _moduleName,
            _requestClrName
        );
    }

    private bool IsNotification
    (
        IIdentifier identifier
    )
    {
        return identifier.IsRequestTypeSupported
        (
            _moduleName,
            _notificationClrName
        );
    }
}