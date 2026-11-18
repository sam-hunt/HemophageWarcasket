using HarmonyLib;
using Verse;

namespace HemophageWarcasket.Patches;

// Puts each hemogen glow node after its piece's worn-graphic node among their parent's children
// (PawnRenderNode_HemogenGlow.OrderAfterWorn has why). AddChildren is how PawnRenderTree
// attaches dynamic nodes, so this runs when a render tree is built, never per frame.
[HarmonyPatch(typeof(PawnRenderNode), nameof(PawnRenderNode.AddChildren))]
public static class PawnRenderNode_AddChildren
{
    public static void Postfix(PawnRenderNode __instance) => PawnRenderNode_HemogenGlow.OrderAfterWorn(__instance);
}
