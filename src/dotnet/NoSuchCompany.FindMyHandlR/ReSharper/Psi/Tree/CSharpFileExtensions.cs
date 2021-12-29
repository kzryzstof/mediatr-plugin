// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 27/12/2021 @ 21:05
// Last author: Christophe Commeyne
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Tree
{
    #region Class

    internal static class CSharpFileExtensions
    {
        #region Public Methods

        public static ITreeNode? GetTreeNode(this ICSharpFile file, string typeName)
        {
            Guard.ThrowIfIsNull(typeName, nameof(typeName));

            string namespaceName = GetLongNameFromFullyQualifiedName(typeName);
            string shortName = GetShortNameFromFullyQualifiedName(typeName);

            TreeNodeEnumerable<ICSharpNamespaceDeclaration> namespaceDeclarations = file.NamespaceDeclarationsEnumerable;
        
            ICSharpNamespaceDeclaration? namespaceDeclaration = (from declaration in namespaceDeclarations
                                                                 where declaration.DeclaredName == namespaceName
                                                                 select declaration).FirstOrDefault();

            if (namespaceDeclaration is null)
                return null;

            TreeNodeEnumerable<ICSharpTypeDeclaration> typeDeclaration = namespaceDeclaration.TypeDeclarationsEnumerable;

            List<ICSharpTypeDeclaration> resultList = (from node in typeDeclaration
                                                       where node.DeclaredName == shortName
                                                       select node).ToList();

            return resultList.FirstOrDefault();
        }

        #endregion

        #region Private Methods

        private static string GetLongNameFromFullyQualifiedName(string fullQualifiedName)
        {
            int pos = fullQualifiedName.LastIndexOf(".", StringComparison.Ordinal) + 1;
            return pos > 0 ? fullQualifiedName[..(pos - 1)] : fullQualifiedName;
        }

        private static string GetShortNameFromFullyQualifiedName(string fullyQualifiedName)
        {
            int pos = fullyQualifiedName.LastIndexOf(".", StringComparison.Ordinal) + 1;
            return pos > 0 ? fullyQualifiedName[pos..] : fullyQualifiedName;
        }

        #endregion
    }

    #endregion
}