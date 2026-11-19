#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;
using Xunit;

namespace HemophageWarcasket.Tests;

// DismemberTarget's part-picking on hand-built bodies, written from the contract in its
// header. The game's tag defs are null headless, so the fixture assigns bare ones to the
// BodyPartTagDefOf fields the code reads and puts the previous values back on dispose; no
// other test reads them. Running the DefOf's static constructor headless logs a warning
// through Unity and throws unless the helper believes it is binding, so it runs once under
// that flag.
public class DismemberTargetTests : IDisposable
{
    private static readonly BodyPartTagDef Moving = new() { defName = "MovingLimbCore" };
    private static readonly BodyPartTagDef Manipulation = new() { defName = "ManipulationLimbCore" };
    private static readonly BodyPartTagDef Eating = new() { defName = "EatingSource" };
    private static readonly BodyPartTagDef Consciousness = new() { defName = "ConsciousnessSource" };
    private static readonly BodyPartTagDef Pumping = new() { defName = "BloodPumpingSource", vital = true };

    static DismemberTargetTests()
    {
        FieldInfo binding = typeof(DefOfHelper).GetField("bindingNow", BindingFlags.NonPublic | BindingFlags.Static)!;
        binding.SetValue(null, true);
        try
        {
            RuntimeHelpers.RunClassConstructor(typeof(BodyPartTagDefOf).TypeHandle);
        }
        finally
        {
            binding.SetValue(null, false);
        }
    }

    private readonly BodyPartTagDef? savedMoving = BodyPartTagDefOf.MovingLimbCore;
    private readonly BodyPartTagDef? savedManipulation = BodyPartTagDefOf.ManipulationLimbCore;
    private readonly BodyPartTagDef? savedEating = BodyPartTagDefOf.EatingSource;
    private readonly BodyPartTagDef? savedConsciousness = BodyPartTagDefOf.ConsciousnessSource;

    public DismemberTargetTests()
    {
        BodyPartTagDefOf.MovingLimbCore = Moving;
        BodyPartTagDefOf.ManipulationLimbCore = Manipulation;
        BodyPartTagDefOf.EatingSource = Eating;
        BodyPartTagDefOf.ConsciousnessSource = Consciousness;
    }

    public void Dispose()
    {
        BodyPartTagDefOf.MovingLimbCore = savedMoving!;
        BodyPartTagDefOf.ManipulationLimbCore = savedManipulation!;
        BodyPartTagDefOf.EatingSource = savedEating!;
        BodyPartTagDefOf.ConsciousnessSource = savedConsciousness!;
    }

    private static BodyPartRecord Part(string name, BodyPartRecord? parent, BodyPartDepth depth, params BodyPartTagDef[] tags)
    {
        var record = new BodyPartRecord
        {
            def = new BodyPartDef { defName = name, tags = tags.ToList() },
            depth = depth,
            parent = parent,
        };
        parent?.parts.Add(record);
        return record;
    }

    private static BodyDef Body(BodyPartRecord core) => new() { corePart = core };

    private static IEnumerable<BodyPartRecord> Flatten(BodyPartRecord part)
    {
        yield return part;
        foreach (BodyPartRecord child in part.parts.SelectMany(Flatten))
            yield return child;
    }

    // Torso with a neck-and-head chain, two arms under shoulders with hands, two legs with
    // feet and a jaw tagged both as a manipulation limb core and as an eating source.
    private sealed class Humanlike
    {
        public readonly BodyPartRecord torso, neck, head, skull, brain, jaw;
        public readonly BodyPartRecord[] arms = new BodyPartRecord[2], legs = new BodyPartRecord[2];
        public readonly List<BodyPartRecord> others = [];

        public Humanlike()
        {
            torso = Part("Torso", null, BodyPartDepth.Outside);
            neck = Part("Neck", torso, BodyPartDepth.Outside);
            head = Part("Head", neck, BodyPartDepth.Outside);
            skull = Part("Skull", head, BodyPartDepth.Inside);
            brain = Part("Brain", skull, BodyPartDepth.Inside, Consciousness);
            jaw = Part("Jaw", head, BodyPartDepth.Outside, Manipulation, Eating);
            for (int i = 0; i < 2; i++)
            {
                BodyPartRecord shoulder = Part("Shoulder", torso, BodyPartDepth.Outside);
                arms[i] = Part("Arm", shoulder, BodyPartDepth.Outside, Manipulation);
                others.Add(shoulder);
                others.Add(Part("Hand", arms[i], BodyPartDepth.Outside));
                legs[i] = Part("Leg", torso, BodyPartDepth.Outside, Moving);
                others.Add(Part("Foot", legs[i], BodyPartDepth.Outside));
            }
        }

