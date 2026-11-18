using HarmonyLib;
using RimWorld;
using Verse;

namespace HemophageWarcasket.Patches;

// Drops a sunlight sensitivity gene's stat factors for a pawn the helmet shields
// (SunlightShield has the mechanic).
//
// This is a hot path, so the postfix does nothing of its own until vanilla has answered "in
// sunlight". StatWorker.GetValueUnfinalized calls Applies for every conditional affecter of
// every active gene, twice (the offsets pass, then the factors pass) and for every stat it
// computes, not only the stats the affecter lists; a pawn without such a gene never reaches
// it. A false result, the answer for any pawn under a roof or at night, leaves at the first
// test, and a true one costs one walk of the worn apparel, without allocating. A stat that a
// gene's conditional affecter lists is kept out of the per-thing immutable cache
// (StatDef.PopulateMutableStats), and no vanilla caller reads MoveSpeed through the timed
// cache, so the answer needs no invalidation when the helmet goes on or comes off. Verified
// on 1.6 and 1.7.
[HarmonyPatch(typeof(ConditionalStatAffecter_InSunlight), nameof(ConditionalStatAffecter_InSunlight.Applies))]
public static class ConditionalStatAffecter_InSunlight_Applies
{
    public static void Postfix(StatRequest req, ref bool __result)
    {
        if (__result && req.Thing is Pawn pawn && SunlightShield.Protects(pawn))
            __result = false;
    }
}
