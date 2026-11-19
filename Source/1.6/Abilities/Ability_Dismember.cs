using System.Collections.Generic;
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
// (JobDriver_Dismember). Any pawn may be targeted, hostile or not unless the def's extension
// refuses the latter; vanilla's damage bookkeeping carries the consequences.
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
// The goodwill is vanilla's price for any attack on a faction that is not hostile:
// Faction.Notify_MemberTookDamage, reached from Pawn_HealthTracker.PreApplyDamage before armor,
// takes 1.3 a point of damage up to 100 points, so at this amount every strike costs the most
// one hit can, 130, and Faction.CalculateAdjustedGoodwillChange adds up to a quarter more
// while relations stand above the faction's natural goodwill. One strike turns a neutral
// faction hostile. AngeredFaction repeats that method's exemptions and
// Faction.CanChangeGoodwillFor to say so beside the cursor before the click, in the words
// vanilla warns of a harmful operation with (all the same on 1.6 and 1.7).
//
// TakeDamage writes no combat log line, so the strike writes its own the way
// Verb_MeleeAttack.TryCastShot does (1.6; the entry's constructor and BattleLog.Add are the same
// on 1.7): a BattleLogEntry_MeleeCombat on our rule pack goes into the battle log before the
// damage, so the downed or death line the damage causes follows it, and takes the destroyed
// part from the damage result afterward. The log entry def is required, since the entry reads
// its icons from it. A strike that destroyed nothing takes its entry back out, as the rule pack
// has no line without a part.
//
// The limit is the strikes loaded in the pads: a vanilla reloadable (CompStrikePool) on the
// piece that grants the ability (holder), one spent by every cast, landed or not, as a vanilla
// reloadable's verb spends its own. With none left the button is disabled on vanilla's own
// line for an empty reloadable, and the tooltip's last line names what a strike costs to
// reload, which the def's text cannot, the cost being a mod setting. A piece patched to carry
// no pool has no such limit.
//
// The mod settings write onto the defs. VEF reads the cast time and cooldown off the ability's
// per cast, but a running cooldown is stored as an end tick, so it is capped to the new length
// on the next gizmo request (ReconcileSettings, keyed on the settings generation): VEF's
// cooldown overlay would otherwise overflow the button. The same request drops the cached
// reload line; strikes over a lowered capacity are the pool's own business.
public class Ability_Dismember : Ability
{
    // "Unlimited" as a finite number so VEF's range arithmetic and ring drawing stay sane.
    private const float UnlimitedRange = 10000f;

    // The stun refreshed each tick lasts this long past the last refresh.
    private const int StunRefreshTicks = 5;

    private DismemberExtension extension;
    private int settingsGeneration = -1;
    private string incapableOfViolence;
    private Faction warnedFaction;
    private bool warnedKill;
    private string warning;
    private string noStrikes;
    private string reloadLine;

    // Whether this cast's strike took its part off, read by the driver's drain toil.
    public bool StrikeLanded { get; private set; }

    private DismemberExtension Extension => extension ??= def.GetModExtension<DismemberExtension>();

    private CompApparelReloadable Pool => holder?.TryGetComp<CompApparelReloadable>();

    // "2 / 2", for the command's corner.
    public string StrikesLabel => Pool?.LabelRemaining;

