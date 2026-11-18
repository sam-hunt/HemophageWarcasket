using Verse;

namespace HemophageWarcasket;

// Helmet section: the one switch for the helmet's sunlight protection, written onto the
// blocksSunlight field of the helmet def's SunlightShieldExtension. The patches read the field
// off the worn piece on every call (SunlightShield), so a write reaches a pilot in the sun at
// once but for the Sight cap of another mod's gene, which VEF's comp sets again within 500
// ticks. The header carries the helmet's own item icon, read off the def at draw time as the
// ability sections read theirs.
public partial class HemophageWarcasketSettings
{
    public readonly ToggleSetting helmetSunlight = new("HelmetSunlight");

    private ThingDef helmetDef;

    private void ExposeHelmetSettings() => helmetSunlight.ExposeData();

    private void LoadHelmetDefaults(Defs defs)
    {
        helmetSunlight.DefaultValue = defs.helmetShield.blocksSunlight;
        helmetDef = defs.helmet;
    }

    private void ApplyHelmetToDefs(Defs defs) => defs.helmetShield.blocksSunlight = helmetSunlight.Value;

    private void ResetHelmetSettings() => helmetSunlight.Reset();

    private void DrawHelmetSection(Listing_Standard listing, bool first = false)
    {
        SectionHeader(listing, "HPWC_SectionHelmet", helmetDef.uiIcon,
            HeaderTip("HPWC_SectionHelmet".Translate(), "HPWC_SectionHelmetDesc".Translate()), first);
        helmetSunlight.DoRow(listing);
    }
}
