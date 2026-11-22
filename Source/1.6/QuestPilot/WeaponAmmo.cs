using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

namespace HemophageWarcasket;

// Loads the gun QuestPilot hands a dressed pawn, for a mod that gives guns a magazine and
// fills it as a pawn is generated: a gun exchanged after generation is made empty, and its
// pawn carries the ammunition of the gun it no longer has. Which mod is the def's business:
// QuestPilotExtension names the ThingComp class on a gun, the method that fills its magazine
// and the two properties that give the loaded ammunition's def and the magazine's size, a
// compat root's patch fills them in, and this reaches all four by reflection, so the code
// names no type of another mod and nothing is looked for when the def names none.
//
// The magazine is filled through the mod's own method, called with its one optional parameter
// left null. Then ammoSpareMagazines magazines' worth of the loaded ammunition go into the
// pawn's inventory, in stacks no larger than the ammunition's stack limit, where the one mod
// read so far looks for a reload (Combat Extended's CompAmmoUser.TryFindAmmoInInventory, over
// a list its postfix on ThingOwner.NotifyAdded refreshes as a thing joins
// pawn.inventory.innerContainer, CE 16.7.4). The ammunition the pawn kind's own gun came with
// stays in the inventory.
//
// A throw from the other mod's members is logged here and goes no further: the pawn is dressed
// and armed by then, and QuestPilot's callers read a throw as a pawn left undressed.
//
// The names are resolved once per set of them, since a play-data reload replaces the def and
// keeps its strings. A class or member that is not there, after an update of that mod, is one
// warning and an empty gun from then on.
internal static class WeaponAmmo
{
    private const BindingFlags Members = BindingFlags.Instance | BindingFlags.Public;

    private static string resolvedFor;
    private static Type compType;
    private static MethodInfo loadMethod;
    private static PropertyInfo ammoProperty;
    private static PropertyInfo magazineProperty;

    public static void Load(ThingWithComps gun, Pawn pawn, QuestPilotExtension extension)
    {
        if (extension.ammoComp.NullOrEmpty())
            return;
        Resolve(extension);
        if (loadMethod == null)
            return;
        try
        {
            List<ThingComp> comps = gun.AllComps;
            for (int i = 0; i < comps.Count; i++)
            {
                if (!compType.IsInstanceOfType(comps[i]))
                    continue;
                loadMethod.Invoke(comps[i], [null]);
                AddSpares(comps[i], pawn, extension.ammoSpareMagazines);
                return;
            }
        }
        catch (Exception exception)
        {
            Log.Error($"[Hemophage Warcasket] Could not load {gun} for {pawn}: {exception}");
        }
    }

    private static void AddSpares(ThingComp comp, Pawn pawn, int magazines)
    {
        if (magazines <= 0 || ammoProperty == null || magazineProperty == null || pawn.inventory == null)
            return;
        if (ammoProperty.GetValue(comp) is not ThingDef ammo)
            return;
        int left = (int)magazineProperty.GetValue(comp) * magazines;
        while (left > 0)
        {
            Thing stack = ThingMaker.MakeThing(ammo);
            stack.stackCount = Mathf.Min(left, Mathf.Max(1, ammo.stackLimit));
            left -= stack.stackCount;
            if (!pawn.inventory.innerContainer.TryAdd(stack))
            {
                stack.Destroy();
                return;
            }
        }
    }

    private static void Resolve(QuestPilotExtension extension)
    {
        string names = $"{extension.ammoComp}|{extension.ammoLoadMethod}|{extension.ammoDefProperty}|{extension.ammoMagazineProperty}";
        if (names == resolvedFor)
            return;
        resolvedFor = names;
        compType = GenTypes.GetTypeInAnyAssembly(extension.ammoComp);
        loadMethod = extension.ammoLoadMethod.NullOrEmpty() ? null : compType?.GetMethod(extension.ammoLoadMethod, Members);
        if (loadMethod != null && loadMethod.GetParameters().Length != 1)
            loadMethod = null;
        ammoProperty = extension.ammoDefProperty.NullOrEmpty() ? null : compType?.GetProperty(extension.ammoDefProperty, Members);
        if (ammoProperty != null && !typeof(ThingDef).IsAssignableFrom(ammoProperty.PropertyType))
            ammoProperty = null;
        magazineProperty = extension.ammoMagazineProperty.NullOrEmpty() ? null
            : compType?.GetProperty(extension.ammoMagazineProperty, Members);
        if (magazineProperty != null && magazineProperty.PropertyType != typeof(int))
            magazineProperty = null;
        if (loadMethod == null)
            Log.Warning($"[Hemophage Warcasket] A dressed quest pawn's gun is not loaded: no method {extension.ammoLoadMethod} with one parameter on {extension.ammoComp}.");
        else if (extension.ammoSpareMagazines > 0 && (ammoProperty == null || magazineProperty == null))
            Log.Warning($"[Hemophage Warcasket] A dressed quest pawn carries no spare ammunition: no ThingDef property {extension.ammoDefProperty} or no int property {extension.ammoMagazineProperty} on {extension.ammoComp}.");
    }
}
