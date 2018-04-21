// Copyright Matthias Koch 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/web/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Nuke.Common.Tools.DocFx;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using static CustomToc;
using static Disclaimer;
using static Nuke.Common.IO.FtpTasks;
using static Nuke.Common.IO.SerializationTasks;
using static Nuke.Common.Tools.DocFx.DocFxTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Git.GitTasks;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.ControlFlow;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Logger;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.BuildSite);

    [Parameter] readonly string FtpUsername;
    [Parameter] readonly string FtpPassword;

    string DocFxFile => RootDirectory / "docfx.json";
    string SiteDirectory => OutputDirectory / "site";

    AbsolutePath RepositoriesDirectory => RootDirectory / "repos";
    AbsolutePath ApiDirectory => SourceDirectory / "api";

    IEnumerable<ApiProject> Projects
        => YamlDeserializeFromFile<List<ApiProject>>(RootDirectory / "projects.yml");

    Target Clean => _ => _
        .Executes(() =>
        {
            DeleteDirectory(RepositoriesDirectory);
            DeleteDirectory(ApiDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Clone => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            Projects.Select(x => x.Repository)
                .ForEachLazy(x => Info($"Cloning repository '{x}'..."))
                .ForEach(x => Git($"clone {x.HttpsUrl} {RepositoriesDirectory / x.Identifier}"));
        });

    Target Restore => _ => _
        .DependsOn(Clone)
        .Executes(() =>
        {
            GlobFiles(RepositoriesDirectory, "**/*.sln")
                .ForEach(x =>
                {
                    SuppressErrors(() => DotNetRestore(Path.GetDirectoryName(x)));
                    //SuppressErrors(() => MSBuild(s => DefaultSettings.MSBuildRestore.SetSolutionFile(x)));
                    //SuppressErrors(() => NuGetRestore(x));
                });
        });

    Target CustomToc => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            WriteCustomToc(ApiDirectory / "toc.yml", GlobFiles(RepositoriesDirectory, "**/*.sln"));
        });

    Target Disclaimer => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            Projects.Where(x => !string.IsNullOrWhiteSpace(x.PackageId))
                .ForEachLazy(x => Info($"Writing disclaimer for {x.Repository.Identifier} ({x.PackageId})..."))
                .ForEach(x => WriteDisclaimer(x,
                    RepositoriesDirectory / $"{x.Repository.Identifier.Replace(oldChar: '/', newChar: '.')}.disclaimer.md",
                    GlobFiles(RepositoriesDirectory / x.Repository.Identifier, "**/*.sln")));
        });

    Target Metadata => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            if (IsLocalBuild)
            {
                //SetVariable ("MSBuildSDKsPath", @"C:\Program Files\dotnet\sdk\2.0.0\Sdks");
                SetVariable("VSINSTALLDIR", @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional");
                SetVariable("VisualStudioVersion", "15.0");
            }

            DocFxMetadata(DocFxFile, s => s.SetLogLevel(DocFxLogLevel.Verbose));
        });

    IEnumerable<string> XRefMapFiles
        => GlobFiles(NuGetPackageResolver.GetLocalInstalledPackageDirectory("msdn.4.5.2"), "content/*.zip")
            .Concat(GlobFiles(RepositoriesDirectory, "specs/xrefmap.yml"));

    Target BuildSite => _ => _
        .DependsOn(Metadata, CustomToc, Disclaimer)
        .Executes(() =>
        {
            DocFxBuild(DocFxFile, s => s
                .SetLogLevel(DocFxLogLevel.Verbose)
                .SetXRefMaps(XRefMapFiles)
                .SetServe(IsLocalBuild));
        });

    Target Publish => _ => _
        .DependsOn(BuildSite)
        .Requires(() => FtpUsername, () => FtpPassword)
        .Executes(() =>
        {
            FtpCredentials = new NetworkCredential(FtpUsername, FtpPassword);
            FtpUploadDirectoryRecursively(SiteDirectory, "ftp://www58.world4you.com");
        });
}
