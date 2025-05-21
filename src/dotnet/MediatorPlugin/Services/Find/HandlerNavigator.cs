using System.Collections.Generic;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Diagnostics;

namespace ReSharper.MediatorPlugin.Services.Find;

internal sealed class HandlerNavigator : IHandlerNavigator
{
	private readonly IMediator _mediator;

	public HandlerNavigator
	(
		IMediator mediator
	)
	{
		Guard.ThrowIfIsNull(mediator, nameof(mediator));

		_mediator = mediator;
	}
		
	public bool IsRequest
	(
		IIdentifier identifier
	)
	{
		Guard.ThrowIfIsNull(identifier, nameof(identifier));

		return _mediator.IsRequest(identifier);
	}
		
	public IEnumerable<IDeclaredElement> GetHandlers
	(
		IIdentifier selectedIdentifier
	)
	{
		Guard.ThrowIfIsNull(selectedIdentifier, nameof(selectedIdentifier));

		Logger.Instance.Log(LoggingLevel.INFO, $"Looking for a possible MediatR handler that is using the type '{selectedIdentifier.Name}'");

		return _mediator.FindHandlers(selectedIdentifier);
	}
}