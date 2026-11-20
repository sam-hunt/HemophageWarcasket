using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Ability = VEF.Abilities.Ability;

namespace HemophageWarcasket;

// The shoulder pads' active: the pilot walks to a target pawn, holds it through the def's
// warmup, tears at one of its limbs (DismemberTarget decides which), which comes off unless it
// is too sturdy for the strike, and, for a hemogenic pilot on a humanlike target that is
// bleeding, holds it a few seconds more and drains hemogen from it (JobDriver_Dismember). Any
// pawn may be targeted, hostile or not unless the def's extension refuses the latter;
// vanilla's damage bookkeeping carries the consequences.
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
// guilt, faction goodwill, kill blame and records through TakeDamage (1.6). Its amount is the
// extension's damage times the pilot's MeleeDamageFactor, the stat every vanilla melee verb
// multiplies its own by (VerbProperties.GetDamageFactorFor), so the hemogen rush and a melee
// damage gene strengthen the strike as they do a blow, with none of a blow's random spread:
// what a strike will do is known before the click. Armor is ignored, as the strike wrenches
// the joint and cuts through nothing: IgnoreArmor skips the armor step in
// DamageWorker_AddInjury.ApplyDamageToPart and worn apparel's CheckPreAbsorbDamage in
// Pawn_HealthTracker.PreApplyDamage. The target's IncomingDamageFactor still scales it there
// (Tough, Biotech's robust gene), after any factor a gene or hediff of the target's sets on
// the damage def itself (Pawn.PreApplyDamage). Damage propagation off sends what is left
// straight to FinalizeAndAddInjury, skipping ReduceDamageToPreserveOutsideParts, as one injury
// on the one part. When the part's health less that injury rounds to nothing,
// HediffSet.AddDirect turns it into a fresh Hediff_MissingPart (GetPartHealth rounds;
// DismemberTarget.Tears is the same test); otherwise the part stays on, wounded, and the
// strike is spent all the same. Instant kill protection is ignored, or it would spare a
// colonist's head after the cursor said the strike would kill. All the same on 1.6 and 1.7.
//
// Combat Extended (16.7.4) leaves the strike as it is: its transpiler on ApplyDamageToPart
// replaces the armor call inside the branch IgnoreArmor skips, its ArmorUtilityCE returns at
// once for a damage def with no armorCategory, and its other patches on the worker are on the
// propagation the strike turns off. What it changes is the health of the parts
// (1.6/Mods/CombatExtended/Patches/Dismember.xml).
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
// kills at and Biotech's major blood loss line when it reaches a life threatening stage
// (DismemberDrain.Outlook). A full drain is the most a drain can take, so the warning errs
// toward being given: a pilot whose hemogen fills first stops short of it. It is given when
// the pilot could drain the target now (DrainsFrom) and the target bleeds already or could
// bleed from the strike: it is not deathresting, and one of the parts the strike could take
// is not solid, vanilla's word for a part that never bleeds, an artificial limb and all below
// it among them (BodyPartDef.IsSolid, which Hediff_Injury.BleedRate and
// Hediff_MissingPart.IsFreshNonSolidExtremity both ask, 1.6 and 1.7). Every answer is kept
// for OutlookSeconds of real time, since working them out walks the body and the game may be
// paused while what decides them changes.
//
// TakeDamage writes no combat log line, so the strike writes its own the way
// Verb_MeleeAttack.TryCastShot does (1.6; the entry's constructor and BattleLog.Add are the same
// on 1.7): a BattleLogEntry_MeleeCombat on our rule pack goes into the battle log before the
// damage, so the downed or death line the damage causes follows it, and takes the part and
// whether it was destroyed from the damage result afterward, which the rule pack's lines
// choose between. The log entry def is required, since the entry reads its icons from it. A
// strike that reached no part takes its entry back out, as the rule pack has no line without
// one.
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
// that keeps such hediffs. A corpse is drained as what it was alive.
//
// A pilot in one of the extension's hemohunterStates hunts with the strike instead of the
// bite that state's own mod gives it (JobGiver_HemohunterDismember), while hemohunterStrikes
// is on, the button would be enabled, so a strike is loaded and off cooldown and is spent as on
// any cast, the prey is one the drain feeds from, so never with the drain set to nothing, and
// the pilot's genes make aggressive mental breaks likelier
// (Pawn_GeneTracker.AggroMentalBreakSelectionChanceFactor above 1, vanilla's product over the
// active genes, 1.6 and 1.7: Biotech's aggressive and hyper-aggressive, and any mod's stronger
// one).
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

    // How long the cursor keeps what it foresaw of a strike on the hovered pawn.
    private const float OutlookSeconds = 0.25f;

    private DismemberExtension extension;
    private int settingsGeneration = -1;
    private string incapableOfViolence;
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
    private string noStrikes;
    private string reloadLine;

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
        warnedOutlook = StrikeOutlook.None;
        warnedKill = false;
        warnedDrain = DrainOutlook.Harmless;
        warning = null;
        foreseenFor = null;
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
        return DismemberDrain.Outlook(HediffDefOf.BloodLoss, lost + FullBloodLoss(victim));
    }

    // The strike before it is aimed at a part: the def's damage by the pilot's melee damage
    // factor, armor and instant kill protection ignored and no propagation (see the header).
    private DamageInfo StrikeInfo()
    {
        float amount = Extension.damage * (pawn?.GetStatValue(StatDefOf.MeleeDamageFactor) ?? 1f);
        var dinfo = new DamageInfo(HPWC_DefOf.HPWC_Dismember, amount, instigator: pawn);
        dinfo.SetIgnoreArmor(true);
        dinfo.SetAllowDamagePropagation(false);
        dinfo.SetIgnoreInstantKillProtection(true);
        return dinfo;
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

    // Within the def's distanceToTarget with a clear line, the walk's own stopping rule.
    public bool InReach(Pawn target) =>
        pawn != null && pawn.Spawned && target.Spawned && target.Map == pawn.Map
        && pawn.Position.DistanceTo(target.Position) <= def.distanceToTarget
        && GenSight.LineOfSight(pawn.Position, target.Position, pawn.Map, skipFirstCell: true);

    // The warmup holds a conscious target; one carried out of reach ends the job with no
    // cooldown (the cast toil never ran). The targets are read off the pilot's own comp, not
    // VEF's Ability.Comp, which keeps the comp of whoever wore the pads at its first read
    // while CompAbilitiesApparel hands the same ability to the next wearer.
    public override void WarmupToil(Toil toil)
    {
        base.WarmupToil(toil);
        toil.AddPreTickAction(() =>
        {
            GlobalTargetInfo[] targets = pawn.GetComp<VEF.Abilities.CompAbilities>()?.currentlyCastingTargets;
            if (targets?.Length is not > 0 || targets[0].Thing is not Pawn target)
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
        if (targets.Length == 0 || targets[0].Thing is not Pawn target)
            return;
        Pool?.UsedOnce();
        Strike(target);
    }

    private void Strike(Pawn target)
    {
        if (target.Dead || !target.Spawned)
            return;
        BodyPartRecord part = DismemberTarget.Choose(target, Extension, MeleeAim.HeightFor(pawn, Extension), out _);
        if (part == null)
            return;
        DamageInfo dinfo = StrikeInfo();
        dinfo.SetHitPart(part);
        var log = new BattleLogEntry_MeleeCombat(HPWC_DefOf.HPWC_Combat_Dismember, alwaysShowInCompact: true,
            pawn, target, implementType: null, toolLabel: null, def: LogEntryDefOf.MeleeAttack);
        Find.BattleLog.Add(log);
        IntVec3 center = target.Position;
        Map map = target.Map;
        DamageWorker.DamageResult result = target.TakeDamage(dinfo);
        if (result.parts.NullOrEmpty())
            Find.BattleLog.RemoveEntry(log);
        else
            result.AssociateWithLog(log);
        if (target.Dead || target.health.hediffSet.PartIsMissing(part))
            Spatter(target, center, map);
    }

    // Reused across casts; filled and read within one Spatter call.
    private static readonly List<IntVec3> SpatterCells = [];

    // What a torn limb or a kill throws: the extension's blood filth around where the target
    // stood, the target's own blood
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

    // Whether a drain can run or go on: one could (DrainsFrom), and the target is bleeding now
    // or dead (see the header).
    public bool CanDrain(Pawn target) =>
        DrainsFrom(target) && (target.Dead || target.health.hediffSet.BleedRateTotal > 0f);

    // Whether the pilot could drain this target once it bleeds: the def sets a drain, the pilot
    // has an active hemogen gene with room for more, and the target is one the pilot feeds on
    // that can bleed, alive or a corpse on the map with blood left in it, and holds the hemogen
    // its source asks for.
    private bool DrainsFrom(Pawn target)
    {
        DismemberExtension ext = Extension;
        if (ext == null || ext.drainTicks <= 0 || ext.hemogenGain <= 0f)
            return false;
        Gene_Hemogen hemogen = pawn?.genes?.GetFirstGeneOfType<Gene_Hemogen>();
        if (hemogen == null || !hemogen.CanOffset || hemogen.Value >= hemogen.Max)
            return false;
        Thing body = target.Dead ? target.Corpse : target;
        return body is { Spawned: true } && FeedsOn(target, out DismemberDrainSource source)
            && source?.BelowFloor(target) != true && target.health.CanBleed && !BledOut(target);
    }

    // Whether the pilot drains such a target at all, and through which of the extension's
    // wider sources: none for the plain drain (see the header).
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

    // Vanilla's own test for a hediff that kills, which only a corpse can pass.
    private static bool BledOut(Pawn target) =>
        target.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss)?.CauseDeathNow() == true;

    // One tick's share of a full drain, as SanguophageUtility.DoBite gives its whole one (1.6),
    // the same from a corpse. The blood loss goes last because it can kill a living target.
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

    // Whether the pilot, bound and in a mental state, hunts with the strike (see the header).
    public bool StrikesAsHemohunter()
    {
        DismemberExtension ext = Extension;
        return ext is { hemohunterStrikes: true, hemohunterStates: not null } && pawn?.MentalStateDef is { } state
            && ext.hemohunterStates.Contains(state) && pawn.genes?.AggroMentalBreakSelectionChanceFactor > 1f
            && IsEnabledForPawn(out _);
    }

    // Whether a hunt may take this pawn: a valid target for the strike that the drain could
    // then feed on.
    public bool CanHunt(Pawn target) => ValidateTarget(target, showMessages: false) && DrainsFrom(target);

    public int DrainTicks => Extension.drainTicks;

    private void RefreshStun(Pawn target)
    {
        if (target == pawn || target.Downed || target.Dead)
            return;
        target.stances?.stunner?.StunFor(StunRefreshTicks, pawn, addBattleLog: false, showMote: false);
    }
}
