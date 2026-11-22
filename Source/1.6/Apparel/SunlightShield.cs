using System.Collections.Generic;
using RimWorld;
using Verse;

namespace HemophageWarcasket;

// Sunlight protection: a pawn wearing a piece whose def carries a SunlightShieldExtension with
// blocksSunlight set takes none of what a sunlight sensitivity gene does to a pawn standing in
// sunlight. Nothing tests for a gene, Biotech's or another mod's. Every such effect is a reader
// of SanguophageUtility.InSunlight (the cell is unroofed and the sky glow is over 0.1), which
// knows the cell and not the pawn, so each reader that harms is answered by a postfix in
// Patches/ that asks Protects once the reader has already said "in sunlight":
//
// - ConditionalStatAffecter_InSunlight.Applies: the move speed factor of Biotech's
//   UVSensitivity_Mild and _Intense and of Vanilla Races Expanded - Sanguophage's
//   VRE_Sensitivity_Dangerous.
// - ThoughtWorker_InSunlight.CurrentStateInternal: the mood penalty each of the three carries.
// - VEF's HediffComp_StageBySunlight: the hidden hediff behind VRE_Sensitivity_Dangerous,
//   which caps Sight at 50% in sunlight.
//
// The first two are answered whatever they carry, so a gene that gave its carrier something
// in sunlight through the same classes would lose that under the helmet too. None does in
// Biotech or in Vanilla Races Expanded - Sanguophage.
//
// Two readers are left alone. Pawn_GeneTracker.EnjoysSunlight only keeps an idle pawn from
// wandering into sunlight, a habit the helmet has no reason to break, and Gene_Deathrest wakes
// a deathrester whose bed touches sunlight whatever their genes. All verified on 1.6 and 1.7,
// VEF's comp on both of its builds.
public static class SunlightShield
{
    // Cheap enough for the stat path that calls it: a warcasket pilot wears three pieces, and
    // GetModExtension returns at once for a def without extensions.
    public static bool Protects(Pawn pawn)
    {
        List<Apparel> worn = pawn.apparel?.WornApparel;
        if (worn == null)
            return false;
        for (int i = 0; i < worn.Count; i++)
        {
            if (worn[i].def.GetModExtension<SunlightShieldExtension>() is { blocksSunlight: true })
                return true;
        }
        return false;
    }
}
