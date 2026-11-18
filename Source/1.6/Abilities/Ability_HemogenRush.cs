using RimWorld;
using Verse;
using Ability = VEF.Abilities.Ability;

namespace HemophageWarcasket;

// The armor's hemogen rush toggle. It holds no state: the rush is on while the pilot has the
// HPWC_HemogenRush hediff, which HediffComp_HemogenRush runs and ends. VEF's base gizmo is a
// cast command whose job would stop a moving pilot, so GetGizmo hands out a plain
// Command_Toggle (the Toxin repo's vent is the pattern). VEF asks for gizmos every frame a
// wearer is selected: one command and the strings it shows are kept, and only its disabled
// state is refreshed.
public class Ability_HemogenRush : Ability
{
    private Command_Toggle gizmo;
    private string description;
    private string notHemogenic;
    private string noHemogen;

    private bool Running => pawn.health.hediffSet.HasHediff(HPWC_DefOf.HPWC_HemogenRush);

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
    }

    public override Gizmo GetGizmo()
    {
        gizmo ??= new Command_Toggle
        {
            defaultLabel = def.LabelCap,
            icon = def.icon,
            isActive = () => Running,
            toggleAction = Toggle,
        };
        gizmo.defaultDesc = description ??= GetDescriptionForPawn();
        gizmo.Disabled = false;
        // A running rush can always be switched off.
        if (!Running && !CanStart(out string reason))
            gizmo.Disable(reason);
        return gizmo;
    }

    // The reasons are Biotech's own, as its hemogen abilities give them.
    private bool CanStart(out string reason)
    {
        Gene_Hemogen hemogen = pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
        if (hemogen == null)
        {
            reason = notHemogenic ??= "AbilityDisabledNoHemogenGene".Translate(pawn);
            return false;
        }
        if (hemogen.Value <= HPWC_DefOf.HPWC_HemogenRush.CompProps<HediffCompProperties_HemogenRush>().hemogenReserve)
        {
            reason = noHemogen ??= "AbilityDisabledNoHemogen".Translate(pawn);
            return false;
        }
        return IsEnabledForPawn(out reason);
    }

    private void Toggle()
    {
        Hediff rush = pawn.health.hediffSet.GetFirstHediffOfDef(HPWC_DefOf.HPWC_HemogenRush);
        if (rush != null)
            pawn.health.RemoveHediff(rush);
        else
            pawn.health.AddHediff(HPWC_DefOf.HPWC_HemogenRush);
    }
}
