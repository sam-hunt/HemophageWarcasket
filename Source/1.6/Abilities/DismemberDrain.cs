using Verse;

namespace HemophageWarcasket;

// What the pads' drain would do to the blood of a living target.
public enum DrainOutlook
{
    // Nothing worth a warning, or no drain at all.
    Harmless,

    // It would leave the target at a life threatening stage of blood loss, or at the severity
    // that kills on a target blood loss cannot kill.
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
    // threatening. Blood loss does not kill a target with the deathless gene: at the lethal
    // severity Pawn_HealthTracker.CheckForStateChange puts it into deathrest or a coma instead
    // (SanguophageUtility.ShouldBeDeathrestingOrInComaInsteadOfDead, 1.6 and 1.7), which
    // Bloodfeed's dialog allows for and its cursor line does not. Here that is serious blood
    // loss, whatever the def's stages say.
    public static DrainOutlook Outlook(HediffDef bloodLoss, float severity, bool deathless)
    {
        if (severity >= bloodLoss.lethalSeverity)
            return deathless ? DrainOutlook.SeriousBloodLoss : DrainOutlook.Kills;
        return bloodLoss.stages[bloodLoss.StageAtSeverity(severity)].lifeThreatening
            ? DrainOutlook.SeriousBloodLoss
            : DrainOutlook.Harmless;
    }
}
