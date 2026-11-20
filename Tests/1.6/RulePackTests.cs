#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Xunit;

namespace HemophageWarcasket.Tests;

// The combat log lines in 1.6/Defs/RulePackDefs/, read off the XML. A [symbol] that nothing
// supplies is no error in the build and a red error in game only when the line is first
// resolved, so each pack is held to the symbols its entry supplies: the keywords the pack
// defines itself, the pawn symbols of the INITIATOR and RECIPIENT prefixes, and the one damaged
// part (recipient_part0_*). The destroyed and damaged part trios are left out on purpose: which
// of the two exists depends on the outcome. A pack has no <include>, so nothing here needs the
// game's Data folder, and every r_logentry line must name both pawns and the part exactly once.
public class RulePackTests
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

    // GrammarUtility.RulesForPawn on 1.6: the Rule_String keywords yielded for every pawn. Left
    // out because only some pawns get them: factionName (needs a faction), title, titleIndef,
    // titleDef (need a story title), royalTitle* and bestRoyalTitle* (need a royal title). The
    // target can be an animal or a mechanoid and the pilot any humanlike. relationInfo is out
    // too: BattleLogEntry_MeleeCombat asks for it on 1.6 and not on 1.7.
    private static readonly string[] PawnSuffixes =
    [
        "nameFull", "label", "labelNoParenthesis", "definite", "nameDef", "indefinite", "nameIndef",
        "pronoun", "possessive", "objective", "kind", "kindPlural", "age", "chronologicalAge",
    ];

    private static readonly string[] PartSymbols =
        ["recipient_part0_label", "recipient_part0_definite", "recipient_part0_indefinite"];

    private static readonly Regex Rule = new(@"^(?<key>\w+)(\((?<constraints>[^)]*)\))?->(?<text>.*)$", RegexOptions.Singleline);
    private static readonly Regex Symbol = new(@"\[(?<name>[^\]]+)\]");

    private static string PackDir => Path.Combine(RepoRoot, "1.6/Defs/RulePackDefs");

    public static TheoryData<string> PackFiles()
    {
        var data = new TheoryData<string>();
        foreach (var file in Directory.GetFiles(PackDir, "*.xml", SearchOption.AllDirectories).OrderBy(f => f, StringComparer.Ordinal))
            data.Add(file.Substring(RepoRoot.Length + 1).Replace('\\', '/'));
        return data;
    }

    private static XElement LoadDef(string relative)
    {
        var defs = XDocument.Load(Path.Combine(RepoRoot, relative)).Root!.Elements().ToList();
        Assert.True(defs.Count == 1, $"{relative} holds {defs.Count} defs, one def per file");
        Assert.Equal("RulePackDef", defs[0].Name.LocalName);
        return defs[0];
    }

    private static (string Def, string Key, string Text)[] Rules(XElement def)
    {
        var name = (string?)def.Element("defName") ?? "(no defName)";
        return def.Element("rulePack")!.Element("rulesStrings")!.Elements("li").Select(li =>
        {
            var raw = (string)li;
            var m = Rule.Match(raw);
            Assert.True(m.Success, $"{name}: rule '{raw}' is not keyword->text or keyword(constraints)->text");
            return (name, m.Groups["key"].Value, m.Groups["text"].Value);
        }).ToArray();
    }

    // Dismember's strike takes hold of one part and either tears it off or does not, and the
    // entry says which in the constant recipient_part0_destroyed. A line with no such
    // constraint would be drawn for both outcomes, so every line carries one, and both
    // outcomes have a line.
    [Fact]
    public void EveryDismemberLine_IsForAPartTornOffOrOneThatHeld()
    {
        var def = LoadDef("1.6/Defs/RulePackDefs/Combat_Dismember.xml");
        var outcomes = def.Element("rulePack")!.Element("rulesStrings")!.Elements("li").Select(li => (string)li)
            .Where(raw => raw.StartsWith("r_logentry", StringComparison.Ordinal))
            .Select(raw => (Raw: raw, Outcome: Regex.Match(Rule.Match(raw).Groups["constraints"].Value, @"(?:^|,)recipient_part0_destroyed==(True|False)(?:,|$)")))
            .ToList();
        Assert.All(outcomes, line => Assert.True(line.Outcome.Success, $"'{line.Raw}' is constrained to neither outcome"));
        Assert.Contains(outcomes, line => line.Outcome.Groups[1].Value == "True");
        Assert.Contains(outcomes, line => line.Outcome.Groups[1].Value == "False");
    }

    private static bool Supplied(string symbol, string[] keywords) =>
        keywords.Contains(symbol)
        || PartSymbols.Contains(symbol)
        || PawnSuffixes.Any(s => symbol == "INITIATOR_" + s || symbol == "RECIPIENT_" + s);

    [Theory]
    [MemberData(nameof(PackFiles))]
    public void FileHoldsOneDef_NamedAfterIt(string relative)
    {
        var def = LoadDef(relative);
        var defName = (string?)def.Element("defName");
        Assert.NotNull(defName);
        Assert.True(defName!.StartsWith("HPWC_", StringComparison.Ordinal), $"{relative}: defName {defName} lacks the HPWC_ prefix");
        Assert.Equal(defName.Substring("HPWC_".Length), Path.GetFileNameWithoutExtension(relative));
    }

    [Theory]
    [MemberData(nameof(PackFiles))]
    public void Pack_HasNoInclude(string relative)
    {
        var def = LoadDef(relative);
        var rulePack = def.Element("rulePack")!;
        Assert.True(!rulePack.Descendants("include").Any(), $"{(string?)def.Element("defName")} has an <include>; every line must be self-contained");
    }

    [Theory]
    [MemberData(nameof(PackFiles))]
    public void Pack_HasARootLogEntryRule(string relative)
    {
        var rules = Rules(LoadDef(relative));
        Assert.True(rules.Any(r => r.Key == "r_logentry"), $"{rules.FirstOrDefault().Def ?? relative} has no r_logentry rule");
    }

    [Theory]
    [MemberData(nameof(PackFiles))]
    public void EverySymbol_IsSuppliedByTheEntryOrThePack(string relative)
    {
        var rules = Rules(LoadDef(relative));
        var keywords = rules.Select(r => r.Key).Distinct().ToArray();
        foreach (var (def, key, text) in rules)
        {
            foreach (Match m in Symbol.Matches(text))
            {
                var symbol = m.Groups["name"].Value;
                Assert.True(Supplied(symbol, keywords), $"{def}: rule '{key}->{text}' uses [{symbol}], which the pack does not define and the entry does not supply");
            }
        }
    }

    [Theory]
    [MemberData(nameof(PackFiles))]
    public void EveryLogEntryLine_NamesBothPawnsAndThePartOnce(string relative)
    {
        foreach (var (def, key, text) in Rules(LoadDef(relative)).Where(r => r.Key == "r_logentry"))
        {
            var symbols = Symbol.Matches(text).Cast<Match>().Select(m => m.Groups["name"].Value).ToList();
            Assert.True(symbols.Any(s => s.StartsWith("INITIATOR_", StringComparison.Ordinal)), $"{def}: line '{text}' never names the INITIATOR");
            Assert.True(symbols.Any(s => s.StartsWith("RECIPIENT_", StringComparison.Ordinal)), $"{def}: line '{text}' never names the RECIPIENT");
            var parts = symbols.Count(s => PartSymbols.Contains(s));
            Assert.True(parts == 1, $"{def}: line '{text}' names the damaged part {parts} times, expected once");
        }
    }
}
