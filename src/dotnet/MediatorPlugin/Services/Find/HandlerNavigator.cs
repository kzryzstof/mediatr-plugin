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
		_mediator = mediator;
	}
		
	public bool IsRequest
	(
		IIdentifier identifier
	)
	{
		return _mediator.IsSupported(identifier);
	}
		
	public IEnumerable<IDeclaredElement> GetHandlers
	(
		IIdentifier selectedIdentifier
	)
	{
		Logger.Instance.Log
		(
			LoggingLevel.INFO,
			$"Looking for a possible MediatR handler that is using the type '{selectedIdentifier.Name}'"
		);

		return _mediator.FindHandlers(selectedIdentifier);
	}
}