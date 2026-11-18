# Animated Workshop preview: the Hemophage run (2026-11-18)

The record of turning the artist's animated `Preview.mp4` into `.steamworkshop/Preview.gif`.
The process is the family guide's, `~/dev/PREVIEW_GIF_FROM_MP4.md`; `Scripts/preview-gif.py`'s
header is the compact record of the method. This file keeps what was measured on this
delivery, so a rerun, a new delivery or a sibling's run can start from the numbers.

## Inputs

| item | value |
|---|---|
| delivery | `Preview.mp4`, 9,525,648 bytes, received 2026-11-16, never tracked and not kept in the repo folder |
| sha256 | `8f6ba38806aa71e2ac86514fd3ddb8b70a51d7a11acda1bc3c92ca5d2f9adeac` |
| stream | H.264 Main, yuv420p, no colour tags, 1920x1080, 15.4 Mb/s, 60 frames at 12 fps, AAC audio (ignored) |
| PNG | `About/Preview.png`, 640x360, 26,495 colours, badge already "1.7" |
| tools | Krita's bundled ffmpeg through `wslpath`, Pillow 9.0.1 with libimagequant, gifsicle, no numpy |

A rebuild needs the MP4 back at `.steamworkshop/Preview.mp4` (or `--mp4`). With the same MP4
and PNG the script's output is byte-identical to the committed GIF.

## What the delivery is

- **Cadence:** a true 12 fps, no padding (the siblings' were padded to 29.97 or 30). Frame to
  frame change 0.68 to 3.40 /255, the loop's closing step 2.46, and no shorter period, so all
  60 frames are kept. The script asserts the frame count and that no frame repeats.
- **Colour matrix: BT.709.** Mean RGB of static boxes at 640x360:

  | box | PNG | BT.601 | BT.709 |
  |---|---|---|---|
  | planet limb (40,200)-(110,352) | 75.7 44.7 53.0 | 72.8 39.8 52.5 | 75.4 43.7 52.5 |
  | left background (0,110)-(60,352) | 39.6 23.0 27.0 | 38.8 21.1 27.5 | 40.2 23.2 27.5 |

- **Tones need no curve.** Overlord's per-channel curve, fitted here, left the static mismatch
  where it was (9.6 to 9.9 /255): the mismatch is the nudged layers and the upscale's
  softness, not tone. Blurred, the planet is within 0.4/255 of the PNG, which the script
  asserts in place of the fit.
- **Geometry: 3.0x everywhere, each layer nudged.** Edge-map search, offsets in video px:

  | layer | scale | offset | edge-map mean, at the fit / at 3.0x offset 0 |
  |---|---|---|---|
  | title | 3.01 | -3.0, -0.5 | 2.23 / 3.16 |
  | hex icon | 3.0 | +6.0, -0.5 | 3.28 / 6.39 |
  | badge | 3.04 | -7.5, -1.25 | 3.23 / 4.35 |
  | planet | 3.0 | -3.25, +1.5 | 0.75 / 1.63 |
  | stars | 3.0 | +1.0, -1.0 | 0.38 / 0.44 |
  | hero | 3.0 | +4.5, +3.25 | 1.75 / 4.04 |

  The guide reads disagreeing layers as a re-layout. Here the disagreement is 1 to 2 PNG px
  on layers that never move and that the motion never reaches, so a composite never shows
  it. The deciding test is the script's `PROTECT` assert: no motion over the badge, the title
  or the hex icon. The closest approaches are a shoulder spike 1 px right of the title's box
  and a horn 1 px below the icon's.
- **Motion:** the hero and the sword, with blood running off the blade. 28% of pixels change
  by more than 12/255 against frame 0, and 21% by more than 40. The built mask is 45% of the
  frame, since dilation and the 12,361 filled hole pixels add static area that costs nothing.

## Method

The Overlord composite: the PNG everywhere, the video's hero inside the mask, so the
background, title, icon and badge are the PNG's own pixels. Outside the mask the GIF never
changes after frame 0 and sits 1.3/255 (mean, dithered) from the PNG; the video's own framing
would sit 7.3 away, and its badge would read 1.6.

Dropped from Overlord's script: the frame dedup (nothing is padded), the tone curve, and the
temporal denoise (a 2% saving once the hold below is in). Added: the explicit colour matrix,
the `PROTECT` boxes, and the hold.

## The hold: a band where the siblings use a gate

A clean encode is 2.62 MiB, so later frames may repaint only some pixels. Both schemes let a
pixel keep a stale colour and differ in when it moves and to what.

- **Gate (Overlord, Toxin, UMW):** a pixel keeps its colour until the truth is more than GATE
  from it, then snaps to the truth. On a steady drift the colour is exact at the start of a
  run and GATE behind at the end, so a run spans GATE of drift.
- **Band (here):** a run lasts while the truth's spread over it is at most 2 x BAND, and shows
  one colour inside that spread, so the error is at most BAND either side. A run spans twice
  the drift for the same worst error, which halves the repaints. The runs are found by
  looking ahead over the whole loop, which a gate, deciding frame by frame, cannot do. Within
  a run the colour is the run's mean, clamped to stay within BAND of every frame (same size
  as the midpoint, 9% lower rms).

Measured on the same composite frames, decoded GIF against the clean frames inside the mask,
max channel /255. The palette alone accounts for 3.5 rms and 32 worst.

| candidate | bytes | of limit | hero error rms / worst |
|---|---|---|---|
| clean | 2,743,144 | 262% | 3.5 / 32 |
| band 11 (shipped) | 1,033,780 | 98.6% | 6.1 / 42 |
| band 8 to 18, by local contrast | 1,025,410 | 97.8% | 6.3 / 47 |
| band 9 + `gifsicle --lossy=20` | 995,758 | 95.0% | 6.4 / 42, background 2.2 |
| gate 20 | 1,021,626 | 97.4% | 7.9 / 49 |

Band sweep: 6 1.38 MiB, 8 1.19, 9 1.11, 10 1.05, 11 0.986, 12 0.93. Gate sweep: 17 1.08,
19 1.007, 21 0.94, 23 0.89. Frame 0 alone is 121 KB.

Nothing in the band depends on this art or its palette: it replaces the gate step in any of
the sibling scripts, composite or video framing, given the sibling's MP4.

## Levers that did not earn their place

| lever | result |
|---|---|
| temporal denoise before the hold | 2% smaller, worst error up to 16 at band 10 |
| downscale filter, against Hamming | bilinear 3% smaller and softer; box +3%, bicubic +4%, Lanczos +10% |
| band widened on hard edges (8 to 18 by 3x3 luma range) | fits, no better than the uniform band on any measure |
| lossy LZW | buys two band steps, costs more error than it saves and degrades the frozen background |

## Validation

- 1,033,780 bytes; `gifsicle --info`: 640x360, global colour table of 256, loop forever, 60
  frames, delays summing to 5.00 s.
- Outside the mask, every frame equals frame 0 (max change 0).
- `gif-patch bump --from 1.7 --to 1.7` finds the badge (rms 0.021), changes nothing outside
  it, and writes 59 bytes fewer.
- A rerun from the MP4, extraction included, is byte-identical.
- The author chose it in a browser from the candidates above, side by side with the clean
  encode.
