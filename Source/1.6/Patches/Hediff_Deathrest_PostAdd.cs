using HarmonyLib;
using Verse;

namespace HemophageWarcasket.Patches;

// Lights a pilot's deathrest glow as the deathrest begins (DeathrestGlow has why the game
// does not). Runs once per deathrest begun, for any pawn, and rebuilds graphics only for a
// wearer of the set. The hediff is in the pawn's set by now, so the pawn reads as
// deathresting. Verified on 1.6 and 1.7.
[HarmonyPatch(typeof(Hediff_Deathrest), nameof(Hediff_Deathrest.PostAdd))]
public static class Hediff_Deathrest_PostAdd
{
    public static void Postfix(Hediff_Deathrest __instance) => DeathrestGlow.Refresh(__instance.pawn);
}
