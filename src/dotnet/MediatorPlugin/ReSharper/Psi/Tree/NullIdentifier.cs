using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;
using ReSharper.MediatorPlugin.Diagnostics;

namespace ReSharper.MediatorPlugin.ReSharper.Psi.Tree;

internal sealed class NullIdentifier : IIdentifier
{
    private readonly IPsiModule _psiModule;

    public ITreeNode FirstChild { get; }

    public PsiLanguageType Language { get; }

    public ITreeNode LastChild { get; }

    public string Name => "Null Identifier";

    public ITreeNode NextSibling { get; }

    public NodeType NodeType { get; }

    public ITreeNode Parent { get; }

    public NodeUserData PersistentUserData { get; }

    public ITreeNode PrevSibling { get; }

    public NodeUserData UserData { get; }

    public NullIdentifier(IPsiModule psiModule)
    {
        Guard.ThrowIfIsNull(psiModule, nameof(psiModule));

        _psiModule = psiModule;
    }

    public bool Contains(ITreeNode other)
    {
        throw new NotImplementedException();
    }

    public ITreeNode FindNodeAt(TreeTextRange treeRange)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<ITreeNode> FindNodesAt(TreeOffset treeOffset)
    {
        throw new NotImplementedException();
    }

    public ITreeNode FindTokenAt(TreeOffset treeTextOffset)
    {
        throw new NotImplementedException();
    }

    public TTreeNode GetContainingNode<TTreeNode>(bool returnThis = false) where TTreeNode : ITreeNode
    {
        throw new NotImplementedException();
    }

    public ReferenceCollection GetFirstClassReferences()
    {
        throw new NotImplementedException();
    }

    public DocumentRange GetNavigationRange()
    {
        throw new NotImplementedException();
    }

    public IPsiModule GetPsiModule()
    {
        return _psiModule;
    }

    public IPsiServices GetPsiServices()
    {
        throw new NotImplementedException();
    }

    public IPsiSourceFile GetSourceFile()
    {
        throw new NotImplementedException();
    }

    public StringBuilder GetText(StringBuilder to)
    {
        throw new NotImplementedException();
    }

    public string GetText()
    {
        throw new NotImplementedException();
    }

    public IBuffer GetTextAsBuffer()
    {
        throw new NotImplementedException();
    }

    public int GetTextLength()
    {
        throw new NotImplementedException();
    }

    public TreeOffset GetTreeStartOffset()
    {
        throw new NotImplementedException();
    }

    public bool IsFiltered()
    {
        throw new NotImplementedException();
    }

    public bool IsPhysical()
    {
        throw new NotImplementedException();
    }

    public bool IsValid()
    {
        throw new NotImplementedException();
    }

    public void ProcessDescendantsForResolve(IRecursiveElementProcessor processor)
    {
        throw new NotImplementedException();
    }
}