using System.Collections.Generic;
using Verse;

namespace HemophageWarcasket;

// Ability_Dismember's part in a hemohunter's hunt: whether the pilot hunts with the strike, and
// what it may take (Ability_Dismember.cs has the ability as a whole).
//
// A pilot in one of the extension's hemohunterStates hunts with the strike instead of the
// bite that state's own mod gives it (JobGiver_HemohunterDismember), while hemohunterStrikes
// is on, the button would be enabled, so a strike is loaded and off cooldown and is spent as on
// any cast, the prey is one the drain feeds from, so never with the drain set to nothing, and
// the pilot's genes make aggressive mental breaks likelier
// (Pawn_GeneTracker.AggroMentalBreakSelectionChanceFactor above 1, vanilla's product over the
// active genes, 1.6 and 1.7: Biotech's aggressive and hyper-aggressive, and any mod's stronger
// one). The prey is the bite's, and while hemohunterHuntsByGene is on also whatever the
// pilot's drainSources open to it: a hunter holds to no diet.
//
// A source of the Corpse kind opens a humanlike's corpse lying on the map, to a hunt and to
// nothing else: no order of the player's targets a corpse, and the job's check asks for the
// hunt again once the pilot stands over it. Ability_Dismember.Strike.cs has what the strike
// does to such a corpse and Ability_Dismember.Drain.cs what the drain takes from it.
public partial class Ability_Dismember
{
    // Whether the pilot, bound and in a mental state, hunts with the strike (see the header).
    public bool StrikesAsHemohunter()
    {
        DismemberExtension ext = Extension;
        return ext is { hemohunterStrikes: true, hemohunterStates: not null } && pawn?.MentalStateDef is { } state
            && ext.hemohunterStates.Contains(state) && pawn.genes?.AggroMentalBreakSelectionChanceFactor > 1f
            && IsEnabledForPawn(out _);
    }

    // Whether a hunt may take this pawn: a valid target for the strike that the drain could
    // then feed on.
    public bool CanHunt(Pawn target) => ValidateTarget(target, showMessages: false) && DrainsFrom(target);

    // Whether a hunt reaches past the bite's prey for this pawn: the def lets it, and one of
    // the extension's wider sources opens the pawn to the pilot.
    public bool HuntsByGene(Pawn target) =>
        Extension.hemohunterHuntsByGene && FeedsOn(target, foundDead: false, out DismemberDrainSource source) && source != null;

    // Whether a hunt looks at corpses at all: the def lets it reach past the bite's prey, and
    // the pilot has the gene of a source for them.
    public bool HuntsCorpses()
    {
        List<DismemberDrainSource> sources = Extension.drainSources;
        if (!Extension.hemohunterHuntsByGene || sources == null || pawn == null)
            return false;
        for (int i = 0; i < sources.Count; i++)
        {
            if (sources[i].target == DrainTargetKind.Corpse && sources[i].OpenTo(pawn))
                return true;
        }
        return false;
    }

    // Whether a hunt may take this corpse, found lying: the dead pawn has a part for the
    // strike and the drain could feed on it, which only a source for corpses lets it.
    public bool CanHunt(Corpse corpse) =>
        Extension.hemohunterHuntsByGene && corpse is { Spawned: true, InnerPawn: { } dead }
        && DismemberTarget.CanStrike(dead, Extension, out _) && DrainsFrom(dead, foundDead: true);

    // The job's check toil over a corpse: only a hunt strikes one, so the pilot must still be
    // hunting, which asks for a loaded strike as the check over a living target does.
    public bool CheckBeforeWarmup(Corpse corpse) => StrikesAsHemohunter() && CanHunt(corpse);
}
