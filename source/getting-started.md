---
_disableBreadcrumb: true
jr.disableMetadata: true
jr.disableLeftMenu: true
jr.disableRightMenu: true
uid: getting-started
title: Getting Started
---

# Getting Started

This article walks us through the most essential parts to know when working with NUKE.

## Prerequisites

Generally, we don't need to install anything to use NUKE. It is designed to integrate natively with IDEs such as VisualStudio, Rider or VSCode. However, for a better user experience, it is recommended to:

- Install the global tool via `dotnet tool install -g Nuke.GlobalTool`
- Install one of the IDE extensions for [ReSharper](https://resharper-plugins.jetbrains.com/packages/ReSharper.Nuke/) or [Rider](https://plugins.jetbrains.com/plugin/10803-nuke-support) (VisualStudio and VSCode extensions are work in progress)

## Build Setup

To setup our project with NUKE, we can execute the global tool, or download and invoke a PowerShell/Bash script:

```powershell
# Global Tool
nuke

# PowerShell
powershell -Command iwr https://nuke.build/powershell -OutFile setup.ps1
powershell -ExecutionPolicy ByPass -File ./setup.ps1

# Bash
curl -Lsfo setup.sh https://nuke.build/bash
chmod +x setup.sh; ./setup.sh
```

_Note: the global tool is in fact just a wrapper for the script invocations._

### Required information

To setup our solution, we need to provide the following information:

- **Solution file** selection (if multiple exist)
- **Build project:**
    - **Platform:** .NET Framework/Mono or .NET Core
    - **Format:** SDK-based or legacy
    - **Directory** (default: _./build_)
    - **Name** (default: _.build_)
- **Version of NUKE** framework (default: current latest)
- **Version of NuGet** executable (default: always latest)

It is strongly recommended to chose **.NET Core** as the platform. This solely indicates that the build project is built with .NET Core. You'll still be able to build solutions that require the full .NET Framework.

Whether we chose .NET Core or .NET Framework/Mono as the build project platform will also affect the bootstrapping.

### Effective changes

During execution, the following changes will be applied:

- Generate a _.nuke_ configuration file in the root directory, which references the chosen solution file
- Generate a [_build.ps1_](https://raw.githubusercontent.com/nuke-build/nuke/master/bootstrapping/build.ps1) and [_build.sh_](https://raw.githubusercontent.com/nuke-build/nuke/master/bootstrapping/build.sh) in the current directory
- Copy templates for the build project and minimal build file ([.NET Framework/Mono](https://raw.githubusercontent.com/nuke-build/nuke/master/bootstrapping/Build.netfx.cs) or [.NET Core](https://raw.githubusercontent.com/nuke-build/nuke/master/bootstrapping/Build.netcore.cs))
- Add build project to the solution file (without build configuration)

_Note: for general awareness, we recommend to review the applied changes using `git diff` or similar tools._ 

## Build Invocation

In order to invoke NUKE, we can use the global tool or one of the bootstrapping scripts that corresponds to our operating-system:

```powershell
# Global Tool (anywhere below .nuke file)
nuke [parameters]

# Windows (via build.cmd)
build [parameters]

# PowerShell
./build.ps1 [parameters]

# Bash
./build.sh [parameters]
```

_Note: again, the global tool is in fact just a wrapper for the script invocations._

### Bootstrapping

The bootstrapping scripts _build.ps1_  and _build.sh_ are taking care of executing our build project. Lets have a closer look how this is actually accomplished when choosing .NET Core and .NET Framework/Mono as the platform.

For **.NET Core**, the script will perform the following steps:

1. Check _global.json_ for a [.NET Core tools version](https://docs.microsoft.com/en-us/dotnet/core/tools/global-json)
2. If the global installation of .NET Core matches the version specified in (1) or no version was specified, use the global installation
3. Otherwise, install local version to temp directory:
    1. If no version was specified, download the latest version
    2. If version did not match, download the expected version
4. Compile and execute the build project

For **.NET Framework/Mono**, the script will perform the following steps:

1. Install the specified NuGet executable version into the temp directory
2. If NuGet version is _latest_, try to update
3. Install and execute the _Nuke.MSBuildLocator_, which determines the MSBuild executable
4. Compile and execute the build project

### Argument Specification

Build arguments can simply be passed to the [build invocation](#build-invocation). Below, some of the predefined arguments are explained:

```
build [targets] [-configuration <value>] [-skip [targets]] [...]
```

- `target`: defines the target(s) to be executed; multiple targets are separated by plus sign (i.e., `compile+pack`); if no target is defined, the _default_ will be executed
- `-configuration <value>`: defines the configuration to build. Default is _debug_
- `-verbosity <value>`: supported values are `quiet`, `minimal`, `normal` and `verbose`
- `-skip [targets]`: if no target is defined, only the explicit stated targets will be executed; multiple targets are separated by plus sign (i.e, `-skip clean+push`)
- `-graph`: will generate a HTML view of target dependencies
- `-help`: will show further information about available targets and parameters

NUKE also provides a convenient approach to [declare additional parameters](#parameter-declaration).

## Build Authoring

Builds are written as simple console applications. Targets are implemented as _expression-bodied properties_ and the default target is defined with the `Main` method.

```c#
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.MyTarget);

    Target MyTarget => _ => _
        // other fluent calls
        .Executes(() =>
        {
            Console.WriteLine("Hello from NUKE!");
        });
}
```

The fluent syntax allows to set further target specific options:

- `Description("text")`: description shown in help text
- `DependsOn(SymbolTarget)`: symbolic dependency that is executed before this target
- `DependsOn("NamedTarget")`: named dependency that is - if existent - executed before this
- `Requires(() => Field)`: field that must have been initialized prior to build execution; usually used in combination with `[Parameter]` attribute
- `Requires(() => condition)`: condition that is checked prior to build execution
- `OnlyWhen(() => condition)`: condition that tells whether the target should be executed or skipped
- `WhenSkipped(DependencyBehavior.Execute/Skip)`: whether dependency should be executed or skipped if this target is skipped

### Predefined properties

The `NukeBuild` base class provides predefined properties according to [established best practices](https://gist.github.com/davidfowl/ed7564297c61fe9ab814):

- `SkippedTargets`: targets that are skipped via the `-skip` parameter
- `InvokedTargets`: targets that are directly invoked from command-line
- `ExecutingTargets`: targets that are part of the execution list
- `Host`: build execution host (i.e., Console, Jenkins, TeamServices, TeamCity, ...)
- `IsLocalBuild`/`IsServerBuild`: flag that indicates whether the build is running locally (console host) or on a server (CI host)
- `Configuration`: either `Debug` for local builds or `Release` for server builds
- `RootDirectory`: directory where the `.nuke` file is located; usually the repository root
- `SolutionFile`/`SolutionDirectory`: reference to the solution file/directory defined via `.nuke`
- `TemporaryDirectory`: temporary directory at `/.tmp`
- `OutputDirectory`: output directory at `/output`
- `ArtifactsDirectory`: artifacts directory at `/artifacts`
- `SourceDirectory`: source directory at either `/src` or `/source`

### CLT Wrappers

NUKE ships with a lot of wrapper APIs for command-line tools like _MSBuild_, _NuGet_, _xUnit.net_  or _OpenCover_. These wrapper APIs are generated from [JSON specification files](https://github.com/nuke-build/nuke/tree/develop/build/specifications), which can either be written manually, e.g. for smaller internal company tools, or converted from other sources like we do for the [Docker](https://github.com/nuke-build/docker/) and [Azure](https://github.com/nuke-build/azure/) addons. Generating code from specifications allows to provide a rich and consistent API with minimal effort.

A call like `msbuild.exe /nologo /targets:Restore;Build /p:configuration=Release /maxCpuCount:2` can easily be constructed using the `MSBuildTasks`:

```csharp
// using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

MSBuild(s => s
    .SetSolutionFile(SolutionFile)
    .EnableNoLogo()
    .SetTargets("Restore", "Build")
    .SetConfiguration("Release")
    .SetMaxCpuCount(2));
```

The generated types implement the following features:

- **Tool path resolution:** whenever possible, the tool path will be set automatically:
    - **Package references:** referencing a NuGet package in the build project, either via [`PackageReference`](https://docs.microsoft.com/en-us/nuget/consume-packages/package-references-in-project-files) or [packages.config](https://docs.microsoft.com/en-us/nuget/reference/packages-config), allows to resolve the executable path from the installed package.
    - **PATH variable:** using the tools `where` (Unix) and `which` (Windows) the required executable is attempted to be resolved from the `PATH` environment variable.
    - **Environment variables:** for any task class, the environment variable `[TASKNAME]_EXE` can be used to provide the required executable. This takes precedence over other resolution methods.
- **Argument construction:** arguments that must be passed can be constructed using a fluent API. Every call creates a new instance, thus allows easy composition. The fluent API includes the following methods:
    - Setting and resetting an argument value (`SetValue`, `ResetValue`).
    - Enabling, disabling and toggling of boolean flags (`EnableFlag`, `DisableFlag`, `ToggleFlag`).
    - Setting, adding, removing and clearing of collection-like arguments (`SetItems`, `AddItems`, `RemoveItems`, `ClearItems`).
    - Adding and removing of dictionary and multi-dictionary arguments (`AddKeyValuePair`, `RemoveKey`)
- **Process invocation:** processes are invoked with the specified working directory, tool path, arguments and environment variables. The process is awaited to be exited; if specified, with a timeout. Afterwards, the exit code is asserted to be zero. 
- **Output capturing:** the process standard and error output is captured in a collection and returned from every task alias.
- **Documentation:** task aliases and arguments include the same xml summaries as described in the individual official tools documentation.

If necessary, extension points can be used to implement:

- **Return value conversion:** the process arguments and output collection are passed to a custom implemented method that creates a proper typed return value.
- **Log level detection:** for each output entry, a custom method is called to determine its log level (error, warning, info, trace.
- **Exit code validation:** the default implementation that asserts a zero exit code can be overriden.
- **Pre/post-processing:** methods are called before and after process invocation. The pre-processing also allows for modifications of the arguments. 

### Parameter Declaration

- Separators
- Lisp-casing
- Single/double dash
- Supported types
- Typo detection

## Common Build Steps

In the following sections, we will illustrate some of the most common build steps used in the .NET ecosystem. We will also explain how NUKE can help with _default settings_ to further standardize such targets.

### Clean, Restore, Compile

This example shows how to clean, restore and compile a solution while using [GitVersion](https://gitversion.readthedocs.io/) to calculate a version number based on our git commit history.

```csharp
[GitVersion] readonly GitVersion GitVersion;

Target Clean => _ => _
    .Executes(() =>
    {
        FileSystemTasks.DeleteDirectories(GlobDirectories(SourceDirectory, "**/bin", "**/obj"));
        FileSystemTasks.EnsureCleanDirectory(OutputDirectory);
    });
    
Target Restore => _ => _
    .DependsOn(Clean)
    .Executes(() =>
    {
        DotNetTasks.DotNetRestore(s => s
            .SetWorkingDirectory(SolutionDirectory)
            .SetProjectFile(SolutionFile));
            
        // Or using static imports and default settings:
        DotNetRestore(s => DefaultDotNetRestore);
    });
    
Target Compile => _ => _
    .DependsOn(Restore)
    .Executes(() =>
    {
        DotNetTasks.DotNetBuild(s => s
            .SetWorkingDirectory(SolutionDirectory)
            .SetProjectFile(SolutionFile)
            .EnableNoRestore()
            .SetConfiguration(Configuration)
            .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
            .SetFileVersion(GitVersion.GetNormalizedFileVersion())
            .SetInformationalVersion(GitVersion.InformationalVersion));
            
        // Or using static imports and default settings:
        DotNetBuild(s => DefaultDotNetBuild);
    });

```

### Test and Coverage

This example shows how to execute tests using [xUnit.net](https://xunit.github.io/) with optionally enabling [OpenCover](https://github.com/OpenCover/opencover) for code coverage reporting.

```csharp
Target TestAndCoverage => _ => _
    .DependsOn(Compile)
    .Executes(() =>
    {
        var assemblies = GlobFiles(SolutionDirectory, $"*/bin/{Configuration}/net4*/Nuke.*.Tests.dll").NotEmpty();
        var xunitSettings = new Xunit2Settings()
            .AddTargetAssemblies(assemblies)
            .AddResultReport(Xunit2ResultFormat.Xml, OutputDirectory / "tests.xml");

        if (EnvironmentInfo.IsWin)
        {
            var searchDirectories = xunitSettings.TargetAssemblyWithConfigs.Select(x => Path.GetDirectoryName(x.Key));
            
            OpenCoverTasks.OpenCover(s => s
                .SetOutput(OutputDirectory / "coverage.xml")
                .SetTargetSettings(xunitSettings)
                .SetSearchDirectories(searchDirectories)
                .SetWorkingDirectory(RootDirectory)
                .SetRegistration(RegistrationType.User)
                .SetTargetExitCodeOffset(targetExitCodeOffset: 0)
                .SetFilters(
                    "+[*]*",
                    "-[xunit.*]*",
                    "-[FluentAssertions.*]*")
                .SetExcludeByAttributes(
                    "*.Explicit*",
                    "*.Ignore*",
                    "System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute")
                .SetExcludeByFile(
                    "*/*.Generated.cs",
                    "*/*.Designer.cs",
                    "*/*.g.cs",
                    "*/*.g.i.cs")
        
            // Or using static imports and default settings:
            OpenCover(s => DefaultOpenCover
                .SetOutput(OutputDirectory / "coverage.xml")
                .SetTargetSettings(xunitSettings)
                .SetSearchDirectories(searchDirectories));
        }
        else
            XunitTasks.Xunit2(s => xunitSettings);
    });
```

### Packaging and Publish

This example shows how to pack and publish build artifacts. Via the `NuGet` switch we can control whether the packages should be pushed to [nuget.org](https://nuget.org/) or [myget.org](https://myget.org/). In any case, we require the `ApiKey` parameter to be specified. When pushing to nuget.org, we also require the build to be executed with the `Release` configuration. For repositories hosted at [GitHub](https://github.com/), we can also include a link to the changelog file.

```csharp
[Parameter("ApiKey for the specified source.")] readonly string ApiKey;

[GitRepository] readonly GitRepository GitRepository;

string Source => NuGet
    ? "https://api.nuget.org/v3/index.json"
    : "https://www.myget.org/F/myfeed/api/v2/package";
    
string Branch => GitRepository.Branch;
string ChangelogFile => RootDirectory / "CHANGELOG.md";

Target Pack => _ => _
    .DependsOn(Compile)
    .Executes(() =>
    {
        var changelogUrl = GitRepository.GetGitHubBrowseUrl(ChangelogFile, branch: "master");
    
        DotNetTasks.DotNetPack(s => s
            .SetPackageReleaseNotes(changelogUrl)
            .SetWorkingDirectory(SolutionDirectory)
            .SetProject(SolutionFile)
            .EnableNoBuild()
            .SetConfiguration(Configuration)
            .EnableIncludeSymbols()
            .SetOutputDirectory(OutputDirectory)
            .SetVersion(GitVersion.NuGetVersionV2));
            
        // Or using default settings:
        DotNetPack(s => DefaultDotNetPack
            .SetPackageReleaseNotes(changelogUrl));
    });

Target Publish => _ => _
    .DependsOn(Pack)
    .Requires(() => ApiKey)
    .Requires(() => !NuGet || Configuration.EqualsOrdinalIgnoreCase("release"))
    .Executes(() =>
    {
        GlobFiles(OutputDirectory, "*.nupkg").NotEmpty()
            .Where(x => !x.EndsWith(".symbols.nupkg"))
            .ForEach(x => DotNetNuGetPush(s => s
                .SetTargetPath(x)
                .SetSource(Source)
                .SetApiKey(ApiKey)));
```

<br/>
<br/>
**Happy building!**
