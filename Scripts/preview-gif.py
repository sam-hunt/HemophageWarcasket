#!/usr/bin/env python3
"""Build the animated Workshop preview (.steamworkshop/Preview.gif) from the artist's Preview.mp4.

    python3 Scripts/preview-gif.py                      # .steamworkshop/Preview.mp4 + About/Preview.png -> .steamworkshop/Preview.gif
    python3 Scripts/preview-gif.py --work /tmp/preview  # keep the intermediates for inspection

The artist animates the hero on a 3x blow-up of About/Preview.png (1920x1080, H.264, 60 frames at a
true 12 fps): the hero and its sword are drawn from their own high-resolution source, everything else is
the 640x360 PNG scaled up and softened. Steam wants a GIF strictly under 1 MiB, at most 640x360. This
script never rescales the PNG's own pixels: the GIF is the PNG, with the video's hero composited into
the region the animation touches. `../PREVIEW_GIF_FROM_MP4.md` is the family guide; the measurements
below are this delivery's answers to its tests.

- Colour: the stream carries no matrix tag. Decoded as BT.709 the static art matches the PNG (planet
  limb 75.4 43.7 52.5 against the PNG's 75.7 44.7 53.0; BT.601 reads 72.8 39.8 52.5), and the tones
  need no curve, so the video's pixels go in as decoded.
- Geometry: every layer is at 3.0x, but the title, hex icon, badge and planet are each nudged 1 to 2
  PNG px from the PNG's layout. None of them moves and the mask reaches none of them (PROTECT is
  asserted clear of motion), so the nudges never show.
- Motion: the hero and the sword only, 28% of the frame; the loop closes as one more ordinary step and
  has no shorter period, so all 60 frames are kept.

Steps, each with its intermediate left in --work:

1. frames/      ffmpeg extracts every frame losslessly (Krita's Windows build through wslpath when no
                `ffmpeg` is on PATH).
2. mask.png     Pixels that ever change by more than MOTION_THRESHOLD against frame 0 (at 640x360,
                box-filtered), opened 3 px, dilated so the moving edges are inside it, holes filled so
                no PNG pixel survives inside the silhouette. Feathered 2 px for the composite; the
                binary mask tells the encoder which pixels may ever change.
3. c640/        The composite frames: the PNG everywhere, the video inside the mask, downscaled with a
                Hamming filter (Lanczos rings on a 3x reduction, and LZW pays for the ringing on every
                frame).
4. held/        The frames as the GIF shows them. A clean encode of the sway is 2.6 times the limit, so
                each pixel holds one colour for as long as the true colour stays within BAND of it, then
                moves on: every frame shows every pixel within BAND of the truth. The runs are the longest
                possible (a run ends when the truth's spread over it would exceed 2 x BAND, and shows its
                mean, kept inside the band), which is half the repaints of holding a colour until the
                truth has left it behind, for the same worst error.
5. Preview.gif  One global 256-colour palette (libimagequant over three spread frames). Frame 0 is the
                composite remapped with libimagequant's own gentle dithering, except inside the mask,
                which is remapped without dithering like every later frame. Later frames carry only
                the pixels whose run ended; gifsicle -O3 turns the rest into transparency. Outside the
                mask nothing ever changes, and the badge is the PNG's own, so the gif-patch skill can
                bump it in place. Delays run 8/8/9 cs so 60 frames take exactly 5.00 s.

Tuning record (60 frames, 45% mask): a clean encode is 2.62 MiB; BAND 8 1.19 MiB, 10 1.05, 11 0.986
(shipped), 12 0.93. Holding each colour until the truth has left it behind needs 20 to fit. The build is
deterministic: a rerun on the same inputs is byte-identical. `Docs/Research/Preview-GIF-Transcode.md` has
the measurements behind the method and the levers that did not earn their place.

Requires Pillow built with libimagequant (Pillow 9.0's binding returns a one-colour palette for any image
that is not 640x360, so the palette is always learned at 640x360 and only remapped elsewhere), gifsicle,
and ffmpeg. Nothing here ships; the MP4 is never tracked.
"""
import argparse, glob, os, shutil, subprocess, sys, tempfile
from PIL import Image, ImageChops, ImageDraw, ImageFilter, ImageMath, ImageStat

