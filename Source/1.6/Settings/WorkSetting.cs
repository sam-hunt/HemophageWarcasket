using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace HemophageWarcasket;

// The pilot's work: one checkbox per work type the trait mirror leaves to the player
// (WorkProfile.Toggleable, every work type VFEP's trait blocks). Until the player flips one it
// is WorkProfile.OpenByDefault, the rule the mirror applies on its own (the Intellectual work
// types open, the rest blocked), read live so the boxes follow whatever the loaded mods make of
// VFEP's trait. Only an override is saved, by the work type's defName, so it outlives the def
// instances a play-data reload replaces and a work type whose DLC or mod is off keeps its
// override for when it is back; flipping a box back to its default clears it. Written onto the
// trait by the settings' ApplyToDefs through WorkProfile.Apply.
public sealed class WorkSetting
{
    private const string Key = "Work";

    // The grid: the cells of one row, a cell's height, its box and the padding around the box.
    private const int GridColumns = 3;
    private const float CellHeight = 28f;
    private const float BoxSize = 24f;
    private const float CellPad = 4f;

    // Mixed into each cell's tooltip id, so it is steady from frame to frame and unlike
    // another window's.
    private const int TipIdBase = 0x48505743;

    private Dictionary<string, bool> overrides = [];

    public bool IsOpen(WorkTypeDef work) =>
        overrides.TryGetValue(work.defName, out bool open) ? open : WorkProfile.OpenByDefault(work);

    // The toggleable work types the pilot may take on, in WorkProfile.Toggleable's order.
    public IEnumerable<WorkTypeDef> Open => WorkProfile.Toggleable.Where(IsOpen);

    // The work tab's name for a work type; a modded one without it shows its defName.
    public static string Name(WorkTypeDef work) =>
        work.labelShort.NullOrEmpty() ? work.defName : work.labelShort;

    public void Reset() => overrides.Clear();

    public void ExposeData()
    {
        Scribe_Collections.Look(ref overrides, Key, LookMode.Value, LookMode.Value);
        overrides ??= [];
    }

    // A grid of GridColumns cells per row, filled across then down over WorkProfile.Toggleable.
    // Each cell is the box with the work tab's name for the work type to its right; a press
    // anywhere on the cell flips it and a drag paints that first flip's state across the cells
    // the stroke crosses. A cell's tooltip is the work type's own description over
    // "HPWC_WorkDesc", which ends on the default with its state's word in the box's colour.
    public void DoGrid(Listing_Standard listing)
    {
        IReadOnlyList<WorkTypeDef> works = WorkProfile.Toggleable;
        GameFont prevFont = Text.Font;
        TextAnchor prevAnchor = Text.Anchor;
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleLeft;

        for (int i = 0; i < works.Count; i += GridColumns)
        {
            Rect row = listing.GetRect(CellHeight);
            row.xMin += Tunable.RowIndent;
            row.xMax -= Tunable.RowEndPad;
            float cellWidth = row.width / GridColumns;
            for (int col = 0; col < GridColumns && i + col < works.Count; col++)
                DoCell(new Rect(row.x + (col * cellWidth), row.y, cellWidth, row.height), works[i + col]);
        }

        Text.Anchor = prevAnchor;
        Text.Font = prevFont;
        listing.Gap(Tunable.RowGap);
    }

    private void DoCell(Rect rect, WorkTypeDef work)
    {
        bool byDefault = WorkProfile.OpenByDefault(work);
        if (Mouse.IsOver(rect))
        {
            Widgets.DrawHighlight(rect);
            TooltipHandler.TipRegion(rect, new TipSignal(() => Tooltip(work, byDefault), TipIdBase ^ work.GetHashCode()));
        }

        bool open = IsOpen(work);
        Widgets.ToggleInvisibleDraggable(rect, ref open, doMouseoverSound: true, paintable: true);
        if (open == byDefault)
            overrides.Remove(work.defName);
        else
            overrides[work.defName] = open;

        var boxRect = new Rect(rect.x + CellPad, rect.y + ((rect.height - BoxSize) / 2f), BoxSize, BoxSize);
        Widgets.CheckboxDraw(boxRect.x, boxRect.y, open, disabled: false, BoxSize);
        var labelRect = new Rect(boxRect.xMax + CellPad, rect.y, rect.xMax - boxRect.xMax - (2f * CellPad), rect.height);
        Widgets.Label(labelRect, Name(work).CapitalizeFirst().Truncate(labelRect.width));
    }

    // Resolved by hand: a TaggedString handed over as a string loses its tags (the implicit
    // conversion strips them), colour tags included.
    private static string Tooltip(WorkTypeDef work, bool byDefault)
    {
        string ours = "HPWC_WorkDesc".Translate(ToggleSetting.StateWord(byDefault)).Resolve();
        return work.description.NullOrEmpty() ? ours : work.description + "\n\n" + ours;
    }
}
