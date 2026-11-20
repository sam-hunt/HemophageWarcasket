using HarmonyLib;
using Verse;

namespace HemophageWarcasket.Patches;

// Puts a pilot's deathrest glow out as the deathrest ends (DeathrestGlow has why the game
// does not). Runs once per deathrest ended, for any pawn, and rebuilds graphics only for a
// wearer of the set. The hediff has left the pawn's set by now, so the pawn no longer reads
// as deathresting. Verified on 1.6 and 1.7.
[HarmonyPatch(typeof(Hediff_Deathrest), nameof(Hediff_Deathrest.PostRemoved))]
public static class Hediff_Deathrest_PostRemoved
{
    public static void Postfix(Hediff_Deathrest __instance) => DeathrestGlow.Refresh(__instance.pawn);
}
