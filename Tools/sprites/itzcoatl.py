"""Genera el sprite sheet de Itzcóatl (pixel art 32x48 por frame) para TOLLAN."""
from PIL import Image
from ixtli import W, H, new, line, outline

C = {
    "hair": (22, 16, 14), "hair_hi": (52, 38, 30), "skin": (178, 114, 76), "skin_sh": (140, 86, 56),
    "band": (216, 67, 60), "tunic": (232, 220, 196), "tunic_sh": (196, 182, 156), "border": (46, 150, 130),
    "border2": (242, 176, 76), "jade": (60, 180, 120), "sash": (190, 58, 50), "sash_sh": (150, 42, 38),
    "loin": (214, 200, 172), "sandal": (110, 70, 40), "wood": (140, 96, 52), "iron": (150, 156, 162),
    "iron_sh": (104, 110, 118), "eye": (22, 16, 14), "dust": (206, 190, 160),
}


def px(im, x, y, c):
    if 0 <= x < W and 0 <= y < H:
        im.putpixel((x, y), C[c] + (255,))


def rect(im, x0, y0, x1, y1, c):
    for y in range(y0, y1 + 1):
        for x in range(x0, x1 + 1):
            px(im, x, y, c)


def ln(im, x0, y0, x1, y1, c, thick=1):
    line(im, x0, y0, x1, y1, C[c] + (255,), thick)


def leg(im, hip_x, foot_dx, lift, bob):
    top = 37 + bob
    foot_y = 44 - lift
    knee_y = (top + foot_y) // 2
    kx, fx = hip_x + foot_dx // 2, hip_x + foot_dx
    rect(im, kx, top, kx + 2, knee_y, "skin")
    rect(im, fx, knee_y, fx + 2, foot_y, "skin")
    px(im, fx, foot_y - 1, "sandal")  # correa
    rect(im, fx - 1, foot_y + 1, fx + 3, foot_y + 2, "sandal")


def shovel(im, hx, hy, ex, ey, blade):
    """Pala de Cantera: mango de hx,hy a ex,ey y hoja de hierro con glifo."""
    ln(im, hx, hy, ex, ey, "wood")
    if blade == "up":
        rect(im, ex - 2, ey - 5, ex + 2, ey - 1, "iron"); rect(im, ex - 1, ey - 6, ex + 1, ey - 6, "iron"); px(im, ex, ey - 3, "iron_sh")
    elif blade == "right":
        rect(im, ex + 1, ey - 2, ex + 5, ey + 2, "iron"); rect(im, ex + 6, ey - 1, ex + 6, ey + 1, "iron"); px(im, ex + 3, ey, "iron_sh")
    elif blade == "down":
        rect(im, ex - 2, ey + 1, ex + 2, ey + 5, "iron"); rect(im, ex - 1, ey + 6, ex + 1, ey + 6, "iron"); px(im, ex, ey + 3, "iron_sh")
    elif blade == "downleft":
        rect(im, ex - 5, ey - 2, ex - 1, ey + 2, "iron"); px(im, ex - 3, ey, "iron_sh")


