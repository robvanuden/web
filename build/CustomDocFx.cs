// Copyright Matthias Koch 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/web/blob/master/LICENSE

using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.IO.TextTasks;
using static Nuke.Common.Logger;

static class CustomDocFx
{
    public static void WriteCustomDocFx(string docFxFile, string docFxTemplateFile, string generationDirectory, AbsolutePath apiDirectory)
    {
        dynamic json = JObject.Parse(ReadAllText(docFxTemplateFile));

        var metadata = new JArray();
        Directory.GetDirectories(generationDirectory)
            .ForEachLazy(x => Info($"Processing {x}..."))
            .Select(directory => CreateMetadataItem(directory, apiDirectory))
            .ForEach(metadata.Add);

        json.metadata = metadata;
        WriteAllText(docFxFile, json.ToString(Formatting.Indented));
    }

    static JObject CreateMetadataItem(string directory, AbsolutePath apiDirectory)
    {
        var framework = GetFrameworkToAnalyze(directory);
        var name = new DirectoryInfo(directory).Name;

        var src = GetRootRelativePath(directory).Replace(oldChar: '\\', newChar: '/');
        var dest = GetRootRelativePath(apiDirectory / name).Replace(oldChar: '\\', newChar: '/');
        var packages = GetRootRelativePath(Nuke.Common.NukeBuild.Instance.TemporaryDirectory).Replace(oldChar: '\\', newChar: '/');

        dynamic srcObject = new JObject();
        srcObject.src = src;
        srcObject.files = new JArray($"lib/{framework}/*.dll");

        dynamic result = new JObject();
        result.src = srcObject;
        result.dest = dest;
        result.references = new JArray($"{packages}/System.ValueTuple/lib/netstandard1.0/System.ValueTuple.dll");

        return result;
    }

    static string GetFrameworkToAnalyze(string directory)
    {
        return Directory.GetDirectories(Combine(directory, "lib"))
            .Select(x => new DirectoryInfo(x).Name)
            .OrderBy(x => x.StartsWith("netcore") || x.StartsWith("netstandard") ? $"!{x}" : x)
            .Last();
    }
}
