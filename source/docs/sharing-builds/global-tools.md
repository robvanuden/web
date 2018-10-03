---
_disableBreadcrumb: true
_disableAffix: true
jr.disableMetadata: false
jr.disableLeftMenu: false
jr.disableRightMenu: true
uid: sharing-builds-global-tools
title: Global Tools
---

# Global Tools

Due to its native integration with C# as a host language and existing IDEs, NUKE makes it easy to be introduced to the whole development team, so that everyone can manage the build. However, there are times when this is either not required or even unwanted. For these cases, a build can be compiled and published as [.NET Core Global Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools), which allows to **run a build without having a build project or bootstrapping scripts** in the repository. Typically, such repositories would follow an agreed folder structure and naming scheme. Only the `.nuke` file can still be of use, to mark the root directory. Alternatively, the `--root` parameter can be used, to provide this information ad-hoc.

### Usage

As a first step, the build project file needs to be extended with the [necessary information for global tool packaging](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create#setup-the-global-tool):

```xml
<PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.1</TargetFramework>
    <PackAsTool>true</PackAsTool>
    <ToolCommandName>build</ToolCommandName>
</PropertyGroup>
```

The `PackAsTool` property causes `dotnet` to pack the build project in such a way, that it can be executed on any platform that has the .NET Core SDK installed:

```powershell
$ dotnet tool install CustomGlobalTool --global

You can invoke the tool using the following command: build
Tool 'customglobaltool' (version '1.0.0.0') was successfully installed.
```

After installation, the build can be invoked with the command name defined in `ToolCommandName`. As per the example from above:

```powershell
$ build --root --configuration Release
```

> [!Note]
> One great advantage that NUKE implements, is that all **tool references will be automatically packed with the global tool**. That means that [package references](https://docs.microsoft.com/en-us/nuget/consume-packages/package-references-in-project-files) having a `./tools` folder, like `GitVersion` or `xunit.runner.console`, are automatically part of the global tool package. This makes the build self-contained.
