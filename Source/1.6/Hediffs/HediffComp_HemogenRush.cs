using RimWorld;
using UnityEngine;
using Verse;

namespace HemophageWarcasket;

// Values and their rationale are in the hediff def (1.6/Defs/HediffDefs/HemogenRush.xml).
public class HediffCompProperties_HemogenRush : HediffCompProperties
{
    // Hemogen spent once as the rush starts, on Biotech's 0..1 scale (the game shows 1 as 100).
    public float hemogenStartCost;

    // Hemogen spent per second, on the same scale.
    public float hemogenPerSecond;

    // Hemogen level, on the same scale, at which the rush stops on its own.
    public float hemogenReserve;

    // Whether a pilot at this hemogen level can pay the start cost and have some left to run
    // on, above the reserve.
    public bool Affords(float hemogen) => hemogen - hemogenReserve > hemogenStartCost;

    // The apparel whose injectors run the rush, which ends once the pawn no longer wears it.
    public ThingDef armor;

    // The glow's alpha over one beat (0..1 across glowPulseTicks game ticks), drawn by
    // PawnRenderNodeWorker_HemogenGlow. With glowPulses off, or without a curve, the glow is
    // steady at full strength.
    public bool glowPulses = true;
    public int glowPulseTicks;
    public SimpleCurve glowPulse;

    // The glow of a deathresting pilot (DeathrestGlow), on with deathrestGlows: the same beat
    // turned over, lit where the rush dips and dark where it rests, once every
    // deathrestGlowBeatTicks game ticks and dark until the next. With glowPulses off, or
    // without a curve, it holds deathrestGlowSteady. deathrestLight is the thing that lights
    // the pilot's cell meanwhile.
    public bool deathrestGlows = true;
    public int deathrestGlowBeatTicks;
    public float deathrestGlowSteady;
    public ThingDef deathrestLight;

    // Offset per pawn so a squad does not beat as one.
    public float GlowAlpha(Pawn pawn, bool resting) => GlowAlphaAt(pawn.HashOffsetTicks(), resting);

    // Whole ticks: a float tick count loses its low bits a few game years in. A resting beat
    // lasts as long as the rush's and only comes less often, so a spacing shorter than one
    // beat is read as one beat.
    public float GlowAlphaAt(int ticks, bool resting)
    {
        bool beats = glowPulses && glowPulse != null && glowPulseTicks > 0;
        if (!resting)
            return beats ? glowPulse.Evaluate(Phase(ticks, glowPulseTicks) / (float)glowPulseTicks) : 1f;
        if (!beats)
            return deathrestGlowSteady;
        int phase = Phase(ticks, Mathf.Max(deathrestGlowBeatTicks, glowPulseTicks));
        return phase < glowPulseTicks ? 1f - glowPulse.Evaluate(phase / (float)glowPulseTicks) : 0f;
    }

    private static int Phase(int ticks, int period)
    {
        int phase = ticks % period;
        return phase < 0 ? phase + period : phase;
    }

    public HediffCompProperties_HemogenRush() => compClass = typeof(HediffComp_HemogenRush);
}

// The hemogen rush while it runs. The hediff's presence is the on state and its stage is the
// buff; this comp spends the hemogen, the start cost and then the drain, and ends the rush.
// Deathrest and death end it through the def (removeOnDeathrestStart,
// HediffComp_DisappearsOnDeath).
//
// The pilot's switch settles on the next tick, so clicks made in a pause net out to the last
// one and a rush pays its start cost once:
//  - The hediff is added at the click, which lights the glow at once, and owes the start cost
//    until its first tick. Switched off before then, it is removed unpaid.
//  - A rush that has paid is switched off by a stop request, which the next tick carries out
//    and a second click withdraws. The glow goes out with the request and comes back with
//    its withdrawal: the hediff def's forceRenderTreeRecache covers only the hediff coming
//    and going, so Switch rebuilds the pilot's graphics as the game does for those
//    (Pawn_HealthTracker.RemoveHediff, 1.6 and 1.7).
// Both settle in CompPostTick, which runs every tick and ahead of that tick's
// CompPostTickInterval, whose rate follows the camera (Thing.DoTick, 1.6 and 1.7). Both are
// saved, so a game saved in that pause loads owing the same cost and the same stop.
//
// Spending goes through GeneResourceDrainUtility.OffsetResource rather than
// GeneUtility.OffsetHemogen: for a pawn without a hemogen drain gene the latter sets the value
// directly and skips the check that starts hemogen craving at zero (1.6).
public class HediffComp_HemogenRush : HediffComp
{
    private bool ended;
    private bool startCostDue;
    private bool stopRequested;

    public HediffCompProperties_HemogenRush Props => (HediffCompProperties_HemogenRush)props;

    public override bool CompShouldRemove => ended;

    // Whether the pawn has a rush that has not been told to stop: what the toggle and the glow
    // show.
    public static bool ShowsOn(Pawn pawn) =>
        pawn.health.hediffSet.GetFirstHediffOfDef(HPWC_DefOf.HPWC_HemogenRush)?.TryGetComp<HediffComp_HemogenRush>()
            is { stopRequested: false };

    // The pilot's switch on a rush that exists.
    public void Switch()
    {
        if (startCostDue)
        {
            Pawn.health.RemoveHediff(parent);
            return;
        }
        stopRequested = !stopRequested;
        Pawn.Drawer.renderer.SetAllGraphicsDirty();
    }

    // The time the remaining hemogen lasts, as HediffComp_Disappears shows its own, less a start
    // cost still owed.
    public override string CompLabelInBracketsExtra
    {
        get
        {
            Gene_Hemogen hemogen = Pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
            if (hemogen == null || Props.hemogenPerSecond <= 0f)
                return null;
            float spare = hemogen.Value - Props.hemogenReserve - (startCostDue ? Props.hemogenStartCost : 0f);
            float seconds = Mathf.Max(0f, spare) / Props.hemogenPerSecond;
            return Mathf.CeilToInt(seconds * GenTicks.TicksPerRealSecond).ToStringSecondsFromTicks("F0");
        }
    }

    // A loaded rush is not added again, so it owes the start cost once.
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        base.CompPostPostAdd(dinfo);
        startCostDue = true;
    }

    // Carries out a stop, or else pays a start cost still owed, down to the reserve at most.
    public override void CompPostTick(ref float severityAdjustment)
    {
        if (stopRequested)
        {
            ended = true;
            return;
        }
        if (!startCostDue)
            return;
        startCostDue = false;
        Gene_Hemogen hemogen = Pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
        if (hemogen?.CanOffset != true)
            return;
        float spend = Mathf.Min(Props.hemogenStartCost, hemogen.Value - Props.hemogenReserve);
        if (spend > 0f)
            GeneResourceDrainUtility.OffsetResource(hemogen, -spend);
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
        if (stopRequested || hemogen?.CanOffset != true || pawn.Downed || !pawn.Awake() || uncontrolled || !WearsArmor(pawn))
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

    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_Values.Look(ref startCostDue, "startCostDue");
        Scribe_Values.Look(ref stopRequested, "stopRequested");
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
