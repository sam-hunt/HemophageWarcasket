# TODOs

## Features

- **Dismember** damage amount, something more realistic/smaller perhaps more appropriate? what could/should happen if it fails to dismember?
- **Combat Extended aim height** CE keeps a per-pawn melee aim height (`CompMeleeTargettingGizmo.heightInt`, public). With CE active the dismember strike could take a leg for low and an arm for middle instead of a random limb. Belongs with the CE compat scan.
- **VRE Sanguophage Integration: Drain warning** Bloodfeed and VRES's feeding abilities warn "Will kill" or of serious blood loss beside the cursor when the bite would bleed the target that far. Dismember warns only of a beheading, and an animalfeeder's drain bleeds any small animal dry.
- **Tuning pass** The three pieces are the 10th generation's baseline (VFEP's Sarcophagus row less its medicine and bionics; the def headers argue each number), but for the pads' cost, which pays for Dismember. Settle the set's own stats and costs as the mechanics land, then mirror them into the CE patch.

## Testing

- **Shoulders**
  - **Dismember**
    - should behead on a cobra, not a termite, 3 strikes on centipede
    - The drain, for a hemogenic pilot on a human: the 4 second bar with Bloodfeed's sound and splash.
    - The drain on a kill: a raider who dies to the strike (beheaded, or killed as the lost limb downs them) is drained as a corpse, with the bar over the corpse, the full hemogen gained and "Blood loss" on the corpse's health tab. A target shot dead during the drain is drained on the same way, and one who bleeds to death during it ends the drain there.
    - A bloodfeeder target: struck, never drained, as Bloodfeed refuses one.
    - With Vanilla Races Expanded - Sanguophage:
      - An animalfeeder pilot (a strigoi) drains an animal and gets "consumed animal hemogen". A rat bleeds to death during the drain, an elephant loses about a tenth of its blood. A pilot without the gene only strikes the animal.
      - A sanguofeeder pilot (a bruxa) drains a sanguophage and gets "consumed sanguophage hemogen". Any pilot's drain of an ordinary human removes whichever of the three "consumed ... hemogen" hediffs the pilot had.
      - A sanguofeeder pilot on a sanguophage under 10 hemogen (dev mode: a colonist sanguophage's "DEV: hemogen -10" button): struck, never drained, and no "consumed sanguophage hemogen". One a little over 10 is drained until it falls under 10, and the bar stops short.
      - Under a blood moon (dev mode: the VRE_BloodMoon incident, pilot under 45 hemogen), an aggressive pilot who becomes a hemohunter walks to a non-hemogenic human and dismembers them, spending a strike, then drains. With the strikes spent they bite as VRES has them. A dead calm pilot (a bruxa) only ever bites, as does any pilot with "Hemohunters dismember targets" off.
      - The settings window shows "Hemohunters dismember targets" under Dismember, and without the mod it does not.
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
- **Work trait and its settings** With "Warcaskets can work!" or Hardworking Warcaskets active, the section reads that nothing is blocked.

## Pre-release polish

- **Chinese localizations** How does hemogen sit in vanilla?
- **CE compat scan for any relevant changes they make to sanguophage/hemogenic mechanics** The stat patch is checked against CE 16.7.4 and leaves these open:
  - The pads' strike pool is a `CompApparelReloadable`, which CE's prefix on the reload job and its "Reload worn armor" button both reach (the Overlord repo's `1.6/Mods/CombatExtended/CLAUDE.md` has what each does).
  - Dismember's armorless damage and the rush's stat factors under CE's melee and armor rules.
  - CE's own patch folders for Biotech and for Vanilla Races Expanded - Sanguophage.
- **Workshop gallery** Add images, Medic/Overlord as precendent
- **Optimize images** Run the `../MOD_DOWNLOAD_SIZE_TRIM.md` pass before the first release.
- **Smoke test on 1.6**
- **Full review of player-facing text**
  - The three descriptions, `About.xml`, the README's features and the Workshop page are first cuts written to the design ahead of the code. Check each against what is built. No release, candidates included, before the two agree.
  - Dismember's description and the settings summary say the drain is for a human target. A bloodfeeder is now refused, which Bloodfeed's own description words as "Can only target non-hemogenic humans". A changed description needs the sidecar regenerated (game closed).
- **Initial translation pass** of all 9 planned languages
- **First Workshop publish.** Commit `About/PublishedFileId.txt`, add the Workshop link and the Steam badges to the README
