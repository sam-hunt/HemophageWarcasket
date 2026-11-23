using RimWorld;
using UnityEngine;
using Verse;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace HemophageWarcasket;

// Dismember section: the pads' strike pool, strike and drain. On the pads'
// CompProperties_ApparelReloadable the capacity overrides maxCharges and the reload cost
// ammoCountPerCharge, vanilla's reload fields. On the HPWC_Dismember AbilityDef the cooldown
// overrides cooldownTime (stored in game hours, ticks on the def) and the windup castTime (real
// seconds, ticks on the def). On its DismemberExtension the strike damage overrides damage, the
// drain time drainTicks (real seconds), the hemogen gain hemogenGain (on the 0..100 scale the
// game shows), the blood loss targetBloodLoss (percent), the spatter bloodFilthCount, and the
// boxes canTargetNonHostile and, for the enemy pilots' one, aiUses. bloodFilthRadius and the
// two other AI fields have no setting. The header carries the ability's
// icon and tooltip. The summary quotes the gain as the pilot gets it, times the
// HemogenGainFactor the pads themselves give their wearer.
//
// Every field is read per cast, so a write needs nothing of the live abilities but the running
// cooldown, which each caps on its next gizmo request (Ability_Dismember.ReconcileSettings),
// and nothing of the live pads but the strikes over a lowered capacity, trimmed off the worn
// ones at once (TrimWornPads) and off any other when it is next worn or loaded (CompStrikePool).
public partial class HemophageWarcasketSettings
{
    public readonly Tunable dismemberStrikes = new("DismemberStrikes", 1f, 10f, 1f, Whole);
    public readonly Tunable dismemberReloadCost = new("DismemberReloadCost", 1f, 10f, 1f, Whole);

    public readonly Tunable dismemberCooldown = new("DismemberCooldown", 0f, CooldownMax, 0.25f, Period)
    {
        StepBands = CooldownBands,
    };

    // The family's casting time bands: quarter seconds to three, halves to six, whole to ten.
    public readonly Tunable dismemberWindup = new("DismemberWindup", 0f, 10f, 0.25f, RealSeconds)
    {
        StepBands = [(3f, 0.25f), (6f, 0.5f), (10f, 1f)],
    };

    // From a scratch to more than a thrumbo's leg has (240), past the 80 a compat root sets.
    public readonly Tunable dismemberDamage = new("DismemberDamage", 10f, 300f, 5f, Whole);

    // From half a second: no drain at all is the gain's 0, not a drain of no length.
    public readonly Tunable dismemberDrainTime = new("DismemberDrainTime", 0.5f, 10f, 0.5f, RealSeconds);

    // Up to a full bar's worth; at 0 the pads only tear (Ability_Dismember.CanDrain).
    public readonly Tunable dismemberGain = new("DismemberGain", 0f, 100f, 5f, Whole);

    // Blood loss kills at 100%.
    public readonly Tunable dismemberBloodLoss = new("DismemberBloodLoss", 0f, 100f, 5f, Percent);

    public readonly Tunable dismemberSpatter = new("DismemberSpatter", 0f, 20f, 1f, Whole);

    public readonly ToggleSetting dismemberNonHostile = new("DismemberNonHostile");

    // The strike for a pilot the player does not control.
    public readonly ToggleSetting dismemberAiUse = new("DismemberAiUse");

    // The ability def, for its header icon, and the header's tooltip.
    private AbilityDef dismemberDef;
    private string dismemberTip;

    // The reload item, the pool's charge noun and the pads' HemogenGainFactor on their wearer,
    // for the summary.
    private ThingDef dismemberAmmo;
    private string dismemberNoun;
    private float dismemberPadsGain = 1f;

    private void ExposeDismemberSettings()
    {
        dismemberStrikes.ExposeData();
        dismemberReloadCost.ExposeData();
        dismemberCooldown.ExposeData();
        dismemberWindup.ExposeData();
        dismemberDamage.ExposeData();
        dismemberDrainTime.ExposeData();
        dismemberGain.ExposeData();
        dismemberBloodLoss.ExposeData();
        dismemberSpatter.ExposeData();
        dismemberNonHostile.ExposeData();
        dismemberAiUse.ExposeData();
    }

