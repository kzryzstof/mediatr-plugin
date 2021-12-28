// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 27/12/2021 @ 21:07
// Last author: Christophe Commeyne
// ==========================================================================

using System;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.Navigation.NavigationExtensions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Tree;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Actions;

#region Class

[ContextAction
(
    Name = "Find my HandlR",
    Description = "Looks for the MediatR handler matching the selected request",
    Group = "C#",
    Disabled = false,
    Priority = 1
)]
public sealed class GoToHandlerAction : ContextActionBase
{
    #region Constants

    private readonly IMediatR _mediatR;

    private readonly IIdentifier _mediatrRequestIdentifier;

    #endregion

    #region Properties

    public override string Text => "Find my HandlR!";

    #endregion

    #region Constructors

    /// <param name="dataProvider"></param>
    /// <param name="mediatR"></param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if:
    /// - The <paramref name="dataProvider" /> instance is null.
    /// - The <paramref name="mediatR" /> instance is null.
    /// </exception>
    internal GoToHandlerAction(LanguageIndependentContextActionDataProvider dataProvider, IMediatR mediatR)
    {
        Guard.ThrowIfIsNull(dataProvider, nameof(dataProvider));
        Guard.ThrowIfIsNull(mediatR, nameof(mediatR));

        _mediatR = mediatR;
        _mediatrRequestIdentifier = GetSelectedMediatrRequest(dataProvider);
    }

    /// <param name="dataProvider"></param>
    public GoToHandlerAction(LanguageIndependentContextActionDataProvider dataProvider)
        : this(dataProvider, new MediatR())
    {
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Finds out whether the selected type implements MediatR's 'IBaseRequest'.
    /// </summary>
    /// <param name="_"></param>
    /// <returns></returns>
    public override bool IsAvailable(IUserDataHolder _)
    {
        return _mediatrRequestIdentifier is not NullIdentifier;
    }

    #endregion

    #region Protected Methods

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        try
        {
            Logger.Instance.Log(LoggingLevel.WARN, $"Looking for a possible MediatR handler that is using the type '{_mediatrRequestIdentifier.Name}'");

            ITypeElement? mediatrHandlerTypeElement = _mediatR.FindHandler(solution, _mediatrRequestIdentifier);

            if (mediatrHandlerTypeElement is null)
            {
                Logger.Instance.Log(LoggingLevel.WARN, $"No MediatR handler using the type '{_mediatrRequestIdentifier.Name}' has been found.");
                return DefaultActions.Empty;
            }

            Logger.Instance.Log(LoggingLevel.WARN, $"A MediatR handler using the type '{_mediatrRequestIdentifier.Name}' has been found: '{mediatrHandlerTypeElement.GetClrName().FullName}'");

            (bool found, ICSharpFile? csharpFile) = mediatrHandlerTypeElement.RetrieveCSharpFile();

            if (!found)
            {
                Logger.Instance.Log(LoggingLevel.WARN, "The C# source file of the MediatR handler could not be found.");
                return DefaultActions.Empty;
            }

            Logger.Instance.Log(LoggingLevel.WARN, $"The C# source file of the MediatR handler has been found: {csharpFile!.GetSourceFile()!.DisplayName}");

            ITreeNode? treeNode = csharpFile.GetTreeNode($"{mediatrHandlerTypeElement.GetContainingNamespace().QualifiedName}.{mediatrHandlerTypeElement.ShortName}");

            treeNode.NavigateToTreeNode(true);
        }
        catch (Exception unhandledException)
        {
            Logger.Instance.Log(LoggingLevel.ERROR, unhandledException.ToString());
        }

        return DefaultActions.Empty;
    }

    #endregion

    #region Private Methods

    private IIdentifier GetSelectedMediatrRequest(IContextActionDataProvider dataProvider)
    {
        if (dataProvider.GetSelectedElement<ITreeNode>() is not IIdentifier mediatrRequestIdentifier)
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE, $"The selected tree node is not an instance of {nameof(IIdentifier)}.");
            return new NullIdentifier(dataProvider.PsiModule);
        }

        if (!_mediatR.IsRequest(mediatrRequestIdentifier))
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE, "The selected tree node is not mediatR request.");
            return new NullIdentifier(dataProvider.PsiModule);
        }

        return mediatrRequestIdentifier;
    }

    #endregion
}

#endregion