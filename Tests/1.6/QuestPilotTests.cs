#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using RimWorld.QuestGen;
using Xunit;

namespace HemophageWarcasket.Tests;

// The quest pilot's pure rules: each source has a chance of its own, and the shuttle master's
// needs the quest's threat points at or above the floor (QuestPilotExtension.ChanceFor); a
// sanguophage is handed the gun only with Shooting above Melee (QuestPilot.FightsAtRange). Then
// the def wiring, checked off the XML since the WarcasketDef type pulls in VFEP, which the test
// output never carries: the armor declares the extension once, with chances in range and the
// floor within 0 to 10000, names the set's other two pieces, each a def of this mod, and offers
// research options that end on the set's own research, the two weapons it hands a dressed pawn,
// and the thrall budget's numbers in range with its tiers listed cheapest first, each one VFEP
// set by name. Last, the two vanilla methods the patches bind by name, on the game's 1.6
// assembly: a rename there would otherwise only show as a warning in a player's log.
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
    [InlineData(QuestPilotSource.ShipMaster, 2499f, 0f)]
    [InlineData(QuestPilotSource.ShipMaster, 2500f, 0.2f)]
    [InlineData(QuestPilotSource.ShipMaster, 6000f, 0.2f)]
    [InlineData(QuestPilotSource.MeetingLeader, 0f, 0.5f)]
    [InlineData(QuestPilotSource.MeetingGuest, 0f, 0.9f)]
    public void ChanceFor_IsTheSourcesOwn_AndTheShuttleMastersNeedsTheFloor(
        QuestPilotSource source, float points, float expected)
    {
        var extension = new QuestPilotExtension
        {
            shipMasterChance = 0.2f,
            shipMasterMinPoints = 2500f,
            meetingLeaderChance = 0.5f,
            meetingGuestChance = 0.9f,
        };

        Assert.Equal(expected, extension.ChanceFor(source, points));
    }

    [Fact]
    public void ChanceFor_WithNoFloor_IsTheShuttleMastersChanceAtZeroPoints()
    {
        var extension = new QuestPilotExtension { shipMasterChance = 0.2f, shipMasterMinPoints = 0f };

        Assert.Equal(0.2f, extension.ChanceFor(QuestPilotSource.ShipMaster, 0f));
    }

    [Theory]
    [InlineData(0, 17, false)]
    [InlineData(9, 8, true)]
    [InlineData(6, 6, false)]
    [InlineData(0, 0, false)]
    public void FightsAtRange_OnlyWithShootingAboveMelee(int shooting, int melee, bool expected)
    {
        Assert.Equal(expected, QuestPilot.FightsAtRange(shooting, melee));
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
    public void Armor_ShuttleMasterFloorIsWithinZeroToTenThousand()
    {
        XElement? element = ExtensionEntry().Element("shipMasterMinPoints");

        Assert.NotNull(element);
        Assert.InRange(float.Parse(element!.Value, CultureInfo.InvariantCulture), 0f, 10000f);
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

    [Theory]
    [InlineData("thrallBudgetShare", 0f, 1f)]
    [InlineData("thrallMinPoints", 0f, 10000f)]
    public void Armor_ThrallBudgetNumbersAreInRange(string field, float min, float max)
    {
        XElement? element = ExtensionEntry().Element(field);

        Assert.NotNull(element);
        Assert.InRange(float.Parse(element!.Value, CultureInfo.InvariantCulture), min, max);
    }

    [Fact]
    public void Armor_ThrallMinDressedIsAWholeNumberFromZero()
    {
        XElement? element = ExtensionEntry().Element("thrallMinDressed");

        Assert.NotNull(element);
        Assert.True(int.TryParse(element!.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int count));
        Assert.True(count >= 0);
    }

    [Fact]
    public void Armor_ShipMasterPriceIsPositive()
    {
        XElement? element = ExtensionEntry().Element("shipMasterPrice");

        Assert.NotNull(element);
        Assert.True(float.Parse(element!.Value, CultureInfo.InvariantCulture) > 0f);
    }

    [Theory]
    [InlineData("rangedWeapon")]
    [InlineData("meleeWeapon")]
    public void Armor_NamesTheWeaponsItHandsOut(string field)
    {
        XElement? element = ExtensionEntry().Element(field);

        Assert.NotNull(element);
        Assert.NotEmpty(element!.Value.Trim());
    }

    private static List<XElement> Tiers()
    {
        List<XElement> tiers = ExtensionEntry().Element("thrallTiers")!.Elements("li").ToList();
        Assert.NotEmpty(tiers);
        return tiers;
    }

    [Fact]
    public void ThrallTiers_EachHaveAnArmorAPriceAndTwoOtherPieces_AndNoArmorRepeats()
    {
        List<XElement> tiers = Tiers();

        foreach (XElement tier in tiers)
        {
            Assert.NotEmpty(((string?)tier.Element("armor"))?.Trim() ?? "");
            Assert.True(float.Parse(tier.Element("price")!.Value, CultureInfo.InvariantCulture) > 0f);
            Assert.Equal(2, Names(tier.Element("otherPieces")!).Length);
        }
        string[] armors = tiers.Select(t => t.Element("armor")!.Value.Trim()).ToArray();
        Assert.Equal(armors.Length, armors.Distinct().Count());
    }

    [Fact]
    public void ThrallTiers_EachNameOneVfepSet()
    {
        foreach (XElement tier in Tiers())
        {
            string armor = tier.Element("armor")!.Value.Trim();
            Assert.StartsWith("VFEP_Warcasket_", armor);
            string set = armor.Substring("VFEP_Warcasket_".Length);
            Assert.NotEmpty(set);

            Assert.Equal(
                new[] { "VFEP_WarcasketShoulders_" + set, "VFEP_WarcasketHelmet_" + set }.OrderBy(n => n),
                Names(tier.Element("otherPieces")!).OrderBy(n => n));
        }
    }

    [Fact]
    public void ThrallTiers_AreListedCheapestFirst()
    {
        float[] prices = Tiers()
            .Select(t => float.Parse(t.Element("price")!.Value, CultureInfo.InvariantCulture)).ToArray();

        Assert.Equal(prices.OrderBy(p => p), prices);
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
