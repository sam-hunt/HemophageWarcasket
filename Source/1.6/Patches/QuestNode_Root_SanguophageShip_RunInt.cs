using System.Reflection;
using HarmonyLib;
using RimWorld.QuestGen;
using Verse;

namespace HemophageWarcasket.Patches;

// The sanguophage master of Biotech's crashed sanguophage shuttle quest, offered to QuestPilot.
// RunInt (1.7.5014 and the frozen 1.6 assembly, the same on both) generates the master through
// quest.GeneratePawn, sets health.forceDowned on them, which spares them the death roll when
// they are first downed and downs no one by itself, and stores the pawn in the quest's slate as
// "sanguophage", so a postfix reads it back from QuestGen.slate, which is still this quest's
// while the root node runs. Slate.Get answers null for a name never set. The thralls generated
// beside the master are not hemogenic and are not offered.
//
// The quest's threat points are read from the slate as "points", the read RunInt itself makes
// (slate.Get("points", 0f)), so a quest generated with no points in its slate reads 0 and is
// under any floor.
//
// This runs once for each such quest generated, not when the shuttle crashes: the master waits
// in the quest wearing the set.
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
        Log.Warning($"[Hemophage Warcasket] {nameof(QuestNode_Root_SanguophageShip)}.{Method} not found, so the crashed shuttle's sanguophage master will never wear the hemophage warcasket.");
        return false;
    }

    public static MethodBase TargetMethod() => Target;

    public static void Postfix() =>
        QuestPilot.TryDress(QuestGen.slate?.Get<Pawn>("sanguophage"), QuestPilotSource.ShipMaster,
            QuestGen.slate?.Get("points", 0f) ?? 0f);
}
