// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/nuke-build/web/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Core;
using Nuke.Core.BuildServers;
using Nuke.Core.Execution;
using Nuke.Core.Tooling;
using Nuke.Core.Utilities.Collections;
using static Nuke.Core.Logger;

static class CustomToc
{
    public static void WriteCustomToc (string tocFile, IEnumerable<string> solutionFiles)
    {
        var msBuildWorkspace = MSBuildWorkspace.Create (
            new Dictionary<string, string>
            {
                { "Configuration", "Release" },
                { "TargetFramework", "net46" }
            });

        var solutions = solutionFiles.Select(x => msBuildWorkspace.OpenSolutionAsync(x).Result).ToList();

        var relevantTypeSymbols = (
                    from solution in solutions
                    from project in solution.Projects
                    let compilation = project.GetCompilationAsync().Result
                    from document in project.Documents
                    let syntaxTree = document.GetSyntaxTreeAsync().Result
                    let semanticModel = compilation.GetSemanticModel(syntaxTree)
                    from classDeclarationSyntax in syntaxTree.GetCompilationUnitRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                    let typeSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax)
                    let kind = GetKind(typeSymbol)
                    where typeSymbol.ContainingAssembly.Name != ".build" && kind != Kind.None
                    select new { TypeSymbol = typeSymbol, Kind = kind })
                .Distinct(x => x.TypeSymbol.ToDisplayString())
                .ForEachLazy(x => Info($"Found '{x.TypeSymbol.ToDisplayString()}' ({x.Kind})."))
                .ToLookup(x => x.Kind, x => x.TypeSymbol);

        var iconClasses = (
                    from solution in solutions
                    from project in solution.Projects
                    let compilation = project.GetCompilationAsync().Result
                    from document in project.Documents
                    let syntaxTree = document.GetSyntaxTreeAsync().Result
                    let semanticModel = compilation.GetSemanticModel(syntaxTree)
                    from attributeSyntax in syntaxTree.GetCompilationUnitRoot().DescendantNodes().OfType<AttributeSyntax>()
                    let attributeSymbol = semanticModel.GetSymbolInfo(attributeSyntax.Name).Symbol.ContainingType
                    where typeof(IconClassAttribute).Name.Equals(attributeSymbol.Name)
                    let arguments = attributeSyntax.ArgumentList.Arguments
                    let typeOfExpression = (TypeOfExpressionSyntax) arguments.First().Expression
                    select new
                           {
                               ClassFullName = semanticModel.GetSymbolInfo(typeOfExpression.Type).Symbol.ToDisplayString(),
                               IconClass = (string) semanticModel.GetConstantValue(arguments.Last().Expression).Value
                           })
                .ToDictionary(x => x.ClassFullName, x => x.IconClass);

        File.WriteAllText(tocFile,
            new StringBuilder()
                    .WriteBlock(Kind.Entry, relevantTypeSymbols, iconClasses)
                    .WriteBlock(Kind.Servers, relevantTypeSymbols, iconClasses)
                    .WriteBlock(Kind.Common, relevantTypeSymbols, iconClasses)
                    .WriteBlock(Kind.ThirdParty, relevantTypeSymbols, iconClasses)
                    .ToString());
    }

    enum Kind
    {
        None,
        Entry,
        Servers,
        Common,
        ThirdParty
    }

    static Kind GetKind (ITypeSymbol typeSymbol)
    {
        if (IsEntryType(typeSymbol))
            return Kind.Entry;
        if (IsServerType(typeSymbol))
            return Kind.Servers;
        if (typeSymbol.Name.EndsWith("Tasks"))
            return IsCommonType(typeSymbol)
                ? Kind.Common
                : Kind.ThirdParty;

        return Kind.None;
    }


    static StringBuilder WriteBlock (
        this StringBuilder builder,
        Kind kind,
        ILookup<Kind, INamedTypeSymbol> typeSymbols,
        IDictionary<string, string> iconClasses)
        => builder
                .AppendLine($"- separator: {(kind == Kind.ThirdParty ? "Third Party" : kind.ToString())}")
                .ForEach(typeSymbols[kind].OrderBy(x => x.Name), x => builder.WriteType(x, iconClasses));

    static StringBuilder ForEach<T> (
        this StringBuilder builder,
        IEnumerable<T> enumerable,
        Action<T> builderAction)
    {
        foreach (var item in enumerable)
            builderAction(item);
        return builder;
    }

    static StringBuilder WriteType (this StringBuilder builder, ITypeSymbol typeSymbol, IDictionary<string, string> iconClasses)
        => builder
                .AppendLine($"- uid: {typeSymbol.ToDisplayString()}")
                .AppendLine($"  name: {typeSymbol.GetName()}")
                .AppendLine($"  icon: {typeSymbol.GetIconClassText(iconClasses)}");


    static bool IsEntryType (ITypeSymbol typeSymbol)
    {
        if (IsBuildType (typeSymbol))
            return true;

        return new[]
               {
                   typeof(ControlFlow),
                   typeof(EnvironmentInfo),
                   typeof(ProcessTasks),
                   typeof(Logger),
                   typeof(DefaultSettings)
               }.Any(x => typeSymbol.ToDisplayString().Equals(x.FullName));
    }

    static bool IsBuildType (ITypeSymbol typeSymbol)
        => typeSymbol.DescendantsAndSelf(x => x.BaseType).Any(x => x.ToDisplayString().Equals(typeof(Build).FullName));

    static bool IsServerType (this ITypeSymbol typeSymbol)
        => typeSymbol.GetAttributes().Any(x => x.AttributeClass.ToDisplayString().Equals(typeof(BuildServerAttribute).FullName));

    static bool IsCommonType (ITypeSymbol typeSymbol)
        => typeSymbol.ContainingAssembly.Name == typeof(FileSystemTasks).Assembly.GetName().Name;

    static string GetName (this ITypeSymbol typeSymbol)
        => typeSymbol.Name.EndsWith("Tasks")
            ? typeSymbol.Name.Substring(startIndex: 0, length: typeSymbol.Name.Length - "Tasks".Length)
            : typeSymbol.Name;

    static string GetIconClassText (this ITypeSymbol typeSymbol, IDictionary<string, string> iconClasses)
    {
        if (iconClasses.TryGetValue(typeSymbol.ToDisplayString(), out var iconClass))
            return iconClass;
        if (IsEntryType(typeSymbol))
            return "star-full";
        if (IsServerType(typeSymbol))
            return "server";

        return "power-cord2";
    }
}
