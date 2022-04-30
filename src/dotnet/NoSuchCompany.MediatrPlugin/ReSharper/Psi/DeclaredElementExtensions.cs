// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 28/12/2021 @ 09:15
// ==========================================================================

using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.Util;
using JetBrains.Util.DataStructures;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi
{
    internal static class DeclaredElementExtensions
    {
        public static (bool found, ICSharpFile? csharpFile) FindCSharpFile(this IDeclaredElement declaredElement)
        {
            HybridCollection<IPsiSourceFile> sourceFiles = declaredElement.GetSourceFiles();

            var sourceFile = sourceFiles.First() as IPsiProjectFile;

            if (sourceFile is null)
                return (false, null);

            var csharpFile = sourceFile
                .GetPsiFiles<CSharpLanguage>()
                .SafeOfType<ICSharpFile>()
                .SingleOrDefault();

            return (csharpFile is not null, csharpFile);
        }
    }
}