// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 30/12/2021 @ 10:32
// ==========================================================================

using JetBrains.ReSharper.Psi.Tree;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services
{
    public interface IHandlrNavigator
    {
        bool IsRequest(IIdentifier identifier);

        void Navigate(IIdentifier request);
    }
}