using RimWorld;
using UnityEngine;
using Verse;

namespace HemophageWarcasket;

// Values and their rationale are in the hediff def (1.6/Defs/HediffDefs/HemogenRush.xml).
public class HediffCompProperties_HemogenRush : HediffCompProperties
{
    // Hemogen spent per second, on Biotech's 0..1 scale (the game shows 1 as 100).
    public float hemogenPerSecond;

    // Hemogen level, on the same scale, at which the rush stops on its own.
    public float hemogenReserve;

    // The apparel whose injectors run the rush, which ends once the pawn no longer wears it.
    public ThingDef armor;

    // The glow's alpha over one beat (0..1 across glowPulseTicks game ticks), drawn by
    // PawnRenderNodeWorker_HemogenGlow. Without a curve the glow is steady.
    public int glowPulseTicks;
    public SimpleCurve glowPulse;

    // Offset per pawn so a squad does not beat as one. Whole ticks: a float tick count loses
    // its low bits a few game years in.
    public float GlowAlpha(Pawn pawn)
    {
        if (glowPulse == null || glowPulseTicks <= 0)
            return 1f;
        int phase = pawn.HashOffsetTicks() % glowPulseTicks;
        if (phase < 0)
            phase += glowPulseTicks;
        return glowPulse.Evaluate(phase / (float)glowPulseTicks);
    }

    public HediffCompProperties_HemogenRush() => compClass = typeof(HediffComp_HemogenRush);
}

// The hemogen rush while it runs. The hediff's presence is the on state and its stage is the
// buff; this comp spends the hemogen and ends the rush. Deathrest and death end it through the
// def (removeOnDeathrestStart, HediffComp_DisappearsOnDeath).
//
// Spending goes through GeneResourceDrainUtility.OffsetResource rather than
// GeneUtility.OffsetHemogen: for a pawn without a hemogen drain gene the latter sets the value
// directly and skips the check that starts hemogen craving at zero (1.6).
public class HediffComp_HemogenRush : HediffComp
{
    private bool ended;

    public HediffCompProperties_HemogenRush Props => (HediffCompProperties_HemogenRush)props;

    public override bool CompShouldRemove => ended;

    // The time the remaining hemogen lasts, as HediffComp_Disappears shows its own.
    public override string CompLabelInBracketsExtra
    {
        get
        {
            Gene_Hemogen hemogen = Pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
            if (hemogen == null || Props.hemogenPerSecond <= 0f)
                return null;
            float seconds = Mathf.Max(0f, hemogen.Value - Props.hemogenReserve) / Props.hemogenPerSecond;
            return Mathf.CeilToInt(seconds * GenTicks.TicksPerRealSecond).ToStringSecondsFromTicks("F0");
        }
    }

    public override void CompPostTickInterval(ref float severityAdjustment, int delta)
    {
        Pawn pawn = Pawn;
        // GetFirstGeneOfType skips an inactive gene.
        Gene_Hemogen hemogen = pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
        // The toggle shows while VEF counts the pilot as player controlled, so a player's rush
        // must not outlast that: it would drain through a mental break or a caravan trip with
        // no way to switch it off. Undrafting is not among them.
        bool uncontrolled = pawn.Faction?.IsPlayer == true && !pawn.IsColonistPlayerControlled;
        if (hemogen?.CanOffset != true || pawn.Downed || !pawn.Awake() || uncontrolled || !WearsArmor(pawn))
        {
            ended = true;
            return;
        }

        float spend = Props.hemogenPerSecond * delta / GenTicks.TicksPerRealSecond;
        float spare = hemogen.Value - Props.hemogenReserve;
        if (spend >= spare)
        {
            spend = spare;
            ended = true;
        }
        if (spend > 0f)
            GeneResourceDrainUtility.OffsetResource(hemogen, -spend);
    }

    private bool WearsArmor(Pawn pawn)
    {
        var worn = pawn.apparel?.WornApparel;
        if (worn == null)
            return false;
        for (int i = 0; i < worn.Count; i++)
        {
            if (worn[i].def == Props.armor)
                return true;
        }
        return false;
    }
}
