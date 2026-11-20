using UnityEngine;
using Verse;

namespace HemophageWarcasket;

// Worker for PawnRenderNode_HemogenGlow.
//  - Draws while the piece's own sprite draws and the pilot either has a hemogen rush that
//    has not been told to stop or is deathresting (DeathrestGlow.ShowsOn). The tree caches
//    its draw requests, so this is read when they are rebuilt, and which of the two glows it
//    found is kept on the node for the draws until the next rebuild. The rush hediff def's
//    forceRenderTreeRecache rebuilds them, and rebakes portraits and the zoomed-out pawn
//    cache, whenever the rush starts or ends, HediffComp_HemogenRush.Switch does the same for
//    a stop and DeathrestGlow.Refresh for a deathrest.
//  - Takes offset, rotation, scale and layer from the worn-graphic node, plus half a layer so
//    the glow sits above its piece and below the next apparel item, a whole layer up.
//  - Multiplies the pulse into the _Color the base worker writes per draw. Portraits and the
//    pawn cache bake with DrawMeshNow, which ignores property blocks, so they hold each glow
//    as it rests: the rush's steady at full strength, the deathrest's not drawn.
public class PawnRenderNodeWorker_HemogenGlow : PawnRenderNodeWorker
{
    private const float LayerAboveWorn = 0.5f;

    private static PawnRenderNode Worn(PawnRenderNode node) => (node as PawnRenderNode_HemogenGlow)?.WornNode;

    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
    {
        if (node is not PawnRenderNode_HemogenGlow glow || glow.WornNode is not { } worn)
            return false;

        Pawn pawn = node.tree.pawn;
        bool resting = !HediffComp_HemogenRush.ShowsOn(pawn);
        if (resting && (parms.DrawNow || !DeathrestGlow.ShowsOn(pawn)))
            return false;
        glow.Resting = resting;
        return worn.Worker.CanDrawNow(worn, parms) && base.CanDrawNow(node, parms);
    }

    public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    {
        PawnRenderNode worn = Worn(node);
        return worn != null ? worn.Worker.OffsetFor(worn, parms, out pivot) : base.OffsetFor(node, parms, out pivot);
    }

    public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
    {
        PawnRenderNode worn = Worn(node);
        return worn != null ? worn.Worker.RotationFor(worn, parms) : base.RotationFor(node, parms);
    }

    public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
    {
        PawnRenderNode worn = Worn(node);
        return worn != null ? worn.Worker.ScaleFor(worn, parms) : base.ScaleFor(node, parms);
    }

    public override float LayerFor(PawnRenderNode node, PawnDrawParms parms)
    {
        PawnRenderNode worn = Worn(node);
        return (worn != null ? worn.Worker.LayerFor(worn, parms) : base.LayerFor(node, parms)) + LayerAboveWorn;
    }

    public override MaterialPropertyBlock GetMaterialPropertyBlock(PawnRenderNode node, Material material, PawnDrawParms parms)
    {
        MaterialPropertyBlock block = base.GetMaterialPropertyBlock(node, material, parms);
        // A statue keeps the single colour the base worker gave it.
        if (block == null || parms.Statue)
            return block;

        Color color = parms.tint * material.color;
        color.a *= HPWC_DefOf.HPWC_HemogenRush.CompProps<HediffCompProperties_HemogenRush>()
            .GlowAlpha(node.tree.pawn, node is PawnRenderNode_HemogenGlow { Resting: true });
        block.SetColor(ShaderPropertyIDs.Color, color);
        return block;
    }
}
