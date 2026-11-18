# .steamworkshop

Publishing metadata for the mod's Steam Workshop page. Nothing in this folder
ships with the mod (the StageMod manifest never matches it) or is loaded by
RimWorld.

## Description/

One file per language, named after the RimWorld language folders. English is
the source of truth and the only file so far; the others arrive with their
language's first translation pass as machine-assisted first passes pending
native review. Format:

- Line 1: the Workshop title for that language
- Line 2: blank
- Rest: the BBCode description

`English.txt` is a draft until the first release. It is written to the
design, as the page will read once the armor, shoulder pad and helmet
mechanics are in, and none of them is yet; the page describes only what a
release ships (`CLAUDE.md` beside this file), so correct it to the landed
features before the first upload.

Title convention: the English title leans on VFE Pirates' own vocabulary
("warcasket") so players browsing for more sets find the mod; every localized
title must contain the term VFE Pirates' own translation for that language
uses for "warcasket" (the `translate` skill's glossary records it). Titles are
fully localized with no English brand appended: Workshop search is
language-agnostic (any language's title matches regardless of UI language)
and the preview thumbnail already carries the English name. Once the mod has
a settings window, each title must equal that language's
`HPWC_SettingsCategory` Keyed value (the window's header), and the checker
enforces the coupling through `WORKSHOP_TITLE_KEY` in
`Scripts/check-translations.py`; until then the title stands alone.

Player-facing rules from CLAUDE.md apply here too: no em dashes. Numbers on
the page must match the defs; when a tuning change moves one, update this
file in the same commit.

Steam has no API for per-language Workshop text, so updated files are pasted
manually into the Workshop page's edit UI (note Steam's own language names
differ: schinese, koreana, brazilian, latam, ...). The `release` skill diffs
`English.txt` against the last stable release tag and refreshes the translations
whenever it changed.

## Images/

The page's gallery images, numbered in page order as in the sibling repos, and
uploaded by hand through the page's images and videos UI. Only
`1PreviewWorkshop.png` exists so far, the preview art at gallery size; the
rest arrive with the features they show. Steam rejects gallery images of 2 MB
or more and takes no video files, so gameplay clips are GIFs cut down to fit
(lower frame rate, scale and palette) from screen recordings kept outside the
repo.

## Preview.gif

The page's animated thumbnail: `About/Preview.png` with the artist's animated
hero composited in, 640x360, 60 frames at 12 fps, just under the 1 MiB Steam
takes for a preview. `Scripts/preview-gif.py` builds it from the artist's
`Preview.mp4` and the PNG, and its header records the method; the MP4 is
never tracked, in any family repo, and is kept with the gameplay recordings.
Everything outside the hero is the PNG's own pixels, so a redrawn PNG means a
rebuild from the MP4. The `release` skill's preview-only SteamCMD update sets
the GIF on the page after each in-game upload; the game's mod list decodes
only PNG and JPG, so `About/Preview.png` stays a static image. The version
badge is bumped in place by the `gif-patch` skill, on the PNG and the GIF
alike, never by re-exporting.
