"""Abuelo Nabor + 7 enemigos comunes de TOLLAN (pixel art, mirando a la derecha)."""
import sys
from common import Canvas, save_sheet, flash

K = {
    "skin": (178, 114, 76), "skin_sh": (140, 86, 56), "skin_old": (168, 112, 80),
    "straw": (214, 184, 110), "straw_sh": (170, 140, 76), "white": (236, 228, 212), "cream": (226, 212, 184),
    "cream_sh": (190, 176, 150), "pants": (80, 62, 46), "pants_sh": (60, 46, 34), "sandal": (110, 70, 40),
    "iron": (120, 124, 132), "iron_sh": (84, 88, 96), "wood": (120, 84, 50), "wood_sh": (90, 62, 38),
    "clay": (176, 104, 64), "clay_sh": (136, 76, 44), "clay_hi": (204, 134, 90), "pink": (214, 150, 150),
    "glass": (130, 205, 235), "black": (20, 16, 24), "lime": (220, 220, 208), "lime_sh": (176, 176, 166),
    "rust": (150, 74, 40), "obs": (22, 18, 30), "obs_hi": (96, 66, 140), "violet": (190, 110, 255),
    "green": (120, 150, 70), "green_sh": (84, 110, 50), "gourd": (200, 160, 90), "gourd_sh": (160, 120, 60),
    "rag": (120, 96, 110), "rag_sh": (90, 70, 84), "shirt": (220, 214, 200), "jeans": (70, 86, 120),
    "blanket": (150, 130, 110), "blanket_sh": (112, 94, 80), "red": (190, 58, 50), "smoke": (120, 110, 130, 200),
    "smoke2": (80, 72, 90, 180), "ember": (255, 150, 50), "stone": (110, 110, 118), "stone_sh": (80, 80, 88),
    "stone_hi": (140, 140, 150), "orange": (255, 165, 58),
}


def c(name):
    return K[name]


# ------------------------------------------------------------ Abuelo Nabor (32x48)
def nabor(bob=0, blink=False):
    cv = Canvas(32, 48); b = bob
    # piernas (pantalón de manta oscuro)
    cv.rect(12, 33, 14, 44, c("pants")); cv.rect(17, 33, 19, 44, c("pants_sh"))
    cv.rect(11, 45, 15, 46, c("sandal")); cv.rect(16, 45, 20, 46, c("sandal"))
    # torso encorvado (camisa arremangada)
    cv.poly([(10, 20 + b), (19, 18 + b), (22, 33), (10, 33)], c("white"))
    cv.rect(10, 26 + b, 12, 33, c("cream_sh"))
    cv.rect(10, 31, 21, 33, c("wood_sh"))                                  # cinturón
    # brazo con mano enorme y agrietada
    cv.rect(18, 21 + b, 21, 25 + b, c("white"))
    cv.rect(19, 25 + b, 21, 29 + b, c("skin_old"))
    cv.rect(18, 29 + b, 23, 33 + b, c("skin_old")); cv.px(20, 31 + b, c("skin_sh")); cv.px(22, 30 + b, c("skin_sh"))
    # mazo al hombro
    cv.line([(19, 22 + b), (7, 12 + b)], c("wood"), 2)
    cv.rect(3, 7 + b, 9, 13 + b, c("iron")); cv.rect(3, 12 + b, 9, 13 + b, c("iron_sh"))
    # cabeza (más baja, adelantada)
    cv.rect(15, 11 + b, 23, 19 + b, c("skin_old"))
    cv.rect(18, 16 + b, 24, 17 + b, c("white"))                           # bigote blanco
    cv.px(24, 18 + b, c("white"))
    cv.rect(20, 13 + b, 20, 13 + b if blink else 14 + b, c("black"))
    cv.px(23, 14 + b, c("skin_sh"))
    # sombrero de palma
    cv.rect(10, 9 + b, 27, 10 + b, c("straw")); cv.rect(13, 5 + b, 23, 9 + b, c("straw"))
    cv.rect(13, 8 + b, 23, 8 + b, c("red"))
    cv.px(16, 6 + b, c("straw_sh")); cv.px(20, 6 + b, c("straw_sh")); cv.rect(10, 10 + b, 27, 10 + b, c("straw_sh"))
    return cv.done()


