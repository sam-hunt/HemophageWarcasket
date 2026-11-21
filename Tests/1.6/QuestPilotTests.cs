#nullable enable

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using RimWorld.QuestGen;
using Xunit;

namespace HemophageWarcasket.Tests;

// The quest pilot's pure rule (QuestPilotExtension.ChanceFor): each source has a chance of its
// own. Then the def wiring, checked off the XML since the WarcasketDef type pulls in VFEP, which
// the test output never carries: the armor declares the extension once, with chances in range,
// names the set's other two pieces, each a def of this mod, and offers research options that end
// on the set's own research. Last, the two vanilla methods the patches bind by name, on the
// game's 1.6 assembly: a rename there would otherwise only show as a warning in a player's log.
public class QuestPilotTests
{
    private const string DefsFolder = "1.6/Defs/ThingDefs_Misc";
    private const string ArmorFile = "Warcasket_Hemophage.xml";
    private const string ArmorDefName = "HPWC_Warcasket_Hemophage";
    private const string ExtensionClass = "HemophageWarcasket.QuestPilotExtension";

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

    private static XElement ArmorDef() =>
        XDocument.Load(Path.Combine(RepoRoot, DefsFolder, ArmorFile)).Root!.Elements().Single();

    private static XElement ExtensionEntry() =>
        ArmorDef().Element("modExtensions")!.Elements("li")
            .Single(li => (string?)li.Attribute("Class") == ExtensionClass);

    private static string[] Names(XElement list) => list.Elements("li").Select(li => li.Value.Trim()).ToArray();

    [Theory]
    [InlineData(QuestPilotSource.ShipMaster, 0.2f)]
    [InlineData(QuestPilotSource.MeetingLeader, 0.5f)]
    [InlineData(QuestPilotSource.MeetingGuest, 0.9f)]
    public void ChanceFor_IsTheSourcesOwn(QuestPilotSource source, float expected)
    {
        var extension = new QuestPilotExtension
        {
            shipMasterChance = 0.2f,
            meetingLeaderChance = 0.5f,
            meetingGuestChance = 0.9f,
        };

        Assert.Equal(expected, extension.ChanceFor(source));
    }

    [Fact]
    public void Armor_DeclaresTheExtensionOnce()
    {
        XElement armor = ArmorDef();
        Assert.Equal(ArmorDefName, (string?)armor.Element("defName"));

        Assert.Single(armor.Element("modExtensions")!.Elements("li"),
            li => (string?)li.Attribute("Class") == ExtensionClass);
    }

    [Theory]
    [InlineData("shipMasterChance")]
    [InlineData("meetingLeaderChance")]
    [InlineData("meetingGuestChance")]
    public void Armor_ChancesAreWithinZeroToOne(string field)
    {
        XElement? element = ExtensionEntry().Element(field);

        Assert.NotNull(element);
        float chance = float.Parse(element!.Value, CultureInfo.InvariantCulture);
        Assert.InRange(chance, 0f, 1f);
    }

    [Fact]
    public void Armor_NamesTheSetsOtherTwoPieces()
    {
        string[] pieces = Names(ExtensionEntry().Element("otherPieces")!);

        Assert.Equal(
            new[] { "HPWC_WarcasketShoulders_Hemophage", "HPWC_WarcasketHelmet_Hemophage" }.OrderBy(n => n),
            pieces.OrderBy(n => n));
        foreach (string piece in pieces)
        {
            string file = Path.Combine(RepoRoot, DefsFolder, piece.Replace("HPWC_", "") + ".xml");
            Assert.True(File.Exists(file), $"no def file {file}");
            Assert.Equal(piece, (string?)XDocument.Load(file).Root!.Elements().Single().Element("defName"));
        }
    }

    [Fact]
    public void Armor_ResearchOptionsAreDistinct_AndContainTheRequiredOne()
    {
        XElement entry = ExtensionEntry();
        string[] options = Names(entry.Element("researchOptions")!);

        Assert.NotEmpty(options);
        Assert.Equal(options.Length, options.Distinct().Count());
        string? required = ((string?)entry.Element("requiredResearch"))?.Trim();
        if (required != null)
            Assert.Contains(required, options);
    }

    // The options run from the earliest project to the set's own, which the armor's own
    // research prerequisite names.
    [Fact]
    public void Armor_ResearchOptionsEndOnTheSetsOwnResearch()
    {
        string last = Names(ExtensionEntry().Element("researchOptions")!).Last();
        string[] own = Names(ArmorDef().Element("researchPrerequisites")!);

        Assert.Contains(last, own);
    }

    // Patches/QuestNode_Root_SanguophageShip_RunInt and
    // Patches/QuestNode_Root_SanguophageMeetingHost_RunInt: the method only. The slate names
    // their postfixes read are set in the method bodies, out of a test's reach.
    [Theory]
    [InlineData(typeof(QuestNode_Root_SanguophageShip))]
    [InlineData(typeof(QuestNode_Root_SanguophageMeetingHost))]
    public void Vanilla_HasTheQuestsRunInt(Type quest)
    {
        MethodInfo? method = quest.GetMethod(
            "RunInt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

        Assert.NotNull(method);
        Assert.Empty(method!.GetParameters());
    }
}
