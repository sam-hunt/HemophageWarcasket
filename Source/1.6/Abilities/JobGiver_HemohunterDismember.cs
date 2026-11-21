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
// The prey is drawn as the bite's is, at random among the humanlikes on the map with no active
// hemogen gene that the pilot can reach, ghouls aside, and then only those the strike is valid
// on and the drain feeds from. While the ability's extension says so (hemohunterHuntsByGene)
// the draw is also over what the pilot's drain sources feed on, each at the same odds as any
// other prey: the pawns a source opens, and the corpses on the map when a source is for
// those (Ability_Dismember has the rule for a corpse). The tree this sits in is asked on most
// job searches of every humanlike, so the mental state test comes first and only a pawn in
// one goes any further.
public class JobGiver_HemohunterDismember : ThinkNode_JobGiver
{
    // Filled and read within one TryGiveJob call.
    private static readonly List<Thing> Prey = [];

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
        Prey.Clear();
        IReadOnlyList<Pawn> pawns = pilot.Map.mapPawns.AllPawnsSpawned;
        for (int i = 0; i < pawns.Count; i++)
        {
            Pawn other = pawns[i];
            if (other != pilot && !other.IsGhoul && (IsBitePrey(other) || ability.HuntsByGene(other))
                && ability.CanHunt(other) && pilot.CanReach(other, PathEndMode.Touch, Danger.Deadly))
            {
                Prey.Add(other);
            }
        }
        if (ability.HuntsCorpses())
        {
            List<Thing> corpses = pilot.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
            for (int i = 0; i < corpses.Count; i++)
            {
                if (corpses[i] is Corpse corpse && ability.CanHunt(corpse) && pilot.CanReach(corpse, PathEndMode.Touch, Danger.Deadly))
                    Prey.Add(corpse);
            }
        }
        Thing prey = Prey.Count > 0 ? Prey.RandomElement() : null;
        Prey.Clear();
        return prey;
    }

    // The prey of the state's own bite.
    private static bool IsBitePrey(Pawn other) =>
        other.RaceProps.Humanlike && other.genes?.GetFirstGeneOfType<Gene_Hemogen>() == null;
}
