using System;
using RimWorld;
using UnityEngine;
using Verse;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace HemophageWarcasket;

// The set's mod settings: the window frame, the ExposeData / LoadDefaultsFromDefs / ApplyToDefs /
// ResetToDefaults fan-out, the value formats and the shared row helpers. The class is split
// across Settings_<Section>.cs partials, one per window section (the hemogen rush, Dismember,
// the pilot's work), each owning its fields, scribe entries, def-backed defaults, apply, reset
// and draw, so a setting is a one-file edit; each partial's header lists the def fields it
// writes. Each setting overrides a def field, written onto the def by ApplyToDefs (at startup,
// then whenever the window closes), so every reader of those fields, vanilla's and VEF's
// included, sees the setting with no code of its own. The defaults, and the rationale for them,
// are the defs'. Nothing here is saved into a game: a save loads the same with any settings,
// and with none.
//
// The two ability sections write through def handles found by name (TryGetDefs), all or
// nothing; the work section needs none of them and stands when they are missing. Each ability
// section's header carries the ability's own icon, read off the def at draw time: VEF resolves
// def.icon late in loading, and a play-data reload replaces the def and its texture together.
public partial class HemophageWarcasketSettings : ModSettings
{
    private const string RushDefName = "HPWC_HemogenRush";
    private const string DismemberDefName = "HPWC_Dismember";

    // The whitespace above every section header but the first, and below the last row.
    private const float SectionGap = 28f;

    // Hemogen is stored on a 0..1 scale and shown, by the game and by these rows, as 0..100.
    private const float HemogenScale = 100f;

    // A factor or a severity is stored on the def as a fraction and shown as a percentage.
    private const float PercentScale = 100f;

    // Bumped by every ApplyToDefs, the one at startup included, so a live ability can tell a
    // settings write has happened since it last looked (Ability_Dismember.ReconcileSettings).
    public static int Generation { get; private set; }

    // The family's cooldown slider, in game hours up to ten days, banded so one slider spans a
    // gap between strikes in a fight and one between raids: real seconds at normal speed up to
    // ten (quarter, then half, then whole seconds), then half hours to 3 h, hours to 12 h,
    // quarter days to a day, half days to three, and whole days.
    private const float CooldownMax = 10f * GenDate.HoursPerDay;
    private static readonly (float upTo, float step)[] CooldownBands =
    [
        (Seconds(3f), Seconds(0.25f)), (Seconds(5f), Seconds(0.5f)), (Seconds(10f), Seconds(1f)),
        (3f, 0.5f), (12f, 1f), (24f, 6f), (72f, 12f), (CooldownMax, GenDate.HoursPerDay),
    ];

    private bool abilityDefaultsLoaded;
    private Vector2 scrollPosition;
    private float contentHeight;

    private static string Whole(float v) => Mathf.RoundToInt(v).ToString();

    private static string Percent(float v) => Mathf.RoundToInt(v) + "%";

    // Game hours from real seconds at normal speed.
    private static float Seconds(float s) => s * GenTicks.TicksPerRealSecond / GenDate.TicksPerHour;

    // Every time a row or summary shows carries its unit as vanilla's short period form does
    // (GenDate.ToStringTicksToPeriod with shortForm): the number, then the unit letter, no space.
    private static string RealSeconds(float s) => s.ToString("0.##") + "LetterSecond".Translate();

    // A period in game hours, in the unit of the cooldown band it falls in: seconds below the
    // half hour, hours below a day, days from there.
    private static string Period(float hours)
    {
        if (hours < 0.5f)
            return RealSeconds(hours * GenDate.TicksPerHour / GenTicks.TicksPerRealSecond);
        if (hours < GenDate.HoursPerDay)
            return hours.ToString("0.##") + "LetterHour".Translate();
        return (hours / GenDate.HoursPerDay).ToString("0.##") + "LetterDay".Translate();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        ExposeRushSettings();
        ExposeDismemberSettings();
        ExposeWorkSettings();
    }

