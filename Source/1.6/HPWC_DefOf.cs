using RimWorld;
using Verse;

namespace HemophageWarcasket;

[DefOf]
public static class HPWC_DefOf
{
    public static HediffDef HPWC_HemogenRush;

    public static DamageDef HPWC_Dismember;

    public static RulePackDef HPWC_Combat_Dismember;

    // Biotech's break for its FireTerror gene, which the helmet's pyrophobia relief thins.
    public static MentalBreakDef FireTerror;

    // Biotech's bloodfeed effecter, which the drain borrows.
    public static EffecterDef Bloodfeed_Warmup;

    static HPWC_DefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(HPWC_DefOf));
}
