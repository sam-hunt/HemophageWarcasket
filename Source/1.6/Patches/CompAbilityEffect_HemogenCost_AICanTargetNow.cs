using HarmonyLib;
using RimWorld;

namespace HemophageWarcasket.Patches;

// Keeps hemogen back for the rush on a pilot the player does not control. A sanguophage's own
// AI casts piercing spine whenever it can pay, about as fast as a rush drains, and this method
// is the one place the AI asks whether it can: Ability.AICanTargetNow asks it of every effect
// comp before JobGiver_AIFightEnemy.GetAbilityJob may pick the ability, and the game's answer
// is the pilot's hemogen against the cost with nothing held back. The postfix turns a yes into
// a no when the cost would take such a pilot under the level the rush's def keeps
// (AiPilot.AffordsAbility, PilotTactics.AffordsAbility), so the AI sees an ability it cannot
// pay for. Every ability with a hemogen cost is covered, another mod's too. The rush and the
// pads' drain are no such ability and go on as they are.
//
// Only the AI asks: a player's order goes through GizmoDisabled, so no colonist is touched. It
// runs when a fight job giver looks for an ability, for a pawn that has one with a hemogen
// cost, and does nothing unless the game's answer is yes. All the same on 1.6 and 1.7.
[HarmonyPatch(typeof(CompAbilityEffect_HemogenCost), nameof(CompAbilityEffect_HemogenCost.AICanTargetNow))]
public static class CompAbilityEffect_HemogenCost_AICanTargetNow
{
    public static void Postfix(CompAbilityEffect_HemogenCost __instance, ref bool __result)
    {
        if (__result)
            __result = AiPilot.AffordsAbility(__instance.parent?.pawn, __instance.Props.hemogenCost);
    }
}
