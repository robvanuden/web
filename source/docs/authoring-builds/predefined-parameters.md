---
_disableBreadcrumb: true
_disableAffix: true
jr.disableMetadata: false
jr.disableLeftMenu: false
jr.disableRightMenu: true
uid: authoring-builds-predefined-parameters
title: Predefined Parameters
---

# Predefined Parameters

The `NukeBuild` base class implements several predefined parameters:

| Property | Parameter | Description | 
| --- | --- | --- |
| `RootDirectory` | `--root` | Gets the full path to the root directory |
| `TemporaryDirectory` | | Gets the full path to the temporary directory `/.tmp` |
| `BuildAssemblyDirectory` | | Gets the full path to the build assembly directory, or `null` |
| `BuildProjectDirectory` | | Gets the full path to the build project directory, or `null` |
| `InvokedTargets` | `--target` | Gets the list of targets that were invoked |
| `SkippedTargets` | `--skip` | Gets the list of targets that are skipped |
| `ExecutingTargets` | | Gets the list of targets that are executing |
| `Verbosity` | `--verbosity` | Gets the logging verbosity for build execution |

These parameters are implemented as static properties, which allows them to be  used to define default values for [custom parameters](parameter-declaration.md).
