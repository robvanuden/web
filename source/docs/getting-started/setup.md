---
_disableBreadcrumb: true
_disableAffix: true
jr.disableMetadata: false
jr.disableLeftMenu: false
jr.disableRightMenu: true
uid: getting-started-setup
title: Setup
---

# Setup

In order to setup a repository with NUKE, the global tool `Nuke.GlobalTool` must be installed:

```c#
$ dotnet tool install Nuke.GlobalTool --global

You can invoke the tool using the following command: nuke
Tool 'nuke.globaltool' (version '0.10.0') was successfully installed.
```

> [!Warning]
> When using ZSH as a shell, the dotnet tools path `$HOME/.dotnet/tools` must be added manually (see [dotnet/cli#9321](https://github.com/dotnet/cli/issues/9321)). This can be achieved by adding `export PATH=$HOME/.dotnet/tools:$PATH` to the `.zshrc` file.

Afterwards, the setup can be invoked with `nuke :setup`:

![Setup](~/images/setup.gif)

> [!Note]
> It is recommended to choose **.NET Core SDK** as bootstrapping method. This choice solely indicates that the build project is being built using `dotnet` CLI. The build will still be able to compile projects targeting the full .NET framework. For further comparison, the template scripts [build.netcore.ps1](https://github.com/nuke-build/nuke/blob/develop/source/Nuke.GlobalTool/templates/build.netcore.ps1)/[build.netcore.sh](https://github.com/nuke-build/nuke/blob/develop/source/Nuke.GlobalTool/templates/build.netcore.sh) and [build.netfx.ps1](https://github.com/nuke-build/nuke/blob/develop/source/Nuke.GlobalTool/templates/build.netfx.ps1)/[build.netfx.sh](https://github.com/nuke-build/nuke/blob/develop/source/Nuke.GlobalTool/templates/build.netfx.sh) can be checked.

When the setup has finished, the applied changes can be checked (e.g. `git status -s -uall`):

| File | Description | 
| --- | --- |
| `_build.csproj` and `Build.cs` | Build project and default `Build` class implementation |
| `build.ps1` and `build.sh` | Bootstrapping scripts to execute the build on any machine |
| `.nuke` | Root directory marker file; references default solution file |
| _Solution file_ | Includes build project if chosen |

### Wizard

The setup can be continued with an extended wizard, which helps to scaffold the initial `Build` class according to best practices. For instance, it suggests to use [GitVersion](https://gitversion.readthedocs.io/) for generation of version numbers and provides a default implementation for the `Clean`, `Restore` and `Compile` targets.
