using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace HemophageWarcasket.Patches;

// The pawns of Biotech's crashed sanguophage shuttle quest, offered to QuestPilot: the master
// for the hemophage set, the thralls for a lesser warcasket. RunInt (1.7.5014 and the frozen
// 1.6 assembly, the same on both) generates the master through quest.GeneratePawn, sets
// health.forceDowned on them, which spares them the death roll when they are first downed and
// downs no one by itself, and stores the pawn in the quest's slate as "sanguophage", so a
// postfix reads it back from QuestGen.slate, which is still this quest's while the root node
// runs. Slate.Get answers null for a name never set.
//
// The quest's threat points are read from the slate as "points", the read RunInt itself makes
// (slate.Get("points", 0f)), so a quest generated with no points in its slate reads 0, under
// any floor and with no budget. RunInt reads them for head count alone, each curve rounded
// with Mathf.RoundToInt: the thralls who crash with the master, (0, 2) to (2000, 5), and the
// reinforcements dropped 10000 ticks after the crash, (2000, 0), (2500, 2), (5000, 4),
// (8000, 6).
//
// The thralls who crash with the master are in the slate's "shuttlePawns" beside the master,
// whom the list holds first. The reinforcements are in no slate name: RunInt keeps them in a
// local and hands them to quest.DropPods, which stores them in a QuestPart_DropPods (Things,
// the part's items and pawns together). That part is in the quest by the time a postfix runs,
// since quest.Delay runs its inner action as the quest is generated
// (QuestGenUtility.RunInner), and it is the only drop pod part this quest has.
//
// This runs once for each such quest generated, not when the shuttle crashes: the pawns wait
// in the quest wearing their sets. An exception here would cost the player the quest, so the
// read of the quest is guarded as QuestPilot guards the dressing.
//
// RunInt is protected, so it is bound by name, and as this type declares it, since the abstract
// QuestNode.RunInt would answer an inherited lookup: should it move, this patch stands down with
// a warning where a failed lookup would stop PatchAll and take every other patch with it.
[HarmonyPatch]
public static class QuestNode_Root_SanguophageShip_RunInt
{
    private const string Method = "RunInt";

    private static MethodBase Target => AccessTools.DeclaredMethod(typeof(QuestNode_Root_SanguophageShip), Method);

    public static bool Prepare()
    {
        if (Target != null)
            return true;
        Log.Warning($"[Hemophage Warcasket] {nameof(QuestNode_Root_SanguophageShip)}.{Method} not found, so the crashed shuttle's sanguophage master and thralls will never arrive in a warcasket.");
        return false;
    }

    public static MethodBase TargetMethod() => Target;

    public static void Postfix()
    {
        try
        {
            Slate slate = QuestGen.slate;
            Pawn master = slate?.Get<Pawn>("sanguophage");
            float points = slate?.Get("points", 0f) ?? 0f;
            bool masterDressed = QuestPilot.TryDress(master, QuestPilotSource.ShipMaster, points);

            var shuttleThralls = new List<Pawn>();
            foreach (Pawn pawn in slate?.Get<List<Pawn>>("shuttlePawns") ?? [])
                if (pawn != null && pawn != master)
                    shuttleThralls.Add(pawn);
            var reinforcements = new List<Pawn>();
            foreach (QuestPart part in QuestGen.quest?.PartsListForReading ?? [])
                if (part is QuestPart_DropPods pods)
                    foreach (Thing thing in pods.Things)
                        if (thing is Pawn pawn && pawn != master && !shuttleThralls.Contains(pawn))
                            reinforcements.Add(pawn);
            QuestPilot.DressThralls(master, masterDressed, shuttleThralls, reinforcements, points);
        }
        catch (Exception exception)
        {
            Log.Error($"[Hemophage Warcasket] Could not read the crashed shuttle quest's pawns: {exception}");
        }
    }
}
