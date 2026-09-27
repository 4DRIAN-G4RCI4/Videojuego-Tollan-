"""Jefe final: Atlante Corrompido por Tezcatl. Pixel art 128x192 por frame (4x6 tiles de 32 px)."""
from PIL import Image, ImageDraw

W, H = 128, 192
P = {
    "out": (18, 16, 24), "b0": (46, 46, 58), "b1": (70, 70, 84), "b2": (94, 94, 110), "b3": (122, 122, 140),
    "obs": (16, 12, 22), "obs_hi": (84, 58, 124), "glow_dim": (140, 64, 210), "glow": (196, 120, 255), "amber_dim": (210, 120, 40),
    "amber": (255, 165, 58), "wood": (96, 70, 50), "dust": (170, 150, 130), "smoke": (30, 22, 40),
}


def mirror_x(x):
    return W - 1 - x


class Painter:
    def __init__(self):
        self.im = Image.new("RGBA", (W, H), (0, 0, 0, 0))
        self.d = ImageDraw.Draw(self.im)

    def rect(self, x0, y0, x1, y1, c, sym=False):
        self.d.rectangle([x0, y0, x1, y1], fill=P[c] + (255,))
        if sym:
            self.d.rectangle([mirror_x(x1), y0, mirror_x(x0), y1], fill=P[c] + (255,))

    def poly(self, pts, c, sym=False):
        self.d.polygon(pts, fill=P[c] + (255,))
        if sym:
            self.d.polygon([(mirror_x(x), y) for x, y in pts], fill=P[c] + (255,))

    def line(self, pts, c, w=1, sym=False):
        self.d.line(pts, fill=P[c] + (255,), width=w)
        if sym:
            self.d.line([(mirror_x(x), y) for x, y in pts], fill=P[c] + (255,), width=w)

    def ellipse(self, box, c):
        self.d.ellipse(box, fill=P[c] + (255,))


