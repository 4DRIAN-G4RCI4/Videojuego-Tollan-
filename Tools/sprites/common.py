"""Utilidades compartidas para dibujar pixel art con código (TOLLAN)."""
import os
from PIL import Image, ImageDraw

OUTLINE = (18, 16, 24)
BG = (26, 20, 48, 255)


def col(c):
    return tuple(c[:3]) + ((c[3],) if len(c) == 4 else (255,))


class Canvas:
    def __init__(self, w, h):
        self.w, self.h = w, h
        self.im = Image.new("RGBA", (w, h), (0, 0, 0, 0))
        self.d = ImageDraw.Draw(self.im)

    def mx(self, x):
        return self.w - 1 - x

    def rect(self, x0, y0, x1, y1, c, sym=False):
        self.d.rectangle([min(x0, x1), min(y0, y1), max(x0, x1), max(y0, y1)], fill=col(c))
        if sym:
            self.d.rectangle([self.mx(max(x0, x1)), min(y0, y1), self.mx(min(x0, x1)), max(y0, y1)], fill=col(c))

    def px(self, x, y, c):
        if 0 <= x < self.w and 0 <= y < self.h:
            self.im.putpixel((int(x), int(y)), col(c))

    def poly(self, pts, c, sym=False):
        self.d.polygon(pts, fill=col(c))
        if sym:
            self.d.polygon([(self.mx(x), y) for x, y in pts], fill=col(c))

    def line(self, pts, c, w=1, sym=False):
        self.d.line(pts, fill=col(c), width=w)
        if sym:
            self.d.line([(self.mx(x), y) for x, y in pts], fill=col(c), width=w)

    def ell(self, x0, y0, x1, y1, c):
        self.d.ellipse([x0, y0, x1, y1], fill=col(c))

    def arc(self, box, a0, a1, c, w=1):
        self.d.arc(box, a0, a1, fill=col(c), width=w)

    def done(self, aura_rings=0, aura_color=(92, 40, 150)):
        im = outline(self.im)
        if aura_rings:
            im = aura(im, aura_rings, aura_color)
        return im


def outline(im):
    w, h = im.size
    src = im.copy(); p = src.load(); d = im.load()
    for y in range(h):
        for x in range(w):
            if p[x, y][3]:
                continue
            for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                nx, ny = x + dx, y + dy
                if 0 <= nx < w and 0 <= ny < h and p[nx, ny][3] > 128 and p[nx, ny][:3] != OUTLINE:
                    d[x, y] = OUTLINE + (255,)
                    break
    return im


def aura(im, rings, base=(92, 40, 150)):
    w, h = im.size
    for i in range(rings):
        a = max(60, 200 - i * 60)
        c = tuple(int(v * (1 - i * 0.2)) for v in base) + (a,)
        src = im.copy(); p = src.load(); d = im.load()
        for y in range(h):
            for x in range(w):
                if p[x, y][3]:
                    continue
                for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                    nx, ny = x + dx, y + dy
                    if 0 <= nx < w and 0 <= ny < h and p[nx, ny][3]:
                        if i == 0 or (x + y + i) % 2 == 0:
                            d[x, y] = c
                        break
    return im


def flash(im):
    out = im.copy(); p = out.load()
    for y in range(out.height):
        for x in range(out.width):
            r, g, b, a = p[x, y]
            if a:
                p[x, y] = ((r + 255) // 2, (g + 255) // 2, (b + 255) // 2, a)
    return out


def save_sheet(out_dir, name, anims, w, h, gif_scale=4, durations=None):
    """Guarda <name>_sheet.png (una fila por animación), <name>_anim.gif y devuelve la hoja."""
    os.makedirs(out_dir, exist_ok=True)
    rows = list(anims.items())
    cols = max(len(f) for _, f in rows)
    sheet = Image.new("RGBA", (cols * w, len(rows) * h), (0, 0, 0, 0))
    for r, (_, frames) in enumerate(rows):
        for c, f in enumerate(frames):
            sheet.paste(f, (c * w, r * h))
    sheet.save(os.path.join(out_dir, f"{name}_sheet.png"))
    frames = [f for _, fs in rows for f in (fs * 2 if len(fs) <= 2 else fs)]
    gif = [Image.alpha_composite(Image.new("RGBA", (w, h), BG), f).resize((w * gif_scale, h * gif_scale), Image.NEAREST) for f in frames]
    gif[0].save(os.path.join(out_dir, f"{name}_anim.gif"), save_all=True, append_images=gif[1:],
                duration=durations or 220, loop=0, disposal=2)
    # manifiesto para el importador de Unity
    with open(os.path.join(out_dir, f"{name}_sheet.txt"), "w") as fh:
        fh.write(f"frame {w}x{h}\n")
        for r, (anim, fs) in enumerate(rows):
            fh.write(f"{anim} row={r} frames={len(fs)}\n")
    return sheet
