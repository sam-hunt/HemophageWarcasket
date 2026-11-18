using Verse;

namespace HemophageWarcasket;

// Helmet section: the switch for the helmet's sunlight protection, written onto the
// blocksSunlight field of the helmet def's SunlightShieldExtension, and the pyrophobia slider,
// written onto the breakChanceFactor of its PyrophobiaReliefExtension, stored and shown in
// percent. The patches read both fields off the worn piece on every call (SunlightShield,
// PyrophobiaRelief), so a write reaches a pilot at once but for the Sight cap of another mod's
// gene, which VEF's comp sets again within 500 ticks. The header carries the helmet's own item icon, read off the def at draw time as the
// ability sections read theirs.
public partial class HemophageWarcasketSettings
{
    public readonly ToggleSetting helmetSunlight = new("HelmetSunlight");

    // The share of a pyrophobic pilot's fire terror breaks that still happen, from none to
    // all of them. Shown as a chance so that lower reads as calmer, where the mean time
    // between breaks it stands for would read the other way.
    public readonly Tunable helmetPyrophobia = new("HelmetPyrophobia", 0f, 100f, 10f, Percent);

    private ThingDef helmetDef;

    private void ExposeHelmetSettings()
    {
        helmetSunlight.ExposeData();
        helmetPyrophobia.ExposeData();
    }

    private void LoadHelmetDefaults(Defs defs)
    {
        helmetSunlight.DefaultValue = defs.helmetShield.blocksSunlight;
        helmetPyrophobia.DefaultValue = defs.helmetPyrophobia.breakChanceFactor * PercentScale;
        helmetDef = defs.helmet;
    }

    private void ApplyHelmetToDefs(Defs defs)
    {
        defs.helmetShield.blocksSunlight = helmetSunlight.Value;
        defs.helmetPyrophobia.breakChanceFactor = helmetPyrophobia.Value / PercentScale;
    }

    private void ResetHelmetSettings()
    {
        helmetSunlight.Reset();
        helmetPyrophobia.Reset();
    }

    private void DrawHelmetSection(Listing_Standard listing, bool first = false)
    {
        SectionHeader(listing, "HPWC_SectionHelmet", helmetDef.uiIcon,
            HeaderTip("HPWC_SectionHelmet".Translate(), "HPWC_SectionHelmetDesc".Translate()), first);
        helmetSunlight.DoRow(listing);
        helmetPyrophobia.DoRow(listing);
    }
}
