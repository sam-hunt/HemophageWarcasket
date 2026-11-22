using System.Collections.Generic;
using Verse;

namespace HemophageWarcasket;

// The sanguophages Biotech's two sanguophage quests bring to the player's map, dressed in the
// set, and the thralls of the crashed shuttle quest, some of them dressed in a lesser warcasket
// of VFEP's. On the armor (ThingDefs_Misc/Warcasket_Hemophage.xml, whose header has the
// numbers), which with otherPieces is the set a sanguophage wears. QuestPilot has the rules and
// the two patches that call them, ThrallBudget how many thralls a quest dresses; the settings
// write the three chances, the shuttle master's threat-point floor, requiredResearch and
// thrallBudgetShare.
public class QuestPilotExtension : DefModExtension
{
    // The chance that the sanguophage of each source wears the set, 0 to 1. At 0 that source
    // dresses no one and draws nothing from Rand.
    public float shipMasterChance;
    public float meetingLeaderChance;
    public float meetingGuestChance;

    // The quest's threat points the shuttle master's chance needs; under it the master is never
    // dressed. 0 is no floor.
    public float shipMasterMinPoints;

    // The research the player must have finished before any of the chances applies. Null is no
    // requirement.
    public ResearchProjectDef requiredResearch;

    // The projects the settings offer for requiredResearch after "none", earliest first.
    public List<ResearchProjectDef> researchOptions = [];

    // The set's pieces besides the armor carrying this extension. A pawn is dressed in all of
    // them or in none.
    public List<ThingDef> otherPieces = [];

    // What a dressed pawn's weapon is exchanged for, whichever set the pawn was dressed in:
    // rangedWeapon for a sanguophage whose Shooting is above their Melee and for a thrall who
    // held a gun, meleeWeapon for the others. Null leaves those pawns the weapon the pawn kind
    // gave.
    public ThingDef rangedWeapon;
    public ThingDef meleeWeapon;

    // Where another mod keeps a gun's magazine, for WeaponAmmo to load the gun a dressed pawn
    // is handed: the ThingComp class on the gun, its method that fills the magazine, and its
    // properties for the loaded ammunition's def and the magazine's size. Unset here; a compat
    // root's patch fills them in. ammoSpareMagazines is how many more magazines' worth the pawn
    // carries.
    public string ammoComp;
    public string ammoLoadMethod;
    public string ammoDefProperty;
    public string ammoMagazineProperty;
    public int ammoSpareMagazines;

    // The share of the crashed shuttle quest's threat points its pawns may be worth in combat
    // power, 0 to 1, of which what the quest already fields is spent first and the rest goes to
    // dressing thralls. 0 dresses none.
    public float thrallBudgetShare;

    // From thrallMinPoints threat points the quest dresses at least thrallMinDressed thralls,
    // whatever the share leaves, while the share is above 0.
    public float thrallMinPoints;
    public int thrallMinDressed;

    // The combat power the set adds to a dressed shuttle master, which the thralls' budget pays
    // before any thrall is dressed.
    public float shipMasterPrice;

    // The sets a thrall may be dressed in, each offered once the player has finished its own
    // armor's research.
    public List<ThrallTier> thrallTiers = [];

    public float ChanceFor(QuestPilotSource source, float points) => source switch
    {
        QuestPilotSource.ShipMaster => points < shipMasterMinPoints ? 0f : shipMasterChance,
        QuestPilotSource.MeetingLeader => meetingLeaderChance,
        _ => meetingGuestChance,
    };

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string error in base.ConfigErrors())
            yield return error;
        if (shipMasterChance < 0f || shipMasterChance > 1f)
            yield return $"shipMasterChance must be between 0 and 1, is {shipMasterChance}";
        if (shipMasterMinPoints < 0f)
            yield return $"shipMasterMinPoints must not be negative, is {shipMasterMinPoints}";
        if (meetingLeaderChance < 0f || meetingLeaderChance > 1f)
            yield return $"meetingLeaderChance must be between 0 and 1, is {meetingLeaderChance}";
        if (meetingGuestChance < 0f || meetingGuestChance > 1f)
            yield return $"meetingGuestChance must be between 0 and 1, is {meetingGuestChance}";
        if (requiredResearch != null && !researchOptions.Contains(requiredResearch))
            yield return $"requiredResearch {requiredResearch.defName} is not among researchOptions, so the settings cannot show it";
        foreach (ThingDef piece in otherPieces)
        {
            if (piece?.apparel == null)
                yield return $"otherPieces names {piece?.defName ?? "a missing def"}, which is not apparel";
        }
        if (rangedWeapon is { IsRangedWeapon: false })
            yield return $"rangedWeapon {rangedWeapon.defName} is not a ranged weapon";
        if (meleeWeapon is { IsMeleeWeapon: false })
            yield return $"meleeWeapon {meleeWeapon.defName} is not a melee weapon";
        if (ammoSpareMagazines < 0)
            yield return $"ammoSpareMagazines must not be negative, is {ammoSpareMagazines}";
        if (thrallBudgetShare < 0f || thrallBudgetShare > 1f)
            yield return $"thrallBudgetShare must be between 0 and 1, is {thrallBudgetShare}";
        if (thrallMinPoints < 0f)
            yield return $"thrallMinPoints must not be negative, is {thrallMinPoints}";
        if (thrallMinDressed < 0)
            yield return $"thrallMinDressed must not be negative, is {thrallMinDressed}";
        if (shipMasterPrice < 0f)
            yield return $"shipMasterPrice must not be negative, is {shipMasterPrice}";
        foreach (ThrallTier tier in thrallTiers)
        {
            if (tier?.armor?.apparel == null)
                yield return $"a thrall tier names {tier?.armor?.defName ?? "a missing def"} as its armor, which is not apparel";
            if (tier == null)
                continue;
            if (tier.price <= 0f)
                yield return $"the thrall tier of {tier.armor?.defName} must have a price above 0, has {tier.price}";
            foreach (ThingDef piece in tier.otherPieces)
            {
                if (piece?.apparel == null)
                    yield return $"the thrall tier of {tier.armor?.defName} names {piece?.defName ?? "a missing def"}, which is not apparel";
            }
        }
    }
}

// One warcasket set a thrall of the crashed shuttle quest may be dressed in: the armor, the
// set's other pieces and the combat power the set adds to the thrall, which is what dressing
// one costs the quest's budget (ThrallBudget).
public class ThrallTier
{
    public ThingDef armor;
    public List<ThingDef> otherPieces = [];
    public float price;

    // The player's own research: a colony meets only the sets it could build.
    public bool Researched => armor?.researchPrerequisites?.TrueForAll(project => project.IsFinished) ?? true;
}

// Where Biotech generates a sanguophage for a quest: the master of the SanguophageShip quest's
// crashed shuttle, and the sanguophages of the SanguophageMeetingHost quest, of whom the first
// is the one who asks for the meeting and leads it.
public enum QuestPilotSource
{
    ShipMaster,
    MeetingLeader,
    MeetingGuest,
}
