using JetBrains.Diagnostics;
using ReSharper.MediatorPlugin.Actions;

namespace ReSharper.MediatorPlugin.Diagnostics
{
    internal static class Logger
    {
        public static readonly ILog Instance = Log.GetLog<GoToHandlrContextAction>();
    }
}