    private void LoadDismemberDefaults(Defs defs)
    {
        DismemberExtension ext = defs.dismemberExtension;
        dismemberAmmo = defs.dismemberPool.ammoDef;
        dismemberNoun = defs.dismemberPool.chargeNoun;
        dismemberStrikes.DefaultValue = defs.dismemberPool.maxCharges;
        dismemberReloadCost.DefaultValue = defs.dismemberPool.ammoCountPerCharge;
        dismemberReloadCost.DescArgs = [Find.ActiveLanguageWorker.Pluralize(dismemberAmmo.label)];
        dismemberCooldown.DefaultValue = (float)defs.dismember.cooldownTime / GenDate.TicksPerHour;
        dismemberWindup.DefaultValue = (float)defs.dismember.castTime / GenTicks.TicksPerRealSecond;
        dismemberDamage.DefaultValue = ext.damage;
        dismemberDrainTime.DefaultValue = (float)ext.drainTicks / GenTicks.TicksPerRealSecond;
        dismemberGain.DefaultValue = ext.hemogenGain * HemogenScale;
        dismemberBloodLoss.DefaultValue = ext.targetBloodLoss * PercentScale;
        dismemberSpatter.DefaultValue = ext.bloodFilthCount;
        dismemberNonHostile.DefaultValue = ext.canTargetNonHostile;
        dismemberAiUse.DefaultValue = ext.aiUses;
        dismemberPadsGain = defs.padsHemogenGain;
        dismemberDef = defs.dismember;
        dismemberTip = HeaderTip(dismemberDef.LabelCap, dismemberDef.description);
    }

    private void ApplyDismemberToDefs(Defs defs)
    {
        DismemberExtension ext = defs.dismemberExtension;
        defs.dismemberPool.maxCharges = Mathf.Max(1, dismemberStrikes.IntValue);
        defs.dismemberPool.ammoCountPerCharge = Mathf.Max(1, dismemberReloadCost.IntValue);
        defs.dismember.cooldownTime = Mathf.RoundToInt(dismemberCooldown.Value * GenDate.TicksPerHour);
        defs.dismember.castTime = Mathf.RoundToInt(dismemberWindup.Value * GenTicks.TicksPerRealSecond);
        ext.damage = dismemberDamage.Value;
        ext.drainTicks = Mathf.Max(1, Mathf.RoundToInt(dismemberDrainTime.Value * GenTicks.TicksPerRealSecond));
        ext.hemogenGain = dismemberGain.Value / HemogenScale;
        ext.targetBloodLoss = dismemberBloodLoss.Value / PercentScale;
        ext.bloodFilthCount = dismemberSpatter.IntValue;
        ext.canTargetNonHostile = dismemberNonHostile.Value;
        ext.aiUses = dismemberAiUse.Value;
    }

    private void ResetDismemberSettings()
    {
        dismemberStrikes.Reset();
        dismemberReloadCost.Reset();
        dismemberCooldown.Reset();
        dismemberWindup.Reset();
        dismemberDamage.Reset();
        dismemberDrainTime.Reset();
        dismemberGain.Reset();
        dismemberBloodLoss.Reset();
        dismemberSpatter.Reset();
        dismemberNonHostile.Reset();
        dismemberAiUse.Reset();
    }

    private void DrawDismemberSection(Listing_Standard listing)
    {
        SectionHeader(listing, "HPWC_SectionDismember", dismemberDef.icon, dismemberTip);
        SummaryRow(listing, DismemberSummary);
        dismemberStrikes.DoRow(listing);
        dismemberReloadCost.DoRow(listing);
        dismemberWindup.DoRow(listing);
        dismemberCooldown.DoRow(listing);
        dismemberDamage.DoRow(listing);
        dismemberDrainTime.DoRow(listing);
        dismemberGain.DoRow(listing);
        dismemberBloodLoss.DoRow(listing);
        dismemberSpatter.DoRow(listing);
        dismemberNonHostile.DoRow(listing);
        dismemberAiUse.DoRow(listing);
    }

    // What the section's rows amount to: a sentence for the strikes held and what each costs to
    // reload, one for the strike and the drain, one for the cooldown. The last two have a
    // variant for their slider at zero (no drain, no cooldown), and every combination is a key
    // of its own so no language has to join two sentences. All take the same arguments.
    private string DismemberSummary()
    {
        bool cooldown = dismemberCooldown.Value > 0f;
        bool drain = dismemberGain.Value > 0f;
        string key = cooldown
            ? drain ? "HPWC_DismemberSummary" : "HPWC_DismemberSummaryNoDrain"
            : drain ? "HPWC_DismemberSummaryNoCooldown" : "HPWC_DismemberSummaryNoCooldownNoDrain";
        return key.Translate(RealSeconds(dismemberWindup.Value), Period(dismemberCooldown.Value),
            RealSeconds(dismemberDrainTime.Value), Whole(dismemberGain.Value * dismemberPadsGain),
            Percent(dismemberBloodLoss.Value),
            Ability_Dismember.CountedStrikes(dismemberNoun, dismemberStrikes.IntValue),
            Ability_Dismember.CountedReloadItem(dismemberAmmo.label, dismemberReloadCost.IntValue));
    }

    // Strikes over a lowered capacity come off the worn pads at once, before a pilot's next
    // reload can meet them (CompStrikePool has why).
    private static void TrimWornPads()
    {
        if (Current.ProgramState != ProgramState.Playing)
            return;
        foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive)
        {
            if (pawn.apparel == null)
                continue;
            foreach (Apparel apparel in pawn.apparel.WornApparel)
                apparel.TryGetComp<CompStrikePool>()?.Trim();
        }
    }
}