def draw(pose="idle", bob=0, glow="glow_dim", broken=False, impact=False, shards_out=False, ally=False):
    p = Painter()
    b = bob

    # --- piernas y sandalias (no se mueven con el bob)
    p.rect(40, 126, 58, 178, "b1", sym=True)
    p.rect(40, 126, 44, 178, "b0", sym=True)
    p.rect(37, 146, 61, 153, "b2", sym=True)          # rodilleras
    p.rect(46, 148, 52, 151, "b3", sym=True)
    p.rect(32, 178, 62, 189, "b2", sym=True)          # sandalias
    p.rect(32, 170, 38, 178, "b3", sym=True)          # talonera
    p.rect(32, 186, 62, 189, "b0", sym=True)
    for x in range(35, 60, 6):
        p.rect(x, 181, x + 2, 183, "b3", sym=True)

    # --- torso
    p.rect(34, 72 + b, 93, 110 + b, "b1")
    p.rect(34, 72 + b, 40, 110 + b, "b0"); p.rect(87, 72 + b, 93, 110 + b, "b0")
    # pectoral de mariposa
    p.poly([(63, 78 + b), (42, 74 + b), (38, 90 + b), (50, 98 + b), (63, 96 + b)], "b3", sym=True)
    p.poly([(63, 82 + b), (47, 79 + b), (44, 89 + b), (63, 92 + b)], "b2", sym=True)
    p.rect(61, 76 + b, 66, 102 + b, "b0")
    p.line([(58, 76 + b), (54, 70 + b)], "b3", 2, sym=True)  # antenas
    # cinturón con disco
    p.rect(30, 108 + b, 97, 124 + b, "b2")
    p.rect(30, 108 + b, 97, 110 + b, "b3"); p.rect(30, 122 + b, 97, 124 + b, "b0")
    p.ellipse([51, 103 + b, 76, 128 + b], "b3"); p.ellipse([56, 108 + b, 71, 123 + b], "b1"); p.ellipse([61, 113 + b, 66, 118 + b], "b0")
    # taparrabo
    p.poly([(52, 124 + b), (75, 124 + b), (72, 162), (55, 162)], "b2")
    for y in range(132, 160, 7):
        p.rect(58, y, 69, y + 2, "b1")
    # hombreras
    p.rect(24, 70 + b, 42, 86 + b, "b2", sym=True)
    p.rect(24, 70 + b, 42, 72 + b, "b3", sym=True)

    # --- cabeza
    p.rect(52, 62 + b, 75, 72 + b, "b0")                      # cuello
    p.rect(44, 38 + b, 83, 66 + b, "b2")
    p.rect(44, 38 + b, 47, 66 + b, "b1", sym=True)
    p.rect(44, 46 + b, 83, 49 + b, "b1")                      # ceño
    p.rect(50, 51 + b, 58, 54 + b, glow, sym=True)            # ojos
    p.rect(52, 52 + b, 55, 53 + b, "glow" if glow == "glow_dim" else "amber", sym=True)
    p.rect(62, 50 + b, 65, 60 + b, "b3")                      # nariz
    p.rect(55, 61 + b, 72, 63 + b, "b0")                      # boca
    p.rect(35, 42 + b, 44, 62 + b, "b3", sym=True)            # orejeras
    p.rect(38, 46 + b, 41, 58 + b, "b1", sym=True)
    # tocado
    p.rect(40, 27 + b, 87, 38 + b, "b3")
    for x in range(43, 86, 8):
        p.rect(x, 30 + b, x + 3, 34 + b, "b1")                # discos
    p.rect(42, 10 + b, 85, 27 + b, "b2")
    for i, x in enumerate(range(42, 85, 5)):
        tip = 2 + (i % 2) * 3
        p.rect(x, tip + b, x + 3, 12 + b, "b3" if i % 2 else "b2")
        p.rect(x + 1, 14 + b, x + 2, 25 + b, "b1")

    # --- brazo izquierdo (lado izquierdo de la imagen)
    if pose == "throw":
        p.rect(4, 78 + b, 30, 90 + b, "b1"); p.rect(14, 76 + b, 20, 92 + b, "b3")
        p.rect(0, 74 + b, 8, 94 + b, "b2")
    else:
        p.rect(22, 86 + b, 34, 112 + b, "b1"); p.rect(20, 92 + b, 36, 98 + b, "b3")
        p.rect(22, 112 + b, 34, 134 + b, "b1"); p.rect(19, 134 + b, 36, 146 + b, "b2")
        p.poly([(18, 146 + b), (32, 146 + b), (30, 164 + b), (20, 160 + b)], "b0")   # bolsa de copal

    # --- brazo derecho
    if pose in ("idle", "throw"):
        p.rect(93, 86 + b, 105, 112 + b, "b1"); p.rect(91, 92 + b, 107, 98 + b, "b3")
        p.rect(93, 112 + b, 105, 134 + b, "b1"); p.rect(91, 134 + b, 108, 146 + b, "b2")
        p.rect(110, 100 + b, 113, 176, "wood"); p.rect(108, 98 + b, 115, 102 + b, "b3")  # lanzadardos
    elif pose == "windup":
        p.poly([(92, 74 + b), (104, 70 + b), (112, 30 + b), (100, 30 + b)], "b1")
        p.rect(98, 44 + b, 114, 50 + b, "b3")
        p.rect(96, 12 + b, 118, 32 + b, "b2"); p.rect(96, 12 + b, 118, 15 + b, "b3")
    elif pose == "slam":
        p.poly([(92, 76 + b), (106, 80 + b), (100, 150), (84, 146)], "b1")
        p.rect(84, 110 + b, 104, 116 + b, "b3")
        p.rect(78, 146, 108, 170, "b2"); p.rect(78, 146, 108, 149, "b3")

    # --- juntas de sillares (textura de piedra)
    for y in range(134, 176, 11):
        p.rect(41, y, 57, y, "b0", sym=True)
    for y in (80 + b, 92 + b):
        p.rect(35, y, 40, y, "b0"); p.rect(87, y, 92, y, "b0")
    for y in (16 + b, 22 + b):
        p.rect(42, y, 85, y, "b1")

    # --- corrupción de Tezcatl
    shards = [[(26, 72 + b), (16, 46 + b), (33, 70 + b)], [(34, 72 + b), (36, 52 + b), (41, 71 + b)],
              [(96, 72 + b), (110, 50 + b), (103, 72 + b)], [(88, 72 + b), (92, 58 + b), (95, 72 + b)]]
    for s in ([] if ally else shards):
        p.poly(s, "obs")
        p.line([s[0], s[1]], "obs_hi")
    cracks = [[(47, 40 + b), (52, 45 + b), (50, 50 + b)], [(80, 64 + b), (76, 58 + b), (78, 52 + b)],
              [(70, 80 + b), (78, 88 + b), (74, 98 + b), (82, 106 + b)], [(46, 130), (50, 140), (46, 146)],
              [(82, 160), (78, 168), (84, 176)], [(26, 100 + b), (30, 106 + b)]]
    for c in cracks:
        p.line(c, glow, 1)
    # núcleo oscuro en el pecho
    core = 5 if not broken else 11
    if ally:
        p.ellipse([59, 83 + b, 68, 92 + b], "b0"); p.ellipse([61, 85 + b, 66, 90 + b], glow)   # gema del pectoral
    else:
        p.poly([(72, 88 + b), (72 + core, 84 + b), (76 + core, 94 + b), (70 + core, 102 + b), (68, 97 + b)], "obs")
        p.ellipse([72, 91 + b, 72 + core, 97 + b], glow)
    if broken:
        # fase 3: pecho roto, más obsidiana y humo
        for s in ([(44, 38 + b), (38, 14 + b), (50, 36 + b)], [(84, 40 + b), (94, 18 + b), (86, 44 + b)],
                  [(60, 100 + b), (54, 82 + b), (64, 96 + b)], [(40, 128), (30, 118), (42, 136)]):
            p.poly(s, "obs"); p.line([s[0], s[1]], "obs_hi")
        for (x, y, r) in ((60, 4, 5), (70, 0, 4), (50, 0, 3), (82, 2, 4)):
            p.ellipse([x - r, y - r + b, x + r, y + r + b], "smoke")
        p.line([(40, 60 + b), (46, 76 + b), (42, 90 + b)], glow, 2)

    # --- efectos
    if impact:
        for (x0, y0, x1, y1) in ((70, 176, 60, 186), (116, 176, 124, 186), (92, 172, 92, 162), (76, 168, 66, 162), (108, 168, 118, 162)):
            p.line([(x0, y0), (x1, y1)], "amber", 2)
        for (x, y, r) in ((72, 184, 5), (112, 184, 6), (60, 188, 3)):
            p.ellipse([x - r, y - r, x + r, y + r], "dust")
    if shards_out:
        for (x, y) in ((2, 64), (8, 100), (0, 84), (14, 56)):
            p.poly([(x, y), (x + 8, y - 3), (x + 8, y + 3)], "obs"); p.line([(x, y), (x + 8, y - 3)], "obs_hi")

    if ally:
        return outline(p.im)
    return aura(outline(p.im), 3 if glow == "glow" else 2)


