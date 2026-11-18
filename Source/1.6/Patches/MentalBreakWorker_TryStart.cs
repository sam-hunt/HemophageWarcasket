using HarmonyLib;
using Verse;
using Verse.AI;

namespace HemophageWarcasket.Patches;

// Lets only a share of Biotech's fire terror breaks start for a pawn the helmet steadies
// (PyrophobiaRelief has the mechanic and why it is taken here). TryStart runs when a mental
// break is about to start and at no other time, so this is no hot path; every other break
// leaves on the def comparison. MentalBreakWorker_FireTerror does not override TryStart, so
// the base method is the one the gene calls. A break caused by mood is let through: the
// relief is for the gene's own roll, the only caller that starts this break in play. A dev
// tool that forces the break on a pilot is thinned as the gene's roll is.
[HarmonyPatch(typeof(MentalBreakWorker), nameof(MentalBreakWorker.TryStart))]
public static class MentalBreakWorker_TryStart
{
    public static bool Prefix(MentalBreakWorker __instance, Pawn pawn, bool causedByMood, ref bool __result)
    {
        if (__instance.def != HPWC_DefOf.FireTerror || causedByMood || pawn == null)
            return true;
        if (Rand.Chance(PyrophobiaRelief.BreakChanceFactor(pawn)))
            return true;
        __result = false;
        return false;
    }
}
