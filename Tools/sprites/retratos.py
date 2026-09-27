"""Retratos (busto 48x48) para el cuadro de diálogo + muñeco de práctica."""
import sys
from common import Canvas, save_sheet, flash

C = {
    "bg": (38, 28, 54), "bg2": (54, 40, 74), "frame": (201, 138, 62),
    "skin": (196, 132, 92), "skin_sh": (160, 100, 68), "skin_hi": (218, 160, 118),
    "skin2": (178, 114, 76), "skin2_sh": (140, 86, 56), "old": (170, 116, 84), "old_sh": (132, 86, 60),
    "hair": (27, 20, 16), "hair_hi": (64, 48, 40), "white": (240, 236, 226), "eye": (20, 14, 12),
    "red": (216, 67, 60), "red_sh": (160, 44, 40), "gold": (242, 176, 76), "jade": (60, 180, 120), "jade_sh": (40, 130, 90),
    "cream": (236, 222, 196), "cream_sh": (200, 184, 158), "teal": (46, 150, 130),
    "straw": (214, 184, 110), "straw_sh": (170, 140, 76), "shirt": (236, 228, 212),
    "stone": (94, 94, 110), "stone_sh": (70, 70, 84), "stone_hi": (122, 122, 140), "amber": (255, 165, 58),
    "obs": (16, 12, 22), "obs_hi": (84, 58, 124), "mirror": (70, 60, 100), "mirror_hi": (170, 150, 220), "violet": (190, 110, 255),
    "gstone": (125, 114, 102), "gstone_sh": (90, 82, 74), "gstone_hi": (160, 150, 136),
    "wood": (140, 96, 52), "wood_sh": (100, 66, 36), "sack": (196, 170, 120), "sack_sh": (150, 126, 84), "rope": (120, 84, 50),
}


def c(n):
    return C[n]


def backdrop(cv, tint=None):
    cv.rect(0, 0, 47, 47, tint or c("bg"))
    cv.rect(2, 2, 45, 30, c("bg2"))
    return cv


def ixtli():
    cv = backdrop(Canvas(48, 48))
    # hombros: blusa con greca y hombro de piedra
    cv.rect(6, 38, 42, 47, c("cream")); cv.rect(6, 44, 42, 47, c("cream_sh"))
    for x in range(12, 36, 3):
        cv.rect(x, 38, x + 1, 39, c("red")); cv.px(x + 1, 40, c("gold"))
    cv.rect(34, 40, 44, 47, c("stone")); cv.line([(36, 41), (39, 44), (37, 46)], c("amber"))
    # cuello y cara
    cv.rect(20, 32, 27, 38, c("skin_sh"))
    cv.ell(13, 9, 35, 36, c("skin"))
    cv.rect(30, 22, 34, 30, c("skin"))
    cv.ell(15, 27, 20, 31, c("skin_hi"))
    # pelo + trenzas con listón rojo
    cv.ell(11, 4, 35, 22, c("hair")); cv.rect(11, 12, 17, 32, c("hair"))
    cv.poly([(24, 10), (34, 12), (35, 17), (28, 14)], c("hair"))
    cv.px(20, 7, c("hair_hi")); cv.px(26, 6, c("hair_hi")); cv.line([(16, 9), (22, 7)], c("hair_hi"))
    for (x, y) in ((10, 26), (9, 30), (10, 34), (9, 38)):
        cv.ell(x - 2, y - 2, x + 2, y + 2, c("hair"))
    cv.rect(7, 40, 12, 42, c("red"))
    cv.rect(14, 12, 16, 14, c("red"))                       # listón en el pelo
    # ojo, ceja, nariz, boca, arete de jade
    cv.rect(26, 20, 30, 20, c("hair"))
    cv.rect(27, 22, 29, 25, c("eye")); cv.px(28, 22, c("white"))
    cv.px(33, 26, c("skin_sh")); cv.px(34, 27, c("skin_sh"))
    cv.rect(28, 31, 31, 31, c("red_sh"))
    cv.rect(17, 25, 18, 28, c("jade"))
    return cv.done()


