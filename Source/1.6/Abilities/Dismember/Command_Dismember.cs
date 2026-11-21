using Verse;
using VEF.Abilities;

namespace HemophageWarcasket;

// Dismember's gizmo: VEF's ability command plus the "strikes / max" count in the top right
// corner, where vanilla draws a reloadable's on its verb command (Command.TopRightLabel, 1.6).
// VEF's command has no charge slot, so this is named as the def's gizmoClass. The count is read
// off the pads on every draw.
public class Command_Dismember : Command_Ability
{
    public Command_Dismember(Pawn pawn, Ability ability) : base(pawn, ability)
    {
    }

    public override string TopRightLabel => (ability as Ability_Dismember)?.StrikesLabel;
}
