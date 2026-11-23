using RimWorld;
using UnityEngine;
using Verse;
using Ability = VEF.Abilities.Ability;

namespace HemophageWarcasket;

// The armor's hemogen rush toggle. It holds no state: the rush is the HPWC_HemogenRush hediff,
// whose HediffComp_HemogenRush runs it, ends it and settles the switch on the next tick, so the
// toggle shows on for a rush that has not been told to stop. VEF's base gizmo is a cast
// command whose job would stop a moving pilot, so GetGizmo hands out a plain Command_Toggle
// (the Toxin repo's vent is the pattern). VEF asks for gizmos every frame a wearer is
// selected: one command and the strings it shows are kept, and only its disabled state is
// refreshed. The tooltip's last line is the start cost, as Biotech's abilities show theirs
// (CompProperties_AbilityHemogenCost.ExtraStatSummary, 1.6), which the def's text cannot
// carry, the cost being a mod setting: the tooltip is rebuilt after a settings write.
//
// A pilot the player does not control has no toggle to click, and VEF offers an apparel
// ability to no AI, so the ability starts that pilot's rush itself, on VEF's interval tick
// (the def's needsTickingInterval): when the pilot is engaged, as AiPilot and PilotTactics
// have it. The rush's comp ends it once the fight has moved on.
public class Ability_HemogenRush : Ability
{
    private Command_Toggle gizmo;
    private int settingsGeneration = -1;
    private string description;
    private string notHemogenic;
    private string noHemogen;
    private string asleep;

    private static HediffCompProperties_HemogenRush Rush =>
        HPWC_DefOf.HPWC_HemogenRush.CompProps<HediffCompProperties_HemogenRush>();

    private Hediff RushHediff => pawn.health.hediffSet.GetFirstHediffOfDef(HPWC_DefOf.HPWC_HemogenRush);

    // A toggle has nothing to autocast; VEF offers it to any one-target ability.
    public override bool CanAutoCast => false;

    // Drafted or not (the def's showUndrafted). VEF also shows gizmos on caravan members,
    // where a rush could not run.
    public override bool ShowGizmoOnPawn() => base.ShowGizmoOnPawn() && pawn.Spawned;

    // VEF runs Init again when the apparel changes wearer.
    public override void Init()
    {
        base.Init();
        gizmo = null;
        description = null;
        notHemogenic = null;
        noHemogen = null;
        asleep = null;
    }

    public override Gizmo GetGizmo()
    {
        gizmo ??= new Command_Toggle
        {
            defaultLabel = def.LabelCap,
            icon = def.icon,
            isActive = () => HediffComp_HemogenRush.ShowsOn(pawn),
            toggleAction = Toggle,
        };
        if (settingsGeneration != HemophageWarcasketSettings.Generation)
        {
            settingsGeneration = HemophageWarcasketSettings.Generation;
            description = null;
        }
        gizmo.defaultDesc = description ??= GetDescriptionForPawn();
        gizmo.Disabled = false;
        // A running rush can always be switched off, and a stopping one back on.
        if (RushHediff == null && !CanStart(out string reason))
            gizmo.Disable(reason);
        return gizmo;
    }

    // VEF's tooltip, then the start cost on the scale the game shows hemogen on.
    public override string GetDescriptionForPawn()
    {
        string text = base.GetDescriptionForPawn();
        int cost = Mathf.RoundToInt(Rush.hemogenStartCost * 100f);
        return cost > 0 ? text + "\n\n" + "AbilityHemogenCost".Translate() + ": " + cost : text;
    }

    // The reasons are the game's own: Biotech's, as its hemogen abilities give them, and
    // Core's for a sleeper, whose rush the comp would end at once.
    private bool CanStart(out string reason)
    {
        Gene_Hemogen hemogen = pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
        if (hemogen == null)
        {
            reason = notHemogenic ??= "AbilityDisabledNoHemogenGene".Translate(pawn);
            return false;
        }
        if (!Rush.Affords(hemogen.Value))
        {
            reason = noHemogen ??= "AbilityDisabledNoHemogen".Translate(pawn);
            return false;
        }
        if (!pawn.Awake())
        {
            reason = asleep ??= "AbilityCantApplyAsleep".Translate(pawn);
            return false;
        }
        return IsEnabledForPawn(out reason);
    }

    // The start an AI pilot has in place of the toggle (see the header): the same hediff, on
    // the same hemogen test, and never through pawn, which nothing has bound on a pilot nobody
    // has selected.
    public override void TickInterval(int delta)
    {
        if ((holder as Apparel)?.Wearer is not { } wearer || !wearer.IsHashIntervalTick(AiPilot.AskTicks, delta))
            return;
        HediffCompProperties_HemogenRush rush = Rush;
        if (!rush.aiUses || !AiPilot.Acts(wearer) || wearer.health.hediffSet.HasHediff(HPWC_DefOf.HPWC_HemogenRush))
            return;
        Gene_Hemogen hemogen = wearer.genes?.GetFirstGeneOfType<Gene_Hemogen>();
        if (hemogen?.CanOffset != true || !rush.Affords(hemogen.Value))
            return;
        if (AiPilot.RushEngaged(wearer, rush))
            wearer.health.AddHediff(HPWC_DefOf.HPWC_HemogenRush);
    }

    private void Toggle()
    {
        Hediff rush = RushHediff;
        if (rush == null)
            pawn.health.AddHediff(HPWC_DefOf.HPWC_HemogenRush);
        else
            rush.TryGetComp<HediffComp_HemogenRush>()?.Switch();
    }
}
