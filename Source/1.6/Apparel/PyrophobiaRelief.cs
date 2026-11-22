using System.Collections.Generic;
using RimWorld;
using Verse;

namespace HemophageWarcasket;

// Pyrophobia relief: a pawn wearing a piece whose def carries a PyrophobiaReliefExtension
// flees fire in terror less often. Biotech's FireTerror gene rolls every 60 ticks against a
// mean time of 0.1 days (Gene.TickInterval, on GeneDef.mentalBreakMtbDays) and, with a fire
// in sight within 19.9 cells, starts the FireTerror break through its worker's TryStart. The
// gene def's mean time is one number for every carrier, so the relief is taken where the
// break starts instead (Patches/MentalBreakWorker_TryStart): each break goes ahead with the
// chance this returns, which thins the gene's rolls exactly, a chance of 0.5 being a mean
// time of 0.2 days. The gene's mood penalty near fire is untouched. Verified on 1.6 and 1.7.
public static class PyrophobiaRelief
{
    // The lowest factor among the worn pieces, 1 for a pawn wearing none.
    public static float BreakChanceFactor(Pawn pawn)
    {
        List<Apparel> worn = pawn.apparel?.WornApparel;
        float factor = 1f;
        if (worn == null)
            return factor;
        for (int i = 0; i < worn.Count; i++)
        {
            if (worn[i].def.GetModExtension<PyrophobiaReliefExtension>() is { } relief && relief.breakChanceFactor < factor)
                factor = relief.breakChanceFactor;
        }
        return factor;
    }
}
