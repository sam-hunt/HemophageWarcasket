#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Verse;
using Xunit;

namespace HemophageWarcasket.Tests;

// The hemophage trait's work profile (WorkProfile.Derive and Compose) against bare work type
// defs shaped as vanilla's, written from the contract in the trait def's header: which work
// types the settings may toggle and their defaults, and the tags and types a choice writes.
// VFEP's unpatched tags with the defaults must give the shipped trait, which the trait and
// armor XML on disk must agree with.
public class WorkProfileTests
{
    private const WorkTags VfepTags = WorkTags.ManualSkilled | WorkTags.ManualDumb | WorkTags.Animals
        | WorkTags.Artistic | WorkTags.Caring | WorkTags.Intellectual;

    private const WorkTags BaseTags = WorkTags.ManualSkilled | WorkTags.Cleaning | WorkTags.Animals
        | WorkTags.Artistic | WorkTags.Caring | WorkTags.Intellectual;

    private const WorkTags ShippedTags = WorkTags.ManualSkilled | WorkTags.Cleaning | WorkTags.Animals
        | WorkTags.Artistic | WorkTags.Caring;

    private static WorkTypeDef Work(string defName, WorkTags tags) =>
        new() { defName = defName, workTags = tags | WorkTags.AllWork };

    private static WorkTypeDef Common(string defName, WorkTags tags) => Work(defName, tags | WorkTags.Commoner);

    private static readonly WorkTypeDef Firefighter = Common("Firefighter", WorkTags.Firefighting);
    private static readonly WorkTypeDef Doctor = Common("Doctor", WorkTags.Caring);
    private static readonly WorkTypeDef Childcare = Work("Childcare", WorkTags.Social | WorkTags.Caring);
    private static readonly WorkTypeDef Warden = Work("Warden", WorkTags.Social);
    private static readonly WorkTypeDef Handling = Common("Handling", WorkTags.Animals);
    private static readonly WorkTypeDef Cooking = Common("Cooking", WorkTags.Cooking | WorkTags.ManualSkilled);
    private static readonly WorkTypeDef Hunting = Common("Hunting", WorkTags.Violent | WorkTags.Hunting | WorkTags.Shooting);
    private static readonly WorkTypeDef Construction = Common("Construction", WorkTags.ManualSkilled | WorkTags.Constructing);
    private static readonly WorkTypeDef Smithing = Common("Smithing", WorkTags.Crafting | WorkTags.ManualSkilled);
    private static readonly WorkTypeDef Art = Common("Art", WorkTags.Artistic);
    private static readonly WorkTypeDef Crafting = Common("Crafting", WorkTags.Crafting | WorkTags.ManualSkilled);
    private static readonly WorkTypeDef Hauling = Common("Hauling", WorkTags.ManualDumb | WorkTags.Hauling);
    private static readonly WorkTypeDef Cleaning = Common("Cleaning", WorkTags.ManualDumb | WorkTags.Cleaning);
    private static readonly WorkTypeDef Research = Common("Research", WorkTags.Intellectual);
    private static readonly WorkTypeDef DarkStudy = Common("DarkStudy", WorkTags.Intellectual);
    private static readonly WorkTypeDef ModdedBoth = Common("ModdedBoth", WorkTags.Intellectual | WorkTags.ManualSkilled);

    // The work tab's order.
    private static readonly List<WorkTypeDef> All =
    [
        Firefighter, Doctor, Childcare, Warden, Handling, Cooking, Hunting, Construction,
        Smithing, Art, Crafting, Hauling, Cleaning, Research, DarkStudy,
    ];

    private static readonly List<WorkTypeDef> NotToggleable = [Firefighter, Warden, Hunting, Hauling];

    private static WorkProfile.Profile Vfep() => WorkProfile.Derive(VfepTags, [], All);

    private static Func<WorkTypeDef, bool> Only(params WorkTypeDef[] open) => open.Contains;

    private static bool Blocked(WorkTypeDef work, WorkTags tags, List<WorkTypeDef> types) =>
        (work.workTags & tags) != 0 || types.Contains(work);

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

