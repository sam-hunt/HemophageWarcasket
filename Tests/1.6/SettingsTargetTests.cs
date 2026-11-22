#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace HemophageWarcasket.Tests;

// The def fields the ability, helmet and quest settings read their defaults from and write back
// onto, read off the XML. The settings find the abilities' and the helmet's by name at startup,
// all or nothing (HemophageWarcasketSettings.TryGetDefs): with one missing, the game logs a
// single warning and the settings window drops those sections, which no build or boot fails on.
// The quest section finds the armor's extension by itself and is dropped the same way. Each field
// is spelled out in the XML rather than left to its C# default so the def stays the record of
// what a slider's default is.
public class SettingsTargetTests
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

    private static XElement Def(string folder, string file, string defName)
    {
        var def = XDocument.Load(Path.Combine(RepoRoot, "1.6/Defs", folder, file)).Root!.Elements().Single();
        Assert.Equal(defName, (string?)def.Element("defName"));
        return def;
    }

    private static void AssertHas(XElement parent, params string[] fields)
    {
        foreach (string field in fields)
            Assert.True(parent.Element(field) != null, $"<{parent.Name.LocalName}> has no <{field}>");
    }

    [Fact]
    public void HemogenRush_HasEveryFieldItsSettingsWrite()
    {
        XElement def = Def("HediffDefs", "HemogenRush.xml", "HPWC_HemogenRush");
        XElement comp = def.Element("comps")!.Elements("li")
            .Single(li => (string?)li.Attribute("Class") == "HemophageWarcasket.HediffCompProperties_HemogenRush");
        AssertHas(comp, "hemogenStartCost", "hemogenPerSecond", "hemogenReserve", "glowPulses");
        XElement stage = def.Element("stages")!.Elements("li").Single();
        AssertHas(stage, "statFactors");
        AssertHas(stage.Element("statFactors")!, "MoveSpeed", "MeleeDamageFactor");
        // The move speed row reads the blood pumping offset to show the rush's whole gain.
        AssertHas(stage, "capMods");
        AssertHas(stage.Element("capMods")!.Elements("li")
            .Single(li => (string?)li.Element("capacity") == "BloodPumping"), "offset");
    }

    [Fact]
    public void HemogenRushAbility_SharesTheHediffsDefName()
    {
        Def("AbilityDefs", "HemogenRush.xml", "HPWC_HemogenRush");
    }

    [Fact]
    public void Dismember_HasEveryFieldItsSettingsWrite()
    {
        XElement def = Def("AbilityDefs", "Dismember.xml", "HPWC_Dismember");
        AssertHas(def, "castTime", "cooldownTime");
        XElement extension = def.Element("modExtensions")!.Elements("li")
            .Single(li => (string?)li.Attribute("Class") == "HemophageWarcasket.DismemberExtension");
        AssertHas(extension, "damage", "drainTicks", "hemogenGain", "targetBloodLoss", "bloodFilthCount",
            "canTargetNonHostile");
    }

    // The settings write the pool in per-charge mode, so the def must declare that mode, on
    // the comp that survives a lowered capacity, and the ability must name the command that
    // shows the count.
    [Fact]
    public void DismemberPool_HasEveryFieldItsSettingsWrite()
    {
        XElement pads = Def("ThingDefs_Misc", "WarcasketShoulders_Hemophage.xml", "HPWC_WarcasketShoulders_Hemophage");
        XElement pool = pads.Element("comps")!.Elements("li")
            .Single(li => (string?)li.Attribute("Class") == "CompProperties_ApparelReloadable");
        AssertHas(pool, "maxCharges", "ammoDef", "ammoCountPerCharge", "chargeNoun");
        Assert.True((int)pool.Element("maxCharges")! > 0 && (int)pool.Element("ammoCountPerCharge")! > 0);
        Assert.Null(pool.Element("ammoCountToRefill"));
        Assert.Equal("HemophageWarcasket.CompStrikePool", (string?)pool.Element("compClass"));
        XElement ability = Def("AbilityDefs", "Dismember.xml", "HPWC_Dismember");
        Assert.Equal("HemophageWarcasket.Command_Dismember", (string?)ability.Element("gizmoClass"));
    }

    [Fact]
    public void Helmet_HasEveryFieldItsSettingsWrite()
    {
        XElement helmet = Def("ThingDefs_Misc", "WarcasketHelmet_Hemophage.xml", "HPWC_WarcasketHelmet_Hemophage");
        XElement shield = helmet.Element("modExtensions")!.Elements("li")
            .Single(li => (string?)li.Attribute("Class") == "HemophageWarcasket.SunlightShieldExtension");
        AssertHas(shield, "blocksSunlight");
        XElement relief = helmet.Element("modExtensions")!.Elements("li")
            .Single(li => (string?)li.Attribute("Class") == "HemophageWarcasket.PyrophobiaReliefExtension");
        AssertHas(relief, "breakChanceFactor");
    }

    // requiredResearch is left out: it is absent while the default is no requirement.
    [Fact]
    public void QuestPilot_HasEveryFieldItsSettingsWrite()
    {
        XElement armor = Def("ThingDefs_Misc", "Warcasket_Hemophage.xml", "HPWC_Warcasket_Hemophage");
        XElement pilot = armor.Element("modExtensions")!.Elements("li")
            .Single(li => (string?)li.Attribute("Class") == "HemophageWarcasket.QuestPilotExtension");
        AssertHas(pilot, "shipMasterChance", "shipMasterMinPoints", "meetingLeaderChance", "meetingGuestChance",
            "thrallBudgetShare", "researchOptions");
    }
}
