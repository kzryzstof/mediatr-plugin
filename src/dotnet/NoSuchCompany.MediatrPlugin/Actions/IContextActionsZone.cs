using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Psi.CSharp;

namespace NoSuchCompany.ReSharper.MediatrPlugin.Actions;

[ZoneDefinition]
public interface IContextActionsZone : IZone,
    IRequire<ILanguageCSharpZone>
{
}