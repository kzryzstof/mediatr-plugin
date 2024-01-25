using System;
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

            var namespaceName = GetLongNameFromFullyQualifiedName(typeName);
            var shortName = GetShortNameFromFullyQualifiedName(typeName);

            TreeNodeEnumerable<ICSharpNamespaceDeclaration> namespaceDeclarations = file.NamespaceDeclarationsEnumerable;

            var namespaceDeclaration = (from declaration in namespaceDeclarations
                where declaration.DeclaredName == namespaceName
                select declaration).FirstOrDefault();

            if (namespaceDeclaration is null)
                return null;

            TreeNodeEnumerable<ICSharpTypeDeclaration> typeDeclaration = namespaceDeclaration.TypeDeclarationsEnumerable;

            var resultList = (from node in typeDeclaration
                where node.DeclaredName == shortName
                select node).ToList();

            return resultList.FirstOrDefault();
        }

        private static string GetLongNameFromFullyQualifiedName(string fullQualifiedName)
        {
            var pos = fullQualifiedName.LastIndexOf(".", StringComparison.Ordinal) + 1;
            return pos > 0 ? fullQualifiedName[..(pos - 1)] : fullQualifiedName;
        }

        private static string GetShortNameFromFullyQualifiedName(string fullyQualifiedName)
        {
            var pos = fullyQualifiedName.LastIndexOf(".", StringComparison.Ordinal) + 1;
            return pos > 0 ? fullyQualifiedName[pos..] : fullyQualifiedName;
        }
    }
}