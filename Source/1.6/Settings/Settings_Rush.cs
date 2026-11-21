using UnityEngine;
using Verse;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace HemophageWarcasket;

// Rush section: the armor's hemogen rush, all of it on the HPWC_HemogenRush hediff. On its
// HediffCompProperties_HemogenRush the start cost overrides hemogenStartCost, the drain
// hemogenPerSecond and the reserve hemogenReserve, all stored and shown on the 0..100 scale
// the game shows hemogen on, and the pulse box overrides glowPulses. On its one stage the
// melee damage overrides the MeleeDamageFactor statFactor and the move speed the MoveSpeed
// one, both stored and shown in percent. The move speed row is the rush's whole gain, so what
// it writes is that over the share the stage's BloodPumping offset already gives
// (RushPumpingSpeed). The stage's two capacity offsets have no setting. The header carries
// the HPWC_HemogenRush ability's icon and tooltip.
//
// A write reaches a running rush on its own: the comp and the glow worker read their
// properties as they run, and StatWorker reads a stage's statFactors on every call, with no
// per-pawn cache for either stat (1.6 and 1.7).
public partial class HemophageWarcasketSettings
{
    // Hemogen spent as a rush starts, up to five seconds of the default drain. At 0 a rush
    // costs nothing to switch on.
    private const float StartCostMax = 10f;
    public readonly Tunable rushStartCost = new("RushStartCost", 0f, StartCostMax, 1f, Whole);

    // Hemogen spent per second: quarter steps to 5, where the default sits and a full 100 lasts
    // 20 seconds or more, then half steps to 10. At 0 a running rush spends nothing.
    public readonly Tunable rushDrain = new("RushDrain", 0f, 10f, 0.25f, v => v.ToString("0.##"))
    {
        StepBands = [(5f, 0.25f), (10f, 0.5f)],
    };

    // The hemogen level the rush stops at. Short of 100 by more than the highest start cost,
    // so a full pilot can always start one.
    public readonly Tunable rushReserve = new("RushReserve", 0f, HemogenScale - StartCostMax - 5f, 5f, Whole);

    // The buff, from 100%, no change: a rush never weakens. At 100% the MoveSpeed factor
    // written is under 1, cancelling the blood pumping's share.
    public readonly Tunable rushMoveSpeed = new("RushMoveSpeed", 100f, 250f, 10f, Percent);
    public readonly Tunable rushMeleeDamage = new("RushMeleeDamage", 100f, 300f, 10f, Percent);

    public readonly ToggleSetting rushGlowPulse = new("RushGlowPulse");

    // The ability def, for its header icon (the class header of HemophageWarcasketSettings),
    // and the header's tooltip.
    private AbilityDef rushDef;
    private string rushTip;

    // Moving, and move speed with it, takes this share of blood pumping's gain over 100%
    // (PawnCapacityWorker_Moving's lerp; 1.6 and 1.7).
    private const float MovingPerBloodPumping = 0.2f;

    // What the stage's BloodPumping offset multiplies move speed by, for a pilot whose blood
    // pumping is otherwise 100%.
    private static float RushPumpingSpeed(Defs defs) =>
        1f + MovingPerBloodPumping * (defs.rushBloodPumping?.offset ?? 0f);

    private void ExposeRushSettings()
    {
        rushStartCost.ExposeData();
        rushDrain.ExposeData();
        rushReserve.ExposeData();
        rushMoveSpeed.ExposeData();
        rushMeleeDamage.ExposeData();
        rushGlowPulse.ExposeData();
    }

    private void LoadRushDefaults(Defs defs)
    {
        rushStartCost.DefaultValue = defs.rush.hemogenStartCost * HemogenScale;
        rushDrain.DefaultValue = defs.rush.hemogenPerSecond * HemogenScale;
        rushReserve.DefaultValue = defs.rush.hemogenReserve * HemogenScale;
        rushMoveSpeed.DefaultValue = defs.rushMoveSpeed.value * RushPumpingSpeed(defs) * PercentScale;
        rushMeleeDamage.DefaultValue = defs.rushMeleeDamage.value * PercentScale;
        rushGlowPulse.DefaultValue = defs.rush.glowPulses;
        rushDef = defs.rushAbility;
        rushTip = HeaderTip(rushDef.LabelCap, rushDef.description);
    }

    private void ApplyRushToDefs(Defs defs)
    {
        defs.rush.hemogenStartCost = rushStartCost.Value / HemogenScale;
        defs.rush.hemogenPerSecond = rushDrain.Value / HemogenScale;
        defs.rush.hemogenReserve = rushReserve.Value / HemogenScale;
        defs.rushMoveSpeed.value = rushMoveSpeed.Value / PercentScale / RushPumpingSpeed(defs);
        defs.rushMeleeDamage.value = rushMeleeDamage.Value / PercentScale;
        defs.rush.glowPulses = rushGlowPulse.Value;
    }

    private void ResetRushSettings()
    {
        rushStartCost.Reset();
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
        rushStartCost.DoRow(listing);
        rushDrain.DoRow(listing);
        rushReserve.DoRow(listing);
        rushMoveSpeed.DoRow(listing);
        rushMeleeDamage.DoRow(listing);
        rushGlowPulse.DoRow(listing);
    }

    // What the section's rows amount to: the start cost, if any, the drain and how long a rush
    // started at 100 hemogen lasts down to the reserve, then the buff. A rush without a drain
    // has no length to quote. One key per combination of a start cost and a drain.
    private string RushSummary()
    {
        string speed = Percent(rushMoveSpeed.Value);
        string damage = Percent(rushMeleeDamage.Value);
        float cost = rushStartCost.Value;
        float drain = rushDrain.Value;
        if (drain <= 0f)
        {
            return cost > 0f
                ? "HPWC_RushSummaryStartCostNoDrain".Translate(speed, damage, Whole(cost))
                : "HPWC_RushSummaryFree".Translate(speed, damage);
        }
        string lasts = RealSeconds(Mathf.Round((HemogenScale - rushReserve.Value - cost) / drain));
        return (cost > 0f ? "HPWC_RushSummaryStartCost" : "HPWC_RushSummary")
            .Translate(drain.ToString("0.##"), Whole(rushReserve.Value), lasts, speed, damage, Whole(cost));
    }
}
