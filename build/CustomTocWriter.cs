// Copyright Matthias Koch 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/web/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Nuke.Common;
using Nuke.Common.BuildServers;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static Nuke.Common.Logger;
using static Nuke.Common.IO.TextTasks;
using static Nuke.Common.IO.PathConstruction;

// ReSharper disable ArrangeMethodOrOperatorBody
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable InconsistentNaming
class CustomTocWriter : IDisposable
{
    public static void WriteCustomTocs(AbsolutePath apiDirectory, IEnumerable<string> dllFiles)
    {
        using (var tocWriter = new CustomTocWriter(apiDirectory, dllFiles))
        {
            tocWriter.WriteCustomTocs();
        }
    }

    private static Dictionary<string, string> GetIconClasses(IEnumerable<AssemblyDefinition> assemblies)
    {
        return assemblies
            .SelectMany(x => x.CustomAttributes)
            .Where(x => x.AttributeType.FullName.EndsWith("IconClassAttribute"))
            .Distinct(x => ((TypeDefinition) x.ConstructorArguments[index: 0].Value).FullName)
            .ToDictionary(x => ((TypeDefinition) x.ConstructorArguments[index: 0].Value).FullName,
                x => (string) x.ConstructorArguments[index: 1].Value);
    }

    private static string GetName(IMemberDefinition type, string assemblyName, bool removeNamespaceFromName = false)
    {
        var lastAssemblyNameSegment = assemblyName.Split('.').Last();
        var name = removeNamespaceFromName ? type.Name.Replace(lastAssemblyNameSegment, string.Empty) : type.Name;
        return name == "Tasks" ? type.Name : name;
    }

    private static bool IsEntryType(MemberReference typeDefinition, IReadOnlyDictionary<string, string> iconClasses)
    {
        if (!iconClasses.ContainsKey(typeDefinition.FullName.NotNull()))
            return false;
        return new[]
               {
                   typeof(ControlFlow),
                   typeof(EnvironmentInfo),
                   typeof(Logger),
                   typeof(NukeBuild),
                   typeof(PathConstruction),
                   typeof(ProcessTasks),
                   typeof(ToolPathResolver)
               }.Any(x => x.FullName == typeDefinition.FullName);
    }

    private static bool IsServerType(ICustomAttributeProvider typeDefinition)
    {
        return typeDefinition.CustomAttributes.Any(x => x.AttributeType.FullName == typeof(BuildServerAttribute).FullName);
    }

    private static bool IsCommonType(AssemblyDefinition assembly)
    {
        return assembly.FullName == typeof(NukeBuild).Assembly.GetName().FullName;
    }

    private static bool IsInjectionAttribute(MemberReference typeSymbol, IReadOnlyDictionary<string, string> iconClasses)
    {
        return iconClasses.ContainsKey(typeSymbol.FullName.NotNull()) && typeSymbol.Name.EndsWith("Attribute");
    }

    readonly IReadOnlyDictionary<string, string> _iconClasses;
    readonly IReadOnlyCollection<AssemblyDefinition> _assemblies;
    readonly AbsolutePath _apiDirectory;

    CustomTocWriter(AbsolutePath apiDirectory, IEnumerable<string> dllFiles)
    {
        _apiDirectory = apiDirectory;
        _assemblies =
            dllFiles
                .ForEachLazy(x => Info($"Loading {x}"))
                .Select(AssemblyDefinition.ReadAssembly)
                .ToList();
        _iconClasses = GetIconClasses(_assemblies);
    }

    public void Dispose()
    {
        _assemblies?.ForEach(x => x.Dispose());
    }

    private void WriteCustomTocs()
    {
        var relevantTypeDefinitons = GetRelevantTypeDefinitons();
        var addonTocs = CreateAddonTocs(relevantTypeDefinitons[Kind.Addons]);
        var commonToc = CreateCommonToc(relevantTypeDefinitons, addonTocs);

        var serializer = new SerializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build();

        Info("Writing toc.yml...");
        WriteAllText(_apiDirectory / "toc.yml", serializer.Serialize(commonToc));
        addonTocs
            .Where(x => x.Count() > 1)
            .ForEachLazy(x => Info($"Writing {x.Key}/toc.yml..."))
            .ForEach(x => WriteAllText(_apiDirectory / x.Key / "toc.yml", serializer.Serialize(x)));
    }

