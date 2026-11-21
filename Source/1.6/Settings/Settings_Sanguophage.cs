using UnityEngine;
using Verse;

namespace HemophageWarcasket;

// Vanilla Races Expanded - Sanguophage section, drawn last: the two switches for how Dismember
// behaves for a hemohunter pilot. On the HPWC_Dismember DismemberExtension, hemohunterStrikes
// (key DismemberHemohunter) decides whether a pilot in a hemohunter state strikes at all and
// hemohunterHuntsByGene (key HemohunterByGene) whether that pilot also hunts what the
// extension's drainSources feed on. The main tree leaves hemohunterStates and drainSources
// empty and that mod's compat root fills them, so the section stands only while the first is
// filled: without a hemohunter state nothing of it draws, header included, and the second row
// also needs a drain source to widen the hunt with.
//
// The header carries Vanilla Expanded's logo, UI/VELogo, which Vanilla Expanded Framework
// ships and which is always loaded with this mod. A mod-shipped content texture is destroyed
// by a play-data reload while this type's initializer never runs again, so VeLogo resolves it
// again on a Unity-null check, never ??. SectionHeader draws no icon for a null one.
public partial class HemophageWarcasketSettings
{
    private const string VeLogoPath = "UI/VELogo";

    private static Texture2D veLogo;

    private static Texture2D VeLogo
    {
        get
        {
            // Unity-overloaded ==, deliberately not ?? (Source/1.6/CLAUDE.md).
            if (veLogo == null)
                veLogo = ContentFinder<Texture2D>.Get(VeLogoPath, reportFailure: false);
            return veLogo;
        }
    }

    public readonly ToggleSetting dismemberHemohunter = new("DismemberHemohunter");
    public readonly ToggleSetting hemohunterByGene = new("HemohunterByGene");

    // Whether the extension names a hemohunter state, so the strike box has something to do,
    // and whether it names a drain source, so the hunt box has something to widen.
    private bool dismemberHasHemohunters;
    private bool dismemberHasDrainSources;

    private bool SanguophageSectionShown => abilityDefaultsLoaded && dismemberHasHemohunters;

    private void ExposeSanguophageSettings()
    {
        dismemberHemohunter.ExposeData();
        hemohunterByGene.ExposeData();
    }

    private void LoadSanguophageDefaults(Defs defs)
    {
        DismemberExtension ext = defs.dismemberExtension;
        dismemberHemohunter.DefaultValue = ext.hemohunterStrikes;
        hemohunterByGene.DefaultValue = ext.hemohunterHuntsByGene;
        dismemberHasHemohunters = !ext.hemohunterStates.NullOrEmpty();
        dismemberHasDrainSources = !ext.drainSources.NullOrEmpty();
    }

    private void ApplySanguophageToDefs(Defs defs)
    {
        defs.dismemberExtension.hemohunterStrikes = dismemberHemohunter.Value;
        defs.dismemberExtension.hemohunterHuntsByGene = hemohunterByGene.Value;
    }

    private void ResetSanguophageSettings()
    {
        dismemberHemohunter.Reset();
        hemohunterByGene.Reset();
    }

    private void DrawSanguophageSection(Listing_Standard listing)
    {
        if (!SanguophageSectionShown)
            return;
        SectionHeader(listing, "HPWC_SectionSanguophage", VeLogo,
            HeaderTip("HPWC_SectionSanguophage".Translate(), "HPWC_SectionSanguophageDesc".Translate()));
        dismemberHemohunter.DoRow(listing);
        if (dismemberHasDrainSources)
            hemohunterByGene.DoRow(listing);
    }
}
