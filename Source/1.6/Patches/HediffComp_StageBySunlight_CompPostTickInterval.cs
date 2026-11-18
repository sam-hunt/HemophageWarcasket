using System.Reflection;
using HarmonyLib;
using VEF.AnimalBehaviours;
using Verse;

namespace HemophageWarcasket.Patches;

// Holds VEF's sunlight-staged hediff at its sunless stage for a pawn the helmet shields
// (SunlightShield has the mechanic). Vanilla Races Expanded - Sanguophage hangs one on
// VRE_Sensitivity_Dangerous, hidden, to cap Sight at 50% in sunlight; VEF is always loaded
// (VFEP depends on it) and this reads no def of that mod's, so it needs no compat root.
//
// The comp sets its hediff's severity to sunlightStageIndex or sunlessStageIndex every 500
// ticks. The postfix runs on every call and leaves on one float comparison while the hediff
// sits at its sunless stage. Off it, the postfix walks the worn apparel: once per change the
// comp makes for a shielded pilot, who is put straight back, and on every call for an
// unshielded carrier standing in sunlight.
//
// The target is found by name, and a miss skips this patch with a warning: a typed target
// that VEF has since renamed would throw out of PatchAll and take every other patch and the
// startup work with it. The comp has no static state, so patching it runs nothing of VEF's
// (ModInit has the timing rule). Verified on VEF's 1.6 and 1.7 builds.
[HarmonyPatch]
public static class HediffComp_StageBySunlight_CompPostTickInterval
{
    private const string CompTypeName = "VEF.AnimalBehaviours.HediffComp_StageBySunlight";
    private const string PropsTypeName = "VEF.AnimalBehaviours.HediffCompProperties_StageBySunlight";

    private static MethodBase target;

    public static bool Prepare()
    {
        target ??= AccessTools.Method(AccessTools.TypeByName(CompTypeName), "CompPostTickInterval");
        bool found = target != null && AccessTools.Field(AccessTools.TypeByName(PropsTypeName), "sunlessStageIndex") != null;
        if (!found)
            Log.Warning("[Hemophage Warcasket] Vanilla Expanded Framework's sunlight hediff comp was not found; the helmet will not lift a Sight cap that comp applies in sunlight.");
        return found;
    }

    public static MethodBase TargetMethod() => target;

    public static void Postfix(HediffComp __instance)
    {
        float sunless = ((HediffCompProperties_StageBySunlight)__instance.props).sunlessStageIndex;
        Hediff hediff = __instance.parent;
        if (hediff.Severity != sunless && SunlightShield.Protects(hediff.pawn))
            hediff.Severity = sunless;
    }
}
