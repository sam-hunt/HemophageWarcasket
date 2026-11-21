using RimWorld;
using Verse;

namespace HemophageWarcasket;

// Ability_Dismember's foresight: what the cursor says of a strike on the hovered pawn before
// the click, which the question before a lethal order is asked from as well
// (Ability_Dismember.cs has that and the ability as a whole).
//
// The cursor says what the strike would do (ExtraLabelMouseAttachment): Biotech's "Will kill"
// when it would take the head off, and that it will or may only wound when no part or only
// some of the parts it could take would come off (DismemberTarget.Outlook, on ForeseenDamage).
// Under that goes what the drain would then do to the target's blood, in the two warnings
// Bloodfeed gives of its bite and by the same sum
// (CompAbilityEffect_BloodfeederBite.ExtraLabelMouseAttachment, 1.6 and 1.7): the blood loss
// the target has plus a full drain's, "Will kill" when that reaches the severity blood loss
// kills at and Biotech's major blood loss line when it reaches a life threatening stage, or
// when it would kill but for the target's deathless gene (DismemberDrain.Outlook). A full drain
// is the most a drain can take, so the warning errs toward being given: a pilot whose hemogen
// fills first stops short of it. It is given when the pilot could drain the target now
// (DrainsFrom) and the target bleeds already or could bleed from the strike: it is not
// deathresting, and one of the parts the strike could take is not solid, vanilla's word for a
// part that never bleeds, an artificial limb and all below it among them (BodyPartDef.IsSolid,
// which Hediff_Injury.BleedRate and Hediff_MissingPart.IsFreshNonSolidExtremity both ask, 1.6
// and 1.7).
//
// The lines are asked for every frame of the hover and working them out walks the body, so
// what was foreseen of a pawn is kept for OutlookSeconds of real time, the game maybe paused
// while what decides it changes. The hovered pawn's own tooltip is off while the ability is
// targeting (HidePawnTooltips, as on Ideology's Convert and Counsel): it opens at the cursor,
// over those lines, and TooltipGiverList.DispenseAllThingTooltips hands out none while the
// targeting source says so (1.6 and 1.7).
public partial class Ability_Dismember
{
    // How long the cursor keeps what it foresaw of a strike on the hovered pawn.
    private const float OutlookSeconds = 0.25f;

    private Pawn foreseenFor;
    private float foreseenAt;
    private bool foreseenKill;
    private DrainOutlook foreseenDrain;
    private string foreseenLines;

    // The cursor's lines in place of the hovered pawn's tooltip (see the header).
    public override bool HidePawnTooltips => true;

    // What the strike on the hovered pawn would do, a line each as Bloodfeed stacks its own.
    public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
    {
        if (target.Thing is not Pawn victim || !ValidateTarget(victim, showMessages: false))
            return null;
        Foresee(victim);
        return foreseenLines;
    }

    // Works out, or keeps, what a strike on this pawn would do: whether it would take the head
    // off, what the drain would do to a target it leaves alive, and the cursor's lines for
    // both (see the header).
    private void Foresee(Pawn victim)
    {
        float now = RealTime.LastRealTime;
        if (victim == foreseenFor && now >= foreseenAt && now < foreseenAt + OutlookSeconds)
            return;
        foreseenFor = victim;
        foreseenAt = now;
        BodyPartHeight aim = MeleeAim.HeightFor(pawn, Extension);
        StrikeOutlook outlook = DismemberTarget.Outlook(victim, aim, ForeseenDamage(victim), out bool decapitates);
        foreseenKill = decapitates && outlook == StrikeOutlook.Tears;
        foreseenDrain = foreseenKill ? DrainOutlook.Harmless : ForeseenDrain(victim, aim);
        string strike = foreseenKill ? "WillKill"
            : outlook == StrikeOutlook.Wounds ? "HPWC_DismemberWillWound"
            : outlook == StrikeOutlook.MayTear ? "HPWC_DismemberMayWound"
            : null;
        string drain = foreseenDrain == DrainOutlook.Kills ? "WillKill"
            : foreseenDrain == DrainOutlook.SeriousBloodLoss ? "WillCauseSeriousBloodloss"
            : null;
        foreseenLines = strike?.Translate().ToString();
        if (drain != null)
            foreseenLines = (foreseenLines == null ? "" : foreseenLines + "\n") + drain.Translate();
    }

    // What the drain after a strike at this height would do to the target's blood: none of it
    // unless a drain could follow (see the header).
    private DrainOutlook ForeseenDrain(Pawn victim, BodyPartHeight aim)
    {
        if (!DrainsFrom(victim))
            return DrainOutlook.Harmless;
        HediffSet hediffs = victim.health.hediffSet;
        if (hediffs.BleedRateTotal <= 0f && (victim.Deathresting || !DismemberTarget.CouldBleed(victim, aim)))
            return DrainOutlook.Harmless;
        float lost = hediffs.GetFirstHediffOfDef(HediffDefOf.BloodLoss)?.Severity ?? 0f;
        return DismemberDrain.Outlook(HediffDefOf.BloodLoss, lost + FullBloodLoss(victim),
            deathless: victim.genes?.HasActiveGene(GeneDefOf.Deathless) == true);
    }

    // What of it would reach a part of this pawn's: the factors a gene or a hediff sets on
    // the damage def in Pawn.PreApplyDamage, then IncomingDamageFactor as
    // DamageWorker_AddInjury.ApplyDamageToPart applies it to an armorless hit (1.6 and 1.7).
    private float ForeseenDamage(Pawn victim)
    {
        DamageInfo dinfo = StrikeInfo();
        float damage = dinfo.Amount * victim.health.FactorForDamage(dinfo);
        if (victim.genes != null)
            damage *= victim.genes.FactorForDamage(dinfo);
        if (dinfo.Def.ExternalViolenceFor(victim))
            damage *= victim.GetStatValue(StatDefOf.IncomingDamageFactor);
        return damage;
    }
}
