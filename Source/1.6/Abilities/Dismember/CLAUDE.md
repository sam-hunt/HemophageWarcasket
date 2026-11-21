# Source/1.6/Abilities/Dismember/: the pads' Dismember

Loaded when work touches the pads' ability. Each file's header is the record of its own part;
this file says which header to read and holds the rules that cross these files. Outside this
folder the feature is its defs (`1.6/Defs/AbilityDefs/Dismember.xml` has the values and their
rationale, with `DamageDefs/`, `JobDefs/`, `RulePackDefs/` and `ThinkTreeDefs/` beside it), the
strike pool (`Comps/CompStrikePool.cs`) and its settings section
(`Settings/Settings_Dismember.cs`).

`Ability_Dismember` is one class in five files, each named for what it answers:

- **`Ability_Dismember.cs`**: the button and when it is enabled, who can be targeted, how a
  click becomes a job, the question before a lethal order, the hold on the target, the strike
  pool and the catch-up after a settings write.
- **`Ability_Dismember.Strike.cs`**: the damage and what scales it, armor and Combat Extended,
  the combat log line, the strike on a corpse and the blood it throws.
- **`Ability_Dismember.Foresight.cs`**: the cursor's lines, which say what the strike and the
  drain would do and which faction they would anger, and how long an answer is kept.
- **`Ability_Dismember.Drain.cs`**: who is drained and through which source, when a drain runs
  and stops, and the drain on a corpse.
- **`Ability_Dismember.Hunt.cs`**: when a hemohunter pilot strikes and what it may hunt.

Around it:

- **`DismemberTarget.cs`**: which part a strike takes hold of and whether it comes off.
- **`DismemberDrain.cs`**: the blood loss arithmetic behind the drain's warning.
- **`DismemberExtension.cs`, `DismemberDrainSource.cs`**: the def fields. `CompatPatchTests`
  reads both files by path for their public fields, so a move or rename changes the test.
- **`MeleeAim.cs`**: another mod's melee aim height, read by reflection.
- **`JobDriver_Dismember.cs`**: the toils, the binding to VEF's comp and the drain's following
  a target into death.
- **`JobGiver_HemohunterDismember.cs`**: the search for a hemohunter's prey.
- **`Command_Dismember.cs`**: the strike count on the button.

## Rules

- **A rule that can be decided from a body, a def or a number goes in a static class**, as
  `DismemberTarget` and `DismemberDrain` have theirs, and the ability only calls it. The
  headless suite builds no pawn, so those two are what `DismemberTargetTests` and
  `DismemberDrainTests` can reach; a rule written on the ability is checked by hand, from
  `TODOs.md`.
- **A new member of `Ability_Dismember` goes in the file whose header explains it**, and a
  comment that sends the reader to a header names the file when the header is another one's.
- **A walk-up ability is ordered as a vanilla job and bound in its driver.** `Ability_Dismember`
  and `JobDriver_Dismember` are the model, after the Medic repo's: the job goes through
  `TryTakeOrderedJob` so it interrupts and queues as an order does, and the driver writes
  `CompAbilities.currentlyCasting` and the ability's `pawn` when the job starts. A think tree
  gives the same job by making it directly (`JobGiver_HemohunterDismember`), setting the
  ability's `pawn` itself before it asks the ability anything.
- **A stocked ability's charges are a vanilla `CompApparelReloadable` on the piece that grants
  it**, in per-charge mode and without verbs, read off the ability's `holder`; the def's
  `gizmoClass` names a command that returns the count as `TopRightLabel`. The pads' pool,
  `Ability_Dismember` and `Command_Dismember` are the model, after the Medic repo's shot. A
  capacity the settings can lower needs the comp subclassed to stay within it
  (`Comps/CompStrikePool.cs`): vanilla's reload throws on a count over the maximum.
