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
        var json = JObject.Parse(ReadAllText(docFxTemplateFile));

        var metadata = new JArray();
        Directory.GetDirectories(generationDirectory)
            .ForEachLazy(x => Info($"Processing {x}..."))
            .Select(directory => CreateMetadataItem(directory, apiDirectory))
            .ForEach(metadata.Add);

        json["metadata"] = metadata;
        WriteAllText(docFxFile, json.ToString(Formatting.Indented));
    }

    static JObject CreateMetadataItem(string directory, AbsolutePath apiDirectory)
    {
        var framework = GetFrameworkToAnalyze(directory);
        var name = new DirectoryInfo(directory).Name;

        var src = GetRootRelativePath(directory).Replace(oldChar: '\\', newChar: '/');
        var dest = GetRootRelativePath(apiDirectory / name).Replace(oldChar: '\\', newChar: '/');

        var srcObject = new JObject
                        {
                            new JProperty("src", new JValue(src)),
                            new JProperty("files", new JArray { new JValue($"lib/{framework}/*.dll") })
                        };

        return new JObject
               {
                   new JProperty("src", new JArray(srcObject)),
                   new JProperty("dest", new JValue(dest))
               };
    }

    static string GetFrameworkToAnalyze(string directory)
    {
        return Directory.GetDirectories(Combine(directory, "lib"))
            .Select(x => new DirectoryInfo(x).Name)
            .OrderBy(x => x.StartsWith("netcore") || x.StartsWith("netstandard") ? $"!{x}" : x)
            .Last();
    }
}
