# TODOs

## Features

- **Tuning pass** The three pieces are the 10th generation's baseline (VFEP's Sarcophagus row less its medicine and bionics; the def headers argue each number), but for the pads' cost, which pays for Dismember. Settle the set's own stats and costs as the mechanics land, then mirror them into the CE patch.

## Testing

- **Shoulders**
  - **Dismember**
    - should behead on a cobra, not a termite
    - A limb that holds: a centipede's last ring is only wounded ("Will only wound" beside the cursor, a "could not tear it off" line in the combat log, the strike spent) and comes off for a pilot in the hemogen rush. A thrumbo's leg takes three strikes in the rush.
    - The drain after a limb that held: runs on a human who is bleeding from that wound or any other, and not on one who is not (a Tough pilot's target with the robust gene, tended first).
    - A torn off bionic arm on a target with no other wound: no drain.
    - The strike damage slider: at its lowest a human limb holds, at its highest a thrumbo's leg comes off.
    - With Combat Extended: the settings default reads 80. A colonist pilot with Melee 8 set to aim low loses the target a leg, to middle an arm, and to high either. CE's "Reload worn armor" button loads a strike from carried components.
    - The drain, for a hemogenic pilot on a human: the 4 second bar with Bloodfeed's sound and splash.
    - The drain on a kill: a raider who dies to the strike (beheaded, or killed as the lost limb downs them) is drained as a corpse, with the bar over the corpse, the full hemogen gained and "Blood loss" on the corpse's health tab. A target shot dead during the drain is drained on the same way, and one who bleeds to death during it ends the drain there.
    - A bloodfeeder target: struck, never drained, as Bloodfeed refuses one.
    - The drain warning beside the cursor, for a hemogenic pilot with room for hemogen: nothing over an unhurt human, "Will cause major blood loss" over one at 16% blood loss or more and "Will kill" at 56% or more (dev mode: add the BloodLoss hediff and set its severity), under "Will only wound" when both apply and above "Will anger". No such line for a pilot without the gene or at full hemogen, over a bloodfeeder, over a deathresting target that is not bleeding, or over an unhurt target whose limbs are all bionic.
    - With Vanilla Races Expanded - Sanguophage:
      - An animalfeeder pilot (a strigoi) drains an animal and gets "consumed animal hemogen". A rat bleeds to death during the drain, an elephant loses about a tenth of its blood. A pilot without the gene only strikes the animal.
      - The same pilot's cursor reads "Will kill" over a rat or a squirrel, "Will cause major blood loss" over a husky and nothing over an elephant, and a pilot without the gene sees none of the three.
      - A sanguofeeder pilot (a bruxa) drains a sanguophage and gets "consumed sanguophage hemogen". Any pilot's drain of an ordinary human removes whichever of the three "consumed ... hemogen" hediffs the pilot had.
      - A sanguofeeder pilot on a sanguophage under 10 hemogen (dev mode: a colonist sanguophage's "DEV: hemogen -10" button): struck, never drained, and no "consumed sanguophage hemogen". One a little over 10 is drained until it falls under 10, and the bar stops short.
      - Under a blood moon (dev mode: the VRE_BloodMoon incident, pilot under 45 hemogen), an aggressive pilot who becomes a hemohunter walks to a non-hemogenic human and dismembers them, spending a strike, then drains. With the strikes spent they bite as VRES has them. A dead calm pilot (a bruxa) only ever bites, as does any pilot with "Hemohunters dismember targets" off.
      - The settings window shows "Hemohunters dismember targets" under Dismember, and without the mod it does not.
    - A non-hostile target: "Will anger" and the faction's name beside the cursor over a visitor or a trader's pack animal, under "Will kill" when both apply, and no such line over a colonist, a wild animal or a berserk visitor. The goodwill cost on a visitor's faction (78 or more, 1.3 a point of the strike's 60, which turns a neutral faction hostile), guilt for a prisoner or colonist.
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
- **Workshop gallery** Add images, Medic/Overlord as precendent
- **Optimize images** Run the `../MOD_DOWNLOAD_SIZE_TRIM.md` pass before the first release.
- **Smoke test on 1.6**
- **Full review of player-facing text**
  - The three descriptions, `About.xml`, the README's features and the Workshop page are first cuts written to the design ahead of the code. Check each against what is built. No release, candidates included, before the two agree.
  - Dismember's description and the settings summary say the drain is for a human target. A bloodfeeder is now refused, which Bloodfeed's own description words as "Can only target non-hemogenic humans". A changed description needs the sidecar regenerated (game closed).
- **Initial translation pass** of all 9 planned languages
- **First Workshop publish.** Commit `About/PublishedFileId.txt`, add the Workshop link and the Steam badges to the README
