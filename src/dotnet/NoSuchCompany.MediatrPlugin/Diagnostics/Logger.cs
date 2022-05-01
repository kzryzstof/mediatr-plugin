// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 27/12/2021 @ 19:39
// ==========================================================================

using JetBrains.Diagnostics;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Actions;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics
{
    internal static class Logger
    {
        public static readonly ILog Instance = Log.GetLog<GoToHandlrContextAction>();
    }
}