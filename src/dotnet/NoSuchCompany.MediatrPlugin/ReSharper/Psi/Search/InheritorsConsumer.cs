// ==========================================================================
// Copyright (C) 2021 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 27/12/2021 @ 20:07
// Last author: Christophe Commeyne
// ==========================================================================

using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Search;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.ReSharper.Psi.Search
{
    internal sealed class InheritorsConsumer : IFindResultConsumer<ITypeElement>
    {
        #region Constants

        private const int MaxInheritors = 50;

        private readonly HashSet<ITypeElement> _elements = new();

        #endregion

        #region Properties

        public IEnumerable<ITypeElement> FoundElements => _elements;

        #endregion

        #region Public Methods

        public ITypeElement Build(FindResult result)
        {
            if (result is FindResultInheritedElement inheritedElement)
                return (ITypeElement)inheritedElement.DeclaredElement;

            return null;
        }

        public FindExecution Merge(ITypeElement data)
        {
            _elements.Add(data);
            return _elements.Count < MaxInheritors ? FindExecution.Continue : FindExecution.Stop;
        }

        #endregion
    }
}
