#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Verse;
using Xunit;

namespace HemophageWarcasket.Tests;

// The glow's beat arithmetic (HediffCompProperties_HemogenRush.GlowAlphaAt), run on the numbers
// the rush def ships: the rush's heartbeat curve, the deathrest beat as its mirror (one beat
// every deathrestGlowBeatTicks, dark between), the spacing's wrap and the steady strength with
// pulses off. The props are built off the XML so a retuned curve is held, not a copy of it.
public class GlowPulseTests
{
    private static readonly string RepoRoot = FindRepoRoot();

    private static string FindRepoRoot()
    {
        for (var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory); dir != null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "HemophageWarcasket.sln")))
                return dir.FullName;
        }
        throw new InvalidOperationException("HemophageWarcasket.sln not found above the test output folder");
    }

    private static List<(float X, float Y)> Points()
    {
        XElement comp = Comp();
        return comp.Element("glowPulse")!.Element("points")!.Elements("li")
            .Select(li =>
            {
                string[] xy = li.Value.Trim('(', ')', ' ').Split(',');
                return (float.Parse(xy[0], CultureInfo.InvariantCulture), float.Parse(xy[1], CultureInfo.InvariantCulture));
            })
            .ToList();
    }

    private static XElement Comp() =>
        XDocument.Load(Path.Combine(RepoRoot, "1.6/Defs/HediffDefs/HemogenRush.xml")).Root!.Elements().Single()
            .Element("comps")!.Elements("li")
            .Single(li => (string?)li.Attribute("Class") == "HemophageWarcasket.HediffCompProperties_HemogenRush");

    private static HediffCompProperties_HemogenRush Props(bool pulses = true)
    {
        XElement comp = Comp();
        var curve = new SimpleCurve();
        foreach ((float x, float y) in Points())
            curve.Add(new CurvePoint(x, y));
        return new HediffCompProperties_HemogenRush
        {
            glowPulses = pulses,
            glowPulseTicks = (int)comp.Element("glowPulseTicks")!,
            glowPulse = curve,
            deathrestGlowBeatTicks = (int)comp.Element("deathrestGlowBeatTicks")!,
            deathrestGlowSteady = float.Parse((string)comp.Element("deathrestGlowSteady")!, CultureInfo.InvariantCulture),
        };
    }

    private static float Lowest => Points().Min(p => p.Y);

    // The deathrest beat is one minus the curve, so a curve that starts and ends at full strength
    // is what lets a rest begin and end dark.
    [Fact]
    public void Curve_StartsAndEndsAtOne()
    {
        List<(float X, float Y)> points = Points();
        Assert.Equal(1f, points[0].Y);
        Assert.Equal(1f, points[^1].Y);
    }

    // The rush starts at full strength and dips to the curve's lowest point without going dark.
    [Fact]
    public void Rush_StartsFullAndNeverGoesDark()
    {
        var props = Props();
        Assert.Equal(1f, props.GlowAlphaAt(0, false));
        float min = Enumerable.Range(0, props.glowPulseTicks).Min(t => props.GlowAlphaAt(t, false));
        Assert.Equal(Lowest, min, 2);
        Assert.True(min > 0f);
    }

    // A rest is dark at the start of its spacing and from the end of the beat on, and its peak is
    // the curve's deepest dip turned over.
    [Fact]
    public void Deathrest_BeatsOnceThenRestsDark()
    {
        var props = Props();
        float[] alpha = Enumerable.Range(0, props.deathrestGlowBeatTicks)
            .Select(t => props.GlowAlphaAt(t, true)).ToArray();
        Assert.Equal(0f, alpha[0]);
        for (int t = props.glowPulseTicks; t < alpha.Length; t++)
            Assert.Equal(0f, alpha[t]);
        Assert.All(alpha, a => Assert.True(a >= 0f));
        Assert.Equal(1f - Lowest, alpha.Max(), 2);
        Assert.InRange(Array.IndexOf(alpha, alpha.Max()), 1, props.glowPulseTicks - 1);
    }

    // Inside a beat the resting glow is the rush's complement.
    [Fact]
    public void Deathrest_MirrorsTheRush()
    {
        var props = Props();
        for (int t = 0; t < props.glowPulseTicks; t++)
            Assert.Equal(1f, props.GlowAlphaAt(t, false) + props.GlowAlphaAt(t, true), 5);
    }

    // The rest repeats every deathrestGlowBeatTicks and the rush every glowPulseTicks.
    [Fact]
    public void Beats_RepeatAtTheirSpacing()
    {
        var props = Props();
        for (int t = 0; t < props.deathrestGlowBeatTicks; t++)
            Assert.Equal(props.GlowAlphaAt(t, true), props.GlowAlphaAt(t + props.deathrestGlowBeatTicks, true));
        for (int t = 0; t < props.glowPulseTicks; t++)
            Assert.Equal(props.GlowAlphaAt(t, false), props.GlowAlphaAt(t + props.glowPulseTicks, false));
    }

    // With pulses off the rush holds full strength and a rest holds its steady strength.
    [Fact]
    public void PulsesOff_HoldSteady()
    {
        var props = Props(pulses: false);
        foreach (int t in new[] { 0, 1, 17, 60, 479, 480, 100000 })
        {
            Assert.Equal(1f, props.GlowAlphaAt(t, false));
            Assert.Equal(props.deathrestGlowSteady, props.GlowAlphaAt(t, true));
        }
    }

    // A tick count below zero wraps into the beat rather than reading past the curve.
    [Fact]
    public void NegativeTicks_Wrap()
    {
        var props = Props();
        Assert.Equal(props.GlowAlphaAt(props.glowPulseTicks - 1, false), props.GlowAlphaAt(-1, false));
        Assert.Equal(props.GlowAlphaAt(props.deathrestGlowBeatTicks - 1, true), props.GlowAlphaAt(-1, true));
    }

    // A spacing shorter than one beat is read as one beat, so the rest is the rush's complement
    // at every tick.
    [Fact]
    public void ShortSpacing_ReadsAsOneBeat()
    {
        var props = Props();
        props.deathrestGlowBeatTicks = 10;
        for (int t = -props.glowPulseTicks; t < 3 * props.glowPulseTicks; t++)
            Assert.Equal(1f - props.GlowAlphaAt(t, false), props.GlowAlphaAt(t, true), 5);
    }
}
