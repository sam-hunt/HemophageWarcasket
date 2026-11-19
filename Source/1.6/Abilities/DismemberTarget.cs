using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace HemophageWarcasket;

// What the pads' strike takes off a target, as pure part-picking so it runs on hand-built
// bodies. A limb is a whole arm or leg: a non-core part tagged MovingLimbCore or
// ManipulationLimbCore (1.6: the limb's root, never its hand, foot or fingers) that is not
// tagged EatingSource, which vanilla puts on jaws, beaks and insect mouths, and that has
// nothing vital in it: no tag with BodyPartTagDef.vital on it or on any part below it. Losing
// a limb must not kill outright, which only the head may. A trunk with no eating tag counts.
//
// Vanilla and mods also put the limb tags on what a body with no legs moves on, a centipede's
// body rings among them, each ring holding the next. Those are limbs by the same rule, taken
// from the outside in: a limb waits while a limb with the same limb tags is still attached
// directly under it. A centipede loses its sixth ring, then its fifth; the rings before those
// hold its fluid reprocessors and reactor (a mechanoid dies without either, 1.6) and never
// come off. A diabolus, war queen or termite keeps its only reprocessor in its last ring, so
// none of its rings is a limb. A lost ring costs its share of Moving and Manipulation, as a
// lost leg or arm does (PawnCapacityUtility.CalculateLimbEfficiency, 1.6).
//
// The strike takes a random limb that is not waiting, equal odds. With no limb left it takes
// the head, when the body was built with limbs (walked from BodyDef.corePart, not read off the
// pawn) or when the def says so; otherwise there is nothing to take. The head is the nearest
// Outside, non-core ancestor-or-self of a not-missing ConsciousnessSource part (human: Brain,
// Skull, Head); a brain directly in the core part has none.
//
// A missing part's children carry missing hediffs of their own (Hediff_MissingPart adds them,
// 1.6), so a not-missing part never sits under a missing one. CanStrike is asked by the
// targeter every frame and allocates nothing; Choose may.
public static class DismemberTarget
{
    public static bool CanStrike(Pawn target, DismemberExtension ext, out bool decapitates)
    {
        decapitates = false;
        BodyDef body = target.RaceProps.body;
        HediffSet hediffs = target.health.hediffSet;
        List<BodyPartRecord> all = body.AllParts;
        BodyPartRecord brain = null;
        for (int i = 0; i < all.Count; i++)
        {
            BodyPartRecord part = all[i];
            if (hediffs.PartIsMissing(part))
                continue;
            // A limb that waits has one below it that does not.
            if (IsLimb(body, part))
                return true;
            if (brain == null && HasTag(part, BodyPartTagDefOf.ConsciousnessSource))
                brain = part;
        }
        decapitates = HeadFor(body, brain) != null && (BuiltWithLimbs(body) || ext.decapitateBodiesWithoutLimbs);
        return decapitates;
    }

    public static BodyPartRecord Choose(Pawn target, DismemberExtension ext, out bool decapitates) =>
        Choose(target.RaceProps.body, target.health.hediffSet.GetNotMissingParts(),
            ext.decapitateBodiesWithoutLimbs, n => Rand.Range(0, n), out decapitates);

    // pickIndex returns an index in [0, count).
    public static BodyPartRecord Choose(BodyDef body, IEnumerable<BodyPartRecord> notMissing,
        bool decapitateBodiesWithoutLimbs, Func<int, int> pickIndex, out bool decapitates)
    {
        decapitates = false;
        var limbs = new List<BodyPartRecord>();
        BodyPartRecord brain = null;
        foreach (BodyPartRecord part in notMissing)
        {
            if (IsLimb(body, part))
                limbs.Add(part);
            else if (brain == null && HasTag(part, BodyPartTagDefOf.ConsciousnessSource))
                brain = part;
        }
        if (limbs.Count > 0)
        {
            List<BodyPartRecord> ready = limbs.FindAll(limb => !Waits(limb, limbs));
            return ready[pickIndex(ready.Count)];
        }
        BodyPartRecord head = HeadFor(body, brain);
        if (head == null || !(BuiltWithLimbs(body) || decapitateBodiesWithoutLimbs))
            return null;
        decapitates = true;
        return head;
    }

    public static bool IsLimb(BodyDef body, BodyPartRecord part) =>
        part != body.corePart && LimbTags(part) != 0
        && !HasTag(part, BodyPartTagDefOf.EatingSource) && !HoldsVital(part);

    // Whether one of the attached limbs sits directly under this one with the same limb tags.
    private static bool Waits(BodyPartRecord limb, List<BodyPartRecord> attachedLimbs)
    {
        int tags = LimbTags(limb);
        List<BodyPartRecord> children = limb.parts;
        for (int i = 0; i < children.Count; i++)
            if (LimbTags(children[i]) == tags && attachedLimbs.Contains(children[i]))
                return true;
        return false;
    }

    // Which of the two limb tags the part carries, as bits.
    private static int LimbTags(BodyPartRecord part) =>
        (HasTag(part, BodyPartTagDefOf.MovingLimbCore) ? 1 : 0)
        | (HasTag(part, BodyPartTagDefOf.ManipulationLimbCore) ? 2 : 0);

    private static bool HoldsVital(BodyPartRecord part)
    {
        List<BodyPartTagDef> tags = part.def.tags;
        if (tags != null)
            for (int i = 0; i < tags.Count; i++)
                if (tags[i].vital)
                    return true;
        List<BodyPartRecord> children = part.parts;
        for (int i = 0; i < children.Count; i++)
            if (HoldsVital(children[i]))
                return true;
        return false;
    }

    public static bool BuiltWithLimbs(BodyDef body) => body.corePart != null && HasLimbBelow(body, body.corePart);

    private static bool HasLimbBelow(BodyDef body, BodyPartRecord part)
    {
        List<BodyPartRecord> children = part.parts;
        for (int i = 0; i < children.Count; i++)
            if (IsLimb(body, children[i]) || HasLimbBelow(body, children[i]))
                return true;
        return false;
    }

    private static BodyPartRecord HeadFor(BodyDef body, BodyPartRecord brain)
    {
        for (BodyPartRecord part = brain; part != null; part = part.parent)
            if (part.depth == BodyPartDepth.Outside && part != body.corePart)
                return part;
        return null;
    }

    private static bool HasTag(BodyPartRecord part, BodyPartTagDef tag) =>
        part.def.tags != null && part.def.tags.Contains(tag);
}
