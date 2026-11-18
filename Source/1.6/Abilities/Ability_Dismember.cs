using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Ability = VEF.Abilities.Ability;

namespace HemophageWarcasket;

// The shoulder pads' active: the pilot walks to a target pawn, holds it through the def's
// warmup, tears off one of its limbs (DismemberTarget decides which) and, for a hemogenic pilot
// on a humanlike target, holds it a few seconds more and drains hemogen from it
// (JobDriver_Dismember). Any pawn may be targeted, hostile or not; vanilla's damage bookkeeping
// carries the consequences.
//
// The job is ordered through Pawn_JobTracker.TryTakeOrderedJob, not VEF's StartAbilityJob, so a
// click interrupts like a vanilla order and the queue key appends (1.6); the driver binds
// CompAbilities.currentlyCasting when the job starts. The click is not range limited, since the
// job walks, and autocast is pinned off, since VEF never fires an apparel ability's. The stun
// (StunHandler.StunFor without battle log or mote) is refreshed each tick the target is held, so
// it lapses on its own on any exit; a downed target is skipped, as StunHandlerTick zeroes its
// stun every tick.
//
// The strike is one DamageInfo at the chosen part with the pilot as instigator, which gives
// guilt, faction goodwill, kill blame and records through TakeDamage (1.6). Damage propagation
// off makes DamageWorker_AddInjury.ApplyDamageToPart go straight to FinalizeAndAddInjury,
// skipping ReduceDamageToPreserveOutsideParts, so damage at or above the part's health always
// destroys it, and HediffSet.AddDirect turns the injury into a fresh Hediff_MissingPart.
// IgnoreArmor also skips worn apparel's CheckPreAbsorbDamage in Pawn_HealthTracker.PreApplyDamage.
// The amount is vanilla's surgery idiom (Recipe_RemoveBodyPart), since genes, hediffs and
// IncomingDamageFactor scale it first; instant kill protection would otherwise spare a
// colonist's head.
//
// TakeDamage writes no combat log line, so the strike writes its own the way
// Verb_MeleeAttack.TryCastShot does (1.6; the entry's constructor and BattleLog.Add are the same
// on 1.7): a BattleLogEntry_MeleeCombat on our rule pack goes into the battle log before the
// damage, so the downed or death line the damage causes follows it, and takes the destroyed
// part from the damage result afterward. The log entry def is required, since the entry reads
// its icons from it. A strike that destroyed nothing takes its entry back out, as the rule pack
// has no line without a part.
public class Ability_Dismember : Ability
{
    // "Unlimited" as a finite number so VEF's range arithmetic and ring drawing stay sane.
    private const float UnlimitedRange = 10000f;

    // The stun refreshed each tick lasts this long past the last refresh.
    private const int StunRefreshTicks = 5;

    private DismemberExtension extension;
    private string incapableOfViolence;
    private string willKill;

    // Whether this cast's strike took its part off, read by the driver's drain toil.
    public bool StrikeLanded { get; private set; }

    private DismemberExtension Extension => extension ??= def.GetModExtension<DismemberExtension>();

    // Whether the pilot's rejections go to the player's log; a non-player pilot's are not.
    private bool ReportsToPlayer => pawn?.Faction == Faction.OfPlayer;

    public override float GetRangeForPawn() => UnlimitedRange;

    public override bool CanAutoCast => false;

    public override bool AutoCast => false;

    // Drafted or not (the def's showUndrafted). VEF also shows gizmos on caravan members, who
    // have nobody to walk to.
    public override bool ShowGizmoOnPawn() => base.ShowGizmoOnPawn() && pawn.Spawned;

    // VEF runs Init again when the apparel changes wearer.
    public override void Init()
    {
        base.Init();
        extension = null;
        incapableOfViolence = null;
        willKill = null;
    }

    // VEF asks every frame the pilot is selected, so the reason is cached.
    public override bool IsEnabledForPawn(out string reason)
    {
        if (!base.IsEnabledForPawn(out reason))
            return false;
        if (pawn != null && pawn.WorkTagIsDisabled(WorkTags.Violent))
        {
            reason = incapableOfViolence ??= "IsIncapableOfViolence".Translate(pawn.LabelShort, pawn);
            return false;
        }
        return true;
    }

    public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
    {
        if (!base.ValidateTarget(target, showMessages))
            return false;
        if (target.Thing is not Pawn victim || victim == pawn || !victim.Spawned || victim.Dead)
            return false;
        if (DismemberTarget.CanStrike(victim, Extension, out _))
            return true;
        if (showMessages)
            Messages.Message("HPWC_DismemberNoLimbs".Translate(victim.LabelShortCap), victim, MessageTypeDefOf.RejectInput, historical: false);
        return false;
    }

    // The job's check toil, with the pilot in reach: still off cooldown and still a valid
    // target. Nothing has been spent yet.
    public bool CheckBeforeWarmup(Pawn target)
    {
        if (!IsEnabledForPawn(out string reason))
        {
            if (ReportsToPlayer)
                Messages.Message(reason, pawn, MessageTypeDefOf.RejectInput, historical: false);
            return false;
        }
        return ValidateTarget(target, ReportsToPlayer);
    }

