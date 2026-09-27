"""Genera el sprite sheet de Ixtli (pixel art 32x48 por frame) para TOLLAN."""
from PIL import Image

W, H = 32, 48
C = {
    "hair": (27, 20, 16), "hair_hi": (58, 42, 34), "skin": (196, 132, 92), "skin_sh": (160, 100, 68),
    "ribbon": (216, 67, 60), "blouse": (236, 222, 196), "blouse_sh": (200, 184, 158),
    "collar": (216, 67, 60), "collar2": (242, 176, 76), "apron": (138, 90, 54), "apron_sh": (104, 66, 40),
    "skirt": (70, 52, 92), "skirt_sh": (52, 38, 70), "sandal": (110, 70, 40), "bandage": (228, 220, 200),
    "stone": (125, 114, 102), "stone_sh": (90, 82, 74), "amber": (255, 165, 58),
    "wood": (140, 96, 52), "pick": (150, 140, 128), "eye": (27, 20, 16), "outline": (22, 16, 14),
}


def new():
    return Image.new("RGBA", (W, H), (0, 0, 0, 0))


def px(im, x, y, c):
    if 0 <= x < W and 0 <= y < H:
        im.putpixel((x, y), C[c] + (255,) if isinstance(c, str) else c)


def rect(im, x0, y0, x1, y1, c):
    for y in range(y0, y1 + 1):
        for x in range(x0, x1 + 1):
            px(im, x, y, c)


def line(im, x0, y0, x1, y1, c, thick=1):
    dx, dy = abs(x1 - x0), -abs(y1 - y0)
    sx, sy = (1 if x0 < x1 else -1), (1 if y0 < y1 else -1)
    err = dx + dy
    while True:
        for t in range(thick):
            px(im, x0, y0 + t, c)
        if x0 == x1 and y0 == y1:
            break
        e2 = 2 * err
        if e2 >= dy:
            err += dy; x0 += sx
        if e2 <= dx:
            err += dx; y0 += sy


def leg(im, hip_x, foot_dx, lift, bob):
    top = 38 + bob
    foot_y = 44 - lift
    knee_y = (top + foot_y) // 2
    kx = hip_x + foot_dx // 2
    fx = hip_x + foot_dx
    for y in range(top, knee_y + 1):
        rect(im, kx, y, kx + 2, y, "skin")
    for y in range(knee_y, foot_y + 1):
        rect(im, fx, y, fx + 2, y, "skin")
    px(im, fx, foot_y, "skin_sh")
    rect(im, fx - 1, foot_y + 1, fx + 3, foot_y + 2, "sandal")
    px(im, fx + 1, foot_y, "sandal")  # correa del huarache


def pick(im, hx, hy, ex, ey, head_dir):
    """Pico de Tollan: mango de hx,hy a ex,ey; cabeza de piedra con grieta ámbar."""
    line(im, hx, hy, ex, ey, "wood")
    if head_dir == "up":
        rect(im, ex - 3, ey - 1, ex + 3, ey, "pick"); px(im, ex - 4, ey, "pick"); px(im, ex + 4, ey, "pick")
        px(im, ex, ey - 1, "amber")
    elif head_dir == "right":
        rect(im, ex, ey - 3, ex + 1, ey + 3, "pick"); px(im, ex, ey - 4, "pick"); px(im, ex, ey + 4, "pick")
        px(im, ex + 1, ey, "amber")
    elif head_dir == "down":
        rect(im, ex - 3, ey, ex + 3, ey + 1, "pick"); px(im, ex - 4, ey, "pick"); px(im, ex + 4, ey, "pick")
        px(im, ex, ey + 1, "amber")


