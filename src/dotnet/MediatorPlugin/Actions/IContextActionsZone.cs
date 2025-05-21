using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReSharper.MediatorPlugin.Actions;

[ZoneDefinition]
public interface IContextActionsZone : IZone,
    IRequire<ILanguageCSharpZone>
{
}