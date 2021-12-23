using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

namespace NoSuchCompany.ReSharperPlugin.FindMyHandlR;

#region Interfaces

[ZoneDefinition]
// [ZoneDefinitionConfigurableFeature("Title", "Description", IsInProductSection: false)]
public interface ISandboxPluginZone : IPsiLanguageZone, IRequire<ILanguageCSharpZone>, IRequire<DaemonZone>
{
}

#endregion