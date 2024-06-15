﻿using System.Linq;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Navigation.Descriptors;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Tree;

namespace NoSuchCompany.ReSharper.MediatrPlugin.Actions;

public class HandlrCreator : IHandlrCreator
{
    private const string _handlerTypePostfix = "Handler";
    private const string _requestHandlerInterfaceName = "IRequestHandler";

    public IClassDeclaration CreateHandlrFor(IIdentifier identifier)
    {
        // some unexpected behaviour might show up
        // when query class type parameters (T) not resolved at some level (i.e. not imported)
        // renders as : IRequestHandler<RequestType, IReadOnlyCollection<T>>
        
        IDeclaredType selectedDeclaredType = identifier.ToDeclaredType();
        ITypeElement requestType = selectedDeclaredType.GetTypeElement().NotNull();

        IType? queryReturnType = GetMediatrQueryReturnType(requestType);
        bool hasReturnType = queryReturnType is not null;

        string requestTypeName = requestType.ShortName;
        string handlerTypeName = requestTypeName + _handlerTypePostfix;

        var elementFactory = CSharpElementFactory.GetInstance(identifier);

        (string classFormat, string methodFormat) = GetFormats(hasReturnType);

        var handlerClassDeclaration = (IClassDeclaration)elementFactory.CreateTypeMemberDeclaration
        (
            classFormat,
            handlerTypeName,
            _requestHandlerInterfaceName,
            requestTypeName,
            queryReturnType
        );
        
        handlerClassDeclaration.SetAccessRights(AccessRights.INTERNAL);

        var handleMethod = (IClassMemberDeclaration)elementFactory.CreateTypeMemberDeclaration
        (
            methodFormat,
            queryReturnType,
            requestTypeName
        );
        
        handleMethod.SetAccessRights(AccessRights.PUBLIC);

        handlerClassDeclaration.AddClassMemberDeclaration(handleMethod);

        return handlerClassDeclaration;
    }

    private (string classFormat, string methodFormat) GetFormats(bool isQuery)
    {
        // todo: extract this somewhere
        const string queryHandlerFormat = @"class $0 : $1<$2, $3> {}";
        const string queryStubHandleMethodFormat = @"Task<$0> Handle($1 request, CancellationToken cancellationToken) {throw new NotImplementedException();}";

        const string commandHandlerFormat = @"class $0 : $1<$2> {}";
        const string commandStubHandleMethodFormat = @"public Task Handle($1 request, CancellationToken cancellationToken) {throw new NotImplementedException();}";

        return isQuery ? (queryHandlerFormat, queryStubHandleMethodFormat) : (commandHandlerFormat, commandStubHandleMethodFormat);
    }

    private static IType? GetMediatrQueryReturnType(ITypeElement requestType)
    {
        // search through base classes in case when there is some custom request base type
        var searchRequestBasesRequest = new SearchBasesRequest(requestType);

        // something wrong with SearchBasesDescriptor lifetime.
        // test runs always prompts
        // 'The SearchBasesDescriptor lifetime has been emergency terminated on the finalizer thread upon being lost by its owner.'  
        // todo: fix it
        var searchDescriptor = new SearchBasesDescriptor(searchRequestBasesRequest, null);
        searchDescriptor.Search();

        // find IRequest<T> if there is one
        // otherwise it is command
        IInterface? requestBaseInterfaceWithGenericArg = searchDescriptor
            .Items
            .Select(x => x.GetDeclaredElement())
            .Where(x => x is IInterface { Module.Name: "MediatR" or "MediatR.Contracts" })
            .OfType<IInterface>()
            .FirstOrDefault(x => x.HasTypeParameters());

        ISubstitution? substitution = requestType
            .GetAncestorSubstitution(requestBaseInterfaceWithGenericArg)
            .FirstOrDefault(); // mediatr query has only one generic arg

        return substitution?.Domain.Select(typeParameter => substitution[typeParameter]).FirstOrDefault();
    }
}