def body(im, bob=0, legs=((0, 0), (0, 0)), front_arm="down", back_arm="down", braid_swing=0, pick_pose=None, blink=False):
    b = bob
    # brazo trasero (izquierdo, vendado)
    if back_arm == "down":
        rect(im, 10, 21 + b, 11, 29 + b, "blouse_sh"); rect(im, 10, 27 + b, 11, 30 + b, "bandage"); rect(im, 10, 31 + b, 11, 32 + b, "skin_sh")
    elif back_arm == "up":
        rect(im, 9, 15 + b, 10, 22 + b, "blouse_sh"); rect(im, 9, 14 + b, 10, 15 + b, "skin_sh")
    elif back_arm == "back":
        line(im, 11, 21 + b, 7, 28 + b, "blouse_sh", 2); rect(im, 6, 28 + b, 7, 29 + b, "skin_sh")

    # piernas
    leg(im, 13, legs[0][0], legs[0][1], b)
    leg(im, 17, legs[1][0], legs[1][1], b)

    # falda
    rect(im, 11, 30 + b, 21, 37 + b, "skirt"); rect(im, 11, 36 + b, 21, 37 + b, "skirt_sh")
    for x in range(12, 21, 3):
        px(im, x, 34 + b, "collar2")  # greca
    # torso
    rect(im, 12, 19 + b, 20, 29 + b, "blouse"); rect(im, 12, 26 + b, 13, 29 + b, "blouse_sh")
    rect(im, 13, 19 + b, 19, 20 + b, "collar")
    for x in range(13, 20, 2):
        px(im, x, 20 + b, "collar2")
    # delantal
    rect(im, 15, 25 + b, 20, 33 + b, "apron"); rect(im, 15, 32 + b, 20, 33 + b, "apron_sh"); px(im, 17, 28 + b, "apron_sh"); px(im, 18, 28 + b, "apron_sh")
    # cuello
    rect(im, 15, 17 + b, 17, 18 + b, "skin_sh")

    # trenzas
    bx = 10 - braid_swing
    for i, y in enumerate(range(13 + b, 22 + b)):
        px(im, bx + (1 if i % 3 == 0 else 0), y, "hair")
    rect(im, bx, 22 + b, bx + 1, 23 + b, "ribbon")
    # cabeza
    rect(im, 12, 7 + b, 21, 16 + b, "skin")
    rect(im, 11, 5 + b, 21, 9 + b, "hair"); rect(im, 11, 9 + b, 14, 15 + b, "hair"); rect(im, 13, 4 + b, 19, 5 + b, "hair")
    px(im, 15, 6 + b, "hair_hi"); px(im, 17, 5 + b, "hair_hi")
    rect(im, 20, 9 + b, 21, 10 + b, "hair")  # fleco
    rect(im, 15, 16 + b, 20, 16 + b, "skin_sh")
    if blink:
        rect(im, 18, 12 + b, 19, 12 + b, "eye")
    else:
        rect(im, 18, 11 + b, 18, 12 + b, "eye")
    px(im, 21, 14 + b, "skin_sh")  # nariz
    px(im, 12, 11 + b, "ribbon")  # listón en el pelo

    # brazo delantero (derecho, de piedra) + pico
    if front_arm == "down":
        rect(im, 20, 20 + b, 22, 25 + b, "blouse"); rect(im, 20, 26 + b, 22, 30 + b, "stone"); px(im, 21, 28 + b, "amber"); rect(im, 20, 31 + b, 22, 32 + b, "stone_sh")
        if pick_pose == "hold":
            pick(im, 21, 33 + b, 25, 22 + b, "up")
    elif front_arm == "up":
        rect(im, 20, 19 + b, 22, 21 + b, "blouse"); rect(im, 22, 12 + b, 24, 19 + b, "stone"); px(im, 23, 15 + b, "amber"); rect(im, 22, 10 + b, 24, 11 + b, "stone_sh")
        if pick_pose == "windup":
            pick(im, 23, 10 + b, 15, 2 + b, "down")
    elif front_arm == "fwd":
        rect(im, 20, 20 + b, 23, 22 + b, "blouse"); rect(im, 24, 20 + b, 27, 22 + b, "stone"); px(im, 26, 21 + b, "amber")
        if pick_pose == "swing":
            pick(im, 27, 21 + b, 29, 21 + b, "right")
    elif front_arm == "gun":
        rect(im, 16, 21 + b, 21, 23 + b, "wood")                       # culata al hombro
        line(im, 21, 20 + b, 31, 20 + b, "pick"); line(im, 21, 21 + b, 30, 21 + b, "stone_sh")
        rect(im, 24, 22 + b, 28, 22 + b, "wood")
        rect(im, 20, 20 + b, 22, 23 + b, "blouse"); rect(im, 23, 22 + b, 26, 24 + b, "stone"); px(im, 24, 23 + b, "amber")
        if pick_pose == "fire":
            rect(im, 29, 17 + b, 31, 24 + b, "amber"); rect(im, 30, 19 + b, 31, 22 + b, "blouse")
    elif front_arm == "low":
        line(im, 21, 20 + b, 25, 28 + b, "stone", 3); px(im, 23, 24 + b, "amber")
        if pick_pose == "follow":
            pick(im, 25, 30 + b, 29, 36 + b, "down")


