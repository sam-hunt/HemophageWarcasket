#nullable enable

using System.Collections.Generic;
using Verse;
using Xunit;

namespace HemophageWarcasket.Tests;

// DismemberDrain.Outlook on hand-built blood loss defs, written from the contract in its
// header: Kills at or above the def's lethal severity, otherwise SeriousBloodLoss when the
// stage at that severity is life threatening, otherwise Harmless. The first def has vanilla's
// BloodLoss shape: lethal at 1, stages from 0, 0.15, 0.30, 0.45 and 0.60, only the last life
// threatening.
public class DismemberDrainTests
{
    private static HediffDef BloodLoss() => new()
    {
        lethalSeverity = 1f,
        stages =
        [
            new HediffStage { minSeverity = 0f },
            new HediffStage { minSeverity = 0.15f },
            new HediffStage { minSeverity = 0.30f },
            new HediffStage { minSeverity = 0.45f },
            new HediffStage { minSeverity = 0.60f, lifeThreatening = true },
        ],
    };

    [Theory]
    [InlineData(0f, DrainOutlook.Harmless)]
    [InlineData(0.2f, DrainOutlook.Harmless)]
    [InlineData(0.59f, DrainOutlook.Harmless)]
    [InlineData(0.6f, DrainOutlook.SeriousBloodLoss)]
    [InlineData(0.8f, DrainOutlook.SeriousBloodLoss)]
    [InlineData(0.99f, DrainOutlook.SeriousBloodLoss)]
    [InlineData(1f, DrainOutlook.Kills)]
    [InlineData(1.5f, DrainOutlook.Kills)]
    public void Outlook_FollowsTheStageThenTheLethalSeverity(float severity, DrainOutlook expected)
    {
        Assert.Equal(expected, DismemberDrain.Outlook(BloodLoss(), severity));
    }

    [Fact]
    public void Outlook_OnADefWithNoLifeThreateningStage_IsOnlyHarmlessOrKills()
    {
        var def = new HediffDef
        {
            lethalSeverity = 1f,
            stages = [new HediffStage { minSeverity = 0f }, new HediffStage { minSeverity = 0.5f }],
        };
        Assert.Equal(DrainOutlook.Harmless, DismemberDrain.Outlook(def, 0f));
        Assert.Equal(DrainOutlook.Harmless, DismemberDrain.Outlook(def, 0.99f));
        Assert.Equal(DrainOutlook.Kills, DismemberDrain.Outlook(def, 1f));
    }
}
