using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace HemophageWarcasket;

// Dresses the pawns of Biotech's two sanguophage quests in a warcasket as the quest is
// generated: a sanguophage in the hemophage set (TryDress), by the chance the armor's
// QuestPilotExtension gives its source and only once the player has finished the extension's
// requiredResearch, and some of the crashed shuttle quest's thralls in a lesser set of VFEP's
// (DressThralls), as many as ThrallBudget answers for the quest's threat points. Called by the
// two postfixes on the quest roots that make those pawns
// (Patches/QuestNode_Root_SanguophageShip_RunInt and
// Patches/QuestNode_Root_SanguophageMeetingHost_RunInt, whose headers have the call sites),
// never from pawn generation: PawnGenerator.GeneratePawn has returned by then, so VFEP's
// postfix on it (the one Patches/PawnGenerator_GeneratePawn_Patch_Postfix masks) found no
// warcasket to complete with parts of its own.
//
// The pawn is alive and off the map: quest.GeneratePawn passes it to the world, where it waits
// for the quest to be accepted and its arrival delay to run. The rolls, the research and the
// quest's threat points are therefore read as the quest is generated, and a setting changed or
// a project finished after that does not reach a pawn already made.
//
// What a dressed pawn is left as is what the foundry leaves a pilot as, piece by piece:
// - The set alone. Everything else the pawn wore is destroyed first, as VFEP's postfix strips
//   a generated pilot, and each piece goes on locked, as the foundry and that postfix lock
//   theirs (WarcasketProject.ApplyOn, Wear with locked true). The hemogen packs the pawn kind
//   gave are not apparel and stay.
// - The set's trait, through VEF's hook on Pawn_ApparelTracker.Notify_ApparelAdded
//   (ApparelExtensionUtilities.EquipGear, the armor's traitsOnEquip): ours for the hemophage
//   set, VFEP_WarcasketTrait for a thrall's.
// - The set painted as the pawn's kind paints its clothes, in one colour: once worn each piece
//   goes through vanilla's PawnApparelGenerator.PostProcessApparel, which generation runs on
//   everything a pawn wears and which also sets the ideoligion's and the kind's styles, then
//   the other two take the first piece's colour, since that pass draws a colour for each piece
//   separately and a set built for one pilot is painted as one. Biotech's Sanguophage kind
//   names a dark grey and a dark red for any apparel (its specificApparelRequirements), so a
//   sanguophage's set arrives in one of those. The kind's colours are read there and named
//   nowhere here. A kind that names none, the thrall's, leaves the set the one colour it was
//   made in, the armor's draw from the def's colorGenerator (CompColorable.Initialize draws one
//   for each piece as it is made, and Dress copies the first piece's onto the others).
// - A warcasket weapon in place of the one the pawn kind gave, and nothing for a pawn
//   generated unarmed. A sanguophage takes the one their skills fit, the extension's
//   rangedWeapon when Shooting is the higher level and its meleeWeapon otherwise
//   (FightsAtRange): vanilla arms a pawn by price and commonality, with no look at its skills
//   (PawnWeaponGenerator.TryGenerateWeaponFor), and Biotech's sanguophage has a melee
//   aptitude gene, so most arrive with the melee weapon. A thrall keeps the kind vanilla
//   gave, rangedWeapon for a gun and meleeWeapon for a melee weapon, so the shuttle's fight
//   keeps vanilla's mix of the two.
//   VEF lets a pawn hold one for the trait or the armor (EquipmentUtility_CanEquip_Patch), and
//   VFEP prices its own warcasket pilots with one, which is where the sets' prices come from.
//   The old weapon is destroyed. The crashed shuttle quest biocodes its reinforcements'
//   weapons when it starts (QuestPart_BiocodeWeapons, which codes whatever each pawn then
//   holds that can be coded): VFEP's warcasket weapons cannot be, so a dressed thrall's drops
//   as a junker's does. WeaponAmmo loads a gun where another mod gives guns a magazine.
//
// All or nothing: a pawn who cannot wear every piece (ApparelUtility.HasPartsToWear and
// ApparelProperties.PawnCanWear, the two refusals Pawn_ApparelTracker.Wear only warns of), or a
// set whose pieces another mod has made unwearable together (CanWearTogether, on which Wear
// takes the earlier piece off again), leaves the pawn the clothes and the weapon vanilla gave
// them, so no pawn wears part of a set. The pieces and the weapon are made before anything is
// destroyed, so a def that cannot be made costs the pawn nothing either.
//
// A dressed pawn that leaves the map is a world pawn like any other. Should the generator reuse
// one, PawnGenerator.RedressPawn destroys what it wears (GenerateStartingApparelFor) before
// VFEP's postfix runs, so the mask's condition, no generated pawn in a part of ours, holds.
//
// The hemophage set goes only on a living, hemogenic pawn, told by the gene and not by the
// xenotype: a mod that makes either quest hand over another kind of pawn gets no set for it.
// A thrall needs no gene: the lesser sets have no mechanic of ours.
//
// The thralls. The shuttle quest has two groups of them, those who crash with the master and
// the reinforcements dropped later, and the sets are handed out one to each group in turn,
// reinforcements first, in a random order within each group: a master who travels light, with
// the heavier escort a call away, and warcaskets in both waves once there are two or more.
// Each set on offer needs its own armor's research (ThrallTier.Researched), not
// requiredResearch, which is the hemophage set's. A thrall who cannot wear the set is passed
// over and the set goes to the next. What the quest already fields is the pawns' own
// combatPower, read off their kinds.
//
// A failure here is logged and goes no further: the callers run inside quest generation, where
// an exception from a postfix would cost the player the quest.
//
// Vanilla as read on 1.7.5014; the calls made here compile against 1.6.
public static class QuestPilot
{
    private const string ArmorDefName = "HPWC_Warcasket_Hemophage";

