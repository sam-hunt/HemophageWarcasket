using Verse;

namespace HemophageWarcasket;

// What the pads' drain would do to the blood of a living target.
public enum DrainOutlook
{
    // Nothing worth a warning, or no drain at all.
    Harmless,

    // It would leave the target at a life threatening stage of blood loss.
    SeriousBloodLoss,

    // It would bleed the target to death.
    Kills,
}

// The drain's foresight as plain arithmetic on a hediff def, so it runs on a hand-built one.
// Ability_Dismember's header has when the cursor asks and with what severity.
public static class DismemberDrain
{
    // What a target left at this severity of the blood loss hediff comes to, by the two tests
    // Bloodfeed's own warning makes (CompAbilityEffect_BloodfeederBite.ExtraLabelMouseAttachment,
    // 1.6 and 1.7): the def's lethal severity, then whether the stage at that severity is life
    // threatening.
    public static DrainOutlook Outlook(HediffDef bloodLoss, float severity)
    {
        if (severity >= bloodLoss.lethalSeverity)
            return DrainOutlook.Kills;
        return bloodLoss.stages[bloodLoss.StageAtSeverity(severity)].lifeThreatening
            ? DrainOutlook.SeriousBloodLoss
            : DrainOutlook.Harmless;
    }
}
