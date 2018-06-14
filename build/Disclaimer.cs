// Copyright Matthias Koch 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/web/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Nuke.Common;
using Nuke.Common.Utilities.Collections;

static class Disclaimer
{
    public static void WriteDisclaimer(ApiProject apiProject, string disclaimerFile, IEnumerable<string> dllFiles)
    {
        var assemblies =
            dllFiles
                .Select(AssemblyDefinition.ReadAssembly)
                .ToList();

        var relevantSymbols = assemblies.NotNull()
            .SelectMany(x => x.MainModule.Types)
            .Where(x => x.Namespace != null && x.Namespace.StartsWith("Nuke"))
            .Distinct(x => x.FullName);
        File.WriteAllText(disclaimerFile,
            relevantSymbols.Aggregate(new StringBuilder(),
                    (sb, x) => sb
                        .AppendLine()
                        .AppendLine("---")
                        .AppendLine($"uid: {x}")
                        .AppendLine("---")
                        .AppendLine()
                        .WriteWarning(apiProject)
                        .WriteInformation(apiProject)
                        .AppendLine())
                .ToString());
    }

    static StringBuilder WriteWarning(this StringBuilder builder, ApiProject apiProject)
    {
        const string org = "nuke-build";

        var owner = apiProject.RepositoryUrl.Replace("https://github.com/", string.Empty).Split('/').First();
        if (owner == org)
            return builder;

        return builder
            .AppendLine("<div class=\"alert alert-warning\" role=\"warning\">")
            .AppendLine("  <span class=\"icon icon-warning alert-icon\"></span>")
            .AppendLine($"  Solely maintained by <a href=\"https://github.com/{owner}\"><strong>@{owner}</strong></a>.")
            .AppendLine($"  Listing approved by <a href=\"https://github.com/{org}\"><strong>@{org}</strong></a>.")
            .AppendLine("</div>");
    }

    static StringBuilder WriteInformation(this StringBuilder builder, ApiProject apiProject)
    {
        var packageId = apiProject.PackageId;
        return builder
            .AppendLine("<div class=\"alert alert-info\" role=\"info\">")
            .AppendLine("  <span class=\"icon icon-info alert-icon\"></span>")
            .AppendLine($"  This API is part of the <a href=\"https://nuget.org/packages/{packageId}\"><strong>{packageId}</strong></a> package.")
            .AppendLine(
                $"  The code is available at <a href=\"{apiProject.RepositoryUrl}\"><strong>{apiProject.RepositoryUrl}</strong></a>.")
            .AppendLine("</div>");
    }
}
