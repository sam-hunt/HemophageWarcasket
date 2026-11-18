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
}
