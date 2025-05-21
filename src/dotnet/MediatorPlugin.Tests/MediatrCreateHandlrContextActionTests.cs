using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReSharper.MediatorPlugin.Actions;

namespace NoSuchCompany.MediatrPlugin.Tests;

[TestPackages("MediatR/12.1.0")]
[TestReferences("System.Runtime")] // needed for IReadOnlyCollection type
public class MediatrCreateHandlrContextActionTests : CSharpContextActionExecuteTestBase<CreateHandlrContextAction>
{
    protected override string RelativeTestDataPath => nameof(MediatrCreateHandlrContextActionTests);

    protected override string ExtraPath => "";
    
    [Test, Order(0)] public void TestEnvironmentSetupCorrectly() => Assert.Pass();
    [Test] public void TestCommand() => DoNamedTest();
    [Test] public void TestSimpleQuery() => DoNamedTest();
    [Test] public void TestCustomBaseClassQuery() => DoNamedTest();
    [Test] public void TestCustomNestedBaseClassQuery() => DoNamedTest();
    [Test] public void TestCustomNestedBaseClassMultipleGenericQuery() => DoNamedTest();
}