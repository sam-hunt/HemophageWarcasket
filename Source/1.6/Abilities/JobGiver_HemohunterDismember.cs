using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace HemophageWarcasket;

// Gives a pilot in a hemohunter mental state the pads' strike in place of that state's bite
// (ThinkTreeDefs/HemohunterDismember.xml has where it sits in the think tree and whose state
// it is). Ability_Dismember.StrikesAsHemohunter decides whether this pilot strikes; with no
// strike loaded, or no target for one, this gives no job and the state's own giver bites.
//
// The job is the ability's own, started as any think tree job is: Pawn_JobTracker.StartJob
// sets CurJob before the driver's TryMakePreToilReservations (1.6 and 1.7), which is what
// JobDriver_Dismember binds on. The ability's pawn is set here first, since its checks read it
// and VEF leaves it null until the first gizmo draw. Doors are bashed and designations
// ignored, as the bite's job has them.
//
// The prey is the bite's, the humanlikes on the map with no active hemogen gene, ghouls
// aside, and then only those the strike is valid on and the drain feeds from. While the
// ability's extension says so (hemohunterHuntsByGene) it is also what the pilot's drain
// sources feed on: the pawns a source opens, and the corpses on the map when a source is for
// those (Ability_Dismember has the rule for a corpse).
//
// Of that prey the pilot takes the nearest it can reach, by straight distance, as vanilla's
// berserk takes its own (JobGiver_Berserk, through AttackTargetFinder.BestAttackTarget to
// GenClosest.ClosestThing_Global, 1.6), with no limit on how far that is. The bite draws its
// prey at random over the map, which over prey this wide would walk a pilot past everyone
// near it to a far animal or an old corpse. The distance is tested first, so only prey nearer
// than the nearest so far is asked anything else, and the path last.
//
// The tree this sits in is asked on most job searches of every humanlike, so the mental
// state test comes first and only a pawn in one goes any further.
public class JobGiver_HemohunterDismember : ThinkNode_JobGiver
{
    protected override Job TryGiveJob(Pawn pawn)
    {
        if (!pawn.InMentalState || !pawn.Spawned)
            return null;
        Ability_Dismember ability = JobDriver_Dismember.FindAbility(pawn);
        if (ability == null)
            return null;
        ability.pawn = pawn;
        if (!ability.StrikesAsHemohunter())
            return null;
        Thing target = FindPrey(pawn, ability);
        if (target == null)
            return null;
        Job job = JobMaker.MakeJob(ability.def.jobDef, target);
        job.canBashDoors = true;
        job.ignoreDesignations = true;
        return job;
    }

    private static Thing FindPrey(Pawn pilot, Ability_Dismember ability)
    {
        Thing nearest = null;
        int nearestDistance = int.MaxValue;
        IReadOnlyList<Pawn> pawns = pilot.Map.mapPawns.AllPawnsSpawned;
        for (int i = 0; i < pawns.Count; i++)
        {
            Pawn other = pawns[i];
            if (other != pilot && !other.IsGhoul && IsNearer(pilot, other, nearestDistance, out int distance)
                && (IsBitePrey(other) || ability.HuntsByGene(other))
                && ability.CanHunt(other) && pilot.CanReach(other, PathEndMode.Touch, Danger.Deadly))
            {
                nearest = other;
                nearestDistance = distance;
            }
        }
        if (ability.HuntsCorpses())
        {
            List<Thing> corpses = pilot.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
            for (int i = 0; i < corpses.Count; i++)
            {
                if (corpses[i] is Corpse corpse && IsNearer(pilot, corpse, nearestDistance, out int distance)
                    && ability.CanHunt(corpse) && pilot.CanReach(corpse, PathEndMode.Touch, Danger.Deadly))
                {
                    nearest = corpse;
                    nearestDistance = distance;
                }
            }
        }
        return nearest;
    }

    private static bool IsNearer(Pawn pilot, Thing prey, int than, out int distance)
    {
        distance = pilot.Position.DistanceToSquared(prey.Position);
        return distance < than;
    }

    // The prey of the state's own bite.
    private static bool IsBitePrey(Pawn other) =>
        other.RaceProps.Humanlike && other.genes?.GetFirstGeneOfType<Gene_Hemogen>() == null;
}
