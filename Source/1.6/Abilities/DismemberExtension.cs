using System.Collections.Generic;
using Verse;

namespace HemophageWarcasket;

// Values and their rationale are in the ability def (1.6/Defs/AbilityDefs/Dismember.xml).
public class DismemberExtension : DefModExtension
{
    // How long the drain runs, in ticks. Zero means the pads only tear.
    public int drainTicks;

    // Hemogen a full drain gives, on Biotech's 0..1 scale, before the target's body size and
    // the pilot's HemogenGainFactor.
    public float hemogenGain;

    // BloodLoss severity a full drain adds to the target.
    public float targetBloodLoss;

    // Blood filth a landed strike throws: this many pieces, each on a random cell within
    // bloodFilthRadius of the target that it can see. Zero throws none.
    public int bloodFilthCount;

    public float bloodFilthRadius;

    // Whether a pawn that is not hostile to the pilot (a colonist, a prisoner, a visitor, a
    // tame or wild animal) can be targeted.
    public bool canTargetNonHostile = true;

    // Whether a body that was built without any limbs (a diabolus) loses its head. A body that
    // has lost its limbs always does.
    public bool decapitateBodiesWithoutLimbs;

    // A body built without any limbs loses its head whatever the switch above says when its
    // Pawn.BodySize is at most this (a cobra). Zero leaves it all to the switch.
    public float decapitateWithoutLimbsMaxBodySize;

    // Wider sources for the drain than the plain one, each for a pilot with one gene; the
    // first that fits a target is what the drain is. None here: a compat root's patch adds
    // them.
    public List<DismemberDrainSource> drainSources;

    // Hediffs that mark what a pilot last fed on. A drain takes every one off the pilot but
    // its own source's. None here either.
    public List<HediffDef> feedingHediffs;

    // Mental states in which a pilot's hunt for blood is a strike while one is loaded
    // (JobGiver_HemohunterDismember). None here either.
    public List<MentalStateDef> hemohunterStates;

    // Whether a pilot in one of those states strikes at all.
    public bool hemohunterStrikes = true;
}