def outline(im):
    src = im.copy()
    for y in range(H):
        for x in range(W):
            if src.getpixel((x, y))[3]:
                continue
            for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                nx, ny = x + dx, y + dy
                if 0 <= nx < W and 0 <= ny < H and src.getpixel((nx, ny))[3] and src.getpixel((nx, ny))[:3] != C["outline"]:
                    im.putpixel((x, y), C["outline"] + (255,))
                    break
    return im


def frame(**kw):
    im = new()
    body(im, **kw)
    return outline(im)


ANIMS = {
    "idle": [frame(pick_pose="hold"), frame(bob=1, pick_pose="hold"), frame(bob=1, pick_pose="hold"), frame(pick_pose="hold", blink=True)],
    "walk": [
        frame(legs=((3, 0), (-3, 1)), pick_pose="hold", braid_swing=1),
        frame(bob=-1, legs=((1, 1), (-1, 0)), pick_pose="hold"),
        frame(legs=((-3, 1), (3, 0)), pick_pose="hold", braid_swing=1),
        frame(bob=-1, legs=((-1, 0), (1, 1)), pick_pose="hold"),
    ],
    "jump": [frame(bob=-1, legs=((2, 4), (-1, 2)), front_arm="up", back_arm="up", braid_swing=1)],
    "fall": [frame(legs=((1, 0), (-2, 1)), front_arm="up", back_arm="back", braid_swing=2)],
    "attack": [
        frame(front_arm="up", back_arm="back", pick_pose="windup", legs=((-1, 0), (2, 0))),
        frame(front_arm="fwd", back_arm="back", pick_pose="swing", legs=((-2, 0), (3, 0)), braid_swing=2),
        frame(front_arm="low", back_arm="down", pick_pose="follow", legs=((-2, 0), (3, 0)), braid_swing=1),
    ],
    "shoot": [
        frame(front_arm="gun", legs=((-1, 0), (2, 0))),
        frame(front_arm="gun", pick_pose="fire", legs=((-2, 0), (2, 0)), braid_swing=2),
        frame(front_arm="gun", legs=((-2, 0), (2, 0)), braid_swing=1),
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
    sheet.save(os.path.join(out, "Ixtli_sheet.png"))
    # vista previa grande con fondo
    s = 6
    prev = Image.new("RGBA", (sheet.width * s, sheet.height * s), (26, 20, 48, 255))
    prev.alpha_composite(sheet.resize((sheet.width * s, sheet.height * s), Image.NEAREST))
    prev.save(os.path.join(out, "Ixtli_preview.png"))
    # gif de caminar
    gif = [Image.alpha_composite(Image.new("RGBA", (W, H), (26, 20, 48, 255)), f).resize((W * 8, H * 8), Image.NEAREST)
           for f in ANIMS["walk"] + ANIMS["attack"] + ANIMS["attack"][:1]]
    gif[0].save(os.path.join(out, "Ixtli_anim.gif"), save_all=True, append_images=gif[1:], duration=[120] * 4 + [90, 90, 160, 90], loop=0)
    print("ok", sheet.size)
