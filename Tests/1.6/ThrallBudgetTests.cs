#nullable enable

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace HemophageWarcasket.Tests;

// ThrallBudget.Picks and ThrallBudget.Alternate, the crashed shuttle quest's thrall rule. It
// touches no game type, only the pure static class, so it runs anywhere. The rows are quests as vanilla fields them (100 combat power a pawn, the
// master included) against two sets priced 200 and 250 at a share of a quarter: nobody under
// the minimum points, the cheapest set at the minimum, then the dearest set first as the budget
// grows, with what is left covering a cheaper one. Picks are the indexes into the prices, written
// as a comma separated string, empty for none.
public class ThrallBudgetTests
{
    private static readonly float[] Prices = { 200f, 250f };

    private static string Pick(
        float points, float fielded, int thralls, float masterPrice = 0f, float share = 0.25f,
        float minPoints = 2500f, int minDressed = 1, float[]? prices = null) =>
        string.Join(",", ThrallBudget.Picks(
            points, fielded, thralls, masterPrice, share, minPoints, minDressed, prices ?? Prices)
            .Select(i => i.ToString(CultureInfo.InvariantCulture)));

    [Theory]
    [InlineData(2000f, 600f, 5, "")]
    [InlineData(2499f, 800f, 7, "")]
    [InlineData(2500f, 800f, 7, "0")]
    [InlineData(4000f, 900f, 8, "0")]
    [InlineData(5000f, 1000f, 9, "1")]
    [InlineData(6000f, 1100f, 10, "1")]
    [InlineData(6600f, 1200f, 11, "1,0")]
    [InlineData(7000f, 1100f, 10, "1,1")]
    [InlineData(8000f, 1200f, 11, "1,1,1")]
    [InlineData(10000f, 1200f, 11, "1,1,1,1,1")]
    public void Picks_SpendTheBudgetOnTheDearestSetItCovers(float points, float fielded, int thralls, string expected)
    {
        Assert.Equal(expected, Pick(points, fielded, thralls));
    }

    [Theory]
    [InlineData(5000f, 1000f, 9, "0")]
    [InlineData(8000f, 1200f, 11, "1")]
    [InlineData(10000f, 1200f, 11, "1,1,1")]
    public void Picks_PayTheDressedMastersPriceFirst(float points, float fielded, int thralls, string expected)
    {
        Assert.Equal(expected, Pick(points, fielded, thralls, masterPrice: 450f));
    }

    [Fact]
    public void Picks_AShareOfZeroDressesNoOne_TheMinimumIncluded()
    {
        Assert.Equal("", Pick(10000f, 1200f, 11, share: 0f, minDressed: 1));
    }

    [Fact]
    public void Picks_NoSetsOnOfferDressesNoOne()
    {
        Assert.Equal("", Pick(10000f, 1200f, 11, prices: new float[0]));
    }

    [Fact]
    public void Picks_NeverMoreThanThereAreThralls()
    {
        Assert.Equal("1,1,1", Pick(10000f, 1200f, 3, share: 1f));
    }

    [Fact]
    public void Picks_OneSetOnOfferIsPickedUntilTheBudgetRunsOut()
    {
        Assert.Equal("0,0,0,0,0,0", Pick(10000f, 1200f, 11, prices: new[] { 200f }));
    }

    [Fact]
    public void Picks_ADressedMinimumOfZeroAddsNone()
    {
        Assert.Equal("", Pick(2500f, 800f, 7, minDressed: 0));
    }

    [Theory]
    [InlineData(7, "0,0")]
    [InlineData(1, "0")]
    public void Picks_TheMinimumFillsWithTheCheapestSet_ToTheThrallCount(int thralls, string expected)
    {
        Assert.Equal(expected, Pick(2500f, 800f, thralls, minDressed: 2));
    }

    [Fact]
    public void Picks_DoNotAssumeAnOrderOfThePrices()
    {
        Assert.Equal("0,1", Pick(6600f, 1200f, 11, prices: new[] { 250f, 200f }));
    }

    [Fact]
    public void Alternate_TakesOneOfEachInTurn_ThenTheRestOfTheLongerFirst()
    {
        Assert.Equal(new[] { "a", "x", "b", "c" },
            ThrallBudget.Alternate(new[] { "a", "b", "c" }, new[] { "x" }));
    }

    [Fact]
    public void Alternate_OfAnEmptyFirst_IsTheSecond()
    {
        Assert.Equal(new[] { "x", "y" }, ThrallBudget.Alternate(new string[0], new[] { "x", "y" }));
    }

    [Fact]
    public void Alternate_TakesTheRestOfTheLongerSecond()
    {
        Assert.Equal(new[] { "a", "x", "y", "z" },
            ThrallBudget.Alternate(new[] { "a" }, new[] { "x", "y", "z" }));
    }
}
