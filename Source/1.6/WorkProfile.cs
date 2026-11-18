using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using VEF.Pawns;
using Verse;

namespace HemophageWarcasket;

// Derives the hemophage trait (HPWC_WarcasketTrait_Hemophage) from VFEP_WarcasketTrait at
// startup, so the pilot's work profile and the trait's other effects follow VFEP's trait as the
// full mod list leaves it: a patch mod that opens work for every warcasket opens it here too,
// and whatever VFEP puts on its degree or VEF extension on the running game version (1.6 and
// 1.7 differ) lands on ours. The trait def's header carries the player-facing contract.
//
// The settings may open or block every work type VFEP's trait blocks, one row each. Derive
// records which they are (Toggleable) and the suit's own rule for each (OpenByDefault: the
// Intellectual ones open, the rest blocked); Compose turns a choice of open types into the
// trait's tags and work types. Mirror writes the default profile and Apply the one the settings
// choose, on every ApplyToDefs.
//
// A trait blocks a work type on any shared tag, and its disabledWorkTypes can only add, so
// opening a type means clearing every blocked tag it carries and listing by name the closed
// types that tag was covering. A tag stays while every type under it is closed, so with every
// row unticked the trait is VFEP's block unchanged. The tag matters beyond the work tab: the
// Bio tab's "Incapable of" list and the game's direct tag checks (HackUtility.IsCapableOfHacking
// on Intellectual) read tags alone, and a skill is disabled by its tag or once every work type
// using it is blocked (SkillDef.IsDisabled). So the default pilot can research, hack and train
// Intellectual, and the Bio tab stops naming a tag once one of its rows is ticked.
//
// Mirror runs first in Startup.Run, against defs fresh from XML with every XML patch applied.
// Nothing it writes is cached by then: Trait.GetDisabledWorkTypes reads the def live and
// Pawn.CombinedDisabledWorkTags is uncached. A pawn does cache its disabled work types, so
// Apply refreshes every live pawn carrying the trait.
public static class WorkProfile
{
    private const string VfepTraitName = "VFEP_WarcasketTrait";
    private const string OwnTraitName = "HPWC_WarcasketTrait_Hemophage";

    // The tag whose work types the suit opens on its own.
    public const WorkTags OpenTag = WorkTags.Intellectual;

    // The degree's text stays ours, and so do its identity (degree: the armor grants degree 0 and
    // saves store it) and its selection weight (commonality: irrelevant at TraitDef commonality
    // 0); everything else on it follows VFEP's.
    private static readonly HashSet<string> DegreeOwnFields =
    [
        nameof(TraitDegreeData.label),
        nameof(TraitDegreeData.labelMale),
        nameof(TraitDegreeData.labelFemale),
        nameof(TraitDegreeData.untranslatedLabel),
        nameof(TraitDegreeData.description),
        nameof(TraitDegreeData.degree),
        nameof(TraitDegreeData.commonality),
    ];

    // What VFEP's trait leaves the pilot before the settings: its tags (ManualDumb swapped for
    // Cleaning) and the work types they or VFEP's own list block, with the suit's defaults.
    public sealed class Profile
    {
        public WorkTags baseTags;
        public List<WorkTypeDef> toggleable = [];
        public HashSet<WorkTypeDef> openByDefault = [];
    }

    // The mirror's result, replaced on every run and empty until the first or after a failed
    // one, so Apply then writes nothing and the trait keeps its XML profile.
    private static TraitDef own;
    private static Profile profile = new();

    // What the settings may open or block, row by row: the types open by default first, each
    // group in the work tab's order.
    public static IReadOnlyList<WorkTypeDef> Toggleable => profile.toggleable;

    // The suit's own rule for a toggleable work type (Derive).
    public static bool OpenByDefault(WorkTypeDef work) => profile.openByDefault.Contains(work);

    // Never throws out of Startup.Run: a failure here is logged and the settings still apply.
    public static void Mirror()
    {
        own = null;
        profile = new Profile();
        try
        {
            MirrorFromVfep();
        }
        catch (Exception e)
        {
            Log.Error($"[Hemophage Warcasket] Mirroring the hemophage warcasket trait from {VfepTraitName} failed; the trait keeps its XML profile and effects. {e}");
        }
    }

