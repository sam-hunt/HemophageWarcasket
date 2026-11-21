using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace HemophageWarcasket;

// Ability_Dismember's strike, on a living target and on a corpse found lying
// (Ability_Dismember.cs has the ability as a whole).
//
// The strike is one DamageInfo at the chosen part with the pilot as instigator, which gives
// guilt, faction goodwill, kill blame and records through TakeDamage (1.6). Its amount is the
// extension's damage times the pilot's MeleeDamageFactor, the stat every vanilla melee verb
// multiplies its own by (VerbProperties.GetDamageFactorFor), so the hemogen rush and a melee
// damage gene strengthen the strike as they do a blow, with none of a blow's random spread:
// what a strike will do is known before the click. Armor is ignored, as the strike wrenches
// the joint and cuts through nothing: IgnoreArmor skips the armor step in
// DamageWorker_AddInjury.ApplyDamageToPart and worn apparel's CheckPreAbsorbDamage in
// Pawn_HealthTracker.PreApplyDamage. The target's IncomingDamageFactor still scales it there
// (Tough, Biotech's robust gene), after any factor a gene or hediff of the target's sets on
// the damage def itself (Pawn.PreApplyDamage). Damage propagation off sends what is left
// straight to FinalizeAndAddInjury, skipping ReduceDamageToPreserveOutsideParts, as one injury
// on the one part. When the part's health less that injury rounds to nothing,
// HediffSet.AddDirect turns it into a fresh Hediff_MissingPart (GetPartHealth rounds;
// DismemberTarget.Tears is the same test); otherwise the part stays on, wounded, and the
// strike is spent all the same. Instant kill protection is ignored, or it would spare a
// colonist's head after the cursor said the strike would kill. All the same on 1.6 and 1.7.
//
// Combat Extended (16.7.4) leaves the strike as it is: its transpiler on ApplyDamageToPart
// replaces the armor call inside the branch IgnoreArmor skips, its ArmorUtilityCE returns at
// once for a damage def with no armorCategory, and its other patches on the worker are on the
// propagation the strike turns off. What it changes is the health of the parts
// (1.6/Mods/CombatExtended/Patches/Dismember.xml).
//
// TakeDamage writes no combat log line, so the strike writes its own the way
// Verb_MeleeAttack.TryCastShot does (1.6; the entry's constructor and BattleLog.Add are the same
// on 1.7): a BattleLogEntry_MeleeCombat on our rule pack goes into the battle log before the
// damage, so the downed or death line the damage causes follows it, and takes the part and
// whether it was destroyed from the damage result afterward, which the rule pack's lines
// choose between. The log entry def is required, since the entry reads its icons from it. A
// strike that reached no part takes its entry back out, as the rule pack has no line without
// one.
//
// A corpse found lying, which only a hemohunter's hunt comes to (Ability_Dismember.Hunt.cs), is
// struck another way. Such a strike is no attack, so it goes nowhere near TakeDamage, whose
// bookkeeping is an attack's: it would make the pilot guilty over a dead colonist as over a
// living one (Pawn_HealthTracker.PreApplyDamage, 1.6). It takes the part off as vanilla does
// when a corpse is eaten, a fresh Hediff_MissingPart added to the pawn in the corpse
// (Corpse.IngestedCalculateAmounts, 1.6 and 1.7), under the damage def's own injury so the part
// reads as torn off, when the strike's damage would tear it off a body that does nothing to
// lessen it. It writes no combat log line and throws the same blood.
public partial class Ability_Dismember
{
    // The strike before it is aimed at a part: the def's damage by the pilot's melee damage
    // factor, armor and instant kill protection ignored and no propagation (see the header).
    private DamageInfo StrikeInfo()
    {
        float amount = Extension.damage * (pawn?.GetStatValue(StatDefOf.MeleeDamageFactor) ?? 1f);
        var dinfo = new DamageInfo(HPWC_DefOf.HPWC_Dismember, amount, instigator: pawn);
        dinfo.SetIgnoreArmor(true);
        dinfo.SetAllowDamagePropagation(false);
        dinfo.SetIgnoreInstantKillProtection(true);
        return dinfo;
    }

