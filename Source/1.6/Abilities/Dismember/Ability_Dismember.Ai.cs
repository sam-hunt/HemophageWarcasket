using RimWorld;
using Verse;
using Verse.AI;

namespace HemophageWarcasket;

// Ability_Dismember for a pilot the player does not control (Ability_Dismember.cs has the
// ability as a whole). VEF offers an apparel ability to no AI, so the ability asks on VEF's
// interval tick (the def's needsTickingInterval, and the pads' tickerType Normal, without
// which a worn piece is never ticked) whether its wearer should strike what it is fighting,
// and starts the same job an order does. AiPilot has who such a pilot is and what it is
// fighting, PilotTactics the decision, the def the numbers.
//
// It strikes a humanlike only, so the pads' few strikes are not spent on an animal or a
// mechanoid sent to take them, and only a target the pilot is fighting that still fights back,
// within the extension's aiStrikeDistance and in reach. It never kills outright: a target with
// no limb left, whose head the strike would take, is left alone, and the drain is skipped on
// one it would bleed to death (SparesDrain). The bleeding of a torn limb can still kill.
//
// The job is started with StartJob, not ordered: an order is the player's, and the pilot's
// lord takes the pilot back when the job ends (Pawn_JobTracker.EndCurrentJob goes to the think
// tree). VEF's cast driver marks every cast job player forced, which on such a pilot means
// only that damage does not interrupt it and the close melee threat reaction stands down for
// its few seconds; nothing in Lord, LordToil or LordJob reads the flag, so a duty change
// replaces the job as it would any other (1.6 and 1.7).
//
// The strikes are spaced by VEF's own cooldown, which Cast sets from GetCooldownForPawn and
// saves: aiStrikeSpacingTicks for such a pilot, the def's cooldownTime for the player's. A job
// that ends before the cast has set none, and the pilot may start again at the next ask.
public partial class Ability_Dismember
{
    public override void TickInterval(int delta)
    {
        if ((holder as Apparel)?.Wearer is not { } wearer || !wearer.IsHashIntervalTick(AiPilot.AskTicks, delta))
            return;
        DismemberExtension ext = Extension;
        if (ext is not { aiUses: true } || !AiPilot.Acts(wearer) || wearer.jobs?.curDriver is null or JobDriver_Dismember)
            return;
        if (AiPilot.FightTarget(wearer) is not Pawn target || !target.RaceProps.Humanlike
            || !AiPilot.Threat(wearer, target) || !target.HostileTo(wearer))
            return;
        if (wearer.Position.DistanceTo(target.Position) > ext.aiStrikeDistance
            || !wearer.CanReach(target, PathEndMode.Touch, Danger.Deadly))
            return;
        // The job's driver casts through VEF's comp, which only a humanlike race has.
        if (wearer.GetComp<VEF.Abilities.CompAbilities>() == null)
            return;
        // Nothing has bound the ability on a pilot nobody has selected.
        if (pawn != wearer)
        {
            pawn = wearer;
            Init();
        }
        if (!IsEnabledForPawn(out _) || !ValidateTarget(target, showMessages: false))
            return;
        StrikeOutlook outlook = DismemberTarget.Outlook(target, MeleeAim.HeightFor(pawn, ext),
            ForeseenDamage(target), out bool decapitates);
        if (PilotTactics.WorthStriking(outlook, decapitates))
            wearer.jobs.StartJob(JobMaker.MakeJob(def.jobDef, target), JobCondition.InterruptForced);
    }

    public override int GetCooldownForPawn() =>
        AiPilot.Is(pawn) ? Extension.aiStrikeSpacingTicks : base.GetCooldownForPawn();

    // Whether an AI pilot passes the drain over: a full one would bleed the target to death.
    public bool SparesDrain(Pawn target) => AiPilot.Is(pawn) && FullDrainOutlook(target) == DrainOutlook.Kills;
}
