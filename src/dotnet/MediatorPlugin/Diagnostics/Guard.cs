using System;
using System.Runtime.CompilerServices;

namespace ReSharper.MediatorPlugin.Diagnostics;

internal static class Guard
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfIsNull<TObjectType>
    (
        TObjectType instance,
        string instanceName
    ) where TObjectType : class
    {
        if (instance is null)
            throw new ArgumentNullException(instanceName, $"Parameter {instanceName} is null");
    }
}