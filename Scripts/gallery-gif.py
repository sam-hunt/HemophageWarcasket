#!/usr/bin/env python3
"""Build the hemogen rush gallery GIF (.steamworkshop/Images/3HemogenRush.gif) from its MP4.

    python3 Scripts/gallery-gif.py                      # .steamworkshop/Images/3HemogenRush.mp4 -> .gif beside it
    python3 Scripts/gallery-gif.py --work /tmp/gallery  # keep the intermediates for inspection

The clip is an in-game capture, 432x432, H.264 with no colour tags, 187 frames at 24 fps (7.79 s), 27
of them exact repeats of the frame before. Steam's gallery takes images under 2 MiB, and a clean
encode at the clip's own size and frame rate is 0.93 MiB, so nothing is cut: no resize, no dropped
frame, no lossy LZW. The headroom goes to dithering, which removes the banding a plain remap leaves
in the shield bubble's gradient.

- Colour: decoded as BT.709, as the preview's delivery is. There is no reference image to test it
  against; the choice shows only on the saturated glow (reddest eye pixel 255 30 20, BT.601 243 8 33).
- Palette: one global 256 colours, libimagequant over 16 frames spread across the clip and tiled
  4x4. Fewer frames changes nothing but the worst single pixel.
- Dither: Floyd-Steinberg on every frame, then index coherence: a pixel whose source value is
  bit-identical to the previous frame's keeps the previous frame's index, so the grain never
  repaints a static pixel. Without coherence the grain reshuffles every frame and the encode is
  3.43 MiB; with it, 1.12 MiB.
- Timing: frame i starts at round(i * 100 / 24) cs, so the delays sum to the clip's 779 cs. gifsicle
  -O3 folds each repeated frame into its predecessor's delay: 160 GIF frames.

Measured result: 1,170,620 bytes (56% of the limit), blurred error 0.65/255 against the MP4's frames,
against 1.75 for the plain remap. A band hold (the preview's, holding a colour while the source
stays within B/255 of it) cuts the size to 0.88 MiB at B=1, 0.65 at 2 and 0.42 at 4 for little
error, and is not needed here. `Docs/Research/Gallery-GIF-Transcode.md` has every candidate's
numbers and the levers that did not earn their place.

The build is deterministic: a rerun on the same MP4 is byte-identical. Requires Pillow built with
libimagequant, gifsicle, and ffmpeg (Krita's Windows build through wslpath when none is on PATH).
Nothing here ships.
"""
import argparse, glob, os, shutil, subprocess, sys, tempfile
from PIL import Image, ImageChops

FPS = 24
FRAMES = 187
COLOR_MATRIX = 'bt709'
PALETTE_TILE = 4        # the palette is learned over PALETTE_TILE ** 2 frames, tiled
LIMIT = 2 * 1048576     # Steam's gallery upload limit; the file must be strictly under it
KRITA_FFMPEG = '/mnt/c/Program Files (x86)/Steam/steamapps/common/Krita/krita/bin/ffmpeg.exe'


def log(msg):
    print(f'[gallery-gif] {msg}', flush=True)


def max_channel(im):
    r, g, b = im.split()
    return ImageChops.lighter(ImageChops.lighter(r, g), b)


def extract_frames(mp4, work, ffmpeg):
    out = os.path.join(work, 'frames')
    os.makedirs(out, exist_ok=True)
    if ffmpeg.lower().endswith('.exe'):
        win = lambda p: subprocess.check_output(['wslpath', '-w', p], text=True).strip()
        src, pattern = win(mp4), win(out) + '\\f%03d.png'
    else:
        src, pattern = mp4, os.path.join(out, 'f%03d.png')
    log(f'frames: {os.path.basename(ffmpeg)} -> {out}')
    subprocess.run([ffmpeg, '-v', 'error', '-y', '-i', src, '-an', '-fps_mode', 'passthrough',
                    '-vf', f'scale=in_color_matrix={COLOR_MATRIX}:in_range=tv:out_range=pc,format=rgb24',
                    pattern], check=True)
    frames = [Image.open(p).convert('RGB') for p in sorted(glob.glob(os.path.join(out, 'f*.png')))]
    assert len(frames) == FRAMES, f'{len(frames)} frames, expected {FRAMES}: the clip changed, re-measure it'
    return frames


def palette(frames):
    n, (w, h) = len(frames), frames[0].size
    cells = PALETTE_TILE * PALETTE_TILE
    sample = Image.new('RGB', (w * PALETTE_TILE, h * PALETTE_TILE))
    for k in range(cells):
        sample.paste(frames[round(k * (n - 1) / (cells - 1))], ((k % PALETTE_TILE) * w, (k // PALETTE_TILE) * h))
    pal = sample.quantize(256, method=Image.LIBIMAGEQUANT)
    assert len(pal.getcolors(256)) > 8, 'libimagequant returned a collapsed palette'
    return pal


def encode(frames, pal, out_path, work):
    q = [f.quantize(palette=pal, dither=Image.FLOYDSTEINBERG) for f in frames]
    for i in range(1, len(q)):   # coherence: an unchanged source pixel keeps its index
        same = max_channel(ImageChops.difference(frames[i], frames[i - 1])).point(lambda v: 255 if v == 0 else 0)
        q[i].paste(q[i - 1], (0, 0), same)
    tmp = os.path.join(work, 'gif'); os.makedirs(tmp, exist_ok=True)
    starts = [round(i * 100 / FPS) for i in range(len(q) + 1)]
    args = ['gifsicle', '-O3', '--no-warnings', '--loopcount=forever']
    for i, f in enumerate(q):
        p = os.path.join(tmp, f'f{i:03d}.gif'); f.save(p)
        args += [f'-d{starts[i + 1] - starts[i]}', p]
    subprocess.run(args + ['-o', out_path], check=True)
    return os.path.getsize(out_path)


def main():
    ap = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    ap.add_argument('--mp4', default='.steamworkshop/Images/3HemogenRush.mp4')
    ap.add_argument('--out', help='output GIF (default: the MP4 path with .gif)')
    ap.add_argument('--work', help='intermediates directory (default: a fresh temp dir, removed on success)')
    ap.add_argument('--ffmpeg', default=shutil.which('ffmpeg') or KRITA_FFMPEG)
    a = ap.parse_args()
    out = a.out or os.path.splitext(a.mp4)[0] + '.gif'
    work = a.work or tempfile.mkdtemp(prefix='gallery-gif-')
    os.makedirs(work, exist_ok=True)
    frames = extract_frames(a.mp4, work, a.ffmpeg)
    size = encode(frames, palette(frames), out + '.tmp', work)
    if size >= LIMIT:
        os.remove(out + '.tmp')
        sys.exit(f'[gallery-gif] {size} bytes is over the {LIMIT} byte gallery limit; nothing written')
    os.replace(out + '.tmp', out)
    log(f'wrote {out}: {size} bytes ({size / LIMIT:.1%} of the limit), {len(frames)} source frames')
    if not a.work:
        shutil.rmtree(work)


if __name__ == '__main__':
    main()
