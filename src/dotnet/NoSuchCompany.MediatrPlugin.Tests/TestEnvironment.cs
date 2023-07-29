using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NoSuchCompany.ReSharper.MediatrPlugin.Actions;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace NoSuchCompany.MediatrPlugin.Tests;

[ZoneDefinition]
public class ContextActionsTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IContextActionsZone>
{
}

[ZoneMarker]
public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>, IRequire<ContextActionsTestEnvironmentZone>
{
}


[SetUpFixture]
public class ContextActionsTestsAssembly : ExtensionTestEnvironmentAssembly<ContextActionsTestEnvironmentZone>
{
}