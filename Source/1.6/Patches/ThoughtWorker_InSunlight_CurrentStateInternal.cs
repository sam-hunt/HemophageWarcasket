using HarmonyLib;
using RimWorld;
using Verse;

namespace HemophageWarcasket.Patches;

// Drops a sunlight sensitivity gene's mood penalty for a pawn the helmet shields
// (SunlightShield has the mechanic). One worker serves every such thought, each def naming
// its gene in requiredGenes, which the situational thought handler tests before it asks the
// worker; so this runs for carriers of such a gene only, on the handler's own slow interval.
// The method is protected, hence the name as a string. Verified on 1.6 and 1.7.
[HarmonyPatch(typeof(ThoughtWorker_InSunlight), "CurrentStateInternal")]
public static class ThoughtWorker_InSunlight_CurrentStateInternal
{
    public static void Postfix(Pawn p, ref ThoughtState __result)
    {
        if (__result.Active && SunlightShield.Protects(p))
            __result = ThoughtState.Inactive;
    }
}
