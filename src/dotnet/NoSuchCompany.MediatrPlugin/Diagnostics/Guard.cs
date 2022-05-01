// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 27/12/2021 @ 21:13
// ==========================================================================

using System;
using System.Runtime.CompilerServices;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics
{
    internal static class Guard
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfIsNull<TObjectType>
        (
            TObjectType instance,
            string instanceName
        ) where TObjectType : class
        {
            if (instance is null)
                throw new ArgumentNullException(instanceName, $"Parameter {instanceName} is null");
        }
    }
}