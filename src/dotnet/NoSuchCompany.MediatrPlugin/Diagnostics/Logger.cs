using JetBrains.Diagnostics;
using NoSuchCompany.ReSharperPlugin.FindMyHandlR.Actions;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR.Diagnostics
{
    internal static class Logger
    {
        public static readonly ILog Instance = Log.GetLog<GoToHandlrContextAction>();
    }
}