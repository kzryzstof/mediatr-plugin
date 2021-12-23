using System;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Menu;

namespace ReSharperPlugin.SandboxPlugin.Actions;

#region Classes

[Action(ActionId, Id = 1)]
public class GoToHandlrAction : IExecutableAction, IInsertLast<NavigateMenu>
{
    #region Constants

    public const string ActionId = "go-to-handlr";

    #endregion

    private static readonly ILog GoToHandlrActionLog = Log.GetLog<GoToHandlrAction>();

    public GoToHandlrAction()
    {
        GoToHandlrActionLog.Log(LoggingLevel.INFO, "ctor");
    }
    
    #region Public Methods

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
        GoToHandlrActionLog.Log(LoggingLevel.INFO, "Execute");
    }

    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
        GoToHandlrActionLog.Log(LoggingLevel.INFO, "Update");
        return true;
    }

    #endregion
}

#endregion