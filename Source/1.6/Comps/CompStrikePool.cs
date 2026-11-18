using RimWorld;
using Verse;

namespace HemophageWarcasket;

// The pads' strike pool: vanilla's CompApparelReloadable, kept within its capacity. The capacity
// is a mod setting, and vanilla's reload arithmetic takes remaining <= max for granted: over
// it, NeedsReload is true and ReloadFrom splits a negative count off the ammo, which throws
// (1.6). So the count is trimmed wherever a lowered capacity can meet it: on load, when the
// piece is put on (Pawn_ApparelTracker.Notify_ApparelAdded, 1.6), and on the worn pieces when
// the settings are written (HemophageWarcasketSettings.TrimWornPads). A stored piece over its
// capacity is inert until then, since only worn apparel is reloaded.
public class CompStrikePool : CompApparelReloadable
{
    public void Trim()
    {
        if (remainingCharges > MaxCharges)
            remainingCharges = MaxCharges;
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
            Trim();
    }

    public override void Notify_Equipped(Pawn pawn)
    {
        base.Notify_Equipped(pawn);
        Trim();
    }
}
