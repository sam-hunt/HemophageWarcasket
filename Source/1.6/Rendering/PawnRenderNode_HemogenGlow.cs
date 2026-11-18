using UnityEngine;
using Verse;

namespace HemophageWarcasket;

// A piece's hemogen glow: a second render node from that piece's
// apparel.renderNodeProperties, drawn over its worn sprite while the pilot's hemogen rush runs.
// DynamicPawnRenderNodeSetup_Apparel sets node.apparel on it and attaches it under
// props.parentTagDef, beside the piece's own worn-graphic node, and the base node loads
// props.texPath as a Graphic_Multi. Everything else (mesh, transform, layer, visibility) is
// taken from the worn-graphic node, here and in the worker, so the glow sits on the sprite for
// a body piece and a head piece alike.
public class PawnRenderNode_HemogenGlow : PawnRenderNode
{
    private PawnRenderNode wornNode;

    public PawnRenderNode_HemogenGlow(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
        : base(pawn, props, tree)
    {
    }

    // The worn-graphic node of the same apparel, a sibling under the same parent. Found lazily:
    // siblings are attached after this node is constructed.
    public PawnRenderNode WornNode
    {
        get
        {
            if (wornNode == null && parent?.children != null)
            {
                foreach (PawnRenderNode sibling in parent.children)
                {
                    if (sibling is PawnRenderNode_Apparel && sibling.apparel == apparel)
                    {
                        wornNode = sibling;
                        break;
                    }
                }
            }
            return wornNode;
        }
    }

    // Whichever mesh the piece's sprite draws on, body or head.
    public override Mesh GetMesh(PawnDrawParms parms)
    {
        PawnRenderNode worn = WornNode;
        return worn != null ? worn.GetMesh(parms) : base.GetMesh(parms);
    }

    // Moves every glow node among parent's children behind the worn-graphic node of the same
    // apparel. The setup yields a def's renderNodeProperties nodes before the apparel's own
    // node, and portraits and the zoomed-out pawn cache bake the tree with DrawMeshNow in that
    // order: a transparent overlay drawn before the opaque sprite it sits on is painted over.
    // The live path is indifferent, Unity queues transparent materials after cutout ones.
    public static void OrderAfterWorn(PawnRenderNode parent)
    {
        PawnRenderNode[] children = parent.children;
        if (children == null)
            return;

        for (int g = 0; g < children.Length; g++)
        {
            if (children[g] is not PawnRenderNode_HemogenGlow glow)
                continue;
            for (int w = g + 1; w < children.Length; w++)
            {
                if (children[w] is PawnRenderNode_Apparel worn && worn.apparel == glow.apparel)
                {
                    children[g] = worn;
                    children[w] = glow;
                    break;
                }
            }
        }
    }
}
