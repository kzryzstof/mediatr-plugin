using System;
using JetBrains.TextControl;

namespace ReSharper.MediatorPlugin;

internal static class DefaultActions
{
    public static readonly Action<ITextControl> Empty = _ => { };
}