using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace HemophageWarcasket;

// Dresses a sanguophage that one of Biotech's two sanguophage quests generates in the whole
// set, by the chance the armor's QuestPilotExtension gives its source, and only once the player
// has finished the extension's requiredResearch. Called by the two postfixes on the quest roots
// that make those pawns (Patches/QuestNode_Root_SanguophageShip_RunInt and
// Patches/QuestNode_Root_SanguophageMeetingHost_RunInt, whose headers have the call sites),
// never from pawn generation: PawnGenerator.GeneratePawn has returned by then, so VFEP's
// postfix on it (the one Patches/PawnGenerator_GeneratePawn_Patch_Postfix masks) found no
// warcasket to complete with parts of its own.
//
// The pawn is alive and off the map: quest.GeneratePawn passes it to the world, where it waits
// for the quest to be accepted and its arrival delay to run. Both the roll and the research are
// therefore read as the quest is generated, and a setting changed or a project finished after
// that does not reach a pawn already made.
//
// What the pawn is left as is what the foundry leaves a pilot as, piece by piece:
// - The set alone. Everything else the pawn wore is destroyed first, as VFEP's postfix strips
//   a generated pilot, and each piece goes on locked, as the foundry and that postfix lock
//   theirs (WarcasketProject.ApplyOn, Wear with locked true). The weapon and the hemogen packs
//   the pawn kind gave are not apparel and stay.
// - The hemophage trait, through VEF's hook on Pawn_ApparelTracker.Notify_ApparelAdded
//   (ApparelExtensionUtilities.EquipGear, the armor's traitsOnEquip).
// - One colour across the three pieces, the armor's draw from the def's colorGenerator
//   (CompColorable.Initialize draws one for each piece as it is made), since a set built for
//   one pilot is painted as one.
//
// All or nothing: a pawn who cannot wear every piece (ApparelUtility.HasPartsToWear and
// ApparelProperties.PawnCanWear, the two refusals Pawn_ApparelTracker.Wear only warns of), or a
// set whose pieces another mod has made unwearable together (CanWearTogether, on which Wear
// takes the earlier piece off again), leaves the pawn the clothes vanilla gave them, so no pawn
// wears part of a set. The three pieces are made before those clothes are destroyed, so a def
// that cannot be made costs the pawn nothing either.
//
// A dressed pawn that leaves the map is a world pawn like any other. Should the generator reuse
// one, PawnGenerator.RedressPawn destroys what it wears (GenerateStartingApparelFor) before
// VFEP's postfix runs, so the mask's condition, no generated pawn in a part of ours, holds.
//
// Only a living, hemogenic pawn is dressed, told by the gene and not by the xenotype: a mod that
// makes either quest hand over another kind of pawn gets no set for it.
//
// A failure here is logged and goes no further: the callers run inside quest generation, where
// an exception from a postfix would cost the player the quest.
//
// Vanilla as read on 1.7.5014; the calls made here compile against 1.6.
public static class QuestPilot
{
    private const string ArmorDefName = "HPWC_Warcasket_Hemophage";

    // Null while the armor or its extension is missing (another mod's patch).
    public static QuestPilotExtension Extension =>
        DefDatabase<ThingDef>.GetNamedSilentFail(ArmorDefName)?.GetModExtension<QuestPilotExtension>();

    public static void TryDress(Pawn pawn, QuestPilotSource source)
    {
        try
        {
            ThingDef armor = DefDatabase<ThingDef>.GetNamedSilentFail(ArmorDefName);
            QuestPilotExtension extension = armor?.GetModExtension<QuestPilotExtension>();
            if (extension == null || pawn?.apparel == null || pawn.Dead)
                return;
            if (extension.requiredResearch is { IsFinished: false })
                return;
            if (pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>() == null)
                return;
            if (!Rand.Chance(extension.ChanceFor(source)))
                return;
            List<ThingDef> set = [armor, .. extension.otherPieces];
            if (!set.TrueForAll(piece => Fits(pawn, piece)) || !WearableTogether(set, pawn.RaceProps.body))
                return;
            var pieces = new List<Apparel>();
            foreach (ThingDef piece in set)
            {
                var apparel = (Apparel)ThingMaker.MakeThing(piece, GenStuff.RandomStuffFor(piece));
                if (pieces.Count > 0)
                    apparel.SetColor(pieces[0].DrawColor, reportFailure: false);
                pieces.Add(apparel);
            }
            pawn.apparel.DestroyAll();
            foreach (Apparel apparel in pieces)
                pawn.apparel.Wear(apparel, dropReplacedApparel: false, locked: true);
        }
        catch (Exception exception)
        {
            Log.Error($"[Hemophage Warcasket] Could not dress the quest's sanguophage {pawn} in the hemophage warcasket: {exception}");
        }
    }

    private static bool Fits(Pawn pawn, ThingDef piece) =>
        piece?.apparel != null
        && ApparelUtility.HasPartsToWear(pawn, piece)
        && piece.apparel.PawnCanWear(pawn, ignoreGender: true);

    private static bool WearableTogether(List<ThingDef> set, BodyDef body)
    {
        for (int i = 0; i < set.Count; i++)
            for (int j = i + 1; j < set.Count; j++)
                if (!ApparelUtility.CanWearTogether(set[i], set[j], body))
                    return false;
        return true;
    }
}
