---
_disableBreadcrumb: true
_disableAffix: true
jr.disableMetadata: false
jr.disableLeftMenu: false
jr.disableRightMenu: true
uid: authoring-builds-parameter-declaration
title: Parameter Declaration
---

# Parameter Declaration

Build automation often requires to incorporate parameters that are dynamic or secret. Such parameters can either be passed via command-line arguments or environment variables. Dynamic parameters can have default values, that are only overridden on demand. Meanwhile, secret parameters should never appear in clear text in the build implementation. NUKE offers a declarative approach to define build input parameters by decorating fields and properties with the `Parameter` attribute:

```c#
[Parameter]
readonly string Configuration { get; } = IsLocalBuild ? "Debug" : "Release";
```

This example defines `Configuration` as a parameter with a default value of `Debug` for local builds, and `Release` for server builds. The default value can easily be overriden by setting an environment variable or passing it from the command-line:

```powershell
$ build --configuration Release
```

A declared parameter can also have a description and will automatically show up in the [help output](../running-builds/fundamentals.md#help) of the build.

> [!Note]
> The resolution of parameters is very forgiving. For instance, a parameter `ApiKey` can be passed with pascal-casing `-ApiKey` or lisp-casing `--api-key`. Minor typing mistakes can be detected from calculating the [Levenshtein distance](https://en.wikipedia.org/wiki/Levenshtein_distance) and are reported as a warnings. However, in practice it is more convenient to use the [global tool shell-completion](../running-builds/global-tool.md).

## Supported Types

String values passed as parameters can be automatically converted into a variety of types. Out-of-the-box, NUKE supports primitive types like `bool`, `string`, and other numeric types:

| Parameter Type | Input | Injected Value |
| --- | --- | --- |
| `string` | _none_ | `null` |
| `string` | `--param value` | `value` |
| `int` | _none_ | `0` |
| `bool` | `--param` | `true` |
| `bool` | `--param false` | `false` |
| `bool?` / `int?` | _none_ | `null` |

Also arrays of the above are supported:

| Parameter Type | Input | Injected Value |
| --- | --- | --- |
| `string[]` | _none_ | `null` |
| `string[]` | `--param` | `new string[0]` |
| `int[]` | `--param 1 2` | `new[] { 1, 2 }` |

Even more exotic types are supported, like `AbsolutePath` and types deriving from `Enumeration` (mostly used for [CLI tools](cli-tools.md)):

| Parameter Type | Input | Injected Value |
| --- | --- | --- |
| `AbsolutePath` | `--param /bin/etc` | `/bin/etc` |
| `AbsolutePath` | `--param ./etc` | `/bin/etc` |
| `MSBuildVerbosity` | `--param minimal` | `MSBuildVerbosity.Minimal` |

> [!Note]
> Custom conversions can be added by implementing a `TypeConverter`, which should take the `string` value and convert it to the complex object. In fact, the conversion for  `AbsolutePath` [is implemented this way](https://github.com/nuke-build/nuke/blob/0.12.0/source/Nuke.Common/IO/PathConstruction.cs#L319-L340).
