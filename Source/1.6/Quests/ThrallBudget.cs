using System.Collections.Generic;

namespace HemophageWarcasket;

// How many thralls of the crashed shuttle quest are dressed, and in which set. Pure, so the
// tests hold it; QuestPilot.DressThralls gathers its inputs and dresses what it answers.
//
// The quest reads its threat points for head count alone (the ship patch's header has the two
// curves), so against a raid at the same points, which spends about its points on combat power,
// it shrinks as the colony grows. The budget brings it back up to a share of that raid:
//
//   budget = share * points
//          - fielded        the combat power of the pawns the quest already has
//          - masterPrice    while the shuttle master wears the hemophage set
//
// It is spent one thrall at a time on the dearest set it still covers, so the quest's strength
// is the same every time for the same points. A quest at or above minPoints dresses at least
// minDressed thralls, in the cheapest set, where the budget alone would dress fewer: a flat
// share cannot both show a first warcasket early and stay a fraction of a raid late. A share
// of 0 is off and dresses no one, the minimum included.
public static class ThrallBudget
{
    // The picks are indexes into prices, the sets on offer, in the order to hand them out:
    // never more than there are thralls.
    public static List<int> Picks(
        float points, float fielded, int thralls, float masterPrice, float share,
        float minPoints, int minDressed, IReadOnlyList<float> prices)
    {
        var picks = new List<int>();
        if (share <= 0f || prices.Count == 0)
            return picks;
        float budget = share * points - fielded - masterPrice;
        while (picks.Count < thralls)
        {
            int dearest = -1;
            for (int i = 0; i < prices.Count; i++)
                if (prices[i] <= budget && (dearest < 0 || prices[i] > prices[dearest]))
                    dearest = i;
            if (dearest < 0)
                break;
            picks.Add(dearest);
            budget -= prices[dearest];
        }
        if (points < minPoints)
            return picks;
        int cheapest = 0;
        for (int i = 1; i < prices.Count; i++)
            if (prices[i] < prices[cheapest])
                cheapest = i;
        while (picks.Count < minDressed && picks.Count < thralls)
            picks.Add(cheapest);
        return picks;
    }

    // The order in which thralls are offered a set: one of first, one of second, and so on,
    // then whoever is left of the longer list, so both groups get a mix.
    public static List<T> Alternate<T>(IReadOnlyList<T> first, IReadOnlyList<T> second)
    {
        var order = new List<T>(first.Count + second.Count);
        for (int i = 0; i < first.Count || i < second.Count; i++)
        {
            if (i < first.Count)
                order.Add(first[i]);
            if (i < second.Count)
                order.Add(second[i]);
        }
        return order;
    }
}
