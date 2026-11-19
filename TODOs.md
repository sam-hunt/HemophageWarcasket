# TODOs

## Features

- **Shoulders**
  - **Combat Extended aim height** CE keeps a per-pawn melee aim height (`CompMeleeTargettingGizmo.heightInt`, public). With CE active the strike could take a leg for low and an arm for middle instead of a random limb. Belongs with the CE compat scan.
- **Tuning** The three pieces are the 10th generation's baseline (VFEP's Sarcophagus row less its medicine and bionics; the def headers argue each number), but for the pads' cost, which pays for Dismember. Settle the set's own stats and costs as the mechanics land, then mirror them into the CE patch.
- **VRE Sanguophage Integration**
  - Dismember hemogen drain gating widens when the caster has AnimalFeeder/CorpseFeeder/Sanguofeeder and the target matches, and any associated buffs are applied
  - Hemohunter could allow autocasting dismember instead of bloodfeed if the pilot has >= the Aggressive gene? (default sanguophages do, VRE Hussars/Archons have even higher severity variant genes iirc)
  - Does VRES add any research our set might be better gated behind? Vanilla's own Deathrest doesn't quite fit, because the genes are orthoganal
  - Blood moon event could trigger hemogenic rush for free? investigate balance/flavor before committing?
  - Add a compatibility note to the workshop description
- How does hemogen sit in Chinese localizations in vanilla?

## Testing

- **Torso**
  - **Work trait and its settings** With "Warcaskets can work!" or Hardworking Warcaskets active, the section reads that nothing is blocked.
- **Shoulders**
  - **Dismember**
    - should behead on a cobra, not a termite, 3 strikes on centipede
    - The drain, for a hemogenic pilot on a human: the 4 second bar with Bloodfeed's sound and splash.
    - The drain on a kill: a raider who dies to the strike (beheaded, or killed as the lost limb downs them) is drained as a corpse, with the bar over the corpse, the full hemogen gained and "Blood loss" on the corpse's health tab. A target shot dead during the drain is drained on the same way, and one who bleeds to death during it ends the drain there.
    - A non-hostile target: "Will anger" and the faction's name beside the cursor over a visitor or a trader's pack animal, under "Will kill" when both apply, and no such line over a colonist, a wild animal or a berserk visitor. The goodwill cost on a visitor's faction (130 or more, vanilla's maximum for one hit, which turns a neutral faction hostile), guilt for a prisoner or colonist.
- **Helmet**
  - **Sunlight protection**
    - A sanguophage pilot outdoors by day: no "sunlight sensitivity" thought, the Move Speed breakdown has no "in sunlight" line, and both are there on a sanguophage without the helmet standing beside them.
    - With Vanilla Races Expanded - Sanguophage, a pilot with dangerous UV sensitivity: Sight is not capped at 50% in sunlight, within about 8 seconds of stepping out.
    - With "Sunlight protection" off in the settings, the thought and the slowdown come back when the window closes.
  - **Fire**
    - The foundry asks for 30, 50 and 25 devilstrand (helmet, armor, shoulders) and 70 plasteel for the helmet, and the info cards read Heat 200%, 150%, 150% and the helmet's Blunt 150%.
    - A burning sanguophage pilot takes no burns to the head and visibly fewer elsewhere than one without the set.
    - A pyrophobic pilot near a fire still gets the pyrophobia thought and flees about half as often, and "Fire terror chance" at 0% stops the fleeing while 100% restores vanilla's.

- **Raider gear** VFEP's veteran raiders and hired veterans never arrive in a Hemophage piece (dev mode, a few Junker and Mercenary raids at high points), the log has no `[Hemophage Warcasket]` warning at startup, and the foundry still lists all three pieces.

## Pre-release polish

- **CE compat scan for any relevant changes they make to sanguophage/hemogenic mechanics** The stat patch is checked against CE 16.7.4 and leaves these open:
  - The pads' strike pool is a `CompApparelReloadable`, which CE's prefix on the reload job and its "Reload worn armor" button both reach (the Overlord repo's `1.6/Mods/CombatExtended/CLAUDE.md` has what each does).
  - Dismember's armorless damage and the rush's stat factors under CE's melee and armor rules.
  - CE's own patch folders for Biotech and for Vanilla Races Expanded - Sanguophage.
- **Workshop gallery** Add images, Medic/Overlord as precendent
- **Optimize images** Run the `../MOD_DOWNLOAD_SIZE_TRIM.md` pass before the first release.
- **Smoke test on 1.6**
- **Full review of player-facing text**
  - The three descriptions, `About.xml`, the README's features and the Workshop page are first cuts written to the design ahead of the code. Check each against what is built. No release, candidates included, before the two agree.
- **Initial translation pass** of all 9 planned languages
- **First Workshop publish.** Commit `About/PublishedFileId.txt`, add the Workshop link and the Steam badges to the README
