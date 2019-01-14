---
_disableAffix: true
jr.disableMetadata: false
jr.disableLeftMenu: false
jr.disableRightMenu: true
uid: authoring-builds-solutions-projects
title: Solutions and Projects
---

# Solutions and Projects

Build automation often requires information related to a solution or its projects. Such information is easily duplicated, but then can get outdated very quickly. For instance, in order to successfully compile all projects, the target framework monikers related to individual projects must be known. Another symptom is to have a duplicated list of projects required to test or publish.

NUKE provides a powerful solution parser that also supports solution manipulation, and which deeply integrates with the [Microsoft.Build](https://www.nuget.org/packages/Microsoft.Build) package, that helps to evaluate project files.

## Solution Model

The most conventional approach to access the solution model is to use `ProjectModelTasks.ParseSolution`. A more integrated strategy is the `SolutionAttribute` attribute:

```c#
[Solution] readonly Solution Solution;
```

This field is initialized prior to build execution with an `Solution` object parsed from the location:

- Passed as constructor argument to the attribute
- Passed as `--solution` command-line argument
- Passed as `SOLUTION` environment variable
- Defined in the `.nuke` file

`Solution` defines an implicit conversion to `string`, which allows to conveniently pass the object as the _solution path_.  Also the _solution directory_ is easily accessible:

```c#
Handle(solutionFileString: Solution);
Trace($"Solution directory: {Solution.Directory}");
```

Project objects retrieved from the solution behave similar to this approach:

```c#
var project = Solution.GetProject("SampleApp");
DotNet($"build {project} --configuration release");
```

The `Solution` object is structure-aware in terms of solution folders: projects on the root level, i.e., having the solution node as parent, are accessible via `Solution.Projects`, while others can be found when traversing `Solution.SolutionFolders` or filtering `Solution.AllProjects`.

## Project Model

Based on the [Microsoft.Build](https://www.nuget.org/packages/Microsoft.Build) package, the `ProjectModelTasks.ParseProject` method allows accessing the project model. A more integrated way is to retrieve the project from the solution, and then call `GetMSBuildProject`:

```c#
var commonProject = Solution.GetProject("SampleApp");
var msbuildProject = commonProject.GetMSBuildProject();
```

Basic information like target frameworks, properties and item groups are available right from the common project object:

```
var targetFramework = project.GetTargetFrameworks();
var isPackable = project.GetProperty<bool>("IsPackable");
var compiledFiles = project.GetItems<AbsolutePath>("Compile");
```
