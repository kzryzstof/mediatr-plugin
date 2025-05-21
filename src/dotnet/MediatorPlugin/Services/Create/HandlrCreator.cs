using System.Linq;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Navigation.Descriptors;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.ReSharper.Psi.Tree;

namespace ReSharper.MediatorPlugin.Services.Create;

internal sealed class HandlrCreator : IHandlrCreator
{
    private const string HandlerTypePostfix = "Handler";
    private const string RequestHandlerInterfaceName = "IRequestHandler";

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
        string handlerTypeName = requestTypeName + HandlerTypePostfix;

        var elementFactory = CSharpElementFactory.GetInstance(identifier);

        (string classFormat, string methodFormat) = GetFormats(hasReturnType);

        var handlerClassDeclaration = (IClassDeclaration)elementFactory.CreateTypeMemberDeclaration
        (
            classFormat,
            handlerTypeName,
            RequestHandlerInterfaceName,
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
        //  TODO: extract this somewhere
        const string QueryHandlerFormat = @"class $0 : $1<$2, $3> {}";
        const string QueryStubHandleMethodFormat = @"Task<$0> Handle($1 request, CancellationToken cancellationToken) {throw new NotImplementedException();}";

        const string CommandHandlerFormat = @"class $0 : $1<$2> {}";
        const string CommandStubHandleMethodFormat = @"public Task Handle($1 request, CancellationToken cancellationToken) {throw new NotImplementedException();}";

        return isQuery ? (classFormat: QueryHandlerFormat, methodFormat: QueryStubHandleMethodFormat) : (classFormat: CommandHandlerFormat, methodFormat: CommandStubHandleMethodFormat);
    }

    private static IType? GetMediatrQueryReturnType
    (
        ITypeElement requestType
    )
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