using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.Sound;
using VEF.Abilities;
using Ability = VEF.Abilities.Ability;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace HemophageWarcasket;

// The job behind the pads' ability (Ability_Dismember): the walk, the check, VEF's warmup and
// cast toils (JobDriver_CastAbilityOnce: a progress-bar wait of GetCastTimeForPawn ticks that
// calls the ability's WarmupToil, then an instant cast toil that tears the limb), then the drain.
//
// The binding: VEF's driver reads CompAbilities.currentlyCasting and currentlyCastingTargets
// throughout, which VEF's own path writes at order time. The job is ordered through
// TryTakeOrderedJob so it can be queued, so the binding happens here, in
// TryMakePreToilReservations, and only when the job is starting (StartJob sets CurJob before
// calling it). Vanilla also calls it at order time through the job's cached driver, while the
// pilot may be mid-cast of another VEF ability, so then it returns true without touching the
// comp or calling the base, which would dereference a possibly null currentlyCasting. A job
// that starts with the piece gone returns false, which ends a queued job quietly.
//
// The walk is VEF's JobDriver_GotoTargetAndCastAbilityOnce without its order for a pawn target
// to stand and face the caster: the target is left to whatever it was doing, and the ability's
// stun holds a conscious one. The pilot stops as soon as it stands within the def's
// distanceToTarget with line of sight and is not mid-door; a target it cannot reach ends the job.
//
// The drain toil re-asserts playerForced, which VEF's cast toil drops for a drafted pilot, so the
// melee-threat and hostility-response constant jobs stand down for it too. It counts its own
// duration in ticksLeftThisToil, which vanilla scribes and decrements before each tick action,
// and a drain cut short keeps what it collected.
//
// VEF's FailOnDespawnedOrNull on the target covers every toil, and a pawn that dies is
// despawned on the spot, so a death before the strike ends the job. From the strike on, the
// drain follows the target onto its corpse instead, whatever killed it: FollowIntoDeath swaps
// the job's target for the corpse, so VEF's condition, the progress bar and the effect all
// find it, and Ability_Dismember.CanDrain decides whether a corpse has blood left to give.
// It runs as the first of the driver's end conditions because JobDriver checks those ahead of
// everything a toil does, both each tick and between one toil's end and the next one's
// initAction (DriverTick and TryActuallyStartNextToil, 1.6), which is the only place a death
// between ticks can be caught before VEF's condition fails the job. The drain toil starts in a
// busy stance (atomicWithPrevious, as VEF's cast toil does) so that a pilot stunned at the
// strike is already in it at that first check.
//
// A hemohunter's job may be on a corpse from the start (JobGiver_HemohunterDismember;
// Ability_Dismember.Hunt.cs has the rule). The toils are the same ones: the walk and VEF's
// conditions take a corpse as they take the one the drain followed its target to, the hold in
// the warmup is for a pawn and passes a corpse by, and the check and the cast go to the
// ability's corpse overloads. foundDead keeps that through a save, since a corpse in the job's
// target says nothing by itself once FollowIntoDeath may have put it there.
public class JobDriver_Dismember : JobDriver_CastAbilityOnce
{
    private AbilityDef abilityDef;
    private Toil drain;
    private bool foundDead;

    // The pawn the job is on, living or in the corpse the drain followed it to.
    private Pawn Target => TargetA.Thing is Corpse corpse ? corpse.InnerPawn : TargetA.Pawn;

    // VEF scribes currentlyCasting as a reference, so the binding survives a save and load.
    private Ability_Dismember Casting => CompAbilities?.currentlyCasting as Ability_Dismember;