def body(im, bob=0, legs=((0, 0), (0, 0)), front_arm="down", back_arm="down", band_swing=0, tool=None, blink=False):
    b = bob
    # brazo trasero
    if back_arm == "down":
        rect(im, 9, 20 + b, 10, 31 + b, "skin_sh")
    elif back_arm == "up":
        rect(im, 8, 13 + b, 9, 21 + b, "skin_sh")
    elif back_arm == "back":
        ln(im, 10, 21 + b, 6, 28 + b, "skin_sh", 2)

    # piernas
    leg(im, 13, legs[0][0], legs[0][1], b)
    leg(im, 17, legs[1][0], legs[1][1], b)

    # taparrabo / faldellín
    rect(im, 11, 31 + b, 21, 36 + b, "loin"); rect(im, 11, 35 + b, 21, 36 + b, "tunic_sh")
    for x in range(12, 21, 2):
        px(im, x, 36 + b, "border")
    # túnica sin mangas (hombros anchos)
    rect(im, 11, 19 + b, 21, 30 + b, "tunic"); rect(im, 11, 25 + b, 12, 30 + b, "tunic_sh")
    for x in range(11, 22, 2):
        px(im, x, 27 + b, "border"); px(im, x + 1, 27 + b, "border2")
    # faja roja + tira al frente (maxtlatl)
    rect(im, 11, 29 + b, 21, 30 + b, "sash")
    rect(im, 17, 31 + b, 19, 38 + b, "sash"); rect(im, 17, 38 + b, 19, 38 + b, "sash_sh")
    # cuello + collar de jade
    rect(im, 14, 17 + b, 18, 18 + b, "skin_sh")
    rect(im, 13, 19 + b, 19, 19 + b, "skin")
    for x in (13, 15, 17, 19):
        px(im, x, 20 + b, "jade")
    px(im, 16, 21 + b, "jade")
    # polvo de cantera
    px(im, 14, 24 + b, "dust"); px(im, 19, 32 + b, "dust")

    # cabeza (un poco más ancha, quijada marcada)
    rect(im, 12, 7 + b, 21, 16 + b, "skin")
    rect(im, 13, 16 + b, 20, 16 + b, "skin_sh"); px(im, 21, 15 + b, "skin_sh")
    # pelo corto
    rect(im, 11, 4 + b, 20, 7 + b, "hair"); rect(im, 11, 7 + b, 13, 12 + b, "hair"); rect(im, 21, 6 + b, 21, 7 + b, "hair")
    px(im, 14, 5 + b, "hair_hi"); px(im, 17, 4 + b, "hair_hi")
    # banda roja con cola que ondea atrás
    rect(im, 11, 8 + b, 21, 8 + b, "band")
    tx = 10 - band_swing
    ln(im, 11, 9 + b, tx - 1 - band_swing, 10 + b + (1 if band_swing < 2 else 0), "band")
    ln(im, 11, 10 + b, tx - band_swing, 11 + b + (1 if band_swing < 2 else 0), "band")
    # ojo, ceja, nariz
    rect(im, 18, 9 + b, 20, 9 + b, "skin_sh")  # ceja/sombra bajo la banda
    if blink:
        rect(im, 18, 12 + b, 19, 12 + b, "eye")
    else:
        rect(im, 19, 11 + b, 19, 12 + b, "eye")
    px(im, 21, 13 + b, "skin_sh")

    # brazo delantero + pala
    if front_arm == "down":
        rect(im, 20, 20 + b, 22, 30 + b, "skin"); rect(im, 20, 27 + b, 22, 27 + b, "jade")
        if tool == "hold":
            shovel(im, 21, 36 + b, 24, 22 + b, "up")
            rect(im, 20, 30 + b, 22, 31 + b, "skin_sh")
    elif front_arm == "up":
        rect(im, 22, 11 + b, 24, 20 + b, "skin"); rect(im, 22, 14 + b, 24, 14 + b, "jade")
        if tool == "windup":
            shovel(im, 23, 11 + b, 15, 3 + b, "downleft")
    elif front_arm == "fwd":
        rect(im, 20, 20 + b, 27, 22 + b, "skin"); rect(im, 24, 20 + b, 24, 22 + b, "jade")
        if tool == "swing":
            shovel(im, 26, 21 + b, 27, 21 + b, "right")
    elif front_arm == "gun":
        rect(im, 16, 21 + b, 21, 23 + b, "wood")
        ln(im, 21, 20 + b, 31, 20 + b, "iron"); ln(im, 21, 21 + b, 30, 21 + b, "iron_sh")
        rect(im, 24, 22 + b, 28, 22 + b, "wood")
        rect(im, 20, 20 + b, 26, 23 + b, "skin"); rect(im, 23, 20 + b, 23, 23 + b, "jade")
        if tool == "fire":
            rect(im, 29, 17 + b, 31, 24 + b, "border2"); rect(im, 30, 19 + b, 31, 22 + b, "tunic")
    elif front_arm == "low":
        ln(im, 21, 20 + b, 25, 29 + b, "skin", 3); px(im, 23, 25 + b, "jade")
        if tool == "follow":
            shovel(im, 25, 30 + b, 28, 35 + b, "down")


def frame(**kw):
    im = new()
    body(im, **kw)
    return outline(im)


ANIMS = {
    "idle": [frame(tool="hold"), frame(bob=1, tool="hold"), frame(bob=1, tool="hold", band_swing=1), frame(tool="hold", blink=True)],
    "walk": [
        frame(legs=((3, 0), (-3, 1)), tool="hold", band_swing=1),
        frame(bob=-1, legs=((1, 1), (-1, 0)), tool="hold", band_swing=2),
        frame(legs=((-3, 1), (3, 0)), tool="hold", band_swing=1),
        frame(bob=-1, legs=((-1, 0), (1, 1)), tool="hold", band_swing=2),
    ],
    "jump": [frame(bob=-1, legs=((2, 4), (-1, 2)), front_arm="up", back_arm="up", band_swing=2)],
    "fall": [frame(legs=((1, 0), (-2, 1)), front_arm="up", back_arm="back", band_swing=3)],
    "attack": [
        frame(front_arm="up", back_arm="back", tool="windup", legs=((-1, 0), (2, 0))),
        frame(front_arm="fwd", back_arm="back", tool="swing", legs=((-2, 0), (3, 0)), band_swing=3),
        frame(front_arm="low", back_arm="down", tool="follow", legs=((-2, 0), (3, 0)), band_swing=2),
    ],
    "shoot": [
        frame(front_arm="gun", legs=((-1, 0), (2, 0))),
        frame(front_arm="gun", tool="fire", legs=((-2, 0), (2, 0)), band_swing=3),
        frame(front_arm="gun", legs=((-2, 0), (2, 0)), band_swing=2),
    ],
}

if __name__ == "__main__":
    import os, sys
    out = sys.argv[1] if len(sys.argv) > 1 else "."
    os.makedirs(out, exist_ok=True)
    rows = list(ANIMS.items())
    cols = max(len(f) for _, f in rows)
    sheet = Image.new("RGBA", (cols * W, len(rows) * H), (0, 0, 0, 0))
    for r, (name, frames) in enumerate(rows):
        for c, f in enumerate(frames):
            sheet.paste(f, (c * W, r * H))
    sheet.save(os.path.join(out, "Itzcoatl_sheet.png"))
    s = 6
    prev = Image.new("RGBA", (sheet.width * s, sheet.height * s), (26, 20, 48, 255))
    prev.alpha_composite(sheet.resize((sheet.width * s, sheet.height * s), Image.NEAREST))
    prev.save(os.path.join(out, "Itzcoatl_preview.png"))
    gif = [Image.alpha_composite(Image.new("RGBA", (W, H), (26, 20, 48, 255)), f).resize((W * 8, H * 8), Image.NEAREST)
           for f in ANIMS["walk"] + ANIMS["attack"] + ANIMS["attack"][:1]]
    gif[0].save(os.path.join(out, "Itzcoatl_anim.gif"), save_all=True, append_images=gif[1:], duration=[120] * 4 + [90, 90, 160, 90], loop=0)
    print("ok", sheet.size)