    // Null while the armor or its extension is missing (another mod's patch).
    public static QuestPilotExtension Extension =>
        DefDatabase<ThingDef>.GetNamedSilentFail(ArmorDefName)?.GetModExtension<QuestPilotExtension>();

    // True when the pawn now wears the hemophage set.
    public static bool TryDress(Pawn pawn, QuestPilotSource source, float points)
    {
        try
        {
            ThingDef armor = DefDatabase<ThingDef>.GetNamedSilentFail(ArmorDefName);
            QuestPilotExtension extension = armor?.GetModExtension<QuestPilotExtension>();
            if (extension == null || pawn?.apparel == null || pawn.Dead)
                return false;
            if (extension.requiredResearch is { IsFinished: false })
                return false;
            if (pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>() == null)
                return false;
            if (!Rand.Chance(extension.ChanceFor(source, points)))
                return false;
            return Dress(pawn, [armor, .. extension.otherPieces], extension, weaponBySkill: true);
        }
        catch (Exception exception)
        {
            Log.Error($"[Hemophage Warcasket] Could not dress the quest's sanguophage {pawn} in the hemophage warcasket: {exception}");
            return false;
        }
    }

    public static void DressThralls(
        Pawn master, bool masterDressed, List<Pawn> shuttleThralls, List<Pawn> reinforcements, float points)
    {
        try
        {
            QuestPilotExtension extension = Extension;
            if (extension == null || extension.thrallBudgetShare <= 0f)
                return;
            List<ThrallTier> tiers = extension.thrallTiers.FindAll(tier => tier?.armor != null && tier.Researched);
            if (tiers.Count == 0)
                return;
            shuttleThralls = [.. shuttleThralls.InRandomOrder()];
            reinforcements = [.. reinforcements.InRandomOrder()];
            List<Pawn> order = ThrallBudget.Alternate(reinforcements, shuttleThralls);
            float fielded = master?.kindDef.combatPower ?? 0f;
            foreach (Pawn thrall in order)
                fielded += thrall.kindDef.combatPower;
            List<int> picks = ThrallBudget.Picks(
                points, fielded, order.Count, masterDressed ? extension.shipMasterPrice : 0f,
                extension.thrallBudgetShare, extension.thrallMinPoints, extension.thrallMinDressed,
                tiers.ConvertAll(tier => tier.price));
            int next = 0;
            foreach (Pawn thrall in order)
            {
                if (next >= picks.Count)
                    break;
                ThrallTier tier = tiers[picks[next]];
                if (Dress(thrall, [tier.armor, .. tier.otherPieces], extension, weaponBySkill: false))
                    next++;
            }
        }
        catch (Exception exception)
        {
            Log.Error($"[Hemophage Warcasket] Could not dress the crashed shuttle's thralls in warcaskets: {exception}");
        }
    }

