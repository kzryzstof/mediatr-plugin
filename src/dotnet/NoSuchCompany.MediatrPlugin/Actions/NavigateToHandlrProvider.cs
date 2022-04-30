using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Application.DataContext;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace NoSuchCompany.ReSharper.MediatrPlugin.Actions
{
    [ContextNavigationProvider]
    public class NavigateToCtorProvider : INavigateFromHereProvider
    {
        public IEnumerable<ContextNavigation> CreateWorkflow(IDataContext dataContext)
        {
            //var node = dataContext.GetSelectedTreeNode<ITreeNode>();
            //var typeDeclaration = node?.GetParentOfType<IClassDeclaration>();
            //var constructor = typeDeclaration?.ConstructorDeclarations.FirstNotNull();

            //if (constructor != null)
            //{
                yield return new ContextNavigation("Go to handlR", null, NavigationActionGroup.Other, () =>
                {
                    
                });
           // }
        }
    }
}
