using System.Collections.Generic;
using RimWorld;
using Verse;

namespace HemophageWarcasket;

// The set while its pilot deathrests: every worn piece's glow sprite beats slowly
// (PawnRenderNodeWorker_HemogenGlow draws it, HediffCompProperties_HemogenRush.GlowAlphaAt has
// the beat) and a small light sits on the pilot's cell (DeathrestLight). Both are on for a
// living, deathresting pilot with an active Hemogenic gene while the def's deathrestGlows
// stands, the light for one who is also spawned and wears a piece that draws the glow.
// Deathresting is Biotech's own test, the Deathrest hediff, so a regeneration coma and
// deathrest exhaustion light nothing. The rush and a deathrest never overlap: the rush hediff
// is removed as a deathrest starts.
//
// Biotech's Deathrest hediff asks for no graphics rebuild, and the glow node's CanDrawNow is
// read only when the pilot's draw requests are rebuilt (PawnRenderTree.ParallelPreDraw: on a
// change of posture, bed or facing, or a dirtied tree). Lying down and getting up usually
// rebuild them, a deathrest that starts or ends on a pilot who is already down does not. So
// the patches on Hediff_Deathrest call Refresh as the hediff comes and goes, and Sync on an
// interval while it lasts (Patches/Hediff_Deathrest_*). Verified on 1.6 and 1.7.
public static class DeathrestGlow
{
    private static HediffCompProperties_HemogenRush Props =>
        HPWC_DefOf.HPWC_HemogenRush.CompProps<HediffCompProperties_HemogenRush>();

    // Whether the pawn's worn pieces draw the deathrest glow. A corpse keeps the hediff it died
    // with, hence the test for death.
    public static bool ShowsOn(Pawn pawn) =>
        Props.deathrestGlows
        && !pawn.Dead
        && pawn.Deathresting
        && pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>() != null;

    // Whether a light belongs on the pawn's cell.
    public static bool Lights(Pawn pawn) => pawn.Spawned && WearsGlow(pawn) && ShowsOn(pawn);

    // The pawn's deathrest began or ended, or the switch was flipped: rebuilds a wearer's
    // graphics, portraits included, as the game does for a hediff with forceRenderTreeRecache,
    // and sets the light.
    public static void Refresh(Pawn pawn)
    {
        if (WearsGlow(pawn))
            pawn.Drawer.renderer.SetAllGraphicsDirty();
        Sync(pawn);
    }

    // Refresh for every deathresting pawn on a map, for a switch flipped during play.
    public static void RefreshAll()
    {
        if (Current.ProgramState != ProgramState.Playing)
            return;
        foreach (Map map in Find.Maps)
        {
            foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
            {
                if (pawn.Deathresting)
                    Refresh(pawn);
            }
        }
    }

    // Puts a light on the cell of a pawn who should have one and has none, and takes away the
    // light of one who should not. A light whose pilot has left its cell, the map or the
    // living removes itself (DeathrestLight); this is what gives the pilot a new one where
    // they lie now, and the only thing that lights a loaded game, as the light is not saved.
    public static void Sync(Pawn pawn)
    {
        if (!pawn.Spawned)
            return;

        DeathrestLight light = LightOf(pawn);
        if (!Lights(pawn))
        {
            light?.Destroy();
            return;
        }
        if (light != null || Props.deathrestLight == null)
            return;

        if (ThingMaker.MakeThing(Props.deathrestLight) is DeathrestLight made)
        {
            made.Pilot = pawn;
            GenSpawn.Spawn(made, pawn.Position, pawn.Map);
        }
    }

    private static DeathrestLight LightOf(Pawn pawn)
    {
        List<Thing> things = pawn.Map.thingGrid.ThingsListAtFast(pawn.Position);
        for (int i = 0; i < things.Count; i++)
        {
            if (things[i] is DeathrestLight light && light.Pilot == pawn)
                return light;
        }
        return null;
    }

    // Whether the pawn wears a piece that declares a glow node, which is what there is to
    // light: a pilot wears three pieces, each with one or two render nodes of its own.
    private static bool WearsGlow(Pawn pawn)
    {
        List<Apparel> worn = pawn.apparel?.WornApparel;
        if (worn == null)
            return false;
        for (int i = 0; i < worn.Count; i++)
        {
            List<PawnRenderNodeProperties> nodes = worn[i].def.apparel?.RenderNodeProperties;
            if (nodes == null)
                continue;
            for (int n = 0; n < nodes.Count; n++)
            {
                if (nodes[n].nodeClass == typeof(PawnRenderNode_HemogenGlow))
                    return true;
            }
        }
        return false;
    }
}