def itzcoatl():
    cv = backdrop(Canvas(48, 48))
    cv.rect(5, 38, 43, 47, c("cream")); cv.rect(5, 44, 43, 45, c("teal"))
    for x in range(14, 34, 4):
        cv.ell(x, 37, x + 3, 40, c("jade"))                  # collar de jade
    cv.rect(20, 32, 28, 38, c("skin2_sh"))
    cv.ell(13, 9, 35, 36, c("skin2")); cv.rect(30, 22, 34, 31, c("skin2"))
    cv.rect(14, 32, 30, 35, c("skin2"))                     # quijada
    cv.ell(10, 3, 34, 18, c("hair")); cv.rect(10, 10, 16, 24, c("hair"))
    cv.px(18, 6, c("hair_hi")); cv.px(24, 5, c("hair_hi"))
    # banda roja con cola
    cv.rect(11, 13, 35, 15, c("red")); cv.poly([(11, 14), (3, 18), (5, 22), (12, 16)], c("red"))
    cv.line([(4, 19), (8, 18)], c("red_sh"))
    cv.rect(25, 19, 30, 19, c("hair"))
    cv.rect(27, 21, 29, 24, c("eye")); cv.px(28, 21, c("white"))
    cv.px(33, 26, c("skin2_sh")); cv.px(34, 27, c("skin2_sh"))
    cv.rect(27, 31, 31, 31, c("skin2_sh"))
    return cv.done()


def abuelo():
    cv = backdrop(Canvas(48, 48))
    cv.rect(4, 38, 44, 47, c("shirt")); cv.rect(20, 38, 28, 47, c("cream_sh"))
    cv.rect(20, 33, 28, 38, c("old_sh"))
    cv.ell(13, 12, 35, 38, c("old")); cv.rect(30, 24, 34, 32, c("old"))
    cv.line([(18, 20), (22, 21)], c("old_sh")); cv.line([(17, 30), (20, 33)], c("old_sh"))   # arrugas
    cv.rect(24, 22, 29, 23, c("white"))                     # cejas blancas
    cv.rect(26, 25, 28, 26, c("eye"))
    cv.px(33, 28, c("old_sh")); cv.px(34, 29, c("old_sh"))
    cv.poly([(21, 30), (27, 28), (33, 28), (37, 31), (36, 35), (32, 32), (27, 33), (22, 35)], (214, 210, 200))  # bigote
    cv.line([(24, 31), (34, 30)], c("white"))
    # sombrero de palma
    cv.rect(3, 13, 44, 16, c("straw")); cv.rect(3, 16, 44, 16, c("straw_sh"))
    cv.ell(11, 2, 36, 16, c("straw")); cv.rect(11, 11, 36, 12, c("red"))
    for x in range(14, 34, 4):
        cv.px(x, 6, c("straw_sh")); cv.px(x + 2, 9, c("straw_sh"))
    return cv.done()


def atlante():
    cv = backdrop(Canvas(48, 48))
    cv.rect(6, 40, 42, 47, c("stone_hi"))                   # hombros / pectoral
    cv.poly([(24, 40), (12, 42), (14, 47), (24, 46)], c("stone")); cv.poly([(24, 40), (36, 42), (34, 47), (24, 46)], c("stone"))
    cv.rect(12, 16, 36, 39, c("stone")); cv.rect(12, 16, 14, 39, c("stone_sh")); cv.rect(34, 16, 36, 39, c("stone_sh"))
    cv.rect(12, 22, 36, 24, c("stone_sh"))
    cv.rect(16, 26, 21, 28, c("amber")); cv.rect(27, 26, 32, 28, c("amber"))
    cv.rect(23, 25, 25, 33, c("stone_hi")); cv.rect(19, 35, 29, 36, c("stone_sh"))
    cv.rect(6, 18, 12, 34, c("stone_hi")); cv.rect(36, 18, 42, 34, c("stone_hi"))  # orejeras
    cv.rect(9, 10, 39, 16, c("stone_hi"))
    for x in range(11, 38, 5):
        cv.rect(x, 12, x + 2, 14, c("stone_sh"))
    for i, x in enumerate(range(10, 38, 4)):
        cv.rect(x, 2 + (i % 2) * 2, x + 2, 10, c("stone_hi") if i % 2 else c("stone"))
    cv.line([(15, 18), (18, 22)], c("amber")); cv.line([(31, 30), (33, 36)], c("amber"))
    return cv.done()


