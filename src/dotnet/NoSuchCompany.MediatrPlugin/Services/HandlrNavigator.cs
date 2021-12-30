// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 30/12/2021 @ 10:22
// Last author: Christophe Commeyne
// ==========================================================================

using System;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Navigation.NavigationExtensions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Tree;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services
{
    #region Class

    internal sealed class HandlrNavigator : IHandlrNavigator
    {
        #region Constants

        private readonly IMediatR _mediatR;

        #endregion

        #region Constructors

        public HandlrNavigator(IMediatR mediatR)
        {
            Guard.ThrowIfIsNull(mediatR, nameof(mediatR));

            _mediatR = mediatR;
        }

        #endregion

        #region Public Methods

        public bool IsRequest(IIdentifier identifier)
        {
            Guard.ThrowIfIsNull(identifier, nameof(identifier));

            return _mediatR.IsRequest(identifier);
        }

        public void Navigate(IIdentifier request)
        {
            Guard.ThrowIfIsNull(request, nameof(request));

            GoToHandler(request);
        }

        #endregion

        #region Private Methods

        private (bool fileFound, ICSharpFile? csharpFile) FindCSharpFile(IDeclaredElement typeElement)
        {
            (bool fileFound, ICSharpFile? csharpFile) = typeElement.FindCSharpFile();

            if (!fileFound)
                Logger.Instance.Log(LoggingLevel.WARN, "The C# source file of the MediatR handler could not be found.");
            else
                Logger.Instance.Log(LoggingLevel.WARN, $"The C# source file of the MediatR handler has been found: '{csharpFile!.GetSourceFile()!.DisplayName}'");

            return (fileFound, csharpFile);
        }

        private (bool handlerFound, ITypeElement? typeElement) FindHandler(IIdentifier selectedIdentifier)
        {
            ITypeElement? mediatrHandlerTypeElement = _mediatR.FindHandler(selectedIdentifier);

            if (mediatrHandlerTypeElement is null)
                Logger.Instance.Log(LoggingLevel.WARN, $"No MediatR handler using the type '{selectedIdentifier.Name}' has been found.");
            else
                Logger.Instance.Log(LoggingLevel.WARN, $"A MediatR handler using the type '{selectedIdentifier.Name}' has been found: '{mediatrHandlerTypeElement.GetClrName().FullName}'");

            return (mediatrHandlerTypeElement is not null, mediatrHandlerTypeElement);
        }

        private (bool nodeFound, ITreeNode? treeNode) FindTreeNode(ITypeElement typeElement, ICSharpFile csharpFile)
        {
            ITreeNode? treeNode = csharpFile.GetTreeNode(typeElement.GetFullname());

            if (treeNode is null)
                Logger.Instance.Log(LoggingLevel.WARN, $"The tree node for the type '{typeElement.ShortName}' could not be found in the file '{csharpFile.GetSourceFile()!.DisplayName}'.");
            else
                Logger.Instance.Log(LoggingLevel.WARN, $"The tree node for the type '{typeElement.ShortName}' has been found in the file '{csharpFile.GetSourceFile()!.DisplayName}'.");

            return (treeNode is not null, treeNode);
        }

        private void GoToHandler(IIdentifier selectedIdentifier)
        {
            try
            {
                Logger.Instance.Log(LoggingLevel.WARN, $"Looking for a possible MediatR handler that is using the type '{selectedIdentifier.Name}'");

                //  Finds the MediatR handler for the selected request.
                (bool handlerTypeFound, ITypeElement? mediatrHandlerTypeElement) = FindHandler(selectedIdentifier);

                if (!handlerTypeFound)
                    return;

                //  Finds the C# file where the MediatR handler is stored.
                (bool fileFound, ICSharpFile? csharpFile) = FindCSharpFile(mediatrHandlerTypeElement!);

                if (!fileFound)
                    return;

                //  Finds the tree node of the handler in that file.
                (bool nodeFound, ITreeNode? treeNode) = FindTreeNode(mediatrHandlerTypeElement!, csharpFile!);

                if (!nodeFound)
                    return;

                //  Go to the file!
                NavigateToFile(treeNode!);
            }
            catch (Exception unhandledException)
            {
                Logger.Instance.Log(LoggingLevel.ERROR, unhandledException.ToString());
            }
        }

        private void NavigateToFile(ITreeNode treeNode)
        {
            treeNode.NavigateToTreeNode(true);
        }

        #endregion
    }

    #endregion
}