---
_disableAffix: true
jr.disableMetadata: false
jr.disableLeftMenu: false
jr.disableRightMenu: true
uid: sharing-builds-fundamentals
title: Fundamentals
---

# Fundamentals

Working on multiple projects often implies **similar requirements** for the build implementations. A quick and easy solution would be to copy the necessary files across all repositories. However, this can cause technical debt to grow over time. For instance, as soon as the unit testing is extended with a coverage tool, all these copies would need to be updated. Even worse, there is a high chance of ending up with divergent implementations when updating these copies becomes too time-consuming facing critical deadlines.

Naturally, builds written with NUKE are part of the repository and consist of pure C#. That allows to use two very pragmatic yet powerful approaches of maintaining builds in **separate submodules** or shipping **compiled assemblies via NuGet packages**. Furthermore, NUKE provides two additional strategies, namely **pre-build download of external files** and **builds as global tools**, which will be covered more thoroughly in the next sections.

For now, here is a short comparison:

| Strategy | Build Versioning | Build Extensibility | Maintenance Impact | 
| --- | --- | --- | --- |
| Submodules | Yes, including branches | Difficult | Normal |
| NuGet Packages | Yes | Yes, via class inheritance | High |
| [External Files](external-files.md) | Yes, if supported by URL scheme | Yes, via partial declarations | Low |
| [Global Tools](global-tools.md) | Yes | No | High |