    // Vanilla's order semantics in place of VEF's StartAbilityJob (see the header). The def has
    // no AbilityExtension_AbilityMods and the pilot is never a caravan member while the gizmo
    // shows, so skipping the base's Valid, PreCast and caravan branches loses nothing.
    public override void CreateCastJob(params GlobalTargetInfo[] targets)
    {
        currentTargetingIndex = -1;
        currentTargets = new GlobalTargetInfo[def.targetCount];
        if (targets.Length == 0 || targets[0].Thing is not Pawn target || pawn.jobs == null)
            return;
        pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(def.jobDef, target), JobTag.Misc);
    }

    // Vanilla's cursor rule: the no-shoot cross over anything the ability cannot be ordered
    // on, else the def's icon through the base. The vanilla Targeter hands a pawn under the
    // mouse through without its hit test, so the validity is decided here, quietly.
    protected override Texture2D MouseAttachment(GlobalTargetInfo target)
    {
        if (target.Thing is not Pawn victim || !ValidateTarget(victim, showMessages: false))
            return TexCommand.CannotShoot;
        return base.MouseAttachment(target);
    }

    // Biotech's warning, when the strike on the hovered pawn would take its head.
    public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
    {
        if (target.Thing is not Pawn victim || victim == pawn || !victim.Spawned || victim.Dead)
            return null;
        if (DismemberTarget.CanStrike(victim, Extension, out bool decapitates) && decapitates)
            return willKill ??= "WillKill".Translate();
        return null;
    }

    // Within the def's distanceToTarget with a clear line, the walk's own stopping rule.
    public bool InReach(Pawn target) =>
        pawn != null && pawn.Spawned && target.Spawned && target.Map == pawn.Map
        && pawn.Position.DistanceTo(target.Position) <= def.distanceToTarget
        && GenSight.LineOfSight(pawn.Position, target.Position, pawn.Map, skipFirstCell: true);

    // The warmup holds a conscious target; one carried out of reach ends the job with no
    // cooldown (the cast toil never ran).
    public override void WarmupToil(Toil toil)
    {
        base.WarmupToil(toil);
        toil.AddPreTickAction(() =>
        {
            if (Comp?.currentlyCastingTargets?.Length is not > 0 || Comp.currentlyCastingTargets[0].Thing is not Pawn target)
                return;
            if (!InReach(target))
            {
                pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
                return;
            }
            RefreshStun(target);
        });
    }

    // base.Cast sets the cooldown and plays the def's cast effects.
    public override void Cast(params GlobalTargetInfo[] targets)
    {
        base.Cast(targets);
        StrikeLanded = targets.Length > 0 && targets[0].Thing is Pawn target && Strike(target);
    }

    private bool Strike(Pawn target)
    {
        if (target.Dead || !target.Spawned)
            return false;
        BodyPartRecord part = DismemberTarget.Choose(target, Extension, out _);
        if (part == null)
            return false;
        var dinfo = new DamageInfo(HPWC_DefOf.HPWC_Dismember, 99999f, 999f, -1f, pawn, part);
        dinfo.SetIgnoreArmor(true);
        dinfo.SetAllowDamagePropagation(false);
        dinfo.SetIgnoreInstantKillProtection(true);
        var log = new BattleLogEntry_MeleeCombat(HPWC_DefOf.HPWC_Combat_Dismember, alwaysShowInCompact: true,
            pawn, target, implementType: null, toolLabel: null, def: LogEntryDefOf.MeleeAttack);
        Find.BattleLog.Add(log);
        DamageWorker.DamageResult result = target.TakeDamage(dinfo);
        bool landed = target.Dead || target.health.hediffSet.PartIsMissing(part);
        if (landed && !result.parts.NullOrEmpty())
            result.AssociateWithLog(log);
        else
            Find.BattleLog.RemoveEntry(log);
        return landed;
    }

    // Whether a drain can run or go on: the def sets one, the pilot has an active hemogen gene
    // with room for more, and the target is a living humanlike that bleeds.
    public bool CanDrain(Pawn target)
    {
        DismemberExtension ext = Extension;
        if (ext == null || ext.drainTicks <= 0 || ext.hemogenGain <= 0f)
            return false;
        Gene_Hemogen hemogen = pawn?.genes?.GetFirstGeneOfType<Gene_Hemogen>();
        if (hemogen == null || !hemogen.CanOffset || hemogen.Value >= hemogen.Max)
            return false;
        return target.Spawned && !target.Dead && target.RaceProps.Humanlike && target.health.CanBleed;
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
        HealthUtility.AdjustSeverity(target, HediffDefOf.BloodLoss, ext.targetBloodLoss * share);
    }

    public int DrainTicks => Extension.drainTicks;

    private void RefreshStun(Pawn target)
    {
        if (target == pawn || target.Downed || target.Dead)
            return;
        target.stances?.stunner?.StunFor(StunRefreshTicks, pawn, addBattleLog: false, showMote: false);
    }
}