    // The def of the ability this job casts, found by its jobDef; cached per driver instance,
    // which vanilla rebuilds on load.
    private AbilityDef Def => abilityDef ??= FindAbility(pawn, job.def)?.def;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        if (pawn.CurJob != job)
            return true;
        Ability_Dismember ability = FindAbility(pawn, job.def);
        CompAbilities comp = CompAbilities;
        if (ability == null || comp == null || Target == null)
            return false;
        ability.pawn = pawn;
        foundDead = TargetA.Thing is Corpse;
        comp.currentlyCasting = ability;
        comp.currentlyCastingTargets = [new GlobalTargetInfo(TargetA.Thing)];
        return base.TryMakePreToilReservations(errorOnFailed);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref foundDead, "foundDead");
    }

    // The pilot's instance of the ability whose def names jobDef, or of any with none given,
    // among the worn apparel's granted abilities.
    public static Ability_Dismember FindAbility(Pawn pilot, JobDef jobDef = null)
    {
        if (pilot?.apparel == null)
            return null;
        foreach (Apparel apparel in pilot.apparel.WornApparel)
        {
            List<Ability> abilities = apparel.TryGetComp<CompAbilitiesApparel>()?.GivenAbilities;
            if (abilities == null)
                continue;
            for (int i = 0; i < abilities.Count; i++)
                if (abilities[i] is Ability_Dismember ability && (jobDef == null || ability.def.jobDef == jobDef))
                    return ability;
        }
        return null;
    }

    public override string GetReport() => Def?.JobReportString ?? base.GetReport();

    protected override IEnumerable<Toil> MakeNewToils()
    {
        // Before the base's toils, whose building adds VEF's condition behind this one.
        AddEndCondition(FollowIntoDeath);
        yield return GotoTarget();
        yield return Check();
        foreach (Toil toil in base.MakeNewToils())
            yield return toil;
        yield return drain = Drain();
    }

    // Never ends the job: see the header for why this is an end condition at all.
    private JobCondition FollowIntoDeath()
    {
        if (CurToil == drain && job.targetA.Thing is Pawn { Dead: true, Corpse: { Spawned: true } corpse })
            job.targetA = corpse;
        return JobCondition.Ongoing;
    }

    private Toil GotoTarget()
    {
        Toil toil = ToilMaker.MakeToil();
        toil.initAction = () => pawn.pather.StopDead();
        toil.tickAction = () =>
        {
            LocalTargetInfo target = job.targetA;
            IntVec3 cell = target.Cell;
            Map map = pawn.Map;
            pawn.rotationTracker.FaceTarget(cell);
            if (pawn.Position.DistanceTo(cell) <= (Def?.distanceToTarget ?? 0f)
                && GenSight.LineOfSight(pawn.Position, cell, map, skipFirstCell: true)
                && (!pawn.pather.Moving || pawn.pather.nextCell.GetDoor(map) == null))
            {
                pawn.pather.StopDead();
                pawn.rotationTracker.FaceTarget(cell);
                ReadyForNextToil();
            }
            else if (!pawn.pather.Moving)
            {
                if (!pawn.CanReach(target, PathEndMode.Touch, Danger.Deadly))
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }
                pawn.pather.StartPath(target, PathEndMode.Touch);
            }
        };
        toil.handlingFacing = true;
        toil.socialMode = RandomSocialMode.Off;
        toil.defaultCompleteMode = ToilCompleteMode.Never;
        return toil;
    }

    private Toil Check()
    {
        Toil toil = ToilMaker.MakeToil();
        toil.initAction = () =>
        {
            Ability_Dismember ability = Casting;
            bool ready = ability != null
                && (TargetA.Thing is Corpse corpse ? ability.CheckBeforeWarmup(corpse) : ability.CheckBeforeWarmup(Target));
            if (!ready)
                EndJobWith(JobCondition.Incompletable);
        };
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        return toil;
    }

    // Passes straight through unless the pilot can drain the target, bleeding or dead, whether
    // or not the strike took its limb off (Ability_Dismember.CanDrain). A save and load
    // mid-drain resumes on the saved ticksLeftThisToil and the tick's own checks.
    private Toil Drain()
    {
        Toil toil = ToilMaker.MakeToil();
        toil.initAction = () =>
        {
            job.playerForced = true;
            Ability_Dismember ability = Casting;
            Pawn target = Target;
            if (ability == null || target == null || !ability.CanDrain(target, foundDead))
            {
                ReadyForNextToil();
                return;
            }
            pawn.pather.StopDead();
            ticksLeftThisToil = ability.DrainTicks;
            ability.MarkFeeding(target, foundDead);
            SoundDefOf.Bloodfeed_Cast.PlayOneShot(new TargetInfo(TargetA.Cell, pawn.Map));
        };
        toil.tickAction = () =>
        {
            Ability_Dismember ability = Casting;
            Pawn target = Target;
            if (ability == null || target == null || !ability.CanDrain(target, foundDead) || (!target.Dead && !ability.InReach(target)))
            {
                ReadyForNextToil();
                return;
            }
            pawn.rotationTracker.FaceTarget(TargetA);
            // The blood loss in here can kill the target, so nothing below reads it.
            ability.DrainTick(target, foundDead);
            if (ticksLeftThisToil <= 0)
                ReadyForNextToil();
        };
        toil.WithProgressBar(TargetIndex.A, () =>
        {
            Ability_Dismember ability = Casting;
            return ability == null ? 0f : 1f - (float)ticksLeftThisToil / ability.DrainTicks;
        });
        toil.WithEffect(() => HPWC_DefOf.Bloodfeed_Warmup, TargetIndex.A);
        toil.handlingFacing = true;
        toil.socialMode = RandomSocialMode.Off;
        toil.defaultCompleteMode = ToilCompleteMode.Never;
        toil.atomicWithPrevious = true;
        return toil;
    }
}