    // "1 component", "2 components", "2 strikes": a count of something by its label.
    internal static string Counted(string label, int count) =>
        count + " " + Find.ActiveLanguageWorker.Pluralize(label, count);

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
        warnedFaction = null;
        warnedKill = false;
        warning = null;
        noStrikes = null;
        reloadLine = null;
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
        if (Pool is { RemainingCharges: <= 0 } pool)
        {
            reason = noStrikes ??= "CommandReload_NoCharges".Translate(pool.Props.ChargeNounArgument);
            return false;
        }
        return true;
    }

    // VEF's tooltip, then what a strike costs to reload.
    public override string GetDescriptionForPawn()
    {
        string description = base.GetDescriptionForPawn();
        if (Pool?.Props is not { ammoDef: not null } pool)
            return description;
        reloadLine ??= "HPWC_DismemberReload".Translate(Counted(pool.ammoDef.label, pool.ammoCountPerCharge));
        return description + "\n\n" + reloadLine;
    }

    public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
    {
        if (!base.ValidateTarget(target, showMessages))
            return false;
        if (target.Thing is not Pawn victim || victim == pawn || !victim.Spawned || victim.Dead)
            return false;
        string rejection = null;
        if (RefusedAsNonHostile(victim))
            rejection = "HPWC_DismemberNotHostile";
        else if (!DismemberTarget.CanStrike(victim, Extension, out _))
            rejection = "HPWC_DismemberNoLimbs";
        if (rejection == null)
            return true;
        if (showMessages)
            Messages.Message(rejection.Translate(victim.LabelShortCap), victim, MessageTypeDefOf.RejectInput, historical: false);
        return false;
    }

    // GenHostility counts a manhunter, a prison breaker and a predator hunting the pilot as
    // hostile (1.6 and 1.7).
    private bool RefusedAsNonHostile(Pawn victim) =>
        !Extension.canTargetNonHostile && pawn != null && !victim.HostileTo(pawn);

    public override Gizmo GetGizmo()
    {
        ReconcileSettings();
        return base.GetGizmo();
    }

    // Once per settings write.
    private void ReconcileSettings()
    {
        if (settingsGeneration == HemophageWarcasketSettings.Generation)
            return;
        settingsGeneration = HemophageWarcasketSettings.Generation;
        reloadLine = null;
        int ceiling = Find.TickManager.TicksGame + GetCooldownForPawn();
        if (cooldown > ceiling)
            cooldown = ceiling;
    }

    // The job's check toil, with the pilot in reach: still off cooldown, a strike still loaded
    // and still a valid target. Nothing has been spent yet.
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

    // What the strike on the hovered pawn would cost, a line each as Bloodfeed stacks its own:
    // Biotech's warning when it would take the head, then the faction it would anger. Asked
    // every frame of the hover, so the text is kept until either answer changes.
    public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
    {
        if (target.Thing is not Pawn victim || victim == pawn || !victim.Spawned || victim.Dead || RefusedAsNonHostile(victim))
            return null;
        if (!DismemberTarget.CanStrike(victim, Extension, out bool kills))
            return null;
        Faction angered = AngeredFaction(victim);
        if (angered == warnedFaction && kills == warnedKill)
            return warning;
        warnedFaction = angered;
        warnedKill = kills;
        warning = kills ? "WillKill".Translate() : null;
        if (angered != null)
            warning = (warning == null ? "" : warning + "\n") + "HPWC_DismemberWillAnger".Translate(angered.Name);
        return warning;
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
        StrikeLanded = false;
        if (targets.Length == 0 || targets[0].Thing is not Pawn target)
            return;
        Pool?.UsedOnce();
        StrikeLanded = Strike(target);
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
        IntVec3 center = target.Position;
        Map map = target.Map;
        DamageWorker.DamageResult result = target.TakeDamage(dinfo);
        bool landed = target.Dead || target.health.hediffSet.PartIsMissing(part);
        if (landed && !result.parts.NullOrEmpty())
            result.AssociateWithLog(log);
        else
            Find.BattleLog.RemoveEntry(log);
        if (landed)
            Spatter(target, center, map);
        return landed;
    }

    // Reused across casts; filled and read within one Spatter call.
    private static readonly List<IntVec3> SpatterCells = [];

    // Throws the extension's blood filth around where the target stood, the target's own blood
    // as Pawn_HealthTracker.DropBloodFilth picks it (1.6), so a bloodless race throws none.
    // TryMakeFilth reads its cell before any bounds check, hence the InBounds filter; the line
    // of sight keeps the spatter off the far side of a wall. FilthMaker and GenSight are the
    // same on 1.7.
    private void Spatter(Pawn target, IntVec3 center, Map map)
    {
        DismemberExtension ext = Extension;
        if (ext == null || ext.bloodFilthCount <= 0 || map == null)
            return;
        ThingDef blood = target.IsMutant ? target.mutant.Def.bloodDef ?? target.RaceProps.BloodDef : target.RaceProps.BloodDef;
        if (blood == null)
            return;
        SpatterCells.Clear();
        foreach (IntVec3 cell in GenRadial.RadialCellsAround(center, ext.bloodFilthRadius, useCenter: true))
        {
            if (cell.InBounds(map) && GenSight.LineOfSight(center, cell, map))
                SpatterCells.Add(cell);
        }
        string source = target.LabelIndefinite();
        for (int i = 0; i < ext.bloodFilthCount && SpatterCells.Count > 0; i++)
            FilthMaker.TryMakeFilth(SpatterCells.RandomElement(), map, blood, source);
        SpatterCells.Clear();
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
