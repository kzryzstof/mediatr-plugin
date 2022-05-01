// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 28/12/2021 @ 09:05
// ==========================================================================

using JetBrains.ReSharper.Psi;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi
{
    internal static class TypeElementExtensions
    {
        public static string GetFullname(this ITypeElement typeElement)
        {
            return $"{typeElement.GetContainingNamespace().QualifiedName}.{typeElement.ShortName}";
        }
    }
}