using Verse;

namespace HemophageWarcasket;

// Marks a piece of apparel that keeps sunlight off its wearer (SunlightShield has what that
// means). On the helmet def, whose header has the rationale.
public class SunlightShieldExtension : DefModExtension
{
    // The settings' switch. Off, the piece shields nothing.
    public bool blocksSunlight = true;
}