        public BodyDef Def => Body(torso);

        public IEnumerable<BodyPartRecord> Present(params BodyPartRecord[] gone)
        {
            HashSet<BodyPartRecord> missing = [.. gone.SelectMany(Flatten)];
            return Flatten(torso).Where(p => !missing.Contains(p));
        }
    }

    private static BodyPartRecord? Pick(BodyDef body, IEnumerable<BodyPartRecord> present, bool flag, out bool decapitates, int index = 0) =>
        DismemberTarget.Choose(body, present, flag, n => Math.Min(index, n - 1), out decapitates);

    [Fact]
    public void Humanlike_OffersExactlyItsArmsAndLegs()
    {
        var human = new Humanlike();
        var offered = new HashSet<BodyPartRecord>();
        for (int i = 0; i < 4; i++)
            offered.Add(Pick(human.Def, human.Present(), false, out bool decapitates, i)!);
        Assert.Equal(new HashSet<BodyPartRecord>([.. human.arms, .. human.legs]), offered);
        Assert.DoesNotContain(human.jaw, offered);
        Assert.All(human.others, p => Assert.False(DismemberTarget.IsLimb(human.Def, p)));
        Assert.False(DismemberTarget.IsLimb(human.Def, human.jaw));
    }

    [Fact]
    public void MissingLimb_IsNotOffered()
    {
        var human = new Humanlike();
        IEnumerable<BodyPartRecord> present = human.Present(human.arms[0], human.legs[1]);
        for (int i = 0; i < 4; i++)
        {
            BodyPartRecord? part = Pick(human.Def, present, false, out bool decapitates, i);
            Assert.False(decapitates);
            Assert.True(part == human.arms[1] || part == human.legs[0]);
        }
    }

    [Fact]
    public void InsectLegCarryingBothLimbTags_IsOfferedOnce()
    {
        BodyPartRecord core = Part("Thorax", null, BodyPartDepth.Outside);
        BodyPartRecord leg = Part("Leg", core, BodyPartDepth.Outside, Moving, Manipulation);
        int asked = 0;
        BodyPartRecord? part = DismemberTarget.Choose(Body(core), Flatten(core), false, n => { asked = n; return 0; }, out _);
        Assert.Same(leg, part);
        Assert.Equal(1, asked);
    }

    // The head is the nearest Outside ancestor of the brain, not the neck above it.
    [Fact]
    public void NoLimbsLeft_GivesTheHeadAndDecapitates()
    {
        var human = new Humanlike();
        BodyPartRecord? part = Pick(human.Def, human.Present([.. human.arms, .. human.legs]), false, out bool decapitates);
        Assert.Same(human.head, part);
        Assert.True(decapitates);
    }

    [Fact]
    public void BodyBuiltWithoutLimbs_GivesNothingByDefaultAndItsHeadWithTheFlag()
    {
        BodyPartRecord core = Part("Segment", null, BodyPartDepth.Outside);
        BodyPartRecord head = Part("Head", core, BodyPartDepth.Outside);
        Part("Brain", head, BodyPartDepth.Inside, Consciousness);
        BodyDef body = Body(core);
        Assert.False(DismemberTarget.BuiltWithLimbs(body));

        Assert.Null(Pick(body, Flatten(core), false, out bool decapitates));
        Assert.False(decapitates);

        Assert.Same(head, Pick(body, Flatten(core), true, out decapitates));
        Assert.True(decapitates);
    }

    [Fact]
    public void BrainInTheCorePart_GivesNothingEvenWithTheFlag()
    {
        BodyPartRecord core = Part("Blob", null, BodyPartDepth.Outside, Consciousness);
        Assert.Null(Pick(Body(core), Flatten(core), true, out bool decapitates));
        Assert.False(decapitates);
    }

    [Fact]
    public void BodyBuiltWithLimbs_HeadIsNothingWhenItsBrainIsInTheCore()
    {
        BodyPartRecord core = Part("Torso", null, BodyPartDepth.Outside, Consciousness);
        Part("Leg", core, BodyPartDepth.Outside, Moving);
        BodyDef body = Body(core);
        IEnumerable<BodyPartRecord> noLeg = Flatten(core).Where(p => p == core);
        Assert.Null(Pick(body, noLeg, true, out _));
    }

    // A centipede: six rings in a chain, every one tagged as both kinds of limb, with the
    // head on the first and the reactor in the second.
    private sealed class Ringed
    {
        public readonly BodyPartRecord head;
        public readonly BodyPartRecord[] rings = new BodyPartRecord[6];

