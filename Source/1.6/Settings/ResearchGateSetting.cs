using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace HemophageWarcasket;

// One def field holding a research project or none, as a slider over named stops: "none" first,
// then the projects the def offers, in the def's order. Until the player moves it the value is
// Default, the field as the XML (and any patch) leaves it, read with the options by
// LoadDefaults on every fresh load; only an override is saved, by def name, and landing on the
// default's stop clears it. A saved name this load does not offer is dropped for the default,
// and a default the def does not offer (another mod's patch) gets a stop of its own after the
// offered ones, so every value the row can show is a stop the slider can reach. The
// row's label and tooltip are "HPWC_" + key and that with "Desc", the tooltip's one argument
// the default's name, as a Tunable's are.
//
// A row laid out as a Tunable row is, its label in the shared label column, but for the value:
// a project's name is far wider than a number, so the value column is as wide as the widest
// stop's name with the default suffix, and the slider takes what is left.
public sealed class ResearchGateSetting
{
    // Saved in place of a def name for the first stop.
    private const string NoneName = "None";

    // The gap either side of the slider, a Tunable row's.
    private const float SliderPad = 10f;

    private readonly string key;
    private string overrideName;
    private List<ResearchProjectDef> options = [];

    public ResearchGateSetting(string key)
    {
        this.key = key;
        Tunable.AddLabel(Label);
    }

    public ResearchProjectDef Default { get; private set; }

    public ResearchProjectDef Value
    {
        get
        {
            if (overrideName == null)
                return Default;
            if (overrideName == NoneName)
                return null;
            return options.Find(option => option.defName == overrideName);
        }
    }

    public void LoadDefaults(ResearchProjectDef defaultValue, List<ResearchProjectDef> offered)
    {
        Default = defaultValue;
        options = offered == null ? [] : [.. offered];
        if (Default != null && !options.Contains(Default))
            options.Add(Default);
        if (overrideName != null && overrideName != NoneName && !options.Any(option => option.defName == overrideName))
            overrideName = null;
    }

    public void Reset() => overrideName = null;

    public void ExposeData() => Scribe_Values.Look(ref overrideName, key);

    public void DoRow(Listing_Standard listing)
    {
        Rect row = listing.GetRect(Tunable.RowHeight);
        row.xMin += Tunable.RowIndent;
        row.xMax -= Tunable.RowEndPad;
        Rect labelRect = row.LeftPartPixels(Tunable.LabelWidth(row.width));
        float valueWidth = ValueWidth();
        var valueRect = new Rect(row.xMax - valueWidth, row.y, valueWidth, row.height);
        var sliderRect = new Rect(labelRect.xMax + SliderPad, row.y,
            valueRect.x - labelRect.xMax - 2f * SliderPad, row.height);

        Widgets.DrawHighlightIfMouseover(row);
        TooltipHandler.TipRegion(row, ("HPWC_" + key + "Desc").Translate(Name(Default)));

        string shown = Name(Value);
        if (overrideName == null)
            shown += "HPWC_DefaultSuffix".Translate();
        TextAnchor anchor = Text.Anchor;
        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(labelRect, Label());
        Widgets.Label(valueRect, shown);
        Text.Anchor = anchor;

        int current = Stop(Value);
        int index = Mathf.RoundToInt(Widgets.HorizontalSlider(sliderRect, current, 0f, options.Count, middleAlignment: true));
        listing.Gap(Tunable.RowGap);
        if (index == current)
            return;
        if (index == Stop(Default))
            overrideName = null;
        else
            overrideName = index == 0 ? NoneName : options[index - 1].defName;
    }

    private string Label() => ("HPWC_" + key).Translate();

    // The stop a project sits on: 0 for none, then its place among the options.
    private int Stop(ResearchProjectDef project) => project == null ? 0 : options.IndexOf(project) + 1;

    private string Name(ResearchProjectDef project) =>
        project == null ? ("HPWC_" + key + "None").Translate() : project.LabelCap;

    private float ValueWidth()
    {
        string suffix = "HPWC_DefaultSuffix".Translate();
        float width = Text.CalcSize(Name(null) + suffix).x;
        foreach (ResearchProjectDef option in options)
            width = Mathf.Max(width, Text.CalcSize(Name(option) + suffix).x);
        return width;
    }
}
