using UnityEngine;
using Verse;

namespace HemophageWarcasket;

// Worker for PawnRenderNode_HemogenGlow.
//  - Draws only while the pilot has a hemogen rush that has not been told to stop and the
//    piece's own sprite draws. The tree caches its draw requests, so this is read when they
//    are rebuilt; the hediff def's forceRenderTreeRecache rebuilds them, and rebakes portraits
//    and the zoomed-out pawn cache, whenever the rush starts or ends, and
//    HediffComp_HemogenRush.Switch does the same for a stop.
//  - Takes offset, rotation, scale and layer from the worn-graphic node, plus half a layer so
//    the glow sits above its piece and below the next apparel item, a whole layer up.
//  - Multiplies the heartbeat pulse into the _Color the base worker writes per draw. Portraits
//    and the pawn cache bake with DrawMeshNow, which ignores property blocks, so they show the
//    glow steady at full strength.
public class PawnRenderNodeWorker_HemogenGlow : PawnRenderNodeWorker
{
    private const float LayerAboveWorn = 0.5f;

    private static PawnRenderNode Worn(PawnRenderNode node) => (node as PawnRenderNode_HemogenGlow)?.WornNode;

    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
    {
        PawnRenderNode worn = Worn(node);
        return worn != null
            && HediffComp_HemogenRush.ShowsOn(node.tree.pawn)
            && worn.Worker.CanDrawNow(worn, parms)
            && base.CanDrawNow(node, parms);
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
        color.a *= HPWC_DefOf.HPWC_HemogenRush.CompProps<HediffCompProperties_HemogenRush>().GlowAlpha(node.tree.pawn);
        block.SetColor(ShaderPropertyIDs.Color, color);
        return block;
    }
}
