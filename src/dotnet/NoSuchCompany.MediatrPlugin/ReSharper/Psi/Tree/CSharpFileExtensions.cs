using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Tree
{
    internal static class CSharpFileExtensions
    {
        public static ITreeNode? GetTreeNode(this ICSharpFile file, string typeName)
        {
            Guard.ThrowIfIsNull(typeName, nameof(typeName));

            IReadOnlyList<ICSharpNamespaceDeclaration> namespaceDeclarations = file
                .NamespaceDeclarationsEnumerable
                .Where(fileNamespace => typeName.StartsWith(fileNamespace.DeclaredName))
                .ToList();

            foreach (ICSharpNamespaceDeclaration? namespaceDeclaration in namespaceDeclarations)
            {
                ITreeNode? foundTreeNode = SearchTreeNode
                (
                    namespaceDeclaration.TypeDeclarationsEnumerable,
                    typeName
                );

                if (foundTreeNode is not null)
                    return foundTreeNode;
            }

            return null;
        }
        
        private static ITreeNode? SearchTreeNode(IEnumerable<ITypeDeclaration> typeDeclarations, string searchedTypeName)
        {
            foreach (ITypeDeclaration typeDeclaration in typeDeclarations)
            {
                if (IsSearchedType(typeDeclaration, searchedTypeName))
                    return typeDeclaration;

                return SearchTreeNode(typeDeclaration.TypeDeclarationsEnumerable, searchedTypeName);
            }

            return null;
        }

        private static bool IsSearchedType(ITypeDeclaration typeDeclaration, string typeName)
        {
            return string.Equals(typeDeclaration.CLRName, typeName, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}