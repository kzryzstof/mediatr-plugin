// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 27/12/2021 @ 20:32
// Last author: Christophe Commeyne
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Tree;

internal sealed class NullIdentifier : IIdentifier
{
    private readonly IPsiModule _psiModule;
    
    public NullIdentifier(IPsiModule psiModule)
    {
        Guard.ThrowIfIsNull(psiModule, nameof(psiModule));

        _psiModule = psiModule;

    }
    public IPsiServices GetPsiServices()
    {
        throw new NotImplementedException();
    }

    public IPsiModule GetPsiModule()
    {
        return _psiModule;
    }

    public IPsiSourceFile GetSourceFile()
    {
        throw new System.NotImplementedException();
    }

    public ReferenceCollection GetFirstClassReferences()
    {
        throw new System.NotImplementedException();
    }

    public void ProcessDescendantsForResolve(IRecursiveElementProcessor processor)
    {
        throw new System.NotImplementedException();
    }

    public TTreeNode GetContainingNode<TTreeNode>(bool returnThis = false) where TTreeNode : ITreeNode
    {
        throw new System.NotImplementedException();
    }

    public bool Contains(ITreeNode other)
    {
        throw new System.NotImplementedException();
    }

    public bool IsPhysical()
    {
        throw new System.NotImplementedException();
    }

    public bool IsValid()
    {
        throw new System.NotImplementedException();
    }

    public bool IsFiltered()
    {
        throw new System.NotImplementedException();
    }

    public DocumentRange GetNavigationRange()
    {
        throw new System.NotImplementedException();
    }

    public TreeOffset GetTreeStartOffset()
    {
        throw new System.NotImplementedException();
    }

    public int GetTextLength()
    {
        throw new System.NotImplementedException();
    }

    public StringBuilder GetText(StringBuilder to)
    {
        throw new System.NotImplementedException();
    }

    public IBuffer GetTextAsBuffer()
    {
        throw new System.NotImplementedException();
    }

    public string GetText()
    {
        throw new System.NotImplementedException();
    }

    public ITreeNode FindNodeAt(TreeTextRange treeRange)
    {
        throw new System.NotImplementedException();
    }

    public IReadOnlyCollection<ITreeNode> FindNodesAt(TreeOffset treeOffset)
    {
        throw new System.NotImplementedException();
    }

    public ITreeNode FindTokenAt(TreeOffset treeTextOffset)
    {
        throw new System.NotImplementedException();
    }

    public ITreeNode Parent { get; }

    public ITreeNode FirstChild { get; }

    public ITreeNode LastChild { get; }

    public ITreeNode NextSibling { get; }

    public ITreeNode PrevSibling { get; }

    public NodeType NodeType { get; }

    public PsiLanguageType Language { get; }

    public NodeUserData UserData { get; }

    public NodeUserData PersistentUserData { get; }

    public string Name => "Null Identifier";
}