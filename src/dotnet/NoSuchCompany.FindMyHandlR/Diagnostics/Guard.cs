// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 27/12/2021 @ 21:13
// Last author: Christophe Commeyne
// ==========================================================================

using System;
using System.Runtime.CompilerServices;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics
{
    #region Class

    internal static class Guard
    {
        #region Public Methods

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

        #endregion
    }

    #endregion
}