def tezcatl():
    cv = backdrop(Canvas(48, 48), (20, 14, 30))
    cv.poly([(6, 47), (12, 36), (24, 40), (36, 36), (42, 47)], c("obs"))
    cv.line([(12, 38), (18, 47)], c("obs_hi")); cv.line([(36, 38), (30, 47)], c("obs_hi"))
    cv.poly([(16, 6), (32, 6), (38, 20), (32, 36), (16, 36), (10, 20)], c("obs"))
    cv.poly([(18, 9), (30, 9), (35, 20), (30, 33), (18, 33), (13, 20)], c("mirror"))
    cv.line([(19, 12), (24, 24)], c("mirror_hi"), 2); cv.px(30, 14, c("mirror_hi"))
    cv.poly([(10, 20), (4, 12), (12, 16)], c("obs")); cv.poly([(38, 20), (44, 12), (36, 16)], c("obs"))
    return cv.done(2, (150, 80, 220))


def estela():
    cv = backdrop(Canvas(48, 48))
    cv.rect(12, 4, 36, 47, c("gstone")); cv.rect(12, 4, 36, 6, c("gstone_hi")); cv.rect(33, 4, 36, 47, c("gstone_sh"))
    cv.rect(16, 10, 31, 24, c("gstone_sh")); cv.ell(18, 12, 29, 22, c("gstone_hi")); cv.ell(21, 15, 26, 20, c("gstone_sh"))
    cv.px(23, 17, c("amber"))
    for y in (28, 33, 38, 43):
        cv.rect(16, y, 31, y + 1, c("gstone_sh"))
    return cv.done()


# ---------------------------------------------------------------- Muñeco de práctica (32x48)
def muneco(hit=0):
    cv = Canvas(32, 48)
    cv.rect(14, 26, 17, 47, c("wood")); cv.rect(14, 40, 17, 47, c("wood_sh"))
    cv.rect(10, 45, 21, 47, c("wood_sh"))
    t = hit * 2
    cv.rect(4 + t, 20, 27 + t, 22, c("wood"))                 # brazos
    cv.ell(8 + t, 14, 23 + t, 36, c("sack")); cv.ell(10 + t, 16, 17 + t, 26, (214, 190, 140))
    cv.line([(9 + t, 24), (22 + t, 24)], c("rope")); cv.line([(10 + t, 30), (21 + t, 30)], c("rope"))
    cv.ell(10 + t, 3, 21 + t, 14, c("sack")); cv.line([(12 + t, 12), (19 + t, 12)], c("rope"))
    cv.line([(13 + t, 7), (15 + t, 9)], c("red")); cv.line([(15 + t, 7), (13 + t, 9)], c("red"))    # diana en la cara
    cv.ell(14 + t, 22, 19 + t, 27, c("red")); cv.ell(15 + t, 23, 18 + t, 26, c("white")); cv.px(16 + t, 24, c("red"))
    for (x, y) in ((6, 20), (27, 21), (9, 33), (22, 34)):
        cv.line([(x + t, y), (x + t + 2, y + 2)], c("straw"))
    return cv.done()


def build(out):
    save_sheet(out, "Retratos", {
        "ixtli": [ixtli()], "itzcoatl": [itzcoatl()], "abuelo": [abuelo()],
        "atlante": [atlante()], "tezcatl": [tezcatl()], "estela": [estela()],
    }, 48, 48, 5)
    m = muneco(0)
    save_sheet(out, "MunecoPractica", {"idle": [m], "hurt": [flash(muneco(1)), muneco(1)]}, 32, 48, 6)


if __name__ == "__main__":
    build(sys.argv[1] if len(sys.argv) > 1 else "out")
    print("ok")
