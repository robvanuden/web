---
_disableBreadcrumb: true
_disableAffix: true
jr.disableMetadata: false
jr.disableLeftMenu: false
jr.disableRightMenu: true
uid: running-builds-fundamentals
title: Fundamentals
---

# Fundamentals

Depending on the operating system, one of the two bootstrapping scripts for PowerShell or BASH can be used to execute a build:

```powershell
$ ./build.sh [targets] [arguments]
$ ./build.ps1 [targets] [arguments]
```

> [!Note]
> Executing a build triggers the creation of a directory `./tmp` for temporary files that should not be committed. It is recommended to create an VCS ignore entry.

### Arguments

Arguments can be passed either in **lisp-case or camel-case** style with either **single or double dash** as prefix. The preferred variant is lisp-case with two dashes, which is also used for the [global tool shell completion]():

```powershell
$ build --target pack --configuration release --api-key 12345
```

Certain parameters can accept **collections of values** that are separated with a _space_. The built-in parameters `--target` or `--skip` also allow the _plus sign_ for value separation:

```powershell
$ build --target test pack --skip clean+restore
```

> [!Note]
> The `--skip` parameter can also be passed as a pure switch, i.e. without any targets as arguments. In this case, only the invoked targets will be executed, which greatly enhances troubleshooting.

### Help

A list of available targets, [predefined parameters](authoring-builds-predefined-parameters) as well as [custom declared parameters](authoring-builds-parameter-declaration) can always be shown by passing the `--help` switch:

![Help](~/images/help.png)

### Summary

At the end of every build execution, a summary with statuses and elapsed time per each target is displayed. Also error and warning descriptions are repeatedly printed:

```powershell
No errors or warnings.

========================================
Target              Status      Duration
----------------------------------------
Clean               Executed        0:00
Restore             Executed        0:05
Compile             Executed        0:26
----------------------------------------
Total                               0:32
========================================

Build succeeded on 01/01/2018 14:00:00.
```
