using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace HemophageWarcasket;

// Work section: one checkbox per work type VFEP's warcasket trait blocks (WorkSetting over
// WorkProfile.Toggleable), written onto the hemophage trait's disabledWorkTags and
// disabledWorkTypes through WorkProfile.Apply, which also refreshes every entombed pilot; the
// mirror's own rule is each box's default. Its header's icon is vanilla's mining work site, a
// Core texture, so it survives a play-data reload as a plain static. With nothing to toggle (a
// patch mod has emptied VFEP's trait, or the mirror failed) the section is its header and one
// line saying so.
public partial class HemophageWarcasketSettings
{
    public readonly WorkSetting work = new();

    // The section's header icon. Its art is in the base game's Resources, not Ideology's, so it
    // loads whatever DLCs are active.
    private Texture2D workIcon;

    private void ExposeWorkSettings() => work.ExposeData();

    private void LoadWorkDefaults() =>
        workIcon = ContentFinder<Texture2D>.Get("World/WorldObjects/Expanding/Sites/MiningWorkSite");

    private void ApplyWorkToDefs() => WorkProfile.Apply(work.IsOpen);

    private void ResetWorkSettings() => work.Reset();

    private void DrawWorkSection(Listing_Standard listing, bool first = false)
    {
        SectionHeader(listing, "HPWC_SectionWork", workIcon,
            HeaderTip("HPWC_SectionWork".Translate(), "HPWC_SectionWorkDesc".Translate()), first);
        SummaryRow(listing, WorkSummary);
        work.DoGrid(listing);
    }

    // What the section's boxes amount to: the work they open, named as the work tab names it,
    // or that they open none, or that there are no boxes.
    private string WorkSummary()
    {
        if (WorkProfile.Toggleable.Count == 0)
            return "HPWC_WorkSummaryNothingBlocked".Translate();
        List<string> open = work.Open.Select(WorkSetting.Name).ToList();
        if (open.Count == 0)
            return "HPWC_WorkSummaryNone".Translate();
        return "HPWC_WorkSummary".Translate(open.ToCommaList(useAnd: true));
    }
}
