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
// every tick of the drain, which ends when the bleeding does and keeps what it collected.
//
// The drain is on the target's blood, alive or dead. A target that dies, to the strike or
// during the drain, is drained on as a corpse (JobDriver_Dismember follows it there) until its
// blood runs out: the drain adds its blood loss to the dead pawn as to a living one, and stops
// at the severity a pawn dies of blood loss at, so a corpse gives what blood it had left and
// one that bled to death gives none. A corpse is not held to the reach a living target is: it
// goes nowhere on its own, and Pawn.Kill puts it on the nearest free cell when the target died
// standing on an item. Pawn.Kill places the corpse before TakeDamage returns and keeps the
// pawn in it, hediffs and genes included; Pawn_HealthTracker.CanBleed and
// Pawn.BodySize do not ask whether the pawn lives, and CheckForStateChange returns at once
// for a dead one, so a severity change on a corpse does nothing but move the number (1.6, the
// same on 1.7).
//
// Who is drained is Bloodfeed's rule: a humanlike that bleeds and is no bloodfeeder
// (GeneUtility.IsBloodfeeder, what the Bloodfeed ability's canTargetBloodfeeders refuses, 1.6).
// The extension's drainSources widen that for a pilot with the gene each names, to animals or
// to the hemogenic, bloodfeeders among them, and the first that fits a target is what the drain
// is: it can scale the blood loss by the target's size, stop at a floor on the target's hemogen
// and name a hediff the feeding leaves on the pilot. Every drain takes the other feedingHediffs
// off the pilot as it starts, so the plain drain clears them all, as Bloodfeed does under a mod
// that keeps such hediffs. The corpse of a target struck alive is drained as what it was. No
// drain is on a dessicated corpse, which has no blood in it.
//
// A corpse found lying is drained through a source of the Corpse kind and through nothing else,
// which only a hemohunter's hunt comes to (Ability_Dismember.Hunt.cs). That drain is the one a
// target that died gets, on what blood the corpse has left, and leaves the corpse where it lay.
public partial class Ability_Dismember
{
    // Whether a drain can run or go on: one could (DrainsFrom), and the target is bleeding now
    // or dead (see the header). foundDead, here and below, says the pilot came to the target
    // as a corpse lying on the map, which only a source of the Corpse kind feeds from.
    public bool CanDrain(Pawn target, bool foundDead) =>
        DrainsFrom(target, foundDead) && (target.Dead || target.health.hediffSet.BleedRateTotal > 0f);

    // Whether the pilot could drain this target once it bleeds: the def sets a drain, the pilot
    // has an active hemogen gene with room for more, and the target is one the pilot feeds on
    // that can bleed, alive or a corpse on the map with blood left in it, and holds the hemogen
    // its source asks for.
    private bool DrainsFrom(Pawn target, bool foundDead)
    {
        DismemberExtension ext = Extension;
        if (ext.drainTicks <= 0 || ext.hemogenGain <= 0f)
            return false;
        Gene_Hemogen hemogen = pawn?.genes?.GetFirstGeneOfType<Gene_Hemogen>();
        if (hemogen == null || !hemogen.CanOffset || hemogen.Value >= hemogen.Max)
            return false;
        Thing body = target.Dead ? target.Corpse : target;
        return body is { Spawned: true } && FeedsOn(target, foundDead, out DismemberDrainSource source)
            && source?.BelowFloor(target) != true && target.health.CanBleed && !BledOut(target)
            && (body as Corpse)?.GetRotStage() != RotStage.Dessicated;
    }

    // Whether the pilot drains such a target at all, and through which of the extension's
    // wider sources: none for the plain drain, which is never on a corpse found lying (see the
    // header).
    private bool FeedsOn(Pawn target, bool foundDead, out DismemberDrainSource source)
    {
        source = null;
        List<DismemberDrainSource> sources = Extension.drainSources;
        if (sources != null && pawn != null)
        {
            for (int i = 0; i < sources.Count; i++)
            {
                if (sources[i].Fits(pawn, target, foundDead))
                {
                    source = sources[i];
                    return true;
                }
            }
        }
        return !foundDead && target.RaceProps.Humanlike && !target.IsBloodfeeder();
    }

    // As a drain starts: the pilot now carries the mark of this feeding and of no other.
    public void MarkFeeding(Pawn target, bool foundDead)
    {
        FeedsOn(target, foundDead, out DismemberDrainSource source);
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

    // Vanilla's own test for a hediff that kills, which only a corpse can pass.
    private static bool BledOut(Pawn target) =>
        target.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss)?.CauseDeathNow() == true;

    // One tick's share of a full drain, as SanguophageUtility.DoBite gives its whole one (1.6),
    // the same from a corpse. The blood loss goes last because it can kill a living target.
    public void DrainTick(Pawn target, bool foundDead)
    {
        DismemberExtension ext = Extension;
        float share = 1f / ext.drainTicks;
        float gain = ext.hemogenGain * target.BodySize * share;
        GeneUtility.OffsetHemogen(pawn, gain);
        GeneUtility.OffsetHemogen(target, -gain);
        RefreshStun(target);
        HealthUtility.AdjustSeverity(target, HediffDefOf.BloodLoss, FullBloodLoss(target, foundDead) * share);
    }

    // BloodLoss severity a drain run to its end adds to this target: the extension's, scaled
    // by the target's size when its source says so.
    private float FullBloodLoss(Pawn target, bool foundDead)
    {
        FeedsOn(target, foundDead, out DismemberDrainSource source);
        float bySize = source?.bloodLossFactorByBaseBodySize?.Evaluate(target.RaceProps.baseBodySize) ?? 1f;
        return Extension.targetBloodLoss * bySize;
    }

    public int DrainTicks => Extension.drainTicks;
}
