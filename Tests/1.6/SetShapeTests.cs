#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace HemophageWarcasket.Tests;

// Two things about the set's three defs, and one about the mod icon, that nothing else fails
// on, read off the XML and the art on disk.
//
// Art: a texture path that names no file is not an error anywhere in the build. The game draws
// a pink box, and the deploy manifest copies whatever is there. Each piece must have the file
// every graphic it declares will load: the item's Graphic_Single reads the bare path (and
// <path>_m under CutoutComplex), the worn Graphic_Multi reads <path>_north, _east and _south
// (and each one's <facing>m under useWornGraphicMask; west is the east facing mirrored), and so
// does the Graphic_Multi of each apparel.renderNodeProperties entry (the hemogen glow).
//
// Text: the rules translators are told to keep (CONTRIBUTING.md). A piece's shortDescription is
// its description's first paragraph, and the three pieces share their second and third.
//
// Mod icon: the settings window's helmet header draws ModMetaData.Icon, which is safe across a
// play-data reload only while the game loads it from About/ModIcon.png, that is while About.xml
// names no modIconPath (Settings_Helmet's header has the mechanism).
public class SetShapeTests
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

    private static readonly string[] Pieces = ["Warcasket", "WarcasketShoulders", "WarcasketHelmet"];
    private static readonly string[] Facings = ["north", "east", "south"];

    private static XElement Def(string piece)
    {
        var path = Path.Combine(RepoRoot, "1.6/Defs/ThingDefs_Misc", $"{piece}_Hemophage.xml");
        var def = XDocument.Load(path).Root!.Elements().Single();
        Assert.Equal($"HPWC_{piece}_Hemophage", (string?)def.Element("defName"));
        return def;
    }

    // Def text separates paragraphs with a literal backslash-n pair, which the game unescapes.
    private static string[] Paragraphs(XElement def) =>
        ((string)def.Element("description")!).Split(["\\n\\n"], StringSplitOptions.None);

    public static TheoryData<string> PieceNames()
    {
        var data = new TheoryData<string>();
        foreach (var piece in Pieces)
            data.Add(piece);
        return data;
    }

    [Theory]
    [MemberData(nameof(PieceNames))]
    public void EveryDeclaredGraphic_HasItsFilesOnDisk(string piece)
    {
        var def = Def(piece);
        var graphic = def.Element("graphicData")!;
        var apparel = def.Element("apparel")!;
        var expected = new List<string>();

        var item = (string)graphic.Element("texPath")!;
        expected.Add(item);
        if ((string?)graphic.Element("shaderType") == "CutoutComplex")
            expected.Add(item + "_m");

        var worn = (string)apparel.Element("wornGraphicPath")!;
        var masked = (bool?)apparel.Element("useWornGraphicMask") == true;
        foreach (var facing in Facings)
        {
            expected.Add($"{worn}_{facing}");
            if (masked)
                expected.Add($"{worn}_{facing}m");
        }

        var nodes = apparel.Element("renderNodeProperties")!.Elements("li").ToList();
        Assert.NotEmpty(nodes);
        foreach (var node in nodes)
            expected.AddRange(Facings.Select(facing => $"{(string)node.Element("texPath")!}_{facing}"));

        var missing = expected.Where(p => !File.Exists(Path.Combine(RepoRoot, "Textures", p + ".png"))).ToList();
        Assert.True(missing.Count == 0, $"{piece}_Hemophage names art that is not under Textures/: {string.Join(", ", missing)}");
    }

    [Theory]
    [MemberData(nameof(PieceNames))]
    public void MaskedItemAndWornGraphics_GoTogether(string piece)
    {
        var def = Def(piece);
        var itemMasked = (string?)def.Element("graphicData")!.Element("shaderType") == "CutoutComplex";
        var wornMasked = (bool?)def.Element("apparel")!.Element("useWornGraphicMask") == true;
        Assert.True(itemMasked == wornMasked, $"{piece}_Hemophage masks only one of its two graphics, so the icon and the worn piece would tint differently");
    }

    [Fact]
    public void ShortDescription_IsTheDescriptionsFirstParagraph()
    {
        foreach (var def in Pieces.Select(Def))
            Assert.Equal((string?)def.Element("shortDescription"), Paragraphs(def)[0]);
    }

    [Fact]
    public void Descriptions_ShareTheGenerationAndSeriesParagraphs()
    {
        var defs = Pieces.Select(Def).ToList();
        Assert.All(defs, d => Assert.Equal(3, Paragraphs(d).Length));
        Assert.Single(defs.Select(d => Paragraphs(d)[1]).Distinct());
        var series = defs.Select(d => Paragraphs(d)[2]).Distinct().ToList();
        Assert.Single(series);
        Assert.StartsWith("The Hemophage series ", series[0]);
    }

    [Fact]
    public void ModIcon_IsTheFileUnderAbout()
    {
        var about = XDocument.Load(Path.Combine(RepoRoot, "About/About.xml")).Root!;
        Assert.True(about.Element("modIconPath") == null,
            "About.xml sets a modIconPath: the mod icon becomes a content texture, which a play-data reload destroys under the helmet settings header");
        Assert.True(File.Exists(Path.Combine(RepoRoot, "About/ModIcon.png")), "About/ModIcon.png is missing: the helmet settings header falls back to vanilla's generic mod icon");
    }
}
