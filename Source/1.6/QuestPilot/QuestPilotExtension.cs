using System.Collections.Generic;
using Verse;

namespace HemophageWarcasket;

// The sanguophages Biotech's two sanguophage quests bring to the player's map, dressed in the
// set. On the armor (ThingDefs_Misc/Warcasket_Hemophage.xml, whose header has the numbers),
// which with otherPieces is the set the pawn wears. QuestPilot has the rule and the two patches
// that call it; the settings write the three chances, the shuttle master's threat-point floor
// and requiredResearch.
public class QuestPilotExtension : DefModExtension
{
    // The chance that the sanguophage of each source wears the set, 0 to 1. At 0 that source
    // dresses no one and draws nothing from Rand.
    public float shipMasterChance;
    public float meetingLeaderChance;
    public float meetingGuestChance;

    // The quest's threat points the shuttle master's chance needs; under it the master is never
    // dressed. 0 is no floor.
    public float shipMasterMinPoints;

    // The research the player must have finished before any of the chances applies. Null is no
    // requirement.
    public ResearchProjectDef requiredResearch;

    // The projects the settings offer for requiredResearch after "none", earliest first.
    public List<ResearchProjectDef> researchOptions = [];

    // The set's pieces besides the armor carrying this extension. A pawn is dressed in all of
    // them or in none.
    public List<ThingDef> otherPieces = [];

    public float ChanceFor(QuestPilotSource source, float points) => source switch
    {
        QuestPilotSource.ShipMaster => points < shipMasterMinPoints ? 0f : shipMasterChance,
        QuestPilotSource.MeetingLeader => meetingLeaderChance,
        _ => meetingGuestChance,
    };

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string error in base.ConfigErrors())
            yield return error;
        if (shipMasterChance < 0f || shipMasterChance > 1f)
            yield return $"shipMasterChance must be between 0 and 1, is {shipMasterChance}";
        if (shipMasterMinPoints < 0f)
            yield return $"shipMasterMinPoints must not be negative, is {shipMasterMinPoints}";
        if (meetingLeaderChance < 0f || meetingLeaderChance > 1f)
            yield return $"meetingLeaderChance must be between 0 and 1, is {meetingLeaderChance}";
        if (meetingGuestChance < 0f || meetingGuestChance > 1f)
            yield return $"meetingGuestChance must be between 0 and 1, is {meetingGuestChance}";
        if (requiredResearch != null && !researchOptions.Contains(requiredResearch))
            yield return $"requiredResearch {requiredResearch.defName} is not among researchOptions, so the settings cannot show it";
        foreach (ThingDef piece in otherPieces)
        {
            if (piece?.apparel == null)
                yield return $"otherPieces names {piece?.defName ?? "a missing def"}, which is not apparel";
        }
    }
}

// Where Biotech generates a sanguophage for a quest: the master of the SanguophageShip quest's
// crashed shuttle, and the sanguophages of the SanguophageMeetingHost quest, of whom the first
// is the one who asks for the meeting and leads it.
public enum QuestPilotSource
{
    ShipMaster,
    MeetingLeader,
    MeetingGuest,
}