SIZE = (640, 360)
FRAMES = 60
COLOR_MATRIX = 'bt709'
MOTION_THRESHOLD = 12   # /255, against frame 0 at 640x360
MASK_DILATE = 11        # px (odd): the mask's margin around anything that moves
BAND = 11               # /255: the furthest a shown pixel may sit from its true colour
DELAYS = (8, 8, 9)      # centiseconds, repeating: 60 frames = 500 cs
# The static layers the artist nudged against the PNG. A mask that reached one would seam it.
PROTECT = {'badge': (0, 0, 110, 110), 'title': (30, 95, 373, 205), 'hex': (576, 7, 636, 58)}
LIMIT = 1048576
KRITA_FFMPEG = '/mnt/c/Program Files (x86)/Steam/steamapps/common/Krita/krita/bin/ffmpeg.exe'


def log(msg):
    print(f'[preview-gif] {msg}', flush=True)


def max_channel(im):
    r, g, b = im.split()
    return ImageChops.lighter(ImageChops.lighter(r, g), b)


def max_channel_diff(a, b):
    return max_channel(ImageChops.difference(a, b))


def fresh(im):
    """Pillow keeps RGB as four bytes per pixel, and composite() and paste() blend their mask into the
    padding byte, which libimagequant then reads as alpha and builds a visibly worse palette from. A
    fresh buffer pads with 255, exactly as the frame would reload from its PNG."""
    return Image.frombytes('RGB', im.size, im.tobytes())


def extract_frames(mp4, work, ffmpeg):
    out = os.path.join(work, 'frames')
    if glob.glob(os.path.join(out, 'f*.png')):
        log(f'frames: reusing {out}')
    else:
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
    assert len(frames) == FRAMES, f'{len(frames)} frames, expected {FRAMES}: the delivery changed'
    return frames


def check_delivery(png, small):
    """The two properties the method rests on: no padded frames, and static art in the PNG's tones."""
    step = min(ImageStat.Stat(max_channel_diff(a, b)).mean[0] for a, b in zip(small, small[1:] + small[:1]))
    assert step > 0.2, f'a frame repeats its neighbour (change {step:.2f}/255): the delivery is padded'
    box, blur = (0, 110, 120, 352), ImageFilter.GaussianBlur(3)   # the planet: static, saturated, clear of the layers
    off = max(abs(a - b) for a, b in zip(ImageStat.Stat(small[0].filter(blur).crop(box)).mean,
                                         ImageStat.Stat(png.filter(blur).crop(box)).mean))
    assert off < 2.5, f'static art is {off:.1f}/255 off the PNG: wrong colour matrix, or the render changed tones'
    log(f'delivery: smallest step {step:.2f}/255, static tones within {off:.1f}/255 of the PNG')


def build_mask(small, work):
    mx = Image.new('L', SIZE, 0)
    for im in small[1:]:
        mx = ImageChops.lighter(mx, max_channel_diff(im, small[0]))
    m = mx.point(lambda v: 255 if v > MOTION_THRESHOLD else 0)
    m = m.filter(ImageFilter.MinFilter(3)).filter(ImageFilter.MaxFilter(3))
    for name, box in PROTECT.items():
        assert m.crop(box).getbbox() is None, f'something moves over the {name}: a composite would seam it'
    m = m.filter(ImageFilter.MaxFilter(MASK_DILATE))
    inv = m.point(lambda v: 0 if v else 255)
    ImageDraw.floodfill(inv, (0, 0), 128)
    holes = inv.point(lambda v: 255 if v == 255 else 0)
    binary = ImageChops.lighter(m, holes)
    for box in PROTECT.values():
        binary.paste(0, box)
    feathered = binary.filter(ImageFilter.GaussianBlur(2))
    for box in PROTECT.values():
        feathered.paste(0, box)
    binary.save(os.path.join(work, 'mask.png'))
    cov = binary.histogram()[255] / (SIZE[0] * SIZE[1])
    log(f'mask: {cov:.0%} of the frame, bbox {binary.getbbox()}, {holes.histogram()[255]} hole px filled')
    return binary, feathered


def composite(png, frames, feathered, work):
    out = os.path.join(work, 'c640'); os.makedirs(out, exist_ok=True)
    comps = []
    for i, im in enumerate(frames):
        c = fresh(Image.composite(im.resize(SIZE, Image.HAMMING), png, feathered))
        c.save(os.path.join(out, f'c{i:02d}.png')); comps.append(c)
    return comps