    private IEnumerable<Item> CreateCommonToc(
        ILookup<Kind, TypeInfo> typeDefinitons,
        ILookup<string, Item> addonTocs)
    {
        return typeDefinitons
            .Where(x => x.Key != Kind.Addons)
            .SelectMany(x => x.AsEnumerable(), (grouping, typeInfo) => new { TypeInfo = typeInfo, Kind = grouping.Key })
            .Where(x => x.TypeInfo.Assembly.Name.Name == "Nuke.Common")
            .GroupBy(x => x.Kind, x => x.TypeInfo)
            .SelectMany(x => new Item { Separator = x.Key.ToString() }.Concat(CreateItems(x.Select(y => y.Type),
                x.First().Assembly.Name.Name,
                includeHref: true
            )))
            .Concat(new Item { Separator = Kind.Addons.ToString() })
            .Concat(addonTocs.Select(CreateAddonItem))
            .ToList();
    }

    private ILookup<Kind, TypeInfo> GetRelevantTypeDefinitons()
    {
        return _assemblies
            .SelectMany(x => x.MainModule.Types, (assembly, type) => new TypeInfo(type, assembly))
            .Where(x => !x.Type.Namespace.StartsWith("Nuke.Core"))
            .Distinct(x => x.Type.FullName)
            .Select(x => new { TypeInfo = x, Kind = GetKind(x.Type, x.Assembly) })
            .Where(x => x.Kind != Kind.None)
            .ForEachLazy(x => Console.WriteLine($"Found '{x.TypeInfo.Type.FullName}' ({x.Kind})."))
            .ToLookup(x => x.Kind, x => x.TypeInfo);
    }

    private Item CreateAddonItem(IGrouping<string, Item> grouping)
    {
        var isGroup = grouping.Count() > 1;
        var firstItem = grouping.First();
        var firstItemHref = $"{grouping.Key}/{grouping.First().Uid}.yml";
        return new Item
               {
                   Uid = isGroup ? grouping.Key : firstItem.Uid,
                   Href = isGroup ? $"{grouping.Key}/toc.yml" : firstItemHref,
                   TopicUid = isGroup ? firstItem.Uid : null,
                   Icon = GetIconClassText(firstItem.Uid),
                   Name = grouping.Key.Split('.').Last()
               };
    }

    private ILookup<string, Item> CreateAddonTocs(IEnumerable<TypeInfo> typeInfos)
    {
        return typeInfos
            .GroupBy(x => x.Assembly.Name.Name,
                x => x.Type, (name, type) => new
                                             {
                                                 AssemblyName = name,
                                                 Items = CreateItems(type, name, includeIcon: false, removeNamespaceFromName: true)
                                             })
            .SelectMany(x => x.Items, (x, item) => new { x.AssemblyName, Item = item })
            .ToLookup(x => x.AssemblyName, x => x.Item);
    }

    private IEnumerable<Item> CreateItems(
        IEnumerable<TypeDefinition> definitions,
        string assemblyName,
        bool includeIcon = true,
        bool removeNamespaceFromName = false,
        bool includeHref = false)
    {
        return definitions.Select(type => new { Name = GetName(type, assemblyName, removeNamespaceFromName), Type = type })
            .OrderBy(x => x.Name == x.Type.Namespace.Split('.').Last() + "Tasks" ? $"!{x.Name}" : x.Name)
            .Select(x => new Item
                         {
                             Uid = x.Type.FullName,
                             Name = x.Name,
                             Href = $"{(includeHref ? assemblyName + '/' : string.Empty)}{x.Type.FullName}.yml",
                             Icon = includeIcon ? GetIconClassText(x.Type) : null
                         });
    }

    private string GetIconClassText(TypeDefinition typeDefinition)
    {
        if (_iconClasses.TryGetValue(typeDefinition.FullName, out var iconClassText)) return iconClassText;
        return IsServerType(typeDefinition) ? "server" : "power-cord2";
    }

    private string GetIconClassText(string identifier)
    {
        return _iconClasses.TryGetValue(identifier, out var iconClassText) ? iconClassText : "power-cord2";
    }

    private Kind GetKind(TypeDefinition typeDefinition, AssemblyDefinition assembly)
    {
        if (IsEntryType(typeDefinition, _iconClasses))
            return Kind.Entry;
        if (IsServerType(typeDefinition))
            return Kind.Servers;
        if (IsInjectionAttribute(typeDefinition, _iconClasses))
            return Kind.Injection;
        if (typeDefinition.Name.EndsWith("Tasks"))
        {
            return IsCommonType(assembly)
                ? Kind.Common
                : Kind.Addons;
        }

        return Kind.None;
    }

    private enum Kind
    {
        None,
        Entry,
        Servers,
        Injection,
        Common,
        Addons
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
    private struct Item
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public string Href { get; set; }
        public string Icon { get; set; }
        public string TopicUid { get; set; }
        public string Separator { get; set; }
        public Item[] Items { get; set; }
    }

    private struct TypeInfo
    {
        public TypeInfo(TypeDefinition type, AssemblyDefinition assembly)
        {
            Type = type;
            Assembly = assembly;
        }

        public TypeDefinition Type { get; }
        public AssemblyDefinition Assembly { get; }
    }
}
