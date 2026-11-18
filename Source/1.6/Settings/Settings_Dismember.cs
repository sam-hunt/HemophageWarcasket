using RimWorld;
using UnityEngine;
using Verse;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace HemophageWarcasket;

// Dismember section: the pads' strike and drain. On the HPWC_Dismember AbilityDef the cooldown
// overrides cooldownTime (stored in game hours, ticks on the def) and the windup castTime (real
// seconds, ticks on the def). On its DismemberExtension the drain time overrides drainTicks
// (real seconds), the hemogen gain hemogenGain (on the 0..100 scale the game shows), the blood
// loss targetBloodLoss (percent), the spatter bloodFilthCount, and the two boxes
// canTargetNonHostile and decapitateBodiesWithoutLimbs. bloodFilthRadius has no setting. The
// header carries the ability's icon and tooltip.
//
// Every field is read per cast, so a write needs nothing of the live abilities but the running
// cooldown, which each caps on its next gizmo request (Ability_Dismember.ReconcileSettings).
public partial class HemophageWarcasketSettings
{
    public readonly Tunable dismemberCooldown = new("DismemberCooldown", 0f, CooldownMax, 0.25f, Period)
    {
        StepBands = CooldownBands,
    };

    // The family's casting time bands: quarter seconds to three, halves to six, whole to ten.
    public readonly Tunable dismemberWindup = new("DismemberWindup", 0f, 10f, 0.25f, RealSeconds)
    {
        StepBands = [(3f, 0.25f), (6f, 0.5f), (10f, 1f)],
    };

    // From half a second: no drain at all is the gain's 0, not a drain of no length.
    public readonly Tunable dismemberDrainTime = new("DismemberDrainTime", 0.5f, 10f, 0.5f, RealSeconds);

    // Up to a full bar's worth; at 0 the pads only tear (Ability_Dismember.CanDrain).
    public readonly Tunable dismemberGain = new("DismemberGain", 0f, 100f, 5f, Whole);

    // Blood loss kills at 100%.
    public readonly Tunable dismemberBloodLoss = new("DismemberBloodLoss", 0f, 100f, 5f, Percent);

    public readonly Tunable dismemberSpatter = new("DismemberSpatter", 0f, 20f, 1f, Whole);

    public readonly ToggleSetting dismemberNonHostile = new("DismemberNonHostile");
    public readonly ToggleSetting dismemberBehead = new("DismemberBehead");

    // The ability def, for its header icon, and the header's tooltip.
    private AbilityDef dismemberDef;
    private string dismemberTip;

    private void ExposeDismemberSettings()
    {
        dismemberCooldown.ExposeData();
        dismemberWindup.ExposeData();
        dismemberDrainTime.ExposeData();
        dismemberGain.ExposeData();
        dismemberBloodLoss.ExposeData();
        dismemberSpatter.ExposeData();
        dismemberNonHostile.ExposeData();
        dismemberBehead.ExposeData();
    }

    private void LoadDismemberDefaults(Defs defs)
    {
        DismemberExtension ext = defs.dismemberExtension;
        dismemberCooldown.DefaultValue = (float)defs.dismember.cooldownTime / GenDate.TicksPerHour;
        dismemberWindup.DefaultValue = (float)defs.dismember.castTime / GenTicks.TicksPerRealSecond;
        dismemberDrainTime.DefaultValue = (float)ext.drainTicks / GenTicks.TicksPerRealSecond;
        dismemberGain.DefaultValue = ext.hemogenGain * HemogenScale;
        dismemberBloodLoss.DefaultValue = ext.targetBloodLoss * PercentScale;
        dismemberSpatter.DefaultValue = ext.bloodFilthCount;
        dismemberNonHostile.DefaultValue = ext.canTargetNonHostile;
        dismemberBehead.DefaultValue = ext.decapitateBodiesWithoutLimbs;
        dismemberDef = defs.dismember;
        dismemberTip = HeaderTip(dismemberDef.LabelCap, dismemberDef.description);
    }

    private void ApplyDismemberToDefs(Defs defs)
    {
        DismemberExtension ext = defs.dismemberExtension;
        defs.dismember.cooldownTime = Mathf.RoundToInt(dismemberCooldown.Value * GenDate.TicksPerHour);
        defs.dismember.castTime = Mathf.RoundToInt(dismemberWindup.Value * GenTicks.TicksPerRealSecond);
        ext.drainTicks = Mathf.Max(1, Mathf.RoundToInt(dismemberDrainTime.Value * GenTicks.TicksPerRealSecond));
        ext.hemogenGain = dismemberGain.Value / HemogenScale;
        ext.targetBloodLoss = dismemberBloodLoss.Value / PercentScale;
        ext.bloodFilthCount = dismemberSpatter.IntValue;
        ext.canTargetNonHostile = dismemberNonHostile.Value;
        ext.decapitateBodiesWithoutLimbs = dismemberBehead.Value;
    }

    private void ResetDismemberSettings()
    {
        dismemberCooldown.Reset();
        dismemberWindup.Reset();
        dismemberDrainTime.Reset();
        dismemberGain.Reset();
        dismemberBloodLoss.Reset();
        dismemberSpatter.Reset();
        dismemberNonHostile.Reset();
        dismemberBehead.Reset();
    }

    private void DrawDismemberSection(Listing_Standard listing)
    {
        SectionHeader(listing, "HPWC_SectionDismember", dismemberDef.icon, dismemberTip);
        SummaryRow(listing, DismemberSummary);
        dismemberCooldown.DoRow(listing);
        dismemberWindup.DoRow(listing);
        dismemberDrainTime.DoRow(listing);
        dismemberGain.DoRow(listing);
        dismemberBloodLoss.DoRow(listing);
        dismemberSpatter.DoRow(listing);
        dismemberNonHostile.DoRow(listing);
        dismemberBehead.DoRow(listing);
    }

    // What the section's rows amount to, a sentence for the strike and one for the drain. Each
    // has a variant for its slider at zero (no cooldown, no drain), and every combination is a
    // key of its own so no language has to join two sentences. All take the same arguments.
    private string DismemberSummary()
    {
        bool cooldown = dismemberCooldown.Value > 0f;
        bool drain = dismemberGain.Value > 0f;
        string key = cooldown
            ? drain ? "HPWC_DismemberSummary" : "HPWC_DismemberSummaryNoDrain"
            : drain ? "HPWC_DismemberSummaryNoCooldown" : "HPWC_DismemberSummaryNoCooldownNoDrain";
        return key.Translate(RealSeconds(dismemberWindup.Value), Period(dismemberCooldown.Value),
            RealSeconds(dismemberDrainTime.Value), Whole(dismemberGain.Value), Percent(dismemberBloodLoss.Value));
    }
}
