using RimWorld;
using Verse;

namespace HemophageWarcasket;

[DefOf]
public static class HPWC_DefOf
{
    public static HediffDef HPWC_HemogenRush;

    static HPWC_DefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(HPWC_DefOf));
}