# ------------------------------------------------------------ 1. Tlacuachillo de Barro (32x32)
def tlacuachillo(step=0, charge=False):
    cv = Canvas(32, 32)
    dy = 2 if charge else 0
    # cola pelona
    cv.line([(7, 18), (3, 14), (1, 9), (3, 6)], c("pink"), 2)
    # patas
    legs = [(9, 0), (13, 0), (19, 0), (23, 0)]
    for i, (x, _) in enumerate(legs):
        off = (1 if (i + step) % 2 else -1) if step >= 0 else 0
        cv.rect(x + off, 23, x + off + 2, 28, c("clay_sh"))
    # cuerpo
    cv.ell(6, 11 + dy // 2, 26, 25, c("clay"))
    cv.ell(9, 12 + dy // 2, 22, 17, c("clay_hi"))
    cv.line([(12, 16), (15, 19), (13, 22)], c("clay_sh"))
    cv.line([(19, 14), (21, 18)], c("clay_sh"))
    # cabeza con hocico
    cv.poly([(22, 12 + dy), (28, 14 + dy), (31, 18 + dy), (27, 21 + dy), (21, 20 + dy)], c("clay"))
    cv.px(31, 18 + dy, c("black"))
    cv.rect(22, 10 + dy, 24, 12 + dy, c("clay_sh"))                          # oreja
    cv.rect(25, 15 + dy, 26, 16 + dy, c("glass")); cv.px(26, 15 + dy, c("black"))  # ojo de canica
    if charge:
        for y in (12, 17, 22):
            cv.line([(0, y), (4, y)], c("clay_hi"))
    return cv.done()


# ------------------------------------------------------------ 2. Espantapájaros Hueco (32x48)
def espantapajaros(sway=0, attack=False):
    cv = Canvas(32, 48); s = sway
    cv.rect(15, 14, 16, 47, c("wood")); cv.rect(15, 40, 16, 47, c("wood_sh"))
    cv.rect(3, 18 + s, 28, 19 - s, c("wood"))
    # ropa vieja
    cv.poly([(6, 17), (26, 17), (25, 30), (22, 28), (19, 32), (15, 29), (11, 32), (8, 28), (6, 30)], c("rag"))
    cv.rect(13, 20, 18, 21, c("rag_sh")); cv.px(9, 24, c("rag_sh")); cv.px(22, 23, c("rag_sh"))
    # paja
    for (x0, y0, x1, y1) in ((4, 18, 1, 21), (4, 19, 2, 23), (27, 18, 30, 21), (27, 19, 29, 23), (14, 14, 12, 16), (17, 14, 19, 16)):
        cv.line([(x0, y0 + s), (x1, y1 + s)], c("straw"))
    # cabeza de jícara
    cv.ell(10, 3, 21, 14, c("gourd")); cv.ell(12, 4, 17, 8, c("straw"))
    cv.rect(13, 8, 14, 9, c("black")); cv.rect(17, 8, 18, 9, c("black"))
    cv.px(13, 8, c("violet")); cv.px(18, 8, c("violet"))
    cv.line([(13, 11), (15, 12), (16, 11), (18, 12)], c("black"))
    cv.rect(15, 1, 16, 3, c("gourd_sh"))
    if attack:
        for y in (16, 19, 22):
            cv.line([(28, y), (31, y - 1)], c("green"), 1)
        cv.poly([(29, 17), (31, 18), (29, 19)], c("green"))
    return cv.done()


# ------------------------------------------------------------ 3. Minero Olvidado (32x48)
def minero(step=0, attack=False):
    cv = Canvas(32, 48)
    l = 1 if step else -1
    cv.rect(12 + l, 34, 14 + l, 44, c("lime_sh")); cv.rect(17 - l, 34, 19 - l, 44, c("lime"))
    cv.rect(11 + l, 45, 15 + l, 46, c("black")); cv.rect(16 - l, 45, 20 - l, 46, c("black"))
    cv.poly([(9, 20), (19, 17), (22, 34), (10, 34)], c("lime"))            # encorvado
    cv.rect(9, 26, 11, 34, c("lime_sh"))
    cv.line([(12, 22), (14, 28)], c("lime_sh")); cv.line([(18, 26), (20, 31)], c("lime_sh"))
    # cabeza + casco con lámpara apagada
    cv.rect(15, 12, 22, 19, c("lime"))
    cv.rect(13, 8, 23, 12, c("iron_sh")); cv.rect(22, 9, 24, 11, c("black"))
    cv.rect(19, 14, 20, 15, c("black"))
    if attack:
        cv.rect(19, 12, 22, 20, c("lime_sh"))
        cv.line([(21, 12), (27, 2)], c("wood"), 2)
        cv.poly([(22, 0), (31, 2), (30, 4), (24, 3)], c("rust"))
    else:
        cv.rect(19, 21, 22, 30, c("lime_sh"))
        cv.line([(21, 30), (27, 22)], c("wood"), 2)
        cv.poly([(24, 19), (29, 22), (31, 28), (28, 24)], c("rust"))
    return cv.done()


# ------------------------------------------------------------ 4. Murciélago de Obsidiana (32x32)
def murcielago(frame=0):
    cv = Canvas(32, 32)
    if frame == 2:   # picada
        cv.poly([(16, 30), (12, 12), (16, 6), (20, 12)], c("obs"))
        cv.line([(14, 12), (16, 28)], c("obs_hi"))
        cv.poly([(12, 12), (9, 4), (15, 8)], c("obs")); cv.poly([(20, 12), (23, 4), (17, 8)], c("obs"))
        return cv.done(1, (70, 30, 110))
    up = frame == 0
    cv.ell(13, 12, 19, 20, c("obs"))
    cv.poly([(14, 11), (15, 7), (16, 11)], c("obs")); cv.poly([(17, 11), (18, 7), (19, 11)], c("obs"))
    if up:
        wl = [(13, 15), (2, 3), (5, 10), (1, 12), (7, 15)]
    else:
        wl = [(13, 15), (1, 22), (6, 21), (4, 27), (9, 19)]
    cv.poly(wl, c("obs")); cv.poly([(31 - x, y) for x, y in wl], c("obs"))
    cv.line([wl[0], wl[1]], c("obs_hi")); cv.line([(31 - wl[0][0], wl[0][1]), (31 - wl[1][0], wl[1][1])], c("obs_hi"))
    return cv.done(1, (70, 30, 110))


# ------------------------------------------------------------ 5. Aldeano Ensombrecido (32x48)
def aldeano(step=0, attack=False):
    cv = Canvas(32, 48)
    l = 2 if step else -2
    cv.rect(12 + l // 2, 32, 14 + l, 44, c("jeans")); cv.rect(17 - l // 2, 32, 19 - l, 44, c("pants_sh"))
    cv.rect(11 + l, 45, 15 + l, 46, c("sandal")); cv.rect(16 - l, 45, 20 - l, 46, c("sandal"))
    cv.rect(11, 18, 20, 32, c("shirt")); cv.rect(11, 26, 12, 32, c("cream_sh"))
    cv.rect(10, 18, 11, 30, c("skin_sh"))
    # cabeza con mancha negra que escurre sobre los ojos
    cv.rect(12, 7, 20, 16, c("skin"))
    cv.rect(11, 4, 20, 7, c("pants_sh")); cv.rect(11, 7, 13, 11, c("pants_sh"))
    cv.rect(14, 9, 21, 12, c("black"))
    for x, y in ((15, 13), (15, 14), (18, 13), (18, 14), (18, 15), (20, 13)):
        cv.px(x, y, c("black"))
    if attack:
        cv.rect(19, 18, 26, 20, c("skin"))
        cv.line([(26, 19), (31, 12)], c("iron"), 2)
    else:
        cv.rect(19, 19, 21, 29, c("skin"))
        cv.line([(21, 29), (25, 37)], c("iron"), 2); cv.rect(20, 28, 22, 30, c("wood_sh"))
    return cv.done()


# ------------------------------------------------------------ 6. Copalero (32x48)
def copalero(frame=0, attack=False):
    cv = Canvas(32, 48)
    cv.poly([(9, 12), (20, 8), (24, 20), (22, 46), (8, 46)], c("blanket"))
    cv.rect(8, 40, 22, 46, c("blanket_sh"))
    for y in (22, 30, 38):
        for x in range(10, 22, 3):
            cv.px(x, y, c("red"))
    # capucha y cara oculta
    cv.poly([(11, 4), (20, 2), (23, 12), (20, 17), (11, 17)], c("blanket_sh"))
    cv.rect(16, 9, 21, 15, c("black")); cv.px(19, 11, c("ember")); cv.px(17, 11, c("ember"))
    # cadena y sahumerio
    cv.rect(21, 22, 23, 26, c("blanket"))
    cv.line([(23, 26), (26, 33)], c("iron"))
    cv.poly([(23, 33), (30, 33), (29, 38), (24, 38)], c("clay")); cv.rect(24, 32, 29, 33, c("ember"))
    puffs = [(27, 27, 3), (25, 22, 4), (28, 16, 3)] if frame == 0 else [(26, 28, 3), (28, 22, 3), (25, 15, 4)]
    if attack:
        puffs = [(26, 26, 5), (22, 18, 6), (29, 12, 5), (18, 8, 4), (30, 22, 3)]
    for (x, y, r) in puffs:
        cv.ell(x - r, y - r, x + r, y + r, c("smoke") if r < 5 else c("smoke2"))
    return cv.done()


# ------------------------------------------------------------ 7. Guardián Menor (48x56)
def guardian(pose="idle"):
    cv = Canvas(48, 56)
    # piernas y sandalias
    cv.rect(14, 38, 20, 51, c("stone")); cv.rect(24, 38, 30, 51, c("stone_sh"))
    cv.rect(12, 52, 21, 55, c("stone_hi")); cv.rect(23, 52, 32, 55, c("stone_hi"))
    # torso + pectoral + cinturón
    cv.rect(12, 20, 32, 38, c("stone"))
    cv.poly([(22, 22), (14, 21), (14, 28), (22, 30)], c("stone_hi")); cv.poly([(22, 22), (30, 21), (30, 28), (22, 30)], c("stone_hi"))
    cv.rect(21, 21, 23, 31, c("stone_sh"))
    cv.rect(11, 33, 33, 37, c("stone_hi")); cv.ell(19, 32, 25, 38, c("stone_sh"))
    # cabeza + tocado
    cv.rect(15, 9, 29, 20, c("stone")); cv.rect(15, 12, 29, 13, c("stone_sh"))
    cv.rect(18, 14, 20, 15, c("black")); cv.rect(24, 14, 26, 15, c("black"))
    cv.rect(13, 5, 31, 9, c("stone_hi")); cv.rect(15, 1, 29, 5, c("stone"))
    for x in range(15, 30, 3):
        cv.rect(x, 0, x + 1, 4, c("stone_hi"))
    cv.line([(16, 22), (19, 27), (17, 32)], c("black")); cv.line([(28, 10), (26, 15)], c("black"))  # grietas
    # brazo con escudo
    if pose == "block":
        cv.ell(24, 14, 44, 38, c("wood")); cv.ell(28, 18, 40, 34, c("stone_hi")); cv.ell(32, 22, 36, 30, c("red"))
    else:
        cv.rect(8, 21, 12, 36, c("stone_sh"))
        cv.ell(1, 24, 15, 44, c("wood")); cv.ell(4, 27, 12, 41, c("stone_hi")); cv.ell(6, 31, 10, 37, c("red"))
    # brazo con macuahuitl
    if pose == "attack":
        cv.rect(32, 18, 44, 22, c("stone"))
        cv.rect(40, 2, 44, 22, c("wood"))
        for y in range(3, 20, 4):
            cv.poly([(44, y), (47, y + 1), (44, y + 2)], c("obs")); cv.poly([(40, y), (37, y + 1), (40, y + 2)], c("obs"))
    elif pose == "idle":
        cv.rect(32, 21, 36, 36, c("stone"))
        cv.rect(35, 30, 38, 52, c("wood"))
        for y in range(32, 50, 4):
            cv.poly([(38, y), (41, y + 1), (38, y + 2)], c("obs"))
    else:
        cv.rect(32, 21, 36, 34, c("stone"))
    return cv.done()


def build(out):
    sheets = {}
    sheets["AbueloNabor"] = save_sheet(out, "AbueloNabor", {"idle": [nabor(), nabor(1), nabor(1), nabor(blink=True)]}, 32, 48, 6)
    t = tlacuachillo
    sheets["Tlacuachillo"] = save_sheet(out, "Tlacuachillo", {"walk": [t(0), t(1)], "charge": [t(-1, True)], "hurt": [flash(t(0))]}, 32, 32, 6)
    e = espantapajaros
    sheets["Espantapajaros"] = save_sheet(out, "Espantapajaros", {"idle": [e(0), e(1)], "attack": [e(0, True)], "hurt": [flash(e(0))]}, 32, 48, 6)
    m = minero
    sheets["MineroOlvidado"] = save_sheet(out, "MineroOlvidado", {"walk": [m(0), m(1)], "attack": [m(0, True)], "hurt": [flash(m(0))]}, 32, 48, 6)
    b = murcielago
    sheets["MurcielagoObsidiana"] = save_sheet(out, "MurcielagoObsidiana", {"fly": [b(0), b(1)], "dive": [b(2)], "hurt": [flash(b(0))]}, 32, 32, 6)
    a = aldeano
    sheets["AldeanoEnsombrecido"] = save_sheet(out, "AldeanoEnsombrecido", {"walk": [a(0), a(1)], "attack": [a(0, True)], "hurt": [flash(a(0))]}, 32, 48, 6)
    cp = copalero
    sheets["Copalero"] = save_sheet(out, "Copalero", {"idle": [cp(0), cp(1)], "attack": [cp(0, True)], "hurt": [flash(cp(0))]}, 32, 48, 6)
    g = guardian
    sheets["GuardianMenor"] = save_sheet(out, "GuardianMenor", {"idle": [g("idle")], "block": [g("block")], "attack": [g("attack")], "hurt": [flash(g("idle"))]}, 48, 56, 5)
    return sheets


if __name__ == "__main__":
    build(sys.argv[1] if len(sys.argv) > 1 else "out")
    print("ok")
