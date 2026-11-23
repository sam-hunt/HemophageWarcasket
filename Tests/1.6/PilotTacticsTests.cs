#nullable enable

using Xunit;

namespace HemophageWarcasket.Tests;

// PilotTactics, what a pilot the player does not control decides about the rush, the strike and
// the hemogen kept for the rush. It touches no game type, only the pure static class and our own
// StrikeOutlook, so it runs anywhere. The distances and the kept level are the defs' own: the
// rush from 25 cells in a melee job and from 5 in any, half the pilot's hemogen kept.
public class PilotTacticsTests
{
    private const float Start = 25f;
    private const float Close = 5f;

    [Theory]
    // On its way to hit the threat: from the start distance in.
    [InlineData(true, true, 60f, false)]
    [InlineData(true, true, 25.1f, false)]
    [InlineData(true, true, 25f, true)]
    [InlineData(true, true, 1f, true)]
    // In any other job, a gun's or a spine's: only once the threat is close.
    [InlineData(true, false, 25f, false)]
    [InlineData(true, false, 5.1f, false)]
    [InlineData(true, false, 5f, true)]
    [InlineData(true, false, 0f, true)]
    // A door being broken down or a downed pawn, however near.
    [InlineData(false, true, 1f, false)]
    [InlineData(false, false, 1f, false)]
    public void RushEngaged_NeedsAThreatCloseOrBeingClosedOn(bool threat, bool meleeJob, float distance, bool expected)
    {
        Assert.Equal(expected, PilotTactics.RushEngaged(threat, meleeJob, distance, Start, Close));
    }

    [Theory]
    [InlineData(StrikeOutlook.Tears, false, true)]
    [InlineData(StrikeOutlook.MayTear, false, true)]
    [InlineData(StrikeOutlook.Wounds, false, false)]
    [InlineData(StrikeOutlook.None, false, false)]
    // The head is never worth it, whatever the strike would do to it.
    [InlineData(StrikeOutlook.Tears, true, false)]
    [InlineData(StrikeOutlook.MayTear, true, false)]
    [InlineData(StrikeOutlook.Wounds, true, false)]
    [InlineData(StrikeOutlook.None, true, false)]
    public void WorthStriking_IsALimbThatCanComeOff(StrikeOutlook outlook, bool decapitates, bool expected)
    {
        Assert.Equal(expected, PilotTactics.WorthStriking(outlook, decapitates));
    }

    [Theory]
    // Half kept: a spine's 20 is paid from 70 up, so a full pilot has two.
    [InlineData(1f, 0.2f, 0.5f, true)]
    [InlineData(0.8f, 0.2f, 0.5f, true)]
    [InlineData(0.75f, 0.2f, 0.5f, true)]
    [InlineData(0.6f, 0.2f, 0.5f, false)]
    [InlineData(0.5f, 0.2f, 0.5f, false)]
    // Nothing kept is the game's own test, all of it kept refuses every cost.
    [InlineData(0.2f, 0.2f, 0f, true)]
    [InlineData(0.1f, 0.2f, 0f, false)]
    [InlineData(1f, 0.05f, 1f, false)]
    public void AffordsAbility_SpendsOnlyWhatIsAboveTheKeptLevel(float hemogen, float cost, float kept, bool expected)
    {
        Assert.Equal(expected, PilotTactics.AffordsAbility(hemogen, cost, kept));
    }
}