    // Reads the def-backed defaults before the first ApplyToDefs overwrites them; defs must be
    // loaded, so Startup.Run calls this, never the Mod constructor.
    public void LoadDefaultsFromDefs()
    {
        LoadWorkDefaults();
        abilityDefaultsLoaded = TryGetDefs(out Defs defs);
        if (!abilityDefaultsLoaded)
            return;
        LoadRushDefaults(defs);
        LoadDismemberDefaults(defs);
    }

    // The work profile goes first: it needs none of the abilities' defs, so a saved choice
    // still holds while they are missing.
    public void ApplyToDefs()
    {
        ApplyWorkToDefs();
        if (!abilityDefaultsLoaded || !TryGetDefs(out Defs defs))
            return;
        ApplyRushToDefs(defs);
        ApplyDismemberToDefs(defs);
        Generation++;
    }

    public void ResetToDefaults()
    {
        ResetRushSettings();
        ResetDismemberSettings();
        ResetWorkSettings();
    }

    // Each section opens on its summary under its header, then one row per setting, and a reset
    // button sits below the list. The section is headed by whitespace and a medium-font header
    // rather than a rule, and the rows sit indented under it; it closes on the same gap so the
    // list does not end flush on its last row. The list scrolls once it outgrows the window's
    // height, sized from the previous frame's listing height; the scrollbar only takes its
    // width once it is needed. maxOneColumn keeps the listing growing downward: past its rect's
    // height it would start a new column off to the right, outside the window, and never report
    // the height the scroll view needs. While the abilities' defs were not found there is
    // nothing of theirs to tune, so one line says so in place of their sections.
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
        if (abilityDefaultsLoaded)
        {
            DrawRushSection(listing, first: true);
            DrawDismemberSection(listing);
        }
        else
        {
            SummaryRow(listing, () => "HPWC_SettingsUnavailable".Translate());
        }
        DrawWorkSection(listing);
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
        float width = listing.ColumnWidth - Tunable.RowIndent - Tunable.RowEndPad;
        Rect rect = listing.GetRect(Text.CalcHeight(text, width));
        rect.xMin += Tunable.RowIndent;
        rect.xMax -= Tunable.RowEndPad;
        GUI.color = ColoredText.SubtleGrayColor;
        Widgets.Label(rect, text);
        GUI.color = Color.white;
        listing.Gap(Tunable.RowGap);
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

    // The def objects the two ability sections read and write, looked up afresh by every load
    // and apply, since a play-data reload replaces them all.
    private struct Defs
    {
        public AbilityDef rushAbility;
        public HediffCompProperties_HemogenRush rush;
        public StatModifier rushMoveSpeed;
        public StatModifier rushMeleeDamage;
        public AbilityDef dismember;
        public DismemberExtension dismemberExtension;
    }

    // All or nothing: with any target missing (another mod patched it away) the settings leave
    // every ability def at its XML values and say so once, as a warning, since the set itself
    // still works.
    private static bool warned;

    private static bool TryGetDefs(out Defs defs)
    {
        HediffDef rush = DefDatabase<HediffDef>.GetNamedSilentFail(RushDefName);
        HediffStage stage = rush?.stages?.Count == 1 ? rush.stages[0] : null;
        AbilityDef dismember = DefDatabase<AbilityDef>.GetNamedSilentFail(DismemberDefName);
        defs = new Defs
        {
            rushAbility = DefDatabase<AbilityDef>.GetNamedSilentFail(RushDefName),
            rush = rush?.CompProps<HediffCompProperties_HemogenRush>(),
            rushMoveSpeed = stage?.statFactors?.Find(m => m.stat == StatDefOf.MoveSpeed),
            rushMeleeDamage = stage?.statFactors?.Find(m => m.stat == StatDefOf.MeleeDamageFactor),
            dismember = dismember,
            dismemberExtension = dismember?.GetModExtension<DismemberExtension>(),
        };
        if (defs.rushAbility != null && defs.rush != null && defs.rushMoveSpeed != null && defs.rushMeleeDamage != null
            && defs.dismember != null && defs.dismemberExtension != null)
        {
            return true;
        }
        if (!warned)
            Log.Warning("[Hemophage Warcasket] Settings could not find the hemogen rush ability, its hediff's comp, single stage or move speed and melee damage factors, the Dismember ability or its extension; the abilities keep their XML values.");
        warned = true;
        return false;
    }
}
