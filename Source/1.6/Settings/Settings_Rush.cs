using UnityEngine;
using Verse;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace HemophageWarcasket;

// Rush section: the armor's hemogen rush, all of it on the HPWC_HemogenRush hediff. On its
// HediffCompProperties_HemogenRush the drain overrides hemogenPerSecond and the reserve
// hemogenReserve, both stored and shown on the 0..100 scale the game shows hemogen on, and the
// pulse box overrides glowPulses. On its one stage the move speed and melee damage override the
// MoveSpeed and MeleeDamageFactor statFactors, stored and shown in percent. The stage's
// Consciousness offset has no setting. The header carries the HPWC_HemogenRush ability's icon
// and tooltip.
//
// A write reaches a running rush on its own: the comp and the glow worker read their
// properties as they run, and StatWorker reads a stage's statFactors on every call, with no
// per-pawn cache for either stat (1.6 and 1.7).
public partial class HemophageWarcasketSettings
{
    // Hemogen spent per second: quarter steps to 5, where the default sits and a full 100 lasts
    // 20 seconds or more, then half steps to 10. At 0 the rush is free.
    public readonly Tunable rushDrain = new("RushDrain", 0f, 10f, 0.25f, v => v.ToString("0.##"))
    {
        StepBands = [(5f, 0.25f), (10f, 0.5f)],
    };

    // The hemogen level the rush stops at. Short of 100 so a full pilot can always start one.
    public readonly Tunable rushReserve = new("RushReserve", 0f, 90f, 5f, Whole);

    // The buff, from 100%, no change: a rush never weakens.
    public readonly Tunable rushMoveSpeed = new("RushMoveSpeed", 100f, 250f, 10f, Percent);
    public readonly Tunable rushMeleeDamage = new("RushMeleeDamage", 100f, 300f, 10f, Percent);

    public readonly ToggleSetting rushGlowPulse = new("RushGlowPulse");

    // The ability def, for its header icon (the class header of HemophageWarcasketSettings),
    // and the header's tooltip.
    private AbilityDef rushDef;
    private string rushTip;

    private void ExposeRushSettings()
    {
        rushDrain.ExposeData();
        rushReserve.ExposeData();
        rushMoveSpeed.ExposeData();
        rushMeleeDamage.ExposeData();
        rushGlowPulse.ExposeData();
    }

    private void LoadRushDefaults(Defs defs)
    {
        rushDrain.DefaultValue = defs.rush.hemogenPerSecond * HemogenScale;
        rushReserve.DefaultValue = defs.rush.hemogenReserve * HemogenScale;
        rushMoveSpeed.DefaultValue = defs.rushMoveSpeed.value * PercentScale;
        rushMeleeDamage.DefaultValue = defs.rushMeleeDamage.value * PercentScale;
        rushGlowPulse.DefaultValue = defs.rush.glowPulses;
        rushDef = defs.rushAbility;
        rushTip = HeaderTip(rushDef.LabelCap, rushDef.description);
    }

    private void ApplyRushToDefs(Defs defs)
    {
        defs.rush.hemogenPerSecond = rushDrain.Value / HemogenScale;
        defs.rush.hemogenReserve = rushReserve.Value / HemogenScale;
        defs.rushMoveSpeed.value = rushMoveSpeed.Value / PercentScale;
        defs.rushMeleeDamage.value = rushMeleeDamage.Value / PercentScale;
        defs.rush.glowPulses = rushGlowPulse.Value;
    }

    private void ResetRushSettings()
    {
        rushDrain.Reset();
        rushReserve.Reset();
        rushMoveSpeed.Reset();
        rushMeleeDamage.Reset();
        rushGlowPulse.Reset();
    }

    private void DrawRushSection(Listing_Standard listing, bool first = false)
    {
        SectionHeader(listing, "HPWC_SectionRush", rushDef.icon, rushTip, first);
        SummaryRow(listing, RushSummary);
        rushDrain.DoRow(listing);
        rushReserve.DoRow(listing);
        rushMoveSpeed.DoRow(listing);
        rushMeleeDamage.DoRow(listing);
        rushGlowPulse.DoRow(listing);
    }

    // What the section's rows amount to: the drain and how long a rush started at 100 hemogen
    // lasts down to the reserve, then the buff. A free rush has no length to quote.
    private string RushSummary()
    {
        string speed = Percent(rushMoveSpeed.Value);
        string damage = Percent(rushMeleeDamage.Value);
        float drain = rushDrain.Value;
        if (drain <= 0f)
            return "HPWC_RushSummaryFree".Translate(speed, damage);
        string lasts = RealSeconds(Mathf.Round((HemogenScale - rushReserve.Value) / drain));
        return "HPWC_RushSummary".Translate(drain.ToString("0.##"), Whole(rushReserve.Value), lasts, speed, damage);
    }
}
