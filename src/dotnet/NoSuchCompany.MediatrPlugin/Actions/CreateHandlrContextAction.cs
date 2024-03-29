﻿using System;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Tree;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Actions;

[ContextAction(
    Name = "Create HandlR",
    Description = "Creates MediatR handler matching the selected request",
    Group = "C#",
    Disabled = false,
    Priority = 2
)]
public sealed class CreateHandlrContextAction : ContextActionBase
{
    private readonly IMediatR _mediatR;

    private readonly IIdentifier _mediatrRequestIdentifier;
    private readonly LanguageIndependentContextActionDataProvider _dataProvider;

    public override string Text => "Create HandlR";

    internal CreateHandlrContextAction(
        LanguageIndependentContextActionDataProvider dataProvider,
        IMediatR mediatR)
    {
        Guard.ThrowIfIsNull(dataProvider, nameof(dataProvider));
        Guard.ThrowIfIsNull(mediatR, nameof(mediatR));

        Logger.Instance.Log(LoggingLevel.WARN, "Ctor called.");

        _mediatR = mediatR;
        _dataProvider = dataProvider;
        _mediatrRequestIdentifier = GetSelectedMediatrRequest(dataProvider);
    }

    public CreateHandlrContextAction(LanguageIndependentContextActionDataProvider dataProvider)
        : this(dataProvider, new MediatR())
    {
    }

    /// <summary>
    /// Finds out whether the selected type implements MediatR's 'IBaseRequest'.
    /// </summary>
    /// <param name="_"></param>
    /// <returns></returns>
    public override bool IsAvailable(IUserDataHolder _)
    {
        Logger.Instance.Log(LoggingLevel.WARN, "IsAvailable");

        return _mediatrRequestIdentifier is not NullIdentifier;
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        var requestTypeDeclaration = _dataProvider.GetSelectedElement<IClassLikeDeclaration>().NotNull();

        var classDeclaration = _mediatR.CreateHandlrFor(requestTypeDeclaration);

        using (WriteLockCookie.Create())
        {
            // todo: provide some customization where handler class should be placed at
            var handlerTypeDeclarationNode = ModificationUtil.AddChildAfter(requestTypeDeclaration, classDeclaration);

            return textControl =>
            {
                // todo: add customization here as well
                var methodDeclaration = handlerTypeDeclarationNode.MethodDeclarations.First();
                var statement = methodDeclaration.Body.Statements.FirstOrDefault(); // select 'not implemented' part
                var range = statement?.GetDocumentRange() ?? DocumentRange.InvalidRange;
                textControl.Selection.SetRange(range);
            };
        }
    }

    private IIdentifier GetSelectedMediatrRequest(IContextActionDataProvider dataProvider)
    {
        if (dataProvider.GetSelectedElement<ITreeNode>() is not IIdentifier someIdentifier)
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE,
                $"The selected tree node is not an instance of {nameof(IIdentifier)}.");
            return new NullIdentifier(dataProvider.PsiModule);
        }

        if (!_mediatR.IsRequest(someIdentifier))
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE, "The selected tree node is not mediatR request.");
            return new NullIdentifier(dataProvider.PsiModule);
        }

        return someIdentifier;
    }
}