        public Ringed(BodyPartRecord? above = null)
        {
            BodyPartRecord? parent = above;
            for (int i = 0; i < rings.Length; i++)
                parent = rings[i] = Part("Ring" + (i + 1), parent, BodyPartDepth.Outside, Moving, Manipulation);
            head = Part("Head", above ?? rings[0], BodyPartDepth.Outside);
            Part("Brain", head, BodyPartDepth.Inside, Consciousness);
            Part("Reactor", rings[1], BodyPartDepth.Inside, Pumping);
        }
    }

    [Fact]
    public void RingedBody_HasNoLimbsAndLosesItsHeadOnlyWithTheFlag()
    {
        var centipede = new Ringed();
        BodyDef body = Body(centipede.rings[0]);
        Assert.All(centipede.rings, ring => Assert.False(DismemberTarget.IsLimb(body, ring)));
        Assert.False(DismemberTarget.BuiltWithLimbs(body));

        Assert.Null(Pick(body, Flatten(centipede.rings[0]), false, out bool decapitates));
        Assert.False(decapitates);

        Assert.Same(centipede.head, Pick(body, Flatten(centipede.rings[0]), true, out decapitates));
        Assert.True(decapitates);
    }

    // The fifth and sixth rings hold nothing vital, so only the chain rules them out, and the
    // fifth still has no ring below it once the sixth is gone.
    [Fact]
    public void RingWithNothingVital_IsNoLimbEvenAfterItsNeighborIsLost()
    {
        var centipede = new Ringed();
        BodyDef body = Body(centipede.rings[0]);
        IEnumerable<BodyPartRecord> present = Flatten(centipede.rings[0]).Where(p => p != centipede.rings[5]);
        Assert.Null(Pick(body, present, false, out _));
    }

    [Fact]
    public void RingsUnderAnUntaggedCore_AreNoLimbs()
    {
        BodyPartRecord core = Part("Body", null, BodyPartDepth.Outside);
        var rings = new Ringed(core);
        BodyDef body = Body(core);
        Assert.All(rings.rings, ring => Assert.False(DismemberTarget.IsLimb(body, ring)));
        Assert.False(DismemberTarget.BuiltWithLimbs(body));
    }

    // An apocriton's legs hang off a thorax tagged for manipulation, and a tank's legs off a
    // plate in a chain of plates tagged for both.
    [Fact]
    public void LegUnderATaggedTrunk_IsStillALimb()
    {
        BodyPartRecord thorax = Part("Thorax", null, BodyPartDepth.Outside, Manipulation);
        BodyPartRecord leg = Part("Leg", thorax, BodyPartDepth.Outside, Moving);
        Assert.True(DismemberTarget.IsLimb(Body(thorax), leg));

        BodyPartRecord plate = Part("Plate1", null, BodyPartDepth.Outside, Moving, Manipulation);
        BodyPartRecord second = Part("Plate2", plate, BodyPartDepth.Outside, Moving, Manipulation);
        BodyPartRecord front = Part("Leg", plate, BodyPartDepth.Outside, Moving);
        BodyPartRecord rear = Part("Leg", second, BodyPartDepth.Outside, Moving);
        BodyPartRecord claw = Part("Claw", second, BodyPartDepth.Outside, Manipulation);
        BodyDef tank = Body(plate);
        Assert.False(DismemberTarget.IsLimb(tank, second));
        Assert.All(new[] { front, rear, claw }, p => Assert.True(DismemberTarget.IsLimb(tank, p)));
        Assert.True(DismemberTarget.BuiltWithLimbs(tank));
    }

    [Fact]
    public void TaggedPartHoldingSomethingVital_IsNoLimb()
    {
        BodyPartRecord core = Part("Platform", null, BodyPartDepth.Outside);
        BodyPartRecord gravCore = Part("GravCore", core, BodyPartDepth.Outside, Moving, Pumping);
        BodyPartRecord segment = Part("Segment", core, BodyPartDepth.Outside, Moving, Manipulation);
        Part("Heart", Part("Casing", segment, BodyPartDepth.Inside), BodyPartDepth.Inside, Pumping);
        BodyPartRecord arm = Part("Arm", core, BodyPartDepth.Outside, Manipulation);
        BodyDef body = Body(core);
        Assert.False(DismemberTarget.IsLimb(body, gravCore));
        Assert.False(DismemberTarget.IsLimb(body, segment));
        Assert.Same(arm, Pick(body, Flatten(core), false, out bool decapitates, index: 5));
        Assert.False(decapitates);
    }
}
