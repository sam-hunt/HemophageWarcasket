using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;

namespace HemophageWarcasket;

// The melee aim height another mod lets the player set on a pawn, for the pads' strike to
// follow (DismemberTarget has what a height does to the part taken). Which mod is the def's
// business: DismemberExtension names the ThingComp class and its BodyPartHeight field, a
// compat root's patch fills both in, and this reads the field off the pilot's comp by
// reflection, so the code names no type of another mod and nothing is looked for when the
// def names none.
//
// Only a player pawn's height counts, the rule of the one mod read so far: Combat Extended
// keeps the field on every humanlike, lets the player set it from a gizmo and has its own
// melee verb follow it for a pawn of the player's faction alone
// (CompMeleeTargettingGizmo.finalHeight, CE 16.7.4). Undefined is that mod's automatic, and
// the strike's random limb.
//
// The names are resolved once per pair, since a play-data reload replaces the def and keeps
// its strings. A class or field that is not there, after an update of that mod, is one
// warning and a random limb from then on.
internal static class MeleeAim
{
    private static string resolvedComp;
    private static string resolvedField;
    private static Type compType;
    private static FieldInfo heightField;

    public static BodyPartHeight HeightFor(Pawn pilot, DismemberExtension ext)
    {
        if (ext == null || ext.aimHeightComp.NullOrEmpty() || pilot?.Faction != Faction.OfPlayer)
            return BodyPartHeight.Undefined;
        Resolve(ext.aimHeightComp, ext.aimHeightField);
        if (heightField == null)
            return BodyPartHeight.Undefined;
        List<ThingComp> comps = pilot.AllComps;
        for (int i = 0; i < comps.Count; i++)
            if (compType.IsInstanceOfType(comps[i]))
                return (BodyPartHeight)heightField.GetValue(comps[i]);
        return BodyPartHeight.Undefined;
    }

    private static void Resolve(string comp, string field)
    {
        if (comp == resolvedComp && field == resolvedField)
            return;
        resolvedComp = comp;
        resolvedField = field;
        compType = GenTypes.GetTypeInAnyAssembly(comp);
        heightField = field.NullOrEmpty() ? null
            : compType?.GetField(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (heightField != null && heightField.FieldType != typeof(BodyPartHeight))
            heightField = null;
        if (heightField == null)
            Log.Warning($"[Hemophage Warcasket] Dismember's aim height is not read: no BodyPartHeight field {field} on {comp}.");
    }
}
