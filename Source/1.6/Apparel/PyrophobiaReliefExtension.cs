using Verse;

namespace HemophageWarcasket;

// Marks a piece of apparel whose wearer breaks down near fire less often (PyrophobiaRelief has
// the mechanic). On the helmet def, whose header has the rationale for the number.
public class PyrophobiaReliefExtension : DefModExtension
{
    // The share of Biotech's fire terror breaks that still happen to the wearer: 1 changes
    // nothing, 0.5 doubles the mean time between them, 0 ends them. The settings' slider.
    public float breakChanceFactor = 1f;
}
