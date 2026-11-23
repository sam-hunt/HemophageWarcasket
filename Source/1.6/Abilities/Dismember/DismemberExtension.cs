using System.Collections.Generic;
using Verse;

namespace HemophageWarcasket;

// Values and their rationale are in the ability def (1.6/Defs/AbilityDefs/Dismember.xml).
public class DismemberExtension : DefModExtension
{
    // Damage the strike does to the one part it takes hold of, before the pilot's
    // MeleeDamageFactor and the target's IncomingDamageFactor. A part with more health left
    // than what comes of it is wounded and stays on.
    public float damage;

    // Where another mod keeps a melee aim height the player sets on a pawn: the full name of
    // the ThingComp class it gives the pawn and of the BodyPartHeight field on it (MeleeAim).
    // A strike takes a limb at that height while the target has one there. None here: a compat
    // root's patch names them.
    public string aimHeightComp;

    public string aimHeightField;

    // How long the drain runs, in ticks. Zero means the pads only tear.
    public int drainTicks;

    // Hemogen a full drain gives, on Biotech's 0..1 scale, before the target's body size and
    // the pilot's HemogenGainFactor.
    public float hemogenGain;

    // BloodLoss severity a full drain adds to the target.
    public float targetBloodLoss;

    // Blood filth a strike that takes its part off throws: this many pieces, each on a random cell within
    // bloodFilthRadius of the target that it can see. Zero throws none.
    public int bloodFilthCount;

    public float bloodFilthRadius;

    // Whether a pawn that is not hostile to the pilot (a colonist, a prisoner, a visitor, a
    // tame or wild animal) can be targeted.
    public bool canTargetNonHostile = true;

    // Wider sources for the drain than the plain one, each for a pilot with one gene; the
    // first that fits a target is what the drain is. None here: a compat root's patch adds
    // them.
    public List<DismemberDrainSource> drainSources;

    // Hediffs that mark what a pilot last fed on. A drain takes every one off the pilot but
    // its own source's. None here either.
    public List<HediffDef> feedingHediffs;

    // A pilot the player does not control (Ability_Dismember.Ai.cs). With aiUses on, such a
    // pilot strikes the humanlike it is fighting once that is within aiStrikeDistance cells,
    // gives the walk up at twice that, and waits aiStrikeSpacingTicks after a strike before
    // the next, where a player's pilot waits the def's cooldownTime.
    public bool aiUses;

    public float aiStrikeDistance;

    public int aiStrikeSpacingTicks;
}
