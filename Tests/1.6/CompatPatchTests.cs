#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Xunit;

namespace HemophageWarcasket.Tests;

// The gated compat roots under 1.6/Mods/<Mod>/Patches/ only ever run with their mod active, and
// 1.6/Patches/SOS2Patch.xml sits behind a PatchOperationFindMod gate of its own, so a renamed node
// in a def would break any of them silently for every player who has that mod and nobody else.
// Every operation's xpath must resolve against the main tree's defs, merged as the game merges
// them, to one node per def it names (the SOS2 patch names its two defs in one xpath, as VFEP's
// does). A PatchOperationConditional's own xpath is the exception, a probe for a node that should
// be absent.
//
// Every root here patches our own defs. A root that patches a DLC's or another mod's def needs
// that def's XML merged in as well; the Overlord repo's copy of this file (its DlcRoots table)
// is the precedent.
//
// A patch that adds fields to the Dismember ability's extension is held to the fields the C#
// declares, read off the source so the test needs no game type: the game reports an unknown
// field as an error at load, which again only that mod's players would see. A drain source on
// the hemogenic is also held to a floor on its target's hemogen, without which an empty target
// fills the pilot all the same.
public class CompatPatchTests
{
    private static readonly string RepoRoot = FindRepoRoot();

    private static string FindRepoRoot()
    {
        for (var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory); dir != null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "HemophageWarcasket.sln")))
                return dir.FullName;
        }
        throw new InvalidOperationException("HemophageWarcasket.sln not found above the test output folder");
    }

    private static XDocument MergedDefs()
    {
        var defs = new XElement("Defs");
        foreach (var file in Directory.EnumerateFiles(Path.Combine(RepoRoot, "1.6/Defs"), "*.xml", SearchOption.AllDirectories))
            defs.Add(XDocument.Load(file).Root!.Elements());
        return new XDocument(defs);
    }

    public static TheoryData<string> PatchFiles()
    {
        var data = new TheoryData<string>();
        foreach (var file in Directory.EnumerateFiles(Path.Combine(RepoRoot, "1.6"), "*.xml", SearchOption.AllDirectories)
                     .Where(f => f.Replace('\\', '/').Contains("/Patches/"))
                     .OrderBy(f => f, StringComparer.Ordinal))
            data.Add(file.Substring(RepoRoot.Length + 1));
        return data;
    }

    [Theory]
    [MemberData(nameof(PatchFiles))]
    public void EveryXPath_ResolvesToOneNodePerNamedDef(string relativePath)
    {
        var defs = MergedDefs();
        var patch = XDocument.Load(Path.Combine(RepoRoot, relativePath));
        var operations = patch.Descendants().Where(e => e.Attribute("Class") != null && e.Element("xpath") != null);
        Assert.NotEmpty(operations);
        foreach (var op in operations)
        {
            var xpath = op.Element("xpath")!.Value;
            var hits = defs.XPathSelectElements(xpath).Count();
            var expected = (string)op.Attribute("Class") == "PatchOperationConditional"
                ? 0
                : Regex.Matches(xpath, "defName\\s*=\\s*\"").Count;
            Assert.True(hits == expected, $"{relativePath}: {xpath} resolved {hits} node(s), expected {expected}");
        }
    }

    private static string[] PublicFields(string sourceFile) =>
        Regex.Matches(File.ReadAllText(Path.Combine(RepoRoot, "Source/1.6/Abilities", sourceFile)),
                @"^\s*public\s+[\w<>\[\], ]+?\s+(\w+)\s*(?:=[^;]*)?;", RegexOptions.Multiline)
            .Cast<Match>().Select(m => m.Groups[1].Value).ToArray();

    // The <value> of every operation that patches the Dismember ability's extension.
    private static List<XElement> DismemberExtensionPatchValues()
    {
        const string extensionXPath = "li[@Class=\"HemophageWarcasket.DismemberExtension\"]";
        return PatchFiles().Select(row => XDocument.Load(Path.Combine(RepoRoot, (string)row[0])))
            .SelectMany(patch => patch.Descendants().Where(e => e.Element("xpath")?.Value.EndsWith(extensionXPath, StringComparison.Ordinal) == true))
            .Select(op => op.Element("value")!)
            .ToList();
    }

    [Fact]
    public void DismemberExtensionPatches_NameOnlyDeclaredFields()
    {
        var extensionFields = PublicFields("DismemberExtension.cs");
        var sourceFile = File.ReadAllText(Path.Combine(RepoRoot, "Source/1.6/Abilities/DismemberDrainSource.cs"));
        var sourceFields = PublicFields("DismemberDrainSource.cs");
        var targetKinds = Regex.Matches(Regex.Match(sourceFile, @"enum DrainTargetKind\s*\{(.*?)\}", RegexOptions.Singleline).Groups[1].Value,
            @"^\s*(\w+),", RegexOptions.Multiline).Cast<Match>().Select(m => m.Groups[1].Value).ToArray();
        Assert.Contains("drainSources", extensionFields);
        Assert.Contains("pilotGene", sourceFields);
        Assert.NotEmpty(targetKinds);

        var values = DismemberExtensionPatchValues();
        Assert.NotEmpty(values);
        foreach (var field in values.Elements())
            Assert.True(extensionFields.Contains(field.Name.LocalName), $"DismemberExtension has no field {field.Name.LocalName}");
        foreach (var source in values.Elements("drainSources").Elements("li"))
        {
            foreach (var field in source.Elements())
                Assert.True(sourceFields.Contains(field.Name.LocalName), $"DismemberDrainSource has no field {field.Name.LocalName}");
            Assert.Contains((string?)source.Element("target"), targetKinds);
        }
    }

    [Fact]
    public void HemogenicDrainSources_SetAFloorOnTheTargetsHemogen()
    {
        var sources = DismemberExtensionPatchValues().Elements("drainSources").Elements("li")
            .Where(source => (string?)source.Element("target") == "Hemogenic").ToList();
        Assert.NotEmpty(sources);
        foreach (var source in sources)
        {
            var floor = float.Parse(source.Element("minTargetHemogen")?.Value ?? "0", CultureInfo.InvariantCulture);
            Assert.True(floor > 0f, $"the drain source for {(string?)source.Element("pilotGene")} sets no minTargetHemogen");
        }
    }

    // The game loads one XML file per load-root-relative path per mod: DirectXmlLoader.
    // XmlAssetsInModFolder keys every Defs/ and Patches/ file by that path and keeps the
    // later-listed root's (1.6 and 1.7), so a Patches/<File>.xml of the same name in two roots
    // silently loses one of them. Checked per game version, over the roots LoadFolders.xml lists.
    [Fact]
    public void NoTwoLoadRoots_ShareADefOrPatchPath()
    {
        var loadFolders = XDocument.Load(Path.Combine(RepoRoot, "LoadFolders.xml"));
        foreach (var version in loadFolders.Root!.Elements())
        {
            var roots = version.Elements("li").Select(li => li.Value.Trim().Trim('/')).ToList();
            Assert.NotEmpty(roots);
            var shared = roots
                .SelectMany(root => new[] { "Defs", "Patches" }
                    .Select(folder => Path.Combine(RepoRoot, root, folder))
                    .Where(Directory.Exists)
                    .SelectMany(dir => Directory.EnumerateFiles(dir, "*.xml", SearchOption.AllDirectories))
                    .Select(file => (root, path: file.Substring(Path.Combine(RepoRoot, root).TrimEnd(Path.DirectorySeparatorChar).Length + 1).Replace('\\', '/'))))
                .GroupBy(f => f.path, StringComparer.Ordinal)
                .Where(g => g.Count() > 1)
                .Select(g => $"{g.Key} in {string.Join(" and ", g.Select(f => f.root.Length == 0 ? "/" : f.root))}")
                .ToList();
            Assert.True(shared.Count == 0, $"{version.Name.LocalName}: load roots share {string.Join("; ", shared)}");
        }
    }
}
