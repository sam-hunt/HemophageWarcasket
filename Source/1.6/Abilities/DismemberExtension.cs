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

    // Whether a body that was built without any limbs (a centipede) loses its head. A body
    // that has lost its limbs always does.
    public bool decapitateBodiesWithoutLimbs;
}
