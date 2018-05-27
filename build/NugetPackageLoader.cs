// Copyright Matthias Koch 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/web/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using NuGet.Configuration;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.ProjectManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using Nuke.Common;
using Nuke.Common.Utilities.Collections;

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable InconsistentNaming

class NugetPackageLoader
{
    public static void InstallPackages(IEnumerable<string> packageIds, string packageDirectory)
    {
        var packageLoader = new NugetPackageLoader(packageDirectory);
        packageIds.ForEach(packageLoader.InstallPackage);
    }

    private readonly NuGetPackageManager _packageManager;
    private readonly FolderNuGetProject _project;
    private readonly INuGetProjectContext _projectContext;
    private readonly ResolutionContext _resolutionContext;

    private readonly SourceRepository _sourceRepository;

    private NugetPackageLoader(string packagePath, bool includePreReleases = false)
    {
        var resourceProviders = new List<Lazy<INuGetResourceProvider>>(Repository.Provider.GetCoreV3());

        _project = new FolderNuGetProject(packagePath, new PackagePathResolver(packagePath, useSideBySidePaths: false));
        _projectContext = new ProjectContext();
        _sourceRepository = new SourceRepository(new PackageSource("https://api.nuget.org/v3/index.json"), resourceProviders);
        _resolutionContext = new ResolutionContext(DependencyBehavior.Ignore,
            includePreReleases,
            includeUnlisted: false,
            versionConstraints: VersionConstraints.None);

        var settings = Settings.LoadDefaultSettings(packagePath, configFileName: null, machineWideSettings: new XPlatMachineWideSetting());
        var sourceRepositoryProvider = new SourceRepositoryProvider(settings, resourceProviders);

        _packageManager = new NuGetPackageManager(sourceRepositoryProvider, settings, packagePath)
                          {
                              PackagesFolderNuGetProject = _project
                          };
    }

    private void InstallPackage(string packageId)
    {
        _packageManager
            .InstallPackageAsync(_project, packageId, _resolutionContext, _projectContext, _sourceRepository, secondarySources: null,
                token: CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    private class ProjectContext : INuGetProjectContext
    {
        public void Log(MessageLevel level, string message, params object[] args)
        {
            switch (level)
            {
                case MessageLevel.Info:
                    Logger.Info(message, args);
                    break;
                case MessageLevel.Warning:
                    Logger.Warn(message, args);
                    break;
                case MessageLevel.Error:
                    Logger.Error(message, args);
                    break;
            }
        }

        public FileConflictAction ResolveFileConflict(string message) => FileConflictAction.Ignore;

        public PackageExtractionContext PackageExtractionContext { get; set; }

        public NuGet.ProjectManagement.ExecutionContext ExecutionContext => null;
        public XDocument OriginalPackagesConfig { get; set; }

        public ISourceControlManagerProvider SourceControlManagerProvider => null;

        public void ReportError(string message)
        {
            Logger.Error(message);
        }

        public NuGetActionType ActionType { get; set; }
        public Guid OperationId { get; set; }
    }
}
