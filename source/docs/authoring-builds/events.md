---
_disableBreadcrumb: true
_disableAffix: true
jr.disableMetadata: false
jr.disableLeftMenu: false
jr.disableRightMenu: true
uid: authoring-builds-events
title: Events
---

# Events

For additional initialization and more granular reporting, the `NukeBuild` class exposes virtual event methods that can be overridden:

| Event | Description | 
| --- | --- |
| `OnBuildCreated()` | Build was created |
| `OnBuildInitialized()` | Build is initialized; has values injected and requirements validated |
| `OnBuildFinished()` | Build has finished (successful or failed) |
| `OnTargetStart(string target)` | Before a target is about to start |
| `OnTargetAbsent(string target)` | Shadow target is absent |
| `OnTargetSkipped(string target)` | Target is skipped |
| `OnTargetExecuted(string target)` | Target has been executed successfully |
| `OnTargetFailed(string target)` | Target has failed |
