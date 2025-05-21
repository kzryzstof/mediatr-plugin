# MediatR Extension for Rider and ReSharper

[![Version](https://img.shields.io/jetbrains/plugin/v/ca.nosuchcompany.rider.plugins.mediatr)](https://plugins.jetbrains.com/plugin/18313-mediatr-extensions)
[![Version](https://img.shields.io/resharper/v/ca.nosuchcompany.mediatrplugin)](https://plugins.jetbrains.com/plugin/18347-mediatr-extensions)


**How to run on Windows**

- Open the solution `MediatorPlugin.sln` in Visual Studio;
- Make sure there is a valid NuGet feed configured (`https://api.nuget.org/v3/index.json`);
- Open a command prompt in Powershell;
- Run the command `./buildPlugin.ps1` (you might need to remove the Rider project if it causes the build to fail);
- Run the command `./runVisualStudio.ps1` (you might need to remove the Rider project if it causes the build to fail);

At this point, another instance of Visual Studio should have been started.
Go into ReSharper > Extension Manager and look for the MediatR extension plugin. It should be installed and have the version `9999.0`.

**How to run on macOS**

- Run the command `./gradlew :runIde` in a terminal window



