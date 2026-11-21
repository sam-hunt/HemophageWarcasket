using RimWorld;
using Verse;

namespace HemophageWarcasket;

// Which targets a drain source opens to a pilot with its gene.
public enum DrainTargetKind
{
    // Any animal.
    Animal,

    // A humanlike with an active hemogen gene, bloodfeeders included.
    Hemogenic,
}

// One wider source of hemogen for the pads' drain than the plain one (the header of
// Ability_Dismember.Drain.cs has the rule), listed on the ability's DismemberExtension. The
// main tree lists none: each comes from a compat root, with the mod whose gene it reads, and
// its values and their rationale are in that root's patch.
public class DismemberDrainSource
{
    // The gene the pilot needs active.
    public GeneDef pilotGene;

    public DrainTargetKind target;

    // What draining such a target leaves on the pilot, among the extension's feedingHediffs.
    public HediffDef hediff;

    // Scales the extension's targetBloodLoss by the target's RaceProps.baseBodySize. Unset
    // leaves it as it is.
    public SimpleCurve bloodLossFactorByBaseBodySize;

    // The drain stops at a hemogenic target with less hemogen than this, on Biotech's 0..1
    // scale, and never starts on one. Zero asks for nothing.
    public float minTargetHemogen;

    public bool Fits(Pawn pilot, Pawn victim)
    {
        if (pilotGene == null || pilot.genes?.HasActiveGene(pilotGene) != true)
            return false;
        return target switch
        {
            DrainTargetKind.Animal => victim.RaceProps.Animal,
            _ => victim.RaceProps.Humanlike && victim.genes?.GetFirstGeneOfType<Gene_Hemogen>() != null,
        };
    }

    public bool BelowFloor(Pawn victim) =>
        minTargetHemogen > 0f && victim.genes?.GetFirstGeneOfType<Gene_Hemogen>() is { } hemogen
        && hemogen.Value < minTargetHemogen;
}
