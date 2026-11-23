using System.Collections.Generic;
using RimWorld;
using Verse;

namespace HemophageWarcasket;

// Ability_Dismember's drain: when one follows the strike, who it is on and what a tick of it
// takes (JobDriver_Dismember runs it, Ability_Dismember.cs has the ability as a whole).
//
// The drain follows the strike onto a target that is bleeding, from the stump, from the wound
// of a limb that held or from anything else: HediffSet.BleedRateTotal above zero, which is
// zero for a deathresting pawn and one that cannot bleed, and which the stump of an artificial
// limb adds nothing to (HediffSet.AddDirect leaves it not fresh, 1.6 and 1.7). It is asked on
// every tick of the drain, which ends when the bleeding does and keeps what it collected. It
// ends with the target's life as well, to the strike or to the blood loss: the job is on the
// pawn, and the dead are not drained.
//
// Who is drained is Bloodfeed's rule: a humanlike that bleeds and is no bloodfeeder
// (GeneUtility.IsBloodfeeder, what the Bloodfeed ability's canTargetBloodfeeders refuses, 1.6).
// The extension's drainSources widen that for a pilot with the gene each names, to animals or
// to the hemogenic, bloodfeeders among them, and the first that fits a target is what the drain
// is: it can scale the blood loss by the target's size, stop at a floor on the target's hemogen
// and name a hediff the feeding leaves on the pilot. Every drain takes the other feedingHediffs
// off the pilot as it starts, so the plain drain clears them all, as Bloodfeed does under a mod
// that keeps such hediffs.
public partial class Ability_Dismember
{
    // Whether a drain can run or go on: one could (DrainsFrom), and the target is bleeding now.
    public bool CanDrain(Pawn target) => DrainsFrom(target) && target.health.hediffSet.BleedRateTotal > 0f;

    // Whether the pilot could drain this target once it bleeds: the def sets a drain, the pilot
    // has an active hemogen gene with room for more, and the target is a living one the pilot
    // feeds on that can bleed and holds the hemogen its source asks for.
    private bool DrainsFrom(Pawn target)
    {
        DismemberExtension ext = Extension;
        if (ext.drainTicks <= 0 || ext.hemogenGain <= 0f)
            return false;
        Gene_Hemogen hemogen = pawn?.genes?.GetFirstGeneOfType<Gene_Hemogen>();
        if (hemogen == null || !hemogen.CanOffset || hemogen.Value >= hemogen.Max)
            return false;
        return target is { Spawned: true, Dead: false } && FeedsOn(target, out DismemberDrainSource source)
            && source?.BelowFloor(target) != true && target.health.CanBleed;
    }

    // Whether the pilot drains such a target at all, and through which of the extension's
    // wider sources: none for the plain drain.
    private bool FeedsOn(Pawn target, out DismemberDrainSource source)
    {
        source = null;
        List<DismemberDrainSource> sources = Extension.drainSources;
        if (sources != null && pawn != null)
        {
            for (int i = 0; i < sources.Count; i++)
            {
                if (sources[i].Fits(pawn, target))
                {
                    source = sources[i];
                    return true;
                }
            }
        }
        return target.RaceProps.Humanlike && !target.IsBloodfeeder();
    }

    // As a drain starts: the pilot now carries the mark of this feeding and of no other.
    public void MarkFeeding(Pawn target)
    {
        FeedsOn(target, out DismemberDrainSource source);
        HediffDef mark = source?.hediff;
        HediffSet hediffs = pawn.health.hediffSet;
        List<HediffDef> marks = Extension.feedingHediffs;
        if (marks != null)
        {
            for (int i = 0; i < marks.Count; i++)
            {
                if (marks[i] != mark && hediffs.GetFirstHediffOfDef(marks[i]) is { } other)
                    pawn.health.RemoveHediff(other);
            }
        }
        if (mark != null && !hediffs.HasHediff(mark))
            pawn.health.AddHediff(mark);
    }

    // One tick's share of a full drain, as SanguophageUtility.DoBite gives its whole one (1.6).
    // The blood loss goes last because it can kill the target.
    public void DrainTick(Pawn target)
    {
        DismemberExtension ext = Extension;
        float share = 1f / ext.drainTicks;
        float gain = ext.hemogenGain * target.BodySize * share;
        GeneUtility.OffsetHemogen(pawn, gain);
        GeneUtility.OffsetHemogen(target, -gain);
        RefreshStun(target);
        HealthUtility.AdjustSeverity(target, HediffDefOf.BloodLoss, FullBloodLoss(target) * share);
    }

    // BloodLoss severity a drain run to its end adds to this target: the extension's, scaled
    // by the target's size when its source says so.
    private float FullBloodLoss(Pawn target)
    {
        FeedsOn(target, out DismemberDrainSource source);
        float bySize = source?.bloodLossFactorByBaseBodySize?.Evaluate(target.RaceProps.baseBodySize) ?? 1f;
        return Extension.targetBloodLoss * bySize;
    }

    // What a drain run to its end would do to this target's blood, from where it stands now:
    // the sum the cursor warns by (Ability_Dismember.Foresight.cs has it) and an AI pilot
    // spares a life by.
    private DrainOutlook FullDrainOutlook(Pawn target)
    {
        float lost = target.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss)?.Severity ?? 0f;
        return DismemberDrain.Outlook(HediffDefOf.BloodLoss, lost + FullBloodLoss(target),
            deathless: target.genes?.HasActiveGene(GeneDefOf.Deathless) == true);
    }

    public int DrainTicks => Extension.drainTicks;
}
