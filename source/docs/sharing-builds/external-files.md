---
_disableBreadcrumb: true
_disableAffix: true
jr.disableMetadata: false
jr.disableLeftMenu: false
jr.disableRightMenu: true
uid: sharing-builds-external-files
title: External Files
---

# External Files

When using submodules or NuGet packages for build sharing, it usually involves quite a bit of _heavy_ work, like updating the parent repositories or creating a NuGet feed. External files are a more lightweight approach: There is a **primary repository** containing the actual `Build.cs` template file, and several **replica repositories** with only a `Build.cs.ext` file. This `.ext` file contains a URI referencing the `Build.cs` file, which will cause MSBuild to actually download or update that file as a pre-build step when compiling the build project. The generated `Build.cs` file can either be committed or excluded from the repository, depending on the individual requirements.

### Usage

To enable support for external files, the build project file must contain an item group similar to this:

```xml
<ItemGroup>
    <NukeExternalFiles Include="**\*.*.ext" Exclude="bin\**;obj\**" />
</ItemGroup>
```

Downloading or updating a `Build.cs` from an external source would then require to create a `Build.cs.ext` file with the following content:

```text
https://raw.githubusercontent.com/nuke-build/nuke/develop/build/Build.cs
```

Note, that instead of `develop` also a tag like `1.2.3` could be used, allowing to use lightweight versioning.

### Extension Points

Besides natural extensibility points like partial classes or type inheritance, this approach also supports minor output modifications in the form of **conditionals and replacements**. Considering a template like this:

```csharp
class Build
{
    readonly string Source = "_SOURCE_";
    
    [GitVersion] readonly GitVersion GitVersion;    // GITVERSION
}
```

With the following `Build.cs.ext` file, the generated output can be modified to replace the `_SOURCE_` placeholder, and also allows to either include or exclude the `GitVersion` field declaration:

```text
https://raw.githubusercontent.com/nuke-build/nuke/master/build/Build.cs

GITVERSION

SOURCE=https://api.nuget.org/v3/index.json
```

The first line must always be the URI reference. All lines containing a `=` are considered a **replacement**, while all other non-empty lines are **definitions**. Note that the template engine requires definition comments to be prefixed with two whitespace to be distinguishable from normal comments.