    // base.Cast sets the cooldown and plays the def's cast effects.
    public override void Cast(params GlobalTargetInfo[] targets)
    {
        base.Cast(targets);
        Thing target = targets.Length == 0 ? null : targets[0].Thing;
        if (target is not (Pawn or Corpse))
            return;
        Pool?.UsedOnce();
        if (target is Corpse corpse)
            Strike(corpse);
        else
            Strike((Pawn)target);
    }

    private void Strike(Pawn target)
    {
        if (target.Dead || !target.Spawned)
            return;
        BodyPartRecord part = DismemberTarget.Choose(target, Extension, MeleeAim.HeightFor(pawn, Extension), out _);
        if (part == null)
            return;
        DamageInfo dinfo = StrikeInfo();
        dinfo.SetHitPart(part);
        var log = new BattleLogEntry_MeleeCombat(HPWC_DefOf.HPWC_Combat_Dismember, alwaysShowInCompact: true,
            pawn, target, implementType: null, toolLabel: null, def: LogEntryDefOf.MeleeAttack);
        Find.BattleLog.Add(log);
        IntVec3 center = target.Position;
        Map map = target.Map;
        DamageWorker.DamageResult result = target.TakeDamage(dinfo);
        if (result.parts.NullOrEmpty())
            Find.BattleLog.RemoveEntry(log);
        else
            result.AssociateWithLog(log);
        if (target.Dead || target.health.hediffSet.PartIsMissing(part))
            Spatter(target, center, map);
    }

    // The strike on a corpse found lying (see the header).
    private void Strike(Corpse corpse)
    {
        Pawn dead = corpse.InnerPawn;
        if (dead == null || !corpse.Spawned)
            return;
        BodyPartRecord part = DismemberTarget.Choose(dead, Extension, MeleeAim.HeightFor(pawn, Extension), out _);
        if (part == null || !DismemberTarget.Tears(dead.health.hediffSet.GetPartHealth(part), StrikeInfo().Amount))
            return;
        var missing = (Hediff_MissingPart)HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, dead, part);
        missing.lastInjury = HealthUtility.GetHediffDefFromDamage(HPWC_DefOf.HPWC_Dismember, dead, part);
        missing.IsFresh = true;
        dead.health.AddHediff(missing);
        Spatter(dead, corpse.Position, corpse.Map);
    }

    // Reused across casts; filled and read within one Spatter call.
    private static readonly List<IntVec3> SpatterCells = [];

    // What a torn limb or a kill throws: the extension's blood filth around where the target
    // stood, the target's own blood
    // as Pawn_HealthTracker.DropBloodFilth picks it (1.6), so a bloodless race throws none.
    // TryMakeFilth reads its cell before any bounds check, hence the InBounds filter; the line
    // of sight keeps the spatter off the far side of a wall. FilthMaker and GenSight are the
    // same on 1.7.
    private void Spatter(Pawn target, IntVec3 center, Map map)
    {
        DismemberExtension ext = Extension;
        if (ext == null || ext.bloodFilthCount <= 0 || map == null)
            return;
        ThingDef blood = target.IsMutant ? target.mutant.Def.bloodDef ?? target.RaceProps.BloodDef : target.RaceProps.BloodDef;
        if (blood == null)
            return;
        SpatterCells.Clear();
        foreach (IntVec3 cell in GenRadial.RadialCellsAround(center, ext.bloodFilthRadius, useCenter: true))
        {
            if (cell.InBounds(map) && GenSight.LineOfSight(center, cell, map))
                SpatterCells.Add(cell);
        }
        string source = target.LabelIndefinite();
        for (int i = 0; i < ext.bloodFilthCount && SpatterCells.Count > 0; i++)
            FilthMaker.TryMakeFilth(SpatterCells.RandomElement(), map, blood, source);
        SpatterCells.Clear();
    }
}
