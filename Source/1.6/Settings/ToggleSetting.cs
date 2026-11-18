using System;
using UnityEngine;
using Verse;

namespace HemophageWarcasket;

// One on/off def field, what a Tunable is for a number. Until the player flips it the value is
// DefaultValue, the def field as the XML (and any patch) leaves it, read by the settings'
// LoadDefaultsFromDefs; only an override is saved, under the key, and flipping the box back to
// the default clears it. The row's label and tooltip are "HPWC_" + key and that with "Desc",
// as a Tunable's are; the tooltip's one argument is the default's word, StateWord, which the
// work boxes' tooltips share so every box in the window names its default alike. A row that
// names a def sets Term, resolved on every draw so a language reload's label reads through: the
// label's {0} and the tooltip's {1}.
public sealed class ToggleSetting
{
    // A box's state as its tooltip's default names it, "enabled" or "disabled", in the colours
    // of the box's states: pure green for the tick, vanilla's readable red for the cross.
    private static readonly Color EnabledColor = Color.green;
    private static readonly Color DisabledColor = ColorLibrary.RedReadable;

    internal static string StateWord(bool enabled) => enabled
        ? "HPWC_Enabled".Translate().Colorize(EnabledColor)
        : "HPWC_Disabled".Translate().Colorize(DisabledColor);

    private readonly string key;
    private bool? overrideValue;

    public ToggleSetting(string key)
    {
        this.key = key;
    }

    public bool DefaultValue { get; set; }

    public Func<string> Term { get; set; }

    public bool Value => overrideValue ?? DefaultValue;

    public void Reset() => overrideValue = null;

    public void ExposeData() => Scribe_Values.Look(ref overrideValue, key);

    // A checkbox row laid out as a Tunable row is, the box at the row's end; a click anywhere on
    // the row flips it. The label and tooltip are resolved by hand: a TaggedString handed over as a
    // string loses its tags (the implicit conversion strips them), the default's colour and a
    // Term's among them. A forced-off row (a child box whose parent is off, or a box another mod
    // has made moot) is drawn unticked, what the game makes of it, in vanilla's inactive grey,
    // label and box alike; it takes no click and keeps its saved value, which shows again once
    // the row is active. Its tooltip still reads.
    public void DoRow(Listing_Standard listing, bool forcedOff = false)
    {
        Rect row = listing.GetRect(Tunable.RowHeight);
        row.xMin += Tunable.RowIndent;
        row.xMax -= Tunable.RowEndPad;
        if (!forcedOff)
            Widgets.DrawHighlightIfMouseover(row);
        string term = Term?.Invoke();
        TaggedString tip = term == null
            ? ("HPWC_" + key + "Desc").Translate(StateWord(DefaultValue))
            : ("HPWC_" + key + "Desc").Translate(StateWord(DefaultValue), term);
        TooltipHandler.TipRegion(row, tip.Resolve());

        bool on = !forcedOff && Value;
        if (forcedOff)
            GUI.color = Widgets.InactiveColor;
        TaggedString label = term == null ? ("HPWC_" + key).Translate() : ("HPWC_" + key).Translate(term);
        Widgets.CheckboxLabeled(row, label.Resolve(), ref on, forcedOff);
        GUI.color = Color.white;
        if (!forcedOff)
            overrideValue = on == DefaultValue ? null : on;
        listing.Gap(Tunable.RowGap);
    }
}
