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
// The class is in four files. This one has the button, the targeting, the order and the hold on
// the target. The strike, the cursor's foresight and the drain each have a file and a header of
// their own: Ability_Dismember.Strike.cs, Ability_Dismember.Foresight.cs and
// Ability_Dismember.Drain.cs.
//
// The job is ordered through Pawn_JobTracker.TryTakeOrderedJob, not VEF's StartAbilityJob, so a
// click interrupts like a vanilla order and the queue key appends (1.6); the driver binds
// CompAbilities.currentlyCasting when the job starts. The click is not range limited, since the
// job walks, and autocast is pinned off, since VEF never fires an apparel ability's. The stun
// (StunHandler.StunFor without battle log or mote) is refreshed each tick the target is held, so
// it lapses on its own on any exit; a downed target is skipped, as StunHandlerTick zeroes its
// stun every tick.
//
// An order that would kill asks first, as Bloodfeed does before a lethal bite
// (CompAbilityEffect_BloodfeederBite.ConfirmationDialog, 1.6 and 1.7), from the same foresight
// as the cursor's "Will kill": a strike that would take the head off, or a drain that would
// bleed the target to death, which no drain does to a target with the deathless gene. Bloodfeed
// asks over every target it takes, and takes no enemy still standing; the strike is for those,
// so it asks over one of the player's own faction whatever its state, a berserk colonist, a
// rebelling slave and a colony animal turned manhunter among them, and otherwise only over a
// target that is not hostile to the pilot and is a person or belongs to a faction: a prisoner,
// guest or visitor, or an animal or mechanoid of theirs. An enemy and a wild animal are never
// asked about. VEF has no hook for such a dialog, so it goes up in OrderForceTarget, which the
// vanilla Targeter calls on the click, after ValidateTarget and before it stops targeting
// (Targeter.ProcessInputEvents, 1.6 and 1.7), and the job is ordered from the dialog's confirm
// button.
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
public partial class Ability_Dismember : Ability
{
    // "Unlimited" as a finite number so VEF's range arithmetic and ring drawing stay sane.
    private const float UnlimitedRange = 10000f;

    // The stun refreshed each tick lasts this long past the last refresh.
    private const int StunRefreshTicks = 5;

    private DismemberExtension extension;
    private int settingsGeneration = -1;
    private string incapableOfViolence;
    private string noStrikes;
    private string reloadLine;

    private DismemberExtension Extension => extension ??= def.GetModExtension<DismemberExtension>();

    private CompApparelReloadable Pool => holder?.TryGetComp<CompApparelReloadable>();

    // "2 / 2", for the command's corner.
    public string StrikesLabel => Pool?.LabelRemaining;

    // "1 component", "2 components", "2 strikes": a count of something by its label. A count
    // under two keeps the label as it is here, because not every language's worker does:
    // LanguageWorker_French.Pluralize ignores its count and would print "1 composants"
    // (decompiled on 1.7).
    internal static string Counted(string label, int count) =>
        count + " " + (count < 2 ? label : Find.ActiveLanguageWorker.Pluralize(label, count));

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
        else if (!DismemberTarget.CanStrike(victim))
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

    // The targeter's click. An order that would kill a target the player is not fighting waits
    // on the player's answer (see the header); the queue key is read now, as the answer comes
    // on a later event, and the dialog pauses the game until it does.
    public override void OrderForceTarget(LocalTargetInfo target)
    {
        if (target.Thing is not Pawn victim || LethalOrderQuestion(victim) is not { } question)
        {
            base.OrderForceTarget(target);
            return;
        }
        currentTargetingIndex = -1;
        bool queue = KeyBindingDefOf.QueueOrder.IsDownEvent;
        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(question, () => Order(victim, queue), destructive: true));
    }

    // What to ask before a strike on this pawn is ordered, or null when the order needs no
    // answer (see the header).
    private string LethalOrderQuestion(Pawn victim)
    {
        if (pawn == null)
            return null;
        bool asked = victim.Faction is { IsPlayer: true }
            || (!victim.HostileTo(pawn) && (victim.RaceProps.Humanlike || victim.Faction != null));
        if (!asked)
            return null;
        Foresee(victim);
        if (foreseenKill)
            return "HPWC_DismemberConfirmKill".Translate(victim.Named("PAWN"));
        if (foreseenDrain == DrainOutlook.Kills)
            return "HPWC_DismemberConfirmDrainKill".Translate(victim.Named("PAWN"));
        return null;
    }

    // Vanilla's order semantics in place of VEF's StartAbilityJob (see the header). The def has
    // no AbilityExtension_AbilityMods and the pilot is never a caravan member while the gizmo
    // shows, so skipping the base's Valid, PreCast and caravan branches loses nothing.
    public override void CreateCastJob(params GlobalTargetInfo[] targets)
    {
        currentTargetingIndex = -1;
        currentTargets = new GlobalTargetInfo[def.targetCount];
        if (targets.Length > 0 && targets[0].Thing is Pawn target)
            Order(target, queue: false);
    }

    // TryTakeOrderedJob reads the queue key itself on the event it is called from (1.6 and
    // 1.7); queue asks for the same of an order given after that event has passed.
    private void Order(Pawn target, bool queue) =>
        pawn?.jobs?.TryTakeOrderedJob(JobMaker.MakeJob(def.jobDef, target), JobTag.Misc, queue);

    // Vanilla's cursor rule: the no-shoot cross over anything the ability cannot be ordered
    // on, else the def's icon through the base. The vanilla Targeter hands a pawn under the
    // mouse through without its hit test, so the validity is decided here, quietly.
    protected override Texture2D MouseAttachment(GlobalTargetInfo target)
    {
        if (target.Thing is not Pawn victim || !ValidateTarget(victim, showMessages: false))
            return TexCommand.CannotShoot;
        return base.MouseAttachment(target);
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

    private void RefreshStun(Pawn target)
    {
        if (target == pawn || target.Downed || target.Dead)
            return;
        target.stances?.stunner?.StunFor(StunRefreshTicks, pawn, addBattleLog: false, showMote: false);
    }
}
