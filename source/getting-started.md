---
_disableBreadcrumb: true
jr.disableMetadata: true
jr.disableLeftMenu: true
jr.disableRightMenu: true
uid: getting-started
title: Getting Started
---

# Getting Started

This article will walk you through the most essential things to know when writing builds with Nuke. For missing or incomplete information, you can either improve the documentation following the link on the right-hand side, or just ping us on [Slack](https://publicslack.com/slacks/nukebuildnet/invites/new).

## Build Setup

We prepared setup scripts for [PowerShell](https://nuke.build/powershell) and [Bash](https://nuke.build/bash) to help setting up the environment for you. During execution you'll be asked to provide the following information:

- Solution file selection (if multiple exist)
- Build project platform (.NET Framework/Mono, or .NET Core).
- Build project format (SDK-based or legacy). SDK-based can also build legacy formatted projects.
- Version of NUKE framework (default: current latest)
- Version of NuGet executable (default: always latest)
- Directory of the  build project (default: _./build_)
- Name for the build project (default: _.build_; this way it's the first item in the SolutionExplorer)

Note that the current directory of execution is also the location the build scripts will be generated.

```powershell
# PowerShell
powershell -Command iwr https://nuke.build/powershell -OutFile setup.ps1
powershell -ExecutionPolicy ByPass -File ./setup.ps1
```

```bash
# Bash
curl -Lsfo setup.sh https://nuke.build/bash
chmod +x setup.sh; ./setup.sh
```

When executed, the setup scripts will:

- Generate a _.nuke_ configuration file in the root directory, which references the chosen solution file
- Generate a [_build.ps1_](https://raw.githubusercontent.com/nuke-build/nuke/master/bootstrapping/build.ps1) and [_build.sh_](https://raw.githubusercontent.com/nuke-build/nuke/master/bootstrapping/build.sh) in the current directory
- Copy templates for project file and minimal build file ([.NET Framework/Mono](https://raw.githubusercontent.com/nuke-build/nuke/master/bootstrapping/Build.netfx.cs) or [.NET Core](https://raw.githubusercontent.com/nuke-build/nuke/master/bootstrapping/Build.netcore.cs))
- Add build project to the solution file (without build configuration)

For your own awareness, we recommend to review the applied changes using `git diff` or similar tools.

## Build Execution

Without further modifications, executing _build.ps1_ or _build.sh_ will:

1. Download or update .NET Core or NuGet executables
3. Restore dependencies for the build project
2. For .NET Framework/Mono: download and execute _Nuke.MSBuildLocator_
4. Compile and execute the build project

Various parameters can be passed to the build:

```
build [targets] [-configuration <value>] [-skip [targets]] [...]
```

- `target`: defines the target(s) to be executed; multiple targets are separated by plus sign (i.e., `compile+pack`); if no target is defined, the _default_ will be executed
- `-configuration <value>`: defines the configuration to build. Default is _debug_
- `-verbosity <value>`: supported values are `quiet`, `minimal`, `normal` and `verbose`
- `-skip [targets]`: if no target is defined, only the explicit stated targets will be executed; multiple targets are separated by plus sign (i.e, `-skip clean+push`)
- `-graph`: will generate a HTML view of target dependencies
- `-help`: will show further information about available targets and parameters

You can also append custom arguments and access them in your build using the `EnvironmentInfo.Argument` alias.

At the end of the build execution, a detailed summary is provided:

```
========================================
Target              Status      Duration
----------------------------------------
Restore             Executed        0:02
Clean               Skipped         0:00
Compile             Executed        0:06
Pack                Executed        0:06
----------------------------------------
Total                               0:16
========================================

Finished build on 06/08/2017 08:50:38.
```

## Build Authoring

Builds are written as simple console applications. The build class should inherit from the `NukeBuild` base class. Targets are implemented as expression-bodied properties, and therefore seamlessly provide navigation via _go to declaration_. They are typed as `Target`, which actually is a delegate type, this results in the `=> _ => _` language ceremony. A simple build definition can look like this:

```c#
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.MyTarget);

    Target MyTarget => _ => _
        .Executes(() =>
        {
            Console.WriteLine("Hello from NUKE!");
        });
}
```

The `Main` method should return the result from the `Execute` method and also defines the default target to invoke.

### Advanced Example

Let's write a more advanced example:

```c#
[Parameter] string MyGetApiKey;

Target Publish => _ => _
    .Requires(() => MyGetApiKey)
    .OnlyWhen(() => IsServerBuild)
    .DependsOn(Pack)
    .Executes(() =>
    {
        var packages = GlobFiles(OutputDirectory / "packages", "*.nupkg");
        foreach (var package in packages)
        {
            NuGetPush(s => s
                .SetTargetPath(package)
                .SetVerbosity(NuGetVerbosity.Detailed)
                .SetApiKey(MyGetApiKey)
                .SetSource("https://www.myget.org/F/nukebuild/api/v2/package"));
        }
    });
```

- `[Parameter]`: the execution engine will try to inject values based on command-line arguments and environment variables with the same name as the field. This mechanism works for enums, strings, booleans and string collections (requires the _separator_ to be set).
- `Target`: defines a target as _expression-bodied property_. The type itself is a delegate, hence, the property is implemented as `_ => _`.
- `Requires`: prior to execution of all targets, the execution engine checks if `MyGetApiKey` was set (fast fail). Also boolean expressions can be specified here.
- `OnlyWhen`: the target is only executed when running on a server. The property `IsServerBuild` is provided from the `Build` base class, and checks whether any of the known build servers is currently hosting the process (i.e., TeamCity or Bitrise).
- `DependsOn`: this target depends on another target called `Pack`. Multiple dependent targets can be separated by comma since the method accepts `params Target[] targets`. Targets referenced as `string` are so-called _shadow targets_, and will be silently skipped if absent.
- `Executes`:
  - Files are collected using the glob mechanism. The base directory is constructed with the `/` operator that takes care of platform-specific directory separators
  - For each file, `NuGetPush` is executed with several options applied.

In this example, we called a few methods from other classes, namely `FileSystemTasks.GlobFiles` and `NuGetTasks.NuGetPush`. For better readability, those methods are called using static imports:

```c#
using static Nuke.Common.Tools.NuGet.NuGetTasks;
using static Nuke.Core.IO.FileSystemTasks;
```

We will try to provide dedicated tooling for this, so that auto-completion directly offers all `*Tasks` methods. Meanwhile, it's possible that your IDE provides a context action for transforming into a static import usage.

## Tool Orchestration

Many of the tasks provided are simple wrappers around conventional command line tools. They implement a rich interface and are located in the `Nuke.Common.Tools` namespace. Note that the required dependencies are not automatically added. For instance, to use the `InspectCodeTasks` you need to add a package reference to `JetBrains.ReSharper.CommandLineTools`. Package references are handled the same way as for any other of your projects.

When executing a task, the logger will print the exact tool path and arguments, so that you can easily reproduce the invocation. Note that the message is logged as _information_. For `InspectCodeTasks` this would be:

```
> C:\Users\user\.nuget\packages\JetBrains.ReSharper.CommandLineTools\2017.1.20170407.131846\tools\inspectcode.exe C:\code\nuke\source\Nuke.sln --output=C:\code\nuke\output\inspectCode.xml
```

Are you missing some tools? Just navigate to our [FeatHub page](http://feathub.com/nuke-build/nuke) and suggest it for our next release. Supporting new tools is very easy, since we can utilize our powerful generator. Meanwhile you can still use the `ProcessTasks` class and its aliases.

<br/>
<br/>
**Happy building!**
