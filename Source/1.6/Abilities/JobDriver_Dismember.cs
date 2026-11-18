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
// and a drain cut short keeps what it collected. FailOnDespawnedOrNull covers every toil: a
// target that dies, to the strike or to the blood loss, is despawned before the next tick, and
// nothing here touches the target after the tick that could kill it.
public class JobDriver_Dismember : JobDriver_CastAbilityOnce
{
    private AbilityDef abilityDef;

    private Pawn Target => TargetA.Pawn;

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
        comp.currentlyCasting = ability;
        comp.currentlyCastingTargets = [new GlobalTargetInfo(Target)];
        return base.TryMakePreToilReservations(errorOnFailed);
    }

    // The pilot's instance of the ability whose def names jobDef, among the worn apparel's
    // granted abilities.
    public static Ability_Dismember FindAbility(Pawn pilot, JobDef jobDef)
    {
        if (pilot?.apparel == null || jobDef == null)
            return null;
        foreach (Apparel apparel in pilot.apparel.WornApparel)
        {
            List<Ability> abilities = apparel.TryGetComp<CompAbilitiesApparel>()?.GivenAbilities;
            if (abilities == null)
                continue;
            for (int i = 0; i < abilities.Count; i++)
                if (abilities[i] is Ability_Dismember ability && ability.def.jobDef == jobDef)
                    return ability;
        }
        return null;
    }

    public override string GetReport() => Def?.JobReportString ?? base.GetReport();

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return GotoTarget();
        yield return Check();
        foreach (Toil toil in base.MakeNewToils())
            yield return toil;
        yield return Drain();
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
            if (Casting?.CheckBeforeWarmup(Target) != true)
                EndJobWith(JobCondition.Incompletable);
        };
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        return toil;
    }

    // Passes straight through unless the strike landed and the pilot can drain the target. The
    // strike's result is transient, so a save and load mid-drain resumes on the saved
    // ticksLeftThisToil and the tick's own checks.
    private Toil Drain()
    {
        Toil toil = ToilMaker.MakeToil();
        toil.initAction = () =>
        {
            job.playerForced = true;
            Ability_Dismember ability = Casting;
            Pawn target = Target;
            if (ability == null || target == null || !ability.StrikeLanded || !ability.CanDrain(target))
            {
                ReadyForNextToil();
                return;
            }
            pawn.pather.StopDead();
            ticksLeftThisToil = ability.DrainTicks;
            SoundDefOf.Bloodfeed_Cast.PlayOneShot(new TargetInfo(target.Position, target.Map));
        };
        toil.tickAction = () =>
        {
            Ability_Dismember ability = Casting;
            Pawn target = Target;
            if (ability == null || target == null || !ability.InReach(target) || !ability.CanDrain(target))
            {
                ReadyForNextToil();
                return;
            }
            pawn.rotationTracker.FaceTarget(target);
            // The blood loss in here can kill the target, so nothing below reads it.
            ability.DrainTick(target);
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
        return toil;
    }
}
