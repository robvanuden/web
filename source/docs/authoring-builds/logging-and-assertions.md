---
_disableAffix: true
jr.disableMetadata: false
jr.disableLeftMenu: false
jr.disableRightMenu: true
uid: authoring-builds-logging-assertions
title: Logging and Assertions
---

# Logging, Assertions, Error Handling

Diagnosing issues in build projects is easy by itself, because NUKE allows them to be easily debugged. However, it is good habit to avoid debugging whenever possible. Running on CI servers can also increase the ceremony heavily. NUKE ships with convenient logging, troubleshooting, and guarding methods, to improve investigations.

Logging is a technique that probably doesn't require much explanation. The `Logger` class provides the following methods:

```c#
Trace($"Example {Solution}");
Normal("Example {0}", Solution);
Info(Solution);

Warn("Warning!");
Error("Error!");
Error(exception);

Success("Finished.");
```

NUKE is focused on writing compact and readable code. The `ControlFlow` class provides several methods to remove the need for custom exceptions:

```c#
Assert(branch != "master", "Branch must be not master");
Fail("Unrecoverable.");

var semVer = version.NotNull().SemVer;
packages.NotEmpty().ForEach(x => { });
```