    // True for the gun, false for the melee weapon. SkillRecord.Level counts a gene's aptitude
    // and is 0 for a skill the pawn cannot use. A tie goes to the melee weapon: the set is built
    // to fight up close.
    public static bool FightsAtRange(int shooting, int melee) => shooting > melee;

    private static bool Dress(Pawn pawn, List<ThingDef> set, QuestPilotExtension extension, bool weaponBySkill)
    {
        if (pawn?.apparel == null || pawn.Dead)
            return false;
        if (!set.TrueForAll(piece => Fits(pawn, piece)) || !WearableTogether(set, pawn.RaceProps.body))
            return false;
        var pieces = new List<Apparel>();
        foreach (ThingDef piece in set)
        {
            var apparel = (Apparel)ThingMaker.MakeThing(piece, GenStuff.RandomStuffFor(piece));
            if (pieces.Count > 0)
                apparel.SetColor(pieces[0].DrawColor, reportFailure: false);
            pieces.Add(apparel);
        }
        ThingWithComps held = pawn.equipment?.Primary;
        ThingWithComps weapon = WeaponInPlaceOf(pawn, held, extension, weaponBySkill);
        pawn.apparel.DestroyAll();
        foreach (Apparel apparel in pieces)
        {
            pawn.apparel.Wear(apparel, dropReplacedApparel: false, locked: true);
            PawnApparelGenerator.PostProcessApparel(apparel, pawn);
        }
        for (int i = 1; i < pieces.Count; i++)
            pieces[i].SetColor(pieces[0].DrawColor, reportFailure: false);
        if (weapon != null)
        {
            pawn.equipment.DestroyEquipment(held);
            pawn.equipment.AddEquipment(weapon);
            WeaponAmmo.Load(weapon, pawn, extension);
        }
        return true;
    }

    // Null for no exchange: an unarmed pawn, a kind of weapon the extension names nothing for,
    // or a pawn already holding it.
    private static ThingWithComps WeaponInPlaceOf(
        Pawn pawn, ThingWithComps held, QuestPilotExtension extension, bool bySkill)
    {
        if (held == null || !held.def.IsWeapon)
            return null;
        bool ranged = held.def.IsRangedWeapon;
        if (bySkill && pawn.skills is { } skills)
            ranged = FightsAtRange(skills.GetSkill(SkillDefOf.Shooting).Level, skills.GetSkill(SkillDefOf.Melee).Level);
        ThingDef def = ranged ? extension.rangedWeapon : extension.meleeWeapon;
        if (def == null || def == held.def)
            return null;
        return ThingMaker.MakeThing(def, GenStuff.RandomStuffFor(def)) as ThingWithComps;
    }

    private static bool Fits(Pawn pawn, ThingDef piece) =>
        piece?.apparel != null
        && ApparelUtility.HasPartsToWear(pawn, piece)
        && piece.apparel.PawnCanWear(pawn, ignoreGender: true);

    private static bool WearableTogether(List<ThingDef> set, BodyDef body)
    {
        for (int i = 0; i < set.Count; i++)
            for (int j = i + 1; j < set.Count; j++)
                if (!ApparelUtility.CanWearTogether(set[i], set[j], body))
                    return false;
        return true;
    }
}