    private static XElement Def(string relativePath) =>
        XDocument.Load(Path.Combine(RepoRoot, relativePath)).Root!.Elements().Single();

    [Fact]
    public void Derive_UnpatchedVfep_TogglesBlockedTypesIntellectualFirstThenWorkTabOrder()
    {
        WorkProfile.Profile profile = Vfep();
        Assert.Equal(
            [Research, DarkStudy, Doctor, Childcare, Handling, Cooking, Construction, Smithing, Art, Crafting, Cleaning],
            profile.toggleable);
        Assert.Equal([Research, DarkStudy], profile.openByDefault.OrderBy(All.IndexOf));
    }

    [Fact]
    public void Derive_UnpatchedVfep_LeavesHaulingFirefighterWardenAndHuntingOut()
    {
        WorkProfile.Profile profile = Vfep();
        foreach (WorkTypeDef work in NotToggleable)
            Assert.DoesNotContain(work, profile.toggleable);
    }

    [Fact]
    public void Derive_SwapsManualDumbForCleaningInBaseTags()
    {
        Assert.Equal(BaseTags, Vfep().baseTags);
    }

    [Fact]
    public void Compose_Defaults_GiveTheShippedTrait()
    {
        WorkProfile.Profile profile = Vfep();
        (WorkTags tags, List<WorkTypeDef> types) = WorkProfile.Compose(profile, profile.openByDefault.Contains);
        Assert.Equal(ShippedTags, tags);
        Assert.Empty(types);
    }

    [Fact]
    public void ShippedTraitXml_AgreesWithTheDefaultsAndHasNoListedTypes()
    {
        XElement trait = Def("1.6/Defs/TraitDefs/WarcasketTrait_Hemophage.xml");
        Assert.Equal("HPWC_WarcasketTrait_Hemophage", (string?)trait.Element("defName"));
        string[] tags = trait.Element("disabledWorkTags")!.Elements("li").Select(li => li.Value.Trim()).ToArray();
        string[] expected = ShippedTags.ToString().Split([", "], StringSplitOptions.None);
        Assert.Equal(expected.OrderBy(s => s), tags.OrderBy(s => s));
        Assert.Null(trait.Element("disabledWorkTypes"));
    }

    [Fact]
    public void ArmorDef_GrantsTheOwnTraitAndNeverVfepsOnEquip()
    {
        XElement armor = Def("1.6/Defs/ThingDefs_Misc/Warcasket_Hemophage.xml");
        Assert.Contains(armor.Descendants("traitsOnEquip").Elements("li"), li => li.Value.Trim() == "HPWC_WarcasketTrait_Hemophage");
        Assert.DoesNotContain(armor.DescendantsAndSelf(), e => !e.HasElements && e.Value.Trim() == "VFEP_WarcasketTrait");
    }

    [Fact]
    public void Compose_EveryRowClosed_IsVfepsBlockWithIntellectualAndNoListedTypes()
    {
        (WorkTags tags, List<WorkTypeDef> types) = WorkProfile.Compose(Vfep(), _ => false);
        Assert.Equal(BaseTags, tags);
        Assert.Empty(types);
    }

    [Fact]
    public void Compose_ResearchOpenDarkStudyClosed_ClearsIntellectualAndListsDarkStudy()
    {
        (WorkTags tags, List<WorkTypeDef> types) = WorkProfile.Compose(Vfep(), Only(Research));
        Assert.Equal(BaseTags & ~WorkTags.Intellectual, tags);
        Assert.Equal([DarkStudy], types);
    }

    [Fact]
    public void Compose_OpeningOneManualSkilledType_ClearsTheTagAndListsTheOthersByName()
    {
        (WorkTags tags, List<WorkTypeDef> types) = WorkProfile.Compose(Vfep(), Only(Smithing));
        Assert.Equal(BaseTags & ~WorkTags.ManualSkilled, tags);
        Assert.Equal([Cooking, Construction, Crafting], types);
        Assert.DoesNotContain(Smithing, types);
    }

