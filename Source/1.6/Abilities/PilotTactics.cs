namespace HemophageWarcasket;

// What a pilot the player does not control decides about the set's two abilities and about the
// hemogen they run on, as plain arithmetic on numbers, booleans and our own enums. Pure, so the
// tests hold it; AiPilot gathers the arguments from a pawn, and the abilities, the rush's comp
// and the patch on the AI's hemogen cost check do what it answers. The numbers are def fields:
// the rush's in HediffDefs/HemogenRush.xml, the strike's in AbilityDefs/Dismember.xml.
public static class PilotTactics
{
    // Whether the rush should run: the pilot's fight target is a threat, and it is close, or
    // the pilot is on its way to hit it and near enough to sprint the rest. The second clause
    // is a melee pilot's last stretch; the first is a pilot with a gun that something has
    // reached, and a melee pilot between two melee jobs.
    public static bool RushEngaged(bool threat, bool meleeJob, float distance, float startDistance, float closeDistance) =>
        threat && (distance <= closeDistance || (meleeJob && distance <= startDistance));

    // Whether a strike is worth one of the pads' few: a limb can come off, and it is a limb.
    // The head is all a strike takes from a target with no limb left (DismemberTarget), and a
    // pilot the player does not control leaves that target alone.
    public static bool WorthStriking(StrikeOutlook outlook, bool decapitates) =>
        !decapitates && outlook is StrikeOutlook.Tears or StrikeOutlook.MayTear;

    // Whether the pilot may pay for an ability that costs hemogen: only out of what it holds
    // above the level kept for the rush. At a kept level of 0 this is the game's own test.
    public static bool AffordsAbility(float hemogen, float cost, float kept) => hemogen - kept >= cost;
}
