using System;
using UnityEngine;
using Verse;

namespace HemophageWarcasket;

// The set's mod settings: the window frame, the ExposeData / LoadDefaultsFromDefs / ApplyToDefs /
// ResetToDefaults fan-out and the shared row helpers. The class is split across
// Settings_<Section>.cs partials, one per window section, each owning its fields, scribe
// entries, def-backed defaults, apply, reset and draw, so a setting is a one-file edit; each
// partial's header lists the def fields it writes. Each setting overrides a def field, written
// onto the def by ApplyToDefs (at startup, then whenever the window closes), so every reader of
// those fields, vanilla's and VEF's included, sees the setting with no code of its own. The
// defaults, and the rationale for them, are the defs'. Nothing here is saved into a game: a
// save loads the same with any settings, and with none.
//
// The one section so far is the pilot's work (Settings_Work.cs).
public partial class HemophageWarcasketSettings : ModSettings
{
    // A row is as tall as a vanilla SliderLabeled row, with a gap below it so neighbouring rows
    // read apart, indented so it sits under its section header (Listing.Indent only moves the
    // cursor and would push the row off the right) and padded on the right so it does not run
    // up against the scrollbar. The summary rows lay out with these too.
    internal const float RowHeight = 30f;
    internal const float RowGap = 6f;
    internal const float RowIndent = Listing.DefaultIndent;
    internal const float RowEndPad = Listing.DefaultIndent;

    // The whitespace above every section header but the first, and below the last row.
    private const float SectionGap = 28f;

    private Vector2 scrollPosition;
    private float contentHeight;

    public override void ExposeData()
    {
        base.ExposeData();
        ExposeWorkSettings();
    }

    // Reads the def-backed defaults before the first ApplyToDefs overwrites them; defs must be
    // loaded, so Startup.Run calls this, never the Mod constructor.
    public void LoadDefaultsFromDefs()
    {
        LoadWorkDefaults();
    }

    public void ApplyToDefs()
    {
        ApplyWorkToDefs();
    }

    public void ResetToDefaults()
    {
        ResetWorkSettings();
    }

    // Each section opens on its summary under its header, then one row per setting, and a reset
    // button sits below the list. The section is headed by whitespace and a medium-font header
    // rather than a rule, and the rows sit indented under it; it closes on the same gap so the
    // list does not end flush on its last row. The list scrolls once it outgrows the window's
    // height, sized from the previous frame's listing height; the scrollbar only takes its
    // width once it is needed. maxOneColumn keeps the listing growing downward: past its rect's
    // height it would start a new column off to the right, outside the window, and never report
    // the height the scroll view needs.
    public void DoWindowContents(Rect inRect)
    {
        const float buttonHeight = 30f;
        const float buttonWidth = 200f;
        const float scrollbarWidth = 16f;
        var outRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - buttonHeight - Listing.DefaultGap);
        float viewWidth = contentHeight > outRect.height ? outRect.width - scrollbarWidth : outRect.width;
        var viewRect = new Rect(0f, 0f, viewWidth, Mathf.Max(contentHeight, outRect.height));
        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
        var listing = new Listing_Standard { maxOneColumn = true };
        listing.Begin(viewRect);
        DrawWorkSection(listing, first: true);
        listing.Gap(SectionGap);
        contentHeight = listing.CurHeight;
        listing.End();
        Widgets.EndScrollView();

        if (Widgets.ButtonText(new Rect(inRect.x, inRect.yMax - buttonHeight, buttonWidth, buttonHeight),
                "RestoreToDefaultSettings".Translate()))
        {
            ResetToDefaults();
        }
    }

    // A muted line in a section, indented and padded as its rows are, as tall as its text, with
    // a row's gap below it.
    private static void SummaryRow(Listing_Standard listing, Func<string> summary)
    {
        string text = summary();
        float width = listing.ColumnWidth - RowIndent - RowEndPad;
        Rect rect = listing.GetRect(Text.CalcHeight(text, width));
        rect.xMin += RowIndent;
        rect.xMax -= RowEndPad;
        GUI.color = ColoredText.SubtleGrayColor;
        Widgets.Label(rect, text);
        GUI.color = Color.white;
        listing.Gap(RowGap);
    }

    // The header's text in the medium font after its icon, a square as tall as the header; a
    // header whose texture is missing starts its text at the left edge rather than after an
    // empty slot. The tooltip covers the icon and the text only, not the empty width after them.
    // The height is the icon's: the icons are full-bleed art, which a square much under 48px
    // blurs past recognition.
    private static void SectionHeader(Listing_Standard listing, string key, Texture2D icon, string tooltip, bool first = false)
    {
        const float headerGap = 4f;
        const float headerHeight = 48f;
        const float iconPad = 8f;
        if (!first)
            listing.Gap(SectionGap);
        Rect rect = listing.GetRect(headerHeight);
        float left = rect.xMin;
        if (icon != null && icon != BaseContent.BadTex)
        {
            GUI.DrawTexture(rect.LeftPartPixels(headerHeight), icon);
            rect.xMin += headerHeight + iconPad;
        }
        GameFont font = Text.Font;
        TextAnchor anchor = Text.Anchor;
        Text.Font = GameFont.Medium;
        Text.Anchor = TextAnchor.MiddleLeft;
        string label = key.Translate();
        Widgets.Label(rect, label);
        if (!tooltip.NullOrEmpty())
            TooltipHandler.TipRegion(new Rect(left, rect.y, rect.xMin - left + Text.CalcSize(label).x, rect.height), tooltip);
        Text.Anchor = anchor;
        Text.Font = font;
        listing.Gap(headerGap);
    }

    // A section's tooltip as VEF opens an ability's: its name in vanilla's tip title colour over
    // its description.
    private static string HeaderTip(string label, string description) =>
        description.NullOrEmpty() ? null : label.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + description;
}