    [Fact]
    public void Compose_OpeningDoctor_ClearsCaringAndListsChildcare()
    {
        (WorkTags tags, List<WorkTypeDef> types) = WorkProfile.Compose(Vfep(), Only(Doctor));
        Assert.Equal(BaseTags & ~WorkTags.Caring, tags);
        Assert.Equal([Childcare], types);
    }

    [Fact]
    public void Compose_OpeningCleaning_ClearsTheCleaningTagAndListsNothing()
    {
        (WorkTags tags, List<WorkTypeDef> types) = WorkProfile.Compose(Vfep(), Only(Cleaning));
        Assert.Equal(BaseTags & ~WorkTags.Cleaning, tags);
        Assert.Empty(types);
    }

    [Fact]
    public void Derive_VfepListedTypeNoTagBlocks_IsToggleableClosedByDefaultAndListedWhenClosed()
    {
        WorkProfile.Profile profile = WorkProfile.Derive(WorkTags.None, [Warden], All);
        Assert.Equal([Warden], profile.toggleable);
        Assert.DoesNotContain(Warden, profile.openByDefault);

        (WorkTags tags, List<WorkTypeDef> types) = WorkProfile.Compose(profile, profile.openByDefault.Contains);
        Assert.Equal(WorkTags.None, tags);
        Assert.Equal([Warden], types);

        (tags, types) = WorkProfile.Compose(profile, Only(Warden));
        Assert.Equal(WorkTags.None, tags);
        Assert.Empty(types);
    }

    [Fact]
    public void Derive_PatchModEmptiedVfepsTags_TogglesNothingAndComposesNothing()
    {
        WorkProfile.Profile profile = WorkProfile.Derive(WorkTags.None, [], All);
        Assert.Empty(profile.toggleable);
        (WorkTags tags, List<WorkTypeDef> types) = WorkProfile.Compose(profile, profile.openByDefault.Contains);
        Assert.Equal(WorkTags.None, tags);
        Assert.Empty(types);
    }

    [Fact]
    public void Derive_ModdedTypeWithIntellectualAndManualSkilled_IsToggleableOnceAndOpenByDefault()
    {
        List<WorkTypeDef> all = [.. All, ModdedBoth];
        WorkProfile.Profile profile = WorkProfile.Derive(VfepTags, [], all);
        Assert.Single(profile.toggleable, ModdedBoth);
        Assert.Contains(ModdedBoth, profile.openByDefault);

        (WorkTags tags, List<WorkTypeDef> types) = WorkProfile.Compose(profile, profile.openByDefault.Contains);
        Assert.Equal(WorkTags.None, tags & (WorkTags.Intellectual | WorkTags.ManualSkilled));
        Assert.Equal(ShippedTags & ~WorkTags.ManualSkilled, tags);
        Assert.Equal([Cooking, Construction, Smithing, Crafting], types);
    }

    [Fact]
    public void Compose_BlocksExactlyTheToggleableTypesThatAreNotOpen()
    {
        WorkProfile.Profile profile = Vfep();
        var openSets = new List<(string name, Func<WorkTypeDef, bool> open)>
        {
            ("defaults", profile.openByDefault.Contains),
            ("none", _ => false),
            ("all", _ => true),
        };
        foreach (WorkTypeDef single in All)
            openSets.Add(($"only {single.defName}", Only(single)));

        foreach ((string name, Func<WorkTypeDef, bool> open) in openSets)
        {
            (WorkTags tags, List<WorkTypeDef> types) = WorkProfile.Compose(profile, open);
            foreach (WorkTypeDef work in All)
            {
                bool toggleable = profile.toggleable.Contains(work);
                bool expected = toggleable && !open(work);
                Assert.True(expected == Blocked(work, tags, types), $"{work.defName} under open set '{name}': expected blocked={expected}");
            }
            foreach (WorkTypeDef work in NotToggleable)
                Assert.False(Blocked(work, tags, types), $"{work.defName} blocked under open set '{name}'");
        }
    }
}
