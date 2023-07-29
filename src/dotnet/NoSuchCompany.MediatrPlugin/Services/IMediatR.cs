// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 27/12/2021 @ 20:00
// ==========================================================================

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Services
{
    public interface IMediatR
    {
        ITypeElement? FindHandler(IIdentifier identifier);

        bool IsRequest(IIdentifier identifier);
        
        IClassLikeDeclaration CreateHandlrFor(IClassLikeDeclaration requestTypeDeclaration);
    }
}