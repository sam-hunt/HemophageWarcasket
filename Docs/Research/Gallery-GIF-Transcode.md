# Workshop gallery GIF: the hemogen rush clip (2026-11-22)

The record of turning `.steamworkshop/Images/3HemogenRush.mp4` into
`.steamworkshop/Images/3HemogenRush.gif`, a gallery image rather than the preview: Steam's
gallery takes uploads under 2 MiB (2,097,152 bytes), against the preview's 1 MiB. `Scripts/gallery-gif.py` rebuilds the
shipped GIF from the MP4, byte-identical, and its header is the compact record of the method;
this file keeps every candidate's measurements. `Preview-GIF-Transcode.md` is the sibling record, and its band hold is
reused here.

## Inputs

| item | value |
|---|---|
| clip | `3HemogenRush.mp4`, 307,023 bytes |
| sha256 | `c15a0a54a05e479ffa27b2c68e59fe487f416234bd02d37c615b4e7b82810e18` |
| stream | H.264 Constrained Baseline, yuv420p, no colour tags, 432x432, 24 fps, 187 frames, 7.79 s |
| content | in-game capture: the pilot in the shield bubble, eye and pad glow pulsing, blood particles over the head |
| tools | Krita's bundled ffmpeg through `wslpath`, Pillow 9.0.1 with libimagequant, gifsicle, no numpy |

## What the clip is

- **Cadence:** a true 24 fps with 27 exact repeats of the previous frame (most in the first
  second, then one every 30 or so). Frame to frame change is small, 0.32/255 mean at most.
  The repeats are kept in the timeline and gifsicle folds each into its predecessor's delay:
  160 GIF frames, delays on a centisecond clock summing to 7.79 s.
- **Motion:** 14% of pixels ever move by more than 8/255 against frame 0, 11% by more than
  16: the particles above the head, the eye and pad glow, and a sheen across the lower left.
  Another 8% move by 2 to 8, which is H.264 noise on static art.
- **Colour matrix: decoded as BT.709**, as the preview's delivery was. Unlike the preview there
  is no PNG to test it against. The choice matters only on the saturated glow: the reddest eye
  pixel reads 255 30 20 as BT.709 and 243 8 33 as BT.601, while the frame's mean shifts by
  under 0.16/255.
- Frame 0 holds 4,218 distinct colours.

## Method

Every frame at the clip's own 432x432 and 24 fps, then:

1. One global 256-colour palette, libimagequant over 16 frames spread across the clip, tiled
   4x4 (Pillow 9.0's binding works at this tile size, 1728x1728).
2. Each frame remapped with Floyd–Steinberg dithering against that palette.
3. **Index coherence:** a pixel whose source value is bit-identical to the previous frame's
   keeps the previous frame's palette index, so dither grain never repaints static pixels.
   This is what makes dithering affordable: without it the grain reshuffles every frame and the
   encode is 3.43 MiB.
4. `gifsicle -O3 --loopcount=forever`, delays from `round(i * 100 / 24)` differences.

No resize, no frame drop, no hold band, no lossy LZW: a clean encode already fits with half the
limit to spare, so the headroom went to dithering instead.

## Candidates

Error is against the decoded MP4 frames, mean RGB /255: "rms" raw, "blur" after a 1.5 px
Gaussian on both sides (what dither grain reads as at viewing distance), and "worst" the largest
single-pixel channel difference over all frames. The worst figure is isolated dither pixels on
hard edges in every dithered candidate, not a missing colour.

| candidate | dither | hold band | bytes | of limit | rms | blur | worst |
|---|---|---|---|---|---|---|---|
| clean | none | 0 | 973,245 | 46% | 2.06 | 1.75 | 52 |
| **dithered (shipped)** | Floyd–Steinberg | 0 | **1,170,620** | **56%** | 1.62 | **0.65** | 67 |
| dithered, band 1 | Floyd–Steinberg | 1 | 923,857 | 44% | 1.64 | 0.68 | 67 |
| dithered, band 2 | Floyd–Steinberg | 2 | 676,754 | 32% | 1.68 | 0.70 | 67 |
| dithered, band 4 | Floyd–Steinberg | 4 | 440,755 | 21% | 1.74 | 0.73 | 67 |
| ordered | Bayer, scale 2 | 0 | 956,271 | 46% | 5.04 | 2.12 | 46 |

The bands are the preview's band hold: a pixel holds one colour while the source stays within
the band of it, which removes the H.264 shimmer and takes 35% to 42% off the size per two steps, for
a blurred error that barely moves. Everything is at 432x432 and 24 fps.

## Levers that did not earn their place

| lever | result |
|---|---|
| palette over 4 frames (2x2 tile) instead of 16 | 0.3% smaller, blurred error identical, worst 74 against 67 |
| palette over 9 frames (3x3 tile) | no better than 16 on any measure |
| ffmpeg `paletteuse` Bayer scale 3, same palette | 926,472 bytes, blur 2.08: Bayer at scales 2 and 3 is worse than diffusion here |
| ffmpeg `paletteuse` sierra2_4a, same palette | 901,253 bytes, blur 1.70, worst 89: no better than no dither |
| Floyd–Steinberg without index coherence | 3,596,276 bytes, over the limit |
| downscale, frame drop, lossy LZW | not tried: nothing needed to be saved |

## Validation

- 1,170,620 bytes, sha256 `9f63432068c6334804ae6a7415de749188086afa209b02ae4a9b097a2a40a1ed`;
  `gifsicle --info`: 432x432, global colour table of 256, loop forever, 160 frames, delays
  summing to 7.79 s.
- A rerun of `Scripts/gallery-gif.py` from the MP4, extraction included, is byte-identical.
- The author chose it in a browser from the candidates above, side by side with the MP4.
