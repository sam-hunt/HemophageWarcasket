using HarmonyLib;
using Verse;

namespace HemophageWarcasket.Patches;

// Keeps a deathresting pilot's light where the pilot is (DeathrestGlow.Sync): after a load,
// after the pilot is carried to a casket, and when a piece or the gene comes or goes during
// the deathrest. The target runs for deathresting pawns only, on each of their tick
// intervals, and the postfix leaves on one hash test but for once every CheckTicks, a second
// at normal speed, when it walks the pawn's worn apparel and the things on its cell.
// Verified on 1.6 and 1.7.
[HarmonyPatch(typeof(Hediff_Deathrest), nameof(Hediff_Deathrest.PostTickInterval))]
public static class Hediff_Deathrest_PostTickInterval
{
    private const int CheckTicks = 60;

    public static void Postfix(Hediff_Deathrest __instance, int delta)
    {
        Pawn pawn = __instance.pawn;
        if (pawn.IsHashIntervalTick(CheckTicks, delta))
            DeathrestGlow.Sync(pawn);
    }
}