    private static void MirrorFromVfep()
    {
        TraitDef vfep = DefDatabase<TraitDef>.GetNamedSilentFail(VfepTraitName);
        TraitDef ours = DefDatabase<TraitDef>.GetNamedSilentFail(OwnTraitName);
        if (vfep == null || ours == null)
        {
            Log.Error($"[Hemophage Warcasket] Cannot mirror the hemophage warcasket trait: {(vfep == null ? VfepTraitName : OwnTraitName)} is missing. VFE Pirates is a hard dependency; this is a broken install.");
            return;
        }

        profile = Derive(vfep.disabledWorkTags, vfep.disabledWorkTypes, WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder);
        own = ours;
        Write(OpenByDefault);

        // Effects: full overwrites, nulls included, so a field VFEP leaves at its default
        // replaces whatever our XML carried and the pilot matches VFEP's wearers on the running
        // version (1.7 has the pain factor and rest-fall on the degree; 1.6 has the pain factor
        // on the extension and nothing on the degree).
        if (vfep.degreeDatas.Count > 0 && ours.degreeDatas.Count > 0)
            CopyPublicFields(vfep.degreeDatas[0], ours.degreeDatas[0], DegreeOwnFields);
        else
            Log.Warning($"[Hemophage Warcasket] {VfepTraitName} or {OwnTraitName} has no degree; the hemophage trait keeps its XML effects.");

        TraitExtension vfepExtension = vfep.GetModExtension<TraitExtension>();
        TraitExtension ownExtension = ours.GetModExtension<TraitExtension>();
        if (vfepExtension != null && ownExtension != null)
            CopyPublicFields(vfepExtension, ownExtension, null);
        else
            Log.Warning($"[Hemophage Warcasket] {VfepTraitName} or {OwnTraitName} has no VEF TraitExtension; the hemophage trait keeps its XML extension.");
    }

    // The pilot's profile from VFEP's tags and work types, over every work type in the work
    // tab's order. Pure, so the tests reach it with bare defs. VFEP's list is read by value,
    // never aliased.
    public static Profile Derive(WorkTags vfepTags, IEnumerable<WorkTypeDef> vfepTypes, IEnumerable<WorkTypeDef> allTypes)
    {
        // ManualDumb covers only Hauling and Cleaning, and VFEP's hauling exemption is a Harmony
        // postfix keyed on its own def, so the swap keeps hauling open for the pilot.
        WorkTags tags = vfepTags;
        if ((tags & WorkTags.ManualDumb) != 0)
            tags = (tags & ~WorkTags.ManualDumb) | WorkTags.Cleaning;

        var result = new Profile { baseTags = tags };
        var vfepList = new List<WorkTypeDef>(vfepTypes ?? Enumerable.Empty<WorkTypeDef>());
        var closed = new List<WorkTypeDef>();
        foreach (WorkTypeDef work in allTypes)
        {
            if ((tags & work.workTags) == 0 && !vfepList.Contains(work))
                continue;
            if ((work.workTags & OpenTag) != 0)
            {
                result.toggleable.Add(work);
                result.openByDefault.Add(work);
            }
            else
            {
                closed.Add(work);
            }
        }
        result.toggleable.AddRange(closed);
        return result;
    }

    // The trait's tags and work types for a choice of open toggleable types: every tag an open
    // type carries clears, and a closed type is listed unless a remaining tag already blocks it.
    public static (WorkTags tags, List<WorkTypeDef> types) Compose(Profile from, Func<WorkTypeDef, bool> open)
    {
        WorkTags tags = from.baseTags;
        foreach (WorkTypeDef work in from.toggleable)
        {
            if (open(work))
                tags &= ~work.workTags;
        }
        var types = new List<WorkTypeDef>();
        foreach (WorkTypeDef work in from.toggleable)
        {
            if (!open(work) && (tags & work.workTags) == 0)
                types.Add(work);
        }
        return (tags, types);
    }

    // Writes the profile the settings choose, open deciding each toggleable work type, and
    // refreshes every live pawn carrying the trait: Pawn.GetDisabledWorkTypes is cached per
    // pawn, and its Notify_DisabledWorkTypesChanged clears the cache, refreshes the skills'
    // disabled state and zeroes the work priorities the write blocked (a type it opened stays
    // unassigned until the player sets it). World pawns too, since a pilot away on a quest comes
    // back with the cache it left with and nothing on its return clears it before the next save.
    public static void Apply(Func<WorkTypeDef, bool> open)
    {
        if (own == null)
            return;
        Write(open);
        if (Current.ProgramState != ProgramState.Playing)
            return;
        foreach (Pawn pawn in PawnsFinder.AllMapsWorldAndTemporary_Alive)
        {
            if (pawn.story?.traits != null && pawn.story.traits.HasTrait(own))
                pawn.Notify_DisabledWorkTypesChanged();
        }
    }

    private static void Write(Func<WorkTypeDef, bool> open)
    {
        (own.disabledWorkTags, own.disabledWorkTypes) = Compose(profile, open);
    }

    // Copies every public instance field of T from source onto target by reflection, so a field
    // T gains in a later game or VEF version is carried without a code change. Enumerated on T,
    // not the source's runtime type, so a patch that subclasses VFEP's degree or extension still
    // copies cleanly. List<T> values are copied into new lists so the two defs never share one;
    // the elements (StatModifiers, curves) stay shared, which is fine as nothing mutates them.
    private static void CopyPublicFields<T>(T source, T target, ICollection<string> skip)
    {
        foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            if (skip != null && skip.Contains(field.Name))
                continue;
            object value = field.GetValue(source);
            if (value is IList list && field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var copy = (IList)Activator.CreateInstance(field.FieldType);
                foreach (object item in list)
                    copy.Add(item);
                value = copy;
            }
            field.SetValue(target, value);
        }
    }
}
