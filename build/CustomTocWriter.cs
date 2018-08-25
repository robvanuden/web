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
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Logger;
using static Nuke.Common.IO.SerializationTasks;
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

    readonly IReadOnlyDictionary<string, string> _iconClasses;
    readonly IReadOnlyCollection<AssemblyDefinition> _assemblies;
    readonly AbsolutePath _apiDirectory;
    readonly string _commonNamespace = typeof(NukeBuild).Namespace;

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

    private Dictionary<string, string> GetIconClasses(IEnumerable<AssemblyDefinition> assemblies)
    {
        string getIconClassType(CustomAttribute attribute) => ((TypeDefinition) attribute.ConstructorArguments[index: 0].Value).FullName;

        return assemblies
            .SelectMany(x => x.CustomAttributes)
            .Where(x => x.AttributeType.FullName == typeof(IconClassAttribute).FullName)
            .Distinct(getIconClassType)
            .ToDictionary(getIconClassType, x => (string) x.ConstructorArguments[index: 1].Value);
    }

    private string GetName(IMemberDefinition type, string assemblyName, bool removeAssemblyFromName = false)
    {
        const string tasksSuffix = "Tasks";
        const string attributeSuffix = "Attribute";

        var lastAssemblyNameSegment = assemblyName.Split('.').Last();
        var name = removeAssemblyFromName ? type.Name.Replace(lastAssemblyNameSegment, string.Empty) : type.Name;

        name = name == tasksSuffix || name == attributeSuffix ? type.Name : name;

        if (name.EndsWith(tasksSuffix))
            return name.Substring(startIndex: 0, length: name.Length - tasksSuffix.Length);
        if (name.EndsWith(attributeSuffix))
            return name.Substring(startIndex: 0, length: name.Length - attributeSuffix.Length);

        return name;
    }

    private bool IsEntryType(MemberReference typeDefinition)
    {
        if (!_iconClasses.ContainsKey(typeDefinition.FullName.NotNull()))
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

    private bool IsServerType(ICustomAttributeProvider typeDefinition)
    {
        return typeDefinition.CustomAttributes.Any(x => x.AttributeType.FullName == typeof(BuildServerAttribute).FullName);
    }

    private bool IsInjectionAttribute(MemberReference typeSymbol)
    {
        return _iconClasses.ContainsKey(typeSymbol.FullName.NotNull()) && typeSymbol.Name.EndsWith("Attribute");
    }

    private void WriteCustomTocs()
    {
        var relevantTypeDefinitons = GetRelevantTypeDefinitons();
        var taskTocs = CreateTaskTocs(relevantTypeDefinitons[Kind.Tasks]);
        var mainToc = CreateMainToc(relevantTypeDefinitons, taskTocs);

        Info("Writing toc.yml...");
        YamlSerializeToFile(mainToc, _apiDirectory / "toc.yml");
        taskTocs
            .Where(x => x.Count() > 1 && x.Key != _commonNamespace)
            .ForEachLazy(x => Info($"Writing {x.Key}/toc.yml..."))
            .ForEach(x => YamlSerializeToFile(x, _apiDirectory / x.Key / "toc.yml"));
    }

    private IEnumerable<Item> CreateMainToc(
        ILookup<Kind, TypeInfo> typeDefinitons,
        ILookup<string, Item> taskTocs)
    {
        return typeDefinitons
            .SelectMany(x => x.AsEnumerable(), (grouping, typeInfo) => new { TypeInfo = typeInfo, Kind = grouping.Key })
            .GroupBy(x => x.Kind, x => x.TypeInfo)
            .OrderBy(x => (int) x.Key)
            .SelectMany(x => CreateCategoryToc(x.Key, x.AsEnumerable(), taskTocs));
    }

    private IEnumerable<Item> CreateCategoryToc(Kind kind, IEnumerable<TypeInfo> typeInfos, ILookup<string, Item> taskTocs)
    {
        return new[] { new Item { Separator = kind.ToString() } }
            .Concat(CreateItems(kind, typeInfos, taskTocs).OrderBy(x => x.Name));
    }

    private IEnumerable<Item> CreateItems(Kind kind, IEnumerable<TypeInfo> typeInfos, ILookup<string, Item> taskTocs)
    {
        if (kind == Kind.Injection) return CreateInjectionItems(typeInfos);
        if (kind == Kind.Tasks) return taskTocs.SelectMany(CreateTaskItem).OrderBy(x => x.Name);
        return typeInfos
            .GroupBy(x => x.Assembly)
            .SelectMany(x => CreateItems(x.AsEnumerable().Select(y => y.Type), x.Key.Name.Name, removeAssemblyFromName: true, includeHref: true));
    }

    private ILookup<Kind, TypeInfo> GetRelevantTypeDefinitons()
    {
        return _assemblies
            .SelectMany(x => x.MainModule.Types, (assembly, type) => new TypeInfo(type, assembly))
            .Distinct(x => x.Type.FullName)
            .Select(x => new { TypeInfo = x, Kind = GetKind(x.Type) })
            .Where(x => x.Kind != Kind.None)
            .ForEachLazy(x => Console.WriteLine($"Found '{x.TypeInfo.Type.FullName}' ({x.Kind})."))
            .ToLookup(x => x.Kind, x => x.TypeInfo);
    }

    private Item[] CreateTaskItem(IGrouping<string, Item> grouping)
    {
        var isGroup = grouping.Count() > 1;
        var firstItem = grouping.First();
        var firstItemHref = $"{grouping.Key}/{grouping.First().Uid}.yml";

        if (isGroup && grouping.Key == _commonNamespace)
        {
            grouping.ForEach(x => x.Href = $"{grouping.Key}/{x.Uid}.yml");
            return grouping.ToArray();
        }

        return new[]
               {
                   new Item
                   {
                       Uid = isGroup ? grouping.Key : firstItem.Uid,
                       Href = isGroup ? $"{grouping.Key}/toc.yml" : firstItemHref,
                       TopicUid = isGroup ? firstItem.Uid : null,
                       Icon = GetIconClassText(firstItem.Uid),
                       Name = grouping.Key.Split('.').Last()
                   }
               };
    }

    private IEnumerable<Item> CreateInjectionItems(
        IEnumerable<TypeInfo> typeInfos)
    {
        var typeInfoArray = typeInfos as TypeInfo[] ?? typeInfos.ToArray();
        var definitionGroups = typeInfoArray
            .GroupBy(x => x.Type.Namespace)
            .OrderBy(x => x.Key);

        var items = new List<Item>();
        foreach (var definitionGroup in definitionGroups)
        {
            if (definitionGroup.Key.StartsWith(_commonNamespace))
            {
                items.AddRange(CreateItems(definitionGroup.Select(x => x.Type), definitionGroup.First().Assembly.Name.Name,
                    removeAssemblyFromName: false, includeHref: true));
                continue;
            }

            var isGroup = definitionGroup.Count() > 1;
            var firstType = definitionGroup.First();
            var assemblyName = firstType.Assembly.Name.Name;
            var item = new Item
                       {
                           Name = isGroup ? definitionGroup.Key.Split('.').Last() : GetName(firstType.Type, assemblyName),
                           Href = $"{assemblyName}/{firstType.Type.FullName}.yml",
                           Icon = GetIconClassText(firstType.Type),
                           TopicUid = isGroup ? firstType.Type.FullName : null,
                           Items = isGroup
                               ? CreateItems(definitionGroup.Select(x => x.Type), assemblyName, removeAssemblyFromName: false, includeHref: true)
                                   .ToArray()
                               : null
                       };
            items.Add(item);
        }

        return items;
    }

    private IEnumerable<Item> CreateItems(
        IEnumerable<TypeDefinition> definitions,
        string assemblyName,
        bool removeAssemblyFromName = false,
        bool includeHref = false)
    {
        return definitions.Select(type => new { Name = GetName(type, assemblyName, removeAssemblyFromName), Type = type })
            .OrderBy(x => x.Name == x.Type.Namespace.Split('.').Last() ? $"!{x.Name}" : x.Name)
            .Select(x => new Item
                         {
                             Uid = x.Type.FullName,
                             Name = x.Name,
                             Href = $"{(includeHref ? assemblyName + '/' : string.Empty)}{x.Type.FullName}.yml",
                             Icon = GetIconClassText(x.Type)
                         });
    }

    private ILookup<string, Item> CreateTaskTocs(IEnumerable<TypeInfo> typeInfos)
    {
        return typeInfos
            .GroupBy(x => x.Assembly.Name.Name,
                x => x.Type, (name, type) => new
                                             {
                                                 AssemblyName = name,
                                                 Items = CreateItems(type, name, removeAssemblyFromName: true)
                                             })
            .SelectMany(x => x.Items, (x, item) => new { x.AssemblyName, Item = item })
            .ToLookup(x => x.AssemblyName, x => x.Item);
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

    private Kind GetKind(TypeDefinition typeDefinition)
    {
        if (IsEntryType(typeDefinition))
            return Kind.Entry;
        if (IsServerType(typeDefinition))
            return Kind.Servers;
        if (IsInjectionAttribute(typeDefinition))
            return Kind.Injection;
        if (typeDefinition.Name.EndsWith("Tasks") && typeDefinition.BaseType?.FullName != typeof(Enumeration).FullName)
            return Kind.Tasks;

        return Kind.None;
    }

    private enum Kind
    {
        None = 0,
        Entry = 1,
        Injection = 2,
        Servers = 3,
        Tasks = 4
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
    private class Item
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
