using System.Linq;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Navigation.Descriptors;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace NoSuchCompany.ReSharper.MediatrPlugin.Actions;

public class HandlrCreator : IHandlrCreator
{
    private const string _handlerTypePostfix = "Handler";
    private const string _requestHandlerInterfaceName = "IRequestHandler";

    public IClassDeclaration CreateHandlrFor(IClassLikeDeclaration request)
    {
        // some unexpected behaviour might show up
        // when query class type parameters (T) not resolved at some level (i.e. not imported)
        // renders as : IRequestHandler<RequestType, IReadOnlyCollection<T>>

        var requestType = request.DeclaredElement.NotNull();
        
        var queryReturnType = GetMediatrQueryReturnType(requestType);
        var hasReturnType = queryReturnType is not null;

        var requestTypeName = requestType.ShortName;
        var handlerTypeName = requestTypeName + _handlerTypePostfix;

        var elementFactory = CSharpElementFactory.GetInstance(request);

        var (classFormat, methodFormat) = GetFormats(hasReturnType);
         
        var handlerClassDeclaration = (IClassDeclaration)elementFactory.CreateTypeMemberDeclaration(classFormat, handlerTypeName, _requestHandlerInterfaceName, requestTypeName, queryReturnType);
        handlerClassDeclaration.SetAccessRights(request.GetAccessRights());

        var handleMethod = (IClassMemberDeclaration)elementFactory.CreateTypeMemberDeclaration(methodFormat, queryReturnType, requestTypeName);
        handleMethod.SetAccessRights(AccessRights.PUBLIC);

        handlerClassDeclaration.AddClassMemberDeclaration(handleMethod);
        
        return handlerClassDeclaration;
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
        var requestBaseInterfaceWithGenericArg = searchDescriptor
            .Items
            .Select(x => OccurrenceUtil.GetDeclaredElement(x))
            .Where(x => x is IInterface { Module.Name: "MediatR" or "MediatR.Contracts" })
            .OfType<IInterface>()
            .FirstOrDefault(x => x.HasTypeParameters());
        
        var substitution = requestType
            .GetAncestorSubstitution(requestBaseInterfaceWithGenericArg)
            .FirstOrDefault(); // mediatr query has only one generic arg

        return substitution?.Domain.Select(typeParameter => substitution[typeParameter]).FirstOrDefault();
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
}