def aura(im, rings):
    """Anillos de aura morada alrededor de la silueta (poder de Tezcatl)."""
    cols = [(92, 40, 150, 200), (70, 30, 110, 130), (50, 22, 80, 70)]
    for i in range(rings):
        src = im.copy(); px = src.load(); dst = im.load()
        for y in range(H):
            for x in range(W):
                if px[x, y][3]:
                    continue
                for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                    nx, ny = x + dx, y + dy
                    if 0 <= nx < W and 0 <= ny < H and px[nx, ny][3]:
                        if (x + y + i) % (2 if i else 1) == 0:
                            dst[x, y] = cols[min(i, 2)]
                        break
    return im


def outline(im):
    src = im.copy()
    px = src.load(); dst = im.load()
    for y in range(H):
        for x in range(W):
            if px[x, y][3]:
                continue
            for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                nx, ny = x + dx, y + dy
                if 0 <= nx < W and 0 <= ny < H and px[nx, ny][3] and px[nx, ny][:3] != P["out"]:
                    dst[x, y] = P["out"] + (255,)
                    break
    return im


def flash(im):
    out = im.copy(); px = out.load()
    for y in range(H):
        for x in range(W):
            r, g, b, a = px[x, y]
            if a:
                px[x, y] = ((r + 255) // 2, (g + 255) // 2, (b + 255) // 2, a)
    return out


idle = draw()
ANIMS = {
    "idle": [idle, draw(bob=1, glow="glow")],
    "slam": [draw(pose="windup", bob=-1, glow="glow"), draw(pose="slam", bob=2, glow="glow", impact=True), draw(pose="slam", bob=1)],
    "throw": [draw(pose="throw", glow="glow"), draw(pose="throw", glow="glow", shards_out=True)],
    "hurt": [flash(idle)],
    "phase3": [draw(broken=True, glow="glow"), draw(broken=True, bob=1, glow="glow_dim")],
}

if __name__ == "__main__":
    import os, sys
    out = sys.argv[1] if len(sys.argv) > 1 else "."
    os.makedirs(out, exist_ok=True)
    rows = list(ANIMS.items())
    cols = max(len(f) for _, f in rows)
    sheet = Image.new("RGBA", (cols * W, len(rows) * H), (0, 0, 0, 0))
    for r, (_, frames) in enumerate(rows):
        for c, f in enumerate(frames):
            sheet.paste(f, (c * W, r * H))
    sheet.save(os.path.join(out, "AtlanteCorrompido_sheet.png"))

    # vista previa: jefe junto a Ixtli para escala
    from ixtli import ANIMS as IX
    s = 3
    bg = Image.new("RGBA", (W * 3 + 60, H + 20), (26, 20, 48, 255))
    for i, f in enumerate([ANIMS["idle"][0], ANIMS["slam"][1], ANIMS["phase3"][0]]):
        bg.alpha_composite(f, (10 + i * (W + 20), 10))
    bg.alpha_composite(IX["idle"][0], (0, H + 10 - 48))
    bg = bg.resize((bg.width * s, bg.height * s), Image.NEAREST)
    bg.save(os.path.join(out, "AtlanteCorrompido_preview.png"))

    frames = ANIMS["idle"] * 2 + ANIMS["slam"] + ANIMS["throw"] + ANIMS["hurt"] + ANIMS["phase3"] * 2
    gif = [Image.alpha_composite(Image.new("RGBA", (W, H), (26, 20, 48, 255)), f).resize((W * 3, H * 3), Image.NEAREST) for f in frames]
    gif[0].save(os.path.join(out, "AtlanteCorrompido_anim.gif"), save_all=True, append_images=gif[1:],
                duration=[400, 400, 400, 400, 350, 180, 300, 300, 250, 150, 400, 400, 400, 400], loop=0)
    print("ok", sheet.size)
