using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
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
// The strike takes a random limb that is not waiting, equal odds. An aim height narrows that
// to the limbs at it (BodyPartRecord.height: a human's legs are Bottom and arms Middle, 1.6)
// while the target has one there, and to nothing less when it has none: a pilot aiming high at
// a human still takes an arm or a leg. With no limb left the strike takes the head, when the
// body was built with limbs (walked from BodyDef.corePart, not read off the pawn); a body built
// with none, a cobra or a diabolus, has nothing to take and is no target. The head is the
// nearest Outside, non-core ancestor-or-self of a not-missing ConsciousnessSource part (human:
// Brain, Skull, Head); a brain directly in the core part has none. Aim never reaches the head:
// only a target out of limbs loses it.
//
// Taking hold of a part is not tearing it off: the strike does a set damage, and a part with
// more health left than that keeps to the body, wounded (Ability_Dismember.Strike.cs has the
// damage). Outlook says before the click which of the two the parts the strike could take would
// do, and CouldBleed whether any of them would bleed, torn off or wounded
// (Ability_Dismember.Foresight.cs has why a solid part does not).
//
// A missing part's children carry missing hediffs of their own (Hediff_MissingPart adds them,
// 1.6), so a not-missing part never sits under a missing one. CanStrike is asked by the
// targeter every frame and allocates nothing; Choose and Outlook may.
public enum StrikeOutlook
{
    // Nothing to take hold of.
    None,

    // Whichever part the strike takes comes off.
    Tears,

    // Some of the parts it could take would come off and some would not.
    MayTear,

    // No part it could take would come off.
    Wounds,
}

public static class DismemberTarget
{
    public static bool CanStrike(Pawn target)
    {
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
        return HeadFor(body, brain) != null && BuiltWithLimbs(body);
    }

    public static BodyPartRecord Choose(Pawn target, BodyPartHeight aim) =>
        Choose(target.RaceProps.body, target.health.hediffSet.GetNotMissingParts(), n => Rand.Range(0, n), out _, aim);

    // What a strike of this damage, after everything that scales it, would do to the target.
    public static StrikeOutlook Outlook(Pawn target, BodyPartHeight aim, float damage, out bool decapitates)
    {
        HediffSet hediffs = target.health.hediffSet;
        return Outlook(target.RaceProps.body, hediffs.GetNotMissingParts(), aim,
            part => Tears(hediffs.GetPartHealth(part), damage), out decapitates);
    }

    // Whether a strike could leave the target bleeding from the part it takes.
    public static bool CouldBleed(Pawn target, BodyPartHeight aim)
    {
        HediffSet hediffs = target.health.hediffSet;
        return AnyCandidate(target.RaceProps.body, hediffs.GetNotMissingParts(), aim,
            part => !part.def.IsSolid(part, hediffs.hediffs));
    }

    // Whether damage takes a part with this much health off: HediffSet.GetPartHealth rounds
    // what is left to a whole number, and HediffSet.AddDirect removes a part left at zero
    // (1.6 and 1.7).
    public static bool Tears(float partHealth, float damage) => Mathf.RoundToInt(partHealth - damage) <= 0;

    // pickIndex returns an index in [0, count).
    public static BodyPartRecord Choose(BodyDef body, IEnumerable<BodyPartRecord> notMissing,
        Func<int, int> pickIndex, out bool decapitates, BodyPartHeight aim = BodyPartHeight.Undefined)
    {
        List<BodyPartRecord> parts = Candidates(body, notMissing, aim, out decapitates);
        return parts.Count == 0 ? null : parts[pickIndex(parts.Count)];
    }

    // tears says whether the strike would take a given part off.
    public static StrikeOutlook Outlook(BodyDef body, IEnumerable<BodyPartRecord> notMissing,
        BodyPartHeight aim, Predicate<BodyPartRecord> tears, out bool decapitates)
    {
        List<BodyPartRecord> parts = Candidates(body, notMissing, aim, out decapitates);
        if (parts.Count == 0)
            return StrikeOutlook.None;
        int torn = 0;
        for (int i = 0; i < parts.Count; i++)
            if (tears(parts[i]))
                torn++;
        return torn == parts.Count ? StrikeOutlook.Tears : torn == 0 ? StrikeOutlook.Wounds : StrikeOutlook.MayTear;
    }

    // Whether any part the strike could take passes the test.
    public static bool AnyCandidate(BodyDef body, IEnumerable<BodyPartRecord> notMissing,
        BodyPartHeight aim, Predicate<BodyPartRecord> test) =>
        Candidates(body, notMissing, aim, out _).Exists(test);

    // The parts a strike picks among: the limbs not waiting, at the aim height when any is
    // there, else the head alone, else none.
    private static List<BodyPartRecord> Candidates(BodyDef body, IEnumerable<BodyPartRecord> notMissing,
        BodyPartHeight aim, out bool decapitates)
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
            if (aim == BodyPartHeight.Undefined)
                return ready;
            List<BodyPartRecord> aimed = ready.FindAll(limb => limb.height == aim);
            return aimed.Count > 0 ? aimed : ready;
        }
        BodyPartRecord head = HeadFor(body, brain);
        if (head == null || !BuiltWithLimbs(body))
            return limbs;
        decapitates = true;
        limbs.Add(head);
        return limbs;
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
