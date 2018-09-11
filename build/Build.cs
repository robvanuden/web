// Copyright Matthias Koch 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/web/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FluentFTP;
using Nuke.Common.Tools.DocFx;
using Nuke.Common;
using Nuke.Common.Utilities.Collections;
using static CustomTocWriter;
using static Disclaimer;
using static CustomDocFx;
using static NugetPackageLoader;
using static Nuke.Common.IO.SerializationTasks;
using static Nuke.Common.Tools.DocFx.DocFxTasks;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FtpTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Logger;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.BuildSite);

    [Parameter] readonly string FtpUsername;
    [Parameter] readonly string FtpPassword;
    [Parameter] readonly string FtpServer;

    string DocFxFile => RootDirectory / "docfx.json";
    string SiteDirectory => OutputDirectory / "site";

    AbsolutePath GenerationDirectory => TemporaryDirectory / "packages";
    AbsolutePath ApiDirectory => SourceDirectory / "api";

    IEnumerable<ApiProject> Projects => YamlDeserializeFromFile<List<ApiProject>>(RootDirectory / "projects.yml");
    
    Target Clean => _ => _
        .Executes(() =>
        {
            DeleteDirectories(GlobDirectories(SourceDirectory, "*/bin", "*/obj"));
            DeleteDirectory(SolutionDirectory / "obj");
            EnsureCleanDirectory(ApiDirectory);
            EnsureCleanDirectory(GenerationDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target DownloadPackages => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            InstallPackages(Projects.Select(x => x.PackageId).Concat("System.ValueTuple"), GenerationDirectory);
        });

    Target CustomDocFx => _ => _
        .DependsOn(DownloadPackages)
        .Executes(() =>
        {
            WriteCustomDocFx(DocFxFile, BuildProjectDirectory / "docfx.template.json", GenerationDirectory, ApiDirectory);
        });

    Target Disclaimer => _ => _
        .DependsOn(DownloadPackages)
        .Executes(() =>
        {
            var disclaimerDirectory = SourceDirectory / "disclaimers";
            Directory.CreateDirectory(disclaimerDirectory);
            Projects.Where(x => x.IsExternalRepository)
                .ForEachLazy(x => Info($"Writing disclaimer for {x.PackageId}..."))
                .ForEach(x => WriteDisclaimer(x,
                    disclaimerDirectory / $"{x.PackageId}.disclaimer.md",
                    GlobFiles(GenerationDirectory / x.PackageId, "lib/net4*/*.dll")));
        });

    Target Metadata => _ => _
        .DependsOn(DownloadPackages, CustomDocFx)
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

    Target CustomToc => _ => _
        .DependsOn(DownloadPackages, Metadata)
        .Executes(() =>
        {
            GlobFiles(ApiDirectory, "**/toc.yml").ForEach(File.Delete);
            WriteCustomTocs(ApiDirectory, GlobFiles(GenerationDirectory, "**/lib/net4*/*.dll"));
        });

    Target BuildSite => _ => _
        .DependsOn(Metadata, CustomToc, Disclaimer)
        .Executes(() =>
        {
            DocFxBuild(DocFxFile, s => s
                .SetLogLevel(DocFxLogLevel.Verbose)
                .SetServe(IsLocalBuild));
        });

    Target Publish => _ => _
        .DependsOn(BuildSite)
        .Requires(() => FtpUsername, () => FtpPassword, () => FtpServer)
        .Executes(() =>
        {
            FtpCredentials = new NetworkCredential(FtpUsername, FtpPassword);
            FtpUploadDirectoryRecursively(SiteDirectory, FtpServer);

            return;
            var client = new FtpClient(FtpServer, new NetworkCredential(FtpUsername, FtpPassword));
            client.Connect();

            Directory.GetDirectories(SiteDirectory, "*", SearchOption.AllDirectories)
                .ForEach(directory =>
                {
                    var files = GlobFiles(directory, "*").ToArray();
                    var relativePath = GetRelativePath(SiteDirectory, directory);
                    var uploadedFiles = client.UploadFiles(files, relativePath, verifyOptions: FtpVerify.Retry);
                    ControlFlow.Assert(uploadedFiles == files.Length, "uploadedFiles == files.Length");
                });
        });
}
