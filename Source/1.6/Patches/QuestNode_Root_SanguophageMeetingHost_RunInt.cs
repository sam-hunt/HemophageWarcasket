using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld.QuestGen;
using Verse;

namespace HemophageWarcasket.Patches;

// The sanguophages of Biotech's sanguophage meeting quest, offered to QuestPilot. RunInt
// (1.7.5014 and the frozen 1.6 assembly, the same on both) generates two to eight of them
// through quest.GeneratePawn and stores the list in the quest's slate as "sanguophages" and its
// first pawn as "asker", so a postfix reads both back from QuestGen.slate, which is still this
// quest's while the root node runs. Slate.Get answers null for a name never set. The asker is
// the leader the quest's text names, and the one who may offer to join the colony.
//
// This runs once for each such quest generated, not when the sanguophages walk in: they wait in
// the quest wearing the set.
//
// RunInt is protected, so it is bound by name, and as this type declares it, since the abstract
// QuestNode.RunInt would answer an inherited lookup: should it move, this patch stands down with
// a warning where a failed lookup would stop PatchAll and take every other patch with it.
[HarmonyPatch]
public static class QuestNode_Root_SanguophageMeetingHost_RunInt
{
    private const string Method = "RunInt";

    private static MethodBase Target => AccessTools.DeclaredMethod(typeof(QuestNode_Root_SanguophageMeetingHost), Method);

    public static bool Prepare()
    {
        if (Target != null)
            return true;
        Log.Warning($"[Hemophage Warcasket] {nameof(QuestNode_Root_SanguophageMeetingHost)}.{Method} not found, so a meeting's sanguophages will never wear the hemophage warcasket.");
        return false;
    }

    public static MethodBase TargetMethod() => Target;

    public static void Postfix()
    {
        List<Pawn> sanguophages = QuestGen.slate?.Get<List<Pawn>>("sanguophages");
        if (sanguophages == null)
            return;
        Pawn leader = QuestGen.slate.Get<Pawn>("asker");
        foreach (Pawn pawn in sanguophages)
            QuestPilot.TryDress(pawn, pawn == leader ? QuestPilotSource.MeetingLeader : QuestPilotSource.MeetingGuest);
    }
}
