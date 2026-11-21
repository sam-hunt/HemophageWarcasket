using RimWorld;
using Verse;

namespace HemophageWarcasket;

// Ability_Dismember's foresight: what the cursor says of a strike on the hovered pawn before
// the click, which the question before a lethal order is asked from as well
// (Ability_Dismember.cs has that and the ability as a whole).
//
// The goodwill is vanilla's price for any attack on a faction that is not hostile:
// Faction.Notify_MemberTookDamage, reached from Pawn_HealthTracker.PreApplyDamage before armor,
// takes 1.3 a point of the amount dealt up to 100 points, and
// Faction.CalculateAdjustedGoodwillChange adds up to a quarter more while relations stand
// above the faction's natural goodwill. AngeredFaction repeats that method's exemptions and
// Faction.CanChangeGoodwillFor to say so beside the cursor before the click, in the words
// vanilla warns of a harmful operation with (all the same on 1.6 and 1.7).
//
// The cursor also says what the strike would do (ExtraLabelMouseAttachment): Biotech's "Will
// kill" when it would take the head off, and that it will or may only wound when no part or
// only some of the parts it could take would come off (DismemberTarget.Outlook, on
// ForeseenDamage). Under that goes what the drain would then do to a living target's blood, in
// the two warnings Bloodfeed gives of its bite and by the same sum
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
// and 1.7). Every answer is kept for OutlookSeconds of real time, since working them out walks
// the body and the game may be paused while what decides them changes. The hovered pawn's own
// tooltip is off while the ability is targeting (HidePawnTooltips, as on Ideology's Convert and
// Counsel): it opens at the cursor, over those lines, and
// TooltipGiverList.DispenseAllThingTooltips hands out none while the targeting source says so
// (1.6 and 1.7).
public partial class Ability_Dismember
{
    // How long the cursor keeps what it foresaw of a strike on the hovered pawn.
    private const float OutlookSeconds = 0.25f;

    private Faction warnedFaction;
    private StrikeOutlook warnedOutlook;
    private bool warnedKill;
    private DrainOutlook warnedDrain;
    private string warning;
    private Pawn foreseenFor;
    private float foreseenAt;
    private StrikeOutlook foreseen;
    private bool foreseenKill;
    private DrainOutlook foreseenDrain;

    // The cursor's lines in place of the hovered pawn's tooltip (see the header).
    public override bool HidePawnTooltips => true;

    // What the strike on the hovered pawn would do and cost, a line each as Bloodfeed stacks
    // its own: Biotech's warning when it would take the head off, or that it would or might
    // only wound, then what the drain would do to the blood of a target the strike leaves
    // alive, then the faction it would anger. Asked every frame of the hover, so the text is
    // kept until an answer changes.
    public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
    {
        if (target.Thing is not Pawn victim || victim == pawn || !victim.Spawned || victim.Dead || RefusedAsNonHostile(victim))
            return null;
        if (!DismemberTarget.CanStrike(victim, Extension, out _))
            return null;
        StrikeOutlook outlook = Foresee(victim, out bool kills, out DrainOutlook drain);
        Faction angered = AngeredFaction(victim);
        if (angered == warnedFaction && outlook == warnedOutlook && kills == warnedKill && drain == warnedDrain)
            return warning;
        warnedFaction = angered;
        warnedOutlook = outlook;
        warnedKill = kills;
        warnedDrain = drain;
        warning = kills ? "WillKill".Translate()
            : outlook == StrikeOutlook.Wounds ? "HPWC_DismemberWillWound".Translate()
            : outlook == StrikeOutlook.MayTear ? "HPWC_DismemberMayWound".Translate()
            : null;
        if (drain == DrainOutlook.Kills)
            AddWarning("WillKill".Translate());
        else if (drain == DrainOutlook.SeriousBloodLoss)
            AddWarning("WillCauseSeriousBloodloss".Translate());
        if (angered != null)
            AddWarning("HPWC_DismemberWillAnger".Translate(angered.Name));
        return warning;
    }

    private void AddWarning(string line) => warning = warning == null ? line : warning + "\n" + line;

    // What a strike on this pawn would do, whether it would take the head off and, when it
    // would not, what the drain would then do (see the header).
    private StrikeOutlook Foresee(Pawn victim, out bool kills, out DrainOutlook drain)
    {
        float now = RealTime.LastRealTime;
        if (victim != foreseenFor || now < foreseenAt || now >= foreseenAt + OutlookSeconds)
        {
            foreseenFor = victim;
            foreseenAt = now;
            BodyPartHeight aim = MeleeAim.HeightFor(pawn, Extension);
            foreseen = DismemberTarget.Outlook(victim, Extension, aim, ForeseenDamage(victim), out bool decapitates);
            foreseenKill = decapitates && foreseen == StrikeOutlook.Tears;
            foreseenDrain = foreseenKill ? DrainOutlook.Harmless : ForeseenDrain(victim, aim);
        }
        kills = foreseenKill;
        drain = foreseenDrain;
        return foreseen;
    }

    // What the drain after a strike at this height would do to a living target's blood: none
    // of it unless a drain could follow (see the header).
    private DrainOutlook ForeseenDrain(Pawn victim, BodyPartHeight aim)
    {
        if (!DrainsFrom(victim))
            return DrainOutlook.Harmless;
        HediffSet hediffs = victim.health.hediffSet;
        if (hediffs.BleedRateTotal <= 0f && (victim.Deathresting || !DismemberTarget.CouldBleed(victim, Extension, aim)))
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

    // The faction a strike on this pawn would cost the player goodwill with, or null (see the
    // header). A faction set to turn hostile on any harm does so before vanilla looks at the
    // exemptions.
    private Faction AngeredFaction(Pawn victim)
    {
        Faction home = victim.HomeFaction;
        if (home == null || home.IsPlayer || pawn?.Faction != Faction.OfPlayer || home.HostileTo(Faction.OfPlayer))
            return null;
        if (home.factionHostileOnHarmByPlayer)
            return home;
        if (victim.InAggroMentalState || victim.IsSlaveOfColony || victim.IsQuestHelper() || PrisonBreakUtility.IsPrisonBreaking(victim))
            return null;
        if (victim.InMentalState && victim.MentalStateDef.IsExtreme && victim.MentalStateDef.category == MentalStateCategory.Malicious
            && home.PlayerRelationKind == FactionRelationKind.Ally)
            return null;
        return Faction.OfPlayer.CanChangeGoodwillFor(home, -1) ? home : null;
    }
}
