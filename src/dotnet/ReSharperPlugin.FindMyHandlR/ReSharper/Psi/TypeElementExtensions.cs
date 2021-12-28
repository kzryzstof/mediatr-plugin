// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 28/12/2021 @ 09:05
// Last author: Christophe Commeyne
// ==========================================================================

using JetBrains.ReSharper.Psi;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi;

#region Class

internal static class TypeElementExtensions
{
    #region Public Methods

    public static string GetFullname(this ITypeElement typeElement)
    {
        return $"{typeElement.GetContainingNamespace().QualifiedName}.{typeElement.ShortName}";
    }

    #endregion
}

#endregion