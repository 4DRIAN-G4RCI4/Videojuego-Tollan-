"""Jefes de TOLLAN: Tlacuache de Barro, Ahuízotl de Cal, El Campanero y Tezcatl."""
import sys
from common import Canvas, save_sheet, flash
from npcs_enemigos import K


def c(n):
    return K[n]


X = {
    "bronze": (176, 120, 60), "bronze_sh": (130, 84, 40), "bronze_hi": (220, 170, 96),
    "shadow": (30, 22, 40), "shadow2": (46, 34, 60), "eyeglow": (240, 232, 210),
    "water": (40, 70, 90, 220), "water_hi": (90, 130, 150, 230), "foam": (190, 210, 214),
    "chalk": (232, 230, 220), "chalk_sh": (190, 188, 180), "chalk_dk": (150, 148, 140),
    "handskin": (214, 196, 180), "mirror": (70, 60, 100), "mirror_hi": (170, 150, 220),
    "wave": (255, 214, 140), "babies": (160, 92, 56),
}


def x(n):
    return X[n]


# ------------------------------------------------------------ Tlacuache de Barro (96x64)
def tlacuache(breath=0, charge=False):
    cv = Canvas(96, 64); b = breath
    dy = 4 if charge else 0
    # cola pelona enroscada
    cv.line([(14, 40), (6, 34), (2, 24), (4, 14), (10, 10)], c("pink"), 3)
    # patas
    for lx in (18, 30, 56, 68):
        cv.rect(lx, 50, lx + 6, 60, c("clay_sh")); cv.rect(lx - 1, 60, lx + 7, 62, c("clay_sh"))
    # cuerpo
    cv.ell(10, 12 - b + dy // 2, 80, 56, c("clay"))
    cv.ell(18, 14 - b + dy // 2, 64, 30, c("clay_hi"))
    # panza abierta con nido de crías
    cv.ell(28, 34, 62, 54, c("black"))
    cv.ell(31, 38, 59, 53, (60, 36, 26))
    for (bx, by) in ((34, 42), (42, 44), (50, 41), (38, 48), (47, 49), (55, 46)):
        cv.ell(bx, by, bx + 6, by + 5, x("babies")); cv.px(bx + 4, by + 2, c("glass"))
    cv.line([(28, 40), (24, 34)], c("clay_sh"), 2); cv.line([(62, 42), (66, 36)], c("clay_sh"), 2)
    # grietas
    for pts in (((20, 22), (26, 28), (22, 34)), ((46, 16), (50, 24), (46, 28)), ((70, 24), (74, 32))):
        cv.line(list(pts), c("clay_sh"), 1)
    # cabeza
    hy = dy
    cv.poly([(66, 14 + hy), (84, 18 + hy), (95, 30 + hy), (86, 38 + hy), (68, 36 + hy)], c("clay"))
    cv.rect(93, 29 + hy, 95, 31 + hy, c("black"))
    cv.ell(68, 8 + hy, 76, 16 + hy, c("clay_sh")); cv.ell(70, 10 + hy, 74, 14 + hy, c("pink"))
    cv.ell(78, 22 + hy, 84, 28 + hy, c("glass")); cv.rect(81, 24 + hy, 82, 26 + hy, c("black"))
    for i in range(3):
        cv.line([(86, 33 + hy + i * 2), (92, 34 + hy + i * 3)], c("clay_sh"))
    cv.line([(84, 36 + hy), (90, 37 + hy)], c("white"))                         # dientes
    if charge:
        for y in (20, 30, 40):
            cv.line([(0, y), (8, y)], c("clay_hi"), 2)
    return cv.done()


# ------------------------------------------------------------ Ahuízotl de Cal (96x64)
def ahuizotl(bob=0, grab=False):
    cv = Canvas(96, 64); b = bob
    # cola que termina en mano humana
    if grab:
        cv.line([(24, 40 + b), (40, 22), (64, 14), (82, 14)], x("chalk_sh"), 4)
        hx, hy = 84, 10
        cv.rect(hx, hy, hx + 7, hy + 8, x("handskin"))
        for i in range(4):
            cv.rect(hx + 8, hy + i * 2, hx + 11, hy + i * 2, x("handskin"))
        cv.rect(hx + 2, hy + 8, hx + 4, hy + 10, x("handskin"))
    else:
        cv.line([(20, 42 + b), (10, 30), (8, 18), (14, 8)], x("chalk_sh"), 4)
        hx, hy = 10, 0
        cv.rect(hx, hy, hx + 7, hy + 7, x("handskin"))
        for i in range(4):
            cv.rect(hx + i * 2, hy - 3, hx + i * 2, hy, x("handskin"))
        cv.rect(hx + 7, hy + 3, hx + 9, hy + 5, x("handskin"))
    # cuerpo de perro-nutria
    cv.ell(18, 26 + b, 74, 52 + b, x("chalk"))
    cv.ell(24, 30 + b, 64, 40 + b, (244, 242, 236))
    cv.line([(30, 44 + b), (40, 46 + b)], x("chalk_dk")); cv.line([(50, 32 + b), (56, 36 + b)], x("chalk_dk"))
    # patas delanteras con garras
    cv.rect(62, 42 + b, 68, 52 + b, x("chalk_sh"))
    for i in range(3):
        cv.rect(66 + i * 2, 52 + b, 66 + i * 2, 54 + b, c("black"))
    # cabeza
    cv.poly([(62, 22 + b), (78, 18 + b), (92, 28 + b), (90, 36 + b), (68, 38 + b)], x("chalk"))
    cv.poly([(66, 22 + b), (68, 12 + b), (73, 20 + b)], x("chalk_sh"))     # orejas
    cv.poly([(72, 20 + b), (76, 12 + b), (78, 19 + b)], x("chalk_sh"))
    cv.rect(80, 25 + b, 82, 27 + b, c("black")); cv.px(81, 25 + b, (120, 200, 255))
    cv.rect(91, 29 + b, 93, 31 + b, c("black"))
    cv.line([(80, 34 + b), (90, 33 + b)], c("black"))
    for i in range(3):
        cv.px(82 + i * 3, 35 + b, c("white"))
    # agua turbia
    cv.rect(0, 50, 95, 63, x("water"))
    for i, wx in enumerate(range(0, 96, 8)):
        cv.rect(wx, 50 + (i + b) % 2, wx + 4, 50 + (i + b) % 2, x("foam"))
    cv.rect(0, 55, 95, 55, x("water_hi"))
    return cv.done()


# ------------------------------------------------------------ El Campanero (64x80)
def campanero(frame=0, ring=False):
    cv = Canvas(64, 80); b = frame
    # piernas sombra
    cv.rect(18, 62, 24, 78, x("shadow")); cv.rect(34, 62, 40, 78, x("shadow2"))
    # cuerpo encorvado
    cv.poly([(14, 36), (40, 30), (48, 62), (16, 64)], x("shadow"))
    # campana de bronce en la espalda (caparazón)
    sw = -3 if ring else 0
    bx, by = 6 + sw, 14 + b
    cv.poly([(bx + 10, by), (bx + 30, by), (bx + 36, by + 34), (bx + 40, by + 44), (bx, by + 44), (bx + 4, by + 34)], x("bronze"))
    cv.poly([(bx + 12, by + 2), (bx + 18, by + 2), (bx + 14, by + 36), (bx + 8, by + 36)], x("bronze_hi"))
    cv.rect(bx, by + 40, bx + 40, by + 44, x("bronze_sh"))
    cv.rect(bx + 16, by - 5, bx + 24, by, x("bronze_sh"))
    for gx in range(bx + 6, bx + 36, 6):
        cv.rect(gx, by + 30, gx + 2, by + 32, x("bronze_sh"))            # greca
    cv.ell(bx + 17, by + 44, bx + 23, by + 50, x("bronze_sh"))           # badajo
    # torso de sombra al frente de la campana
    cv.poly([(30, 30 + b), (44, 26 + b), (50, 62), (28, 64)], x("shadow"))
    cv.poly([(32, 34 + b), (40, 31 + b), (42, 50), (33, 52)], x("shadow2"))
    # brazo largo con mazo
    cv.line([(42, 38), (54, 50), (58, 44 if ring else 56)], x("shadow2"), 4)
    cv.rect(54, (36 if ring else 54), 62, (44 if ring else 62), x("bronze_sh"))
    # cabeza pequeña con ojos blancos
    cv.ell(38, 22 + b, 54, 36 + b, x("shadow"))
    cv.rect(46, 27 + b, 48, 29 + b, x("eyeglow")); cv.rect(51, 27 + b, 52, 29 + b, x("eyeglow"))
    img = cv
    if ring:
        for r in (8, 14, 20):
            img.arc([bx + 20 - r * 2, by + 20 - r, bx + 20 + r * 2, by + 20 + r], 200, 340, x("wave"), 2)
            img.arc([bx + 20 - r * 2, by + 26 - r, bx + 20 + r * 2, by + 26 + r], 20, 160, x("wave"), 1)
    return cv.done()


# ------------------------------------------------------------ Tezcatl (64x96)
def tezcatl(frame=0, swarm=False):
    cv = Canvas(64, 96); b = frame * 2
    # rastro de humo negro (flota, no pisa el suelo)
    for (sx, sy, r) in ((32, 86, 6), (28, 92, 4), (37, 91, 3), (24, 82, 3)):
        cv.ell(sx - r, sy - r - b, sx + r, sy + r - b, c("smoke2"))
    if swarm:
        import random
        rnd = random.Random(3)
        for _ in range(38):
            px, py = rnd.randint(4, 58), rnd.randint(6, 80)
            s = rnd.randint(2, 5)
            cv.poly([(px, py), (px + s, py - s), (px + s + 1, py + s // 2)], c("obs"))
            cv.px(px + 1, py - 1, c("obs_hi"))
        cv.ell(26, 26, 38, 40, x("mirror")); cv.ell(29, 28, 32, 32, x("mirror_hi"))
        return cv.done(2)
    # cuerpo de lajas de obsidiana
    body = [(20, 30 - b), (44, 30 - b), (48, 50 - b), (42, 70 - b), (36, 82 - b), (28, 82 - b), (22, 70 - b), (16, 50 - b)]
    cv.poly(body, c("obs"))
    for pts in (((22, 34), (30, 50), (24, 64)), ((42, 34), (36, 52), (40, 66)), ((32, 54), (32, 78))):
        cv.line([(px_, py_ - b) for px_, py_ in pts], c("obs_hi"))
    # brazos filosos
    cv.poly([(18, 32 - b), (6, 50 - b), (2, 66 - b), (10, 54 - b), (18, 46 - b)], c("obs"))
    cv.poly([(46, 32 - b), (58, 50 - b), (62, 66 - b), (54, 54 - b), (46, 46 - b)], c("obs"))
    cv.line([(6, 50 - b), (2, 66 - b)], c("obs_hi")); cv.line([(58, 50 - b), (62, 66 - b)], c("obs_hi"))
    # hombros de lajas
    cv.poly([(16, 30 - b), (10, 20 - b), (24, 28 - b)], c("obs")); cv.poly([(48, 30 - b), (54, 20 - b), (40, 28 - b)], c("obs"))
    # cabeza sin rostro: espejo pulido
    cv.poly([(24, 8 - b), (40, 8 - b), (44, 20 - b), (38, 30 - b), (26, 30 - b), (20, 20 - b)], c("obs"))
    cv.poly([(26, 11 - b), (38, 11 - b), (41, 20 - b), (36, 27 - b), (28, 27 - b), (23, 20 - b)], x("mirror"))
    cv.line([(27, 14 - b), (31, 22 - b)], x("mirror_hi"), 2)             # reflejo
    cv.px(36, 15 - b, x("mirror_hi"))
    return cv.done(2)


def build(out):
    t = tlacuache
    save_sheet(out, "TlacuacheDeBarro", {"idle": [t(0), t(1)], "charge": [t(0, True)], "hurt": [flash(t(0))]}, 96, 64, 4)
    a = ahuizotl
    save_sheet(out, "AhuizotlDeCal", {"idle": [a(0), a(1)], "grab": [a(0, True)], "hurt": [flash(a(0))]}, 96, 64, 4)
    cm = campanero
    save_sheet(out, "ElCampanero", {"idle": [cm(0), cm(1)], "ring": [cm(0, True)], "hurt": [flash(cm(0))]}, 64, 80, 4)
    tz = tezcatl
    save_sheet(out, "Tezcatl", {"float": [tz(0), tz(1)], "swarm": [tz(0, True)], "hurt": [flash(tz(0))]}, 64, 96, 4)


if __name__ == "__main__":
    build(sys.argv[1] if len(sys.argv) > 1 else "out")
    print("ok")
