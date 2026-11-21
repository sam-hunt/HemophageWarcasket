using RimWorld;
using Verse;

namespace HemophageWarcasket;

// Quest section: the chance that each sanguophage Biotech's two sanguophage quests generate
// wears the set, three sliders stored and shown in percent and written onto the chances of the
// armor def's QuestPilotExtension, and the research the player must have finished first, written
// onto its requiredResearch. QuestPilot reads all four as a quest is generated, so a write
// reaches the next quest and no pawn already made.
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
    public readonly ResearchGateSetting questResearch = new("QuestResearch");

    private bool questPilotLoaded;

    private void ExposeQuestPilotSettings()
    {
        questMeetingLeader.ExposeData();
        questMeetingGuest.ExposeData();
        questShipMaster.ExposeData();
        questResearch.ExposeData();
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
        questResearch.LoadDefaults(extension.requiredResearch, extension.researchOptions);
    }

    private void ApplyQuestPilotToDefs()
    {
        QuestPilotExtension extension = QuestPilot.Extension;
        if (!questPilotLoaded || extension == null)
            return;
        extension.meetingLeaderChance = questMeetingLeader.Value / PercentScale;
        extension.meetingGuestChance = questMeetingGuest.Value / PercentScale;
        extension.shipMasterChance = questShipMaster.Value / PercentScale;
        extension.requiredResearch = questResearch.Value;
    }

    private void ResetQuestPilotSettings()
    {
        questMeetingLeader.Reset();
        questMeetingGuest.Reset();
        questShipMaster.Reset();
        questResearch.Reset();
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
        questResearch.DoRow(listing);
    }
}
