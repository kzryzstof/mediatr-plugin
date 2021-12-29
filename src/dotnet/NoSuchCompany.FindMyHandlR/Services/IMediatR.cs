// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 27/12/2021 @ 20:00
// Last author: Christophe Commeyne
// ==========================================================================

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services
{
    #region Interface

    public interface IMediatR
    {
        #region Methods

        ITypeElement? FindHandler(ISolution solution, IIdentifier identifier);

        bool IsRequest(IIdentifier identifier);

        #endregion
    }

    #endregion
}