def hold(comps, work):
    """Each pixel's colour over the loop as the fewest constant runs that stay within BAND of the truth."""
    out = os.path.join(work, 'held'); os.makedirs(out, exist_ok=True)
    over = lambda hi, lo: max_channel(ImageChops.subtract(hi, lo)).point(lambda v: 255 if v > 2 * BAND else 0)
    wide = lambda im: [c.convert('I') for c in im.split()]

    def colour(lo, hi, total, count):
        """A run's mean, pulled inside the range that is within BAND of every frame in the run."""
        return Image.merge('RGB', [
            ImageMath.eval('convert(min(max((t + n / 2) / n, h - b), l + b), "L")', t=t, n=count, h=h, l=l, b=BAND)
            for l, h, t in zip(wide(lo), wide(hi), total)])

    lo, hi, total, count, ended = comps[0].copy(), comps[0].copy(), wide(comps[0]), Image.new('I', SIZE, 1), [None]
    for f in comps[1:]:
        new_lo, new_hi = ImageChops.darker(lo, f), ImageChops.lighter(hi, f)
        brk = over(new_hi, new_lo)                              # pixels whose run cannot take this frame
        ended.append((brk, colour(lo, hi, total, count)))       # the colour of the run that just ended
        new_lo.paste(f, (0, 0), brk); new_hi.paste(f, (0, 0), brk)
        lo, hi = new_lo, new_hi
        total = [ImageMath.eval('t + c', t=t, c=c) for t, c in zip(total, wide(f))]
        count = ImageMath.eval('n + 1', n=count)
        for t, c in zip(total, wide(f)):
            t.paste(c, (0, 0), brk)
        count.paste(1, (0, 0), brk)
    cur, held = colour(lo, hi, total, count), [None] * len(comps)
    for i in range(len(comps) - 1, -1, -1):
        held[i] = fresh(cur)
        if i:
            cur.paste(ended[i][1], (0, 0), ended[i][0])
    worst = max(max_channel_diff(h, c).getextrema()[1] for h, c in zip(held, comps))
    assert worst <= BAND, f'a held pixel is {worst}/255 from the truth, over BAND {BAND}'
    for i, h in enumerate(held):
        h.save(os.path.join(out, f'h{i:02d}.png'))
    return held


def encode(comps, held, binary, out_path, work):
    n = len(comps); W, H = SIZE
    sample = Image.new('RGB', (W, H * 3))
    for k, i in enumerate((0, n // 3, 2 * n // 3)):
        sample.paste(comps[i], (0, H * k))
    pal = sample.quantize(256, method=Image.LIBIMAGEQUANT)
    assert len(pal.getcolors(256)) > 8, 'libimagequant returned a collapsed palette'
    remap = lambda im: im.quantize(palette=pal, dither=Image.NONE)
    outside = binary.point(lambda v: 0 if v else 255)
    tmp = os.path.join(work, 'gif'); os.makedirs(tmp, exist_ok=True)
    first = pal.crop((0, 0, W, H)); first.paste(remap(held[0]), (0, 0), binary)
    paths = []
    for i, h in enumerate(held):
        fr = first
        if i:
            fr = remap(h); fr.paste(first, (0, 0), outside)
        p = os.path.join(tmp, f'f{i:03d}.gif'); fr.save(p, interlace=False); paths.append(p)
    args = ['gifsicle', '-O3', '--no-warnings', '--loopcount=forever']
    for i, p in enumerate(paths):
        args += [f'-d{DELAYS[i % len(DELAYS)]}', p]
    subprocess.run(args + ['-o', out_path], check=True)
    return os.path.getsize(out_path)


def main():
    ap = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    ap.add_argument('--mp4', default='.steamworkshop/Preview.mp4')
    ap.add_argument('--png', default='About/Preview.png')
    ap.add_argument('--out', default='.steamworkshop/Preview.gif')
    ap.add_argument('--work', help='intermediates directory (default: a fresh temp dir, removed on success)')
    ap.add_argument('--ffmpeg', default=shutil.which('ffmpeg') or KRITA_FFMPEG)
    a = ap.parse_args()
    work = a.work or tempfile.mkdtemp(prefix='preview-gif-')
    os.makedirs(work, exist_ok=True)
    png = Image.open(a.png).convert('RGB')
    assert png.size == SIZE, f'{a.png} is {png.size}, expected {SIZE}'
    frames = extract_frames(a.mp4, work, a.ffmpeg)
    small = [im.resize(SIZE, Image.BOX) for im in frames]
    check_delivery(png, small)
    binary, feathered = build_mask(small, work)
    comps = composite(png, frames, feathered, work)
    held = hold(comps, work)
    size = encode(comps, held, binary, a.out + '.tmp', work)
    if size >= LIMIT:
        os.remove(a.out + '.tmp')
        sys.exit(f'[preview-gif] {size} bytes is over the {LIMIT} byte Workshop limit; nothing written')
    os.replace(a.out + '.tmp', a.out)
    log(f'wrote {a.out}: {size} bytes ({size / LIMIT:.1%} of the limit), {len(comps)} frames')
    if not a.work:
        shutil.rmtree(work)


if __name__ == '__main__':
    main()
