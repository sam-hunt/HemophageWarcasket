using Verse;

namespace HemophageWarcasket;

// The light on a deathresting pilot's cell (ThingDefs_Misc/DeathrestLight.xml): an unseen
// thing whose CompGlower puts it on the map's glow grid when it spawns and takes it off when
// it goes, which is all the glow grid is asked to do for one deathrest. DeathrestGlow.Sync
// spawns it. It removes itself, on its rare tick, once its pilot is no longer deathresting on
// its cell: carried off, moved, dead, out of the set or with the glow switched off. A pilot
// who is carried or dead is not ticked as a deathrester, so nothing else would.
//
// Not saved (the def's isSaveable), so it holds no reference a save could keep.
public class DeathrestLight : ThingWithComps
{
    public Pawn Pilot { get; set; }

    public override void TickRare()
    {
        base.TickRare();
        Pawn pilot = Pilot;
        if (pilot == null || pilot.Map != Map || pilot.Position != Position || !DeathrestGlow.Lights(pilot))
            Destroy();
    }
}
