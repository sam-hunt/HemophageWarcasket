using RimWorld;
using Verse;

namespace HemophageWarcasket;

// Quest section: the chance that each sanguophage Biotech's two sanguophage quests generate
// wears the set, three sliders stored and shown in percent and written onto the chances of the
// armor def's QuestPilotExtension, the shuttle master's threat-point floor, stored and shown in
// threat points on a banded slider with 0 shown as off and written onto shipMasterMinPoints,
// the research the player must have finished first, written onto its requiredResearch, and the
// share of the crashed shuttle quest's threat points that dresses its thralls, stored and shown
// in percent and written onto thrallBudgetShare. QuestPilot reads all six as a quest is
// generated, so a write reaches the next quest and no pawn already made.
//
// The section needs the armor's extension alone, found afresh by every load and apply since a
// play-data reload replaces it, and stands while the abilities' defs are missing; without the
// extension (another mod's patch) it is not drawn and nothing is written.
//
// The header carries Biotech's sanguophage xenotype icon, the quests' own subject. It is read
// off the def at draw time: a vanilla texture, which a play-data reload leaves alone, on a def
// that reload replaces.
public partial class HemophageWarcasketSettings
{
    public readonly Tunable questMeetingLeader = new("QuestMeetingLeader", 0f, 100f, 5f, Percent);
    public readonly Tunable questMeetingGuest = new("QuestMeetingGuest", 0f, 100f, 5f, Percent);
    public readonly Tunable questShipMaster = new("QuestShipMaster", 0f, 100f, 5f, Percent);

    // The floor's range is the storyteller's, whose threat points stop at 10000. Steps of 250
    // to 3000, across the points at which the quest's reinforcements begin, then 500 to 5000
    // and 1000 beyond.
    public readonly Tunable questShipMasterPoints =
        new("QuestShipMasterPoints", 0f, 10000f, 250f, OffOrWhole)
    {
        StepBands = [(3000f, 250f), (5000f, 500f), (10000f, 1000f)],
    };
    public readonly ResearchGateSetting questResearch = new("QuestResearch");
    public readonly Tunable questThrallShare = new("QuestThrallShare", 0f, 100f, 5f, OffOrPercent);

    private static string OffOrWhole(float v) =>
        v <= 0f ? "HPWC_Off".Translate().ToString() : Whole(v);

    private static string OffOrPercent(float v) =>
        v <= 0f ? "HPWC_Off".Translate().ToString() : Percent(v);

    private bool questPilotLoaded;

    private void ExposeQuestPilotSettings()
    {
        questMeetingLeader.ExposeData();
        questMeetingGuest.ExposeData();
        questShipMaster.ExposeData();
        questShipMasterPoints.ExposeData();
        questResearch.ExposeData();
        questThrallShare.ExposeData();
    }

    private void LoadQuestPilotDefaults()
    {
        QuestPilotExtension extension = QuestPilot.Extension;
        questPilotLoaded = extension != null;
        if (!questPilotLoaded)
            return;
        questMeetingLeader.DefaultValue = extension.meetingLeaderChance * PercentScale;
        questMeetingGuest.DefaultValue = extension.meetingGuestChance * PercentScale;
        questShipMaster.DefaultValue = extension.shipMasterChance * PercentScale;
        questShipMasterPoints.DefaultValue = extension.shipMasterMinPoints;
        questResearch.LoadDefaults(extension.requiredResearch, extension.researchOptions);
        questThrallShare.DefaultValue = extension.thrallBudgetShare * PercentScale;
    }

    private void ApplyQuestPilotToDefs()
    {
        QuestPilotExtension extension = QuestPilot.Extension;
        if (!questPilotLoaded || extension == null)
            return;
        extension.meetingLeaderChance = questMeetingLeader.Value / PercentScale;
        extension.meetingGuestChance = questMeetingGuest.Value / PercentScale;
        extension.shipMasterChance = questShipMaster.Value / PercentScale;
        extension.shipMasterMinPoints = questShipMasterPoints.Value;
        extension.requiredResearch = questResearch.Value;
        extension.thrallBudgetShare = questThrallShare.Value / PercentScale;
    }

    private void ResetQuestPilotSettings()
    {
        questMeetingLeader.Reset();
        questMeetingGuest.Reset();
        questShipMaster.Reset();
        questShipMasterPoints.Reset();
        questResearch.Reset();
        questThrallShare.Reset();
    }

    private void DrawQuestPilotSection(Listing_Standard listing)
    {
        if (!questPilotLoaded)
            return;
        SectionHeader(listing, "HPWC_SectionQuestPilot", XenotypeDefOf.Sanguophage?.Icon,
            HeaderTip("HPWC_SectionQuestPilot".Translate(), "HPWC_SectionQuestPilotDesc".Translate()));
        questMeetingLeader.DoRow(listing);
        questMeetingGuest.DoRow(listing);
        questShipMaster.DoRow(listing);
        questShipMasterPoints.DoRow(listing);
        questResearch.DoRow(listing);
        questThrallShare.DoRow(listing);
    }
}
