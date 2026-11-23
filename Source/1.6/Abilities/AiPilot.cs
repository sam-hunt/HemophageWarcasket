using RimWorld;
using Verse;
using Verse.AI;

namespace HemophageWarcasket;

// A pilot the player does not control, and what it is fighting: the readers that turn a pawn
// into PilotTactics' arguments. VEF offers an apparel ability to no AI, so each of the set's
// two asks these of its own wearer on VEF's interval tick (Ability_HemogenRush,
// Ability_Dismember.Ai.cs), found through the ability's holder, since its pawn is null on a
// pilot nobody has selected.
//
// Who: any pawn not of the player's faction. A prisoner keeps their own faction and so counts,
// in a prison break as in a cell; a slave, a colonist in a mental state and a quest lodger are
// of the player's faction and stay the player's (Pawn_GuestTracker.SetGuestStatus, 1.6 and 1.7).
//
// What it is fighting: the target of a melee job, or the fight job giver's own target while the
// current job is one that giver issued. JobGiver_AIFightEnemy writes mindState.enemyTarget and
// nothing clears it when the fight ends, so it counts only under a job of that giver's
// (Pawn_JobTracker.StartJob records the node in Job.jobGiver); every duty the quests'
// sanguophages fight under reaches the giver through a subclass. AttackMelee is also the job
// for breaking down a door and for finishing off the downed (TrashUtility.TrashJob,
// Job.killIncappedTarget), so a fight target is a threat only by the engine's own test for what
// fights back: an IAttackTarget whose ThreatDisabled is false, which a building other than a
// turret is not and a downed pawn fails. All the same on 1.6 and 1.7.
public static class AiPilot
{
    // How often an ability asks, in ticks. A worn piece's interval comes at the piece's own
    // rate, every 15 ticks at the slowest, with the ticks since the last as its delta
    // (Thing.DoTick, 1.6 and 1.7), and the wearer's hash test on that delta passes once in
    // this many ticks whatever the rate.
    public const int AskTicks = 30;

    public static bool Is(Pawn pawn) => pawn != null && pawn.Faction != Faction.OfPlayer;

    // An AI pilot in a state to start anything.
    public static bool Acts(Pawn pawn) => Is(pawn) && pawn.Spawned && !pawn.Downed && pawn.Awake();

    public static bool InMeleeJob(Pawn pilot) =>
        pilot.CurJob?.def == JobDefOf.AttackMelee || pilot.jobs?.curDriver is JobDriver_Dismember;

    public static Thing FightTarget(Pawn pilot)
    {
        Job job = pilot.CurJob;
        if (job == null)
            return null;
        if (InMeleeJob(pilot))
            return job.targetA.Thing;
        return job.jobGiver is JobGiver_AIFightEnemy ? pilot.mindState?.enemyTarget : null;
    }

    public static bool Threat(Pawn pilot, Thing target) =>
        target is IAttackTarget attackTarget && target.Spawned && target.Map == pilot.Map
        && !attackTarget.ThreatDisabled(pilot);

    // PilotTactics.RushEngaged, of a spawned pilot.
    public static bool RushEngaged(Pawn pilot, HediffCompProperties_HemogenRush rush)
    {
        Thing target = FightTarget(pilot);
        if (target == null || !Threat(pilot, target))
            return false;
        return PilotTactics.RushEngaged(threat: true, InMeleeJob(pilot),
            pilot.Position.DistanceTo(target.Position), rush.aiStartDistance, rush.aiCloseDistance);
    }

    // PilotTactics.AffordsAbility, of a pawn the AI is picking an ability for: the game's own
    // answer stands unless it is an AI pilot in the rush's armor with the rush left to the AI.
    public static bool AffordsAbility(Pawn caster, float cost)
    {
        HediffCompProperties_HemogenRush rush = HPWC_DefOf.HPWC_HemogenRush.CompProps<HediffCompProperties_HemogenRush>();
        if (rush is not { aiUses: true, aiHemogenKept: > 0f } || !Is(caster) || !HediffComp_HemogenRush.Wears(caster, rush.armor))
            return true;
        Gene_Hemogen hemogen = caster.genes?.GetFirstGeneOfType<Gene_Hemogen>();
        return hemogen == null || PilotTactics.AffordsAbility(hemogen.Value, cost, rush.aiHemogenKept);
    }
}
