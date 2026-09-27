"""Armas, potenciadores, proyectiles y decoración de TOLLAN (pixel art)."""
import sys, math
from common import Canvas, save_sheet

P = {
    "wood": (140, 96, 52), "wood_sh": (100, 66, 36), "wood_hi": (176, 126, 74), "iron": (150, 156, 162),
    "iron_sh": (100, 106, 114), "iron_hi": (200, 204, 210), "rust": (150, 74, 40), "stone": (125, 114, 102),
    "stone_sh": (90, 82, 74), "stone_hi": (160, 150, 136), "amber": (255, 165, 58), "amber_hi": (255, 214, 140),
    "obs": (22, 18, 30), "obs_hi": (96, 66, 140), "violet": (190, 110, 255), "leather": (120, 74, 44),
    "bone": (232, 224, 204), "bone_sh": (190, 180, 160), "black": (20, 16, 24), "dry": (150, 130, 70),
    "dry_sh": (110, 94, 50), "green": (92, 130, 70), "green_sh": (62, 94, 48), "green_hi": (130, 168, 90),
    "agave": (100, 140, 120), "agave_sh": (70, 104, 90), "agave_hi": (140, 180, 156), "clay": (176, 104, 64),
    "clay_sh": (136, 76, 44), "clay_hi": (204, 134, 90), "marigold": (255, 150, 30), "marigold_sh": (220, 100, 20),
    "red": (216, 67, 60), "red_sh": (160, 44, 40), "fire": (255, 120, 40), "fire_hi": (255, 220, 120),
    "straw": (214, 184, 110), "straw_sh": (170, 140, 76), "cream": (236, 222, 196), "pink": (240, 110, 170),
    "blue": (60, 140, 220), "yellow": (250, 210, 60), "purple": (150, 80, 200), "gold": (240, 190, 70),
}


def p(n):
    return P[n]


def cv32():
    return Canvas(32, 32)


# ================================================================= ARMAS (íconos 32x32)
def pico():
    c = cv32()
    c.line([(6, 28), (22, 8)], p("wood"), 3); c.line([(7, 28), (22, 9)], p("wood_sh"), 1)
    c.poly([(12, 6), (22, 4), (31, 10), (24, 9), (18, 12)], p("stone"))
    c.poly([(18, 8), (22, 6), (26, 8), (22, 10)], p("stone_hi"))
    c.line([(20, 7), (23, 9)], p("amber")); c.px(25, 9, p("amber"))
    return c.done()


def pala():
    c = cv32()
    c.line([(24, 2), (12, 20)], p("wood"), 3)
    c.rect(22, 1, 28, 3, p("wood_sh"))
    c.poly([(8, 18), (16, 22), (12, 31), (3, 27)], p("iron")); c.poly([(9, 20), (14, 22), (11, 28)], p("iron_hi"))
    c.px(9, 25, p("iron_sh")); c.px(11, 24, p("iron_sh"))
    return c.done()


def escopeta(glow=0):
    c = cv32()
    c.rect(0, 12, 20, 14, p("iron")); c.rect(0, 12, 20, 12, p("iron_hi"))       # cañón largo
    c.rect(0, 15, 12, 15, p("iron_sh"))
    c.rect(5, 15, 13, 17, p("wood")); c.rect(5, 17, 13, 17, p("wood_sh"))       # guardamano
    c.rect(19, 12, 23, 17, p("iron_sh")); c.px(21, 11, p("iron_sh"))            # cajón
    c.poly([(22, 13), (26, 14), (31, 19), (31, 24), (27, 24), (23, 18)], p("wood"))  # culata
    c.line([(24, 16), (29, 22)], p("wood_hi"))
    c.line([(25, 18), (28, 16)], p("leather"), 2); c.line([(27, 21), (30, 19)], p("leather"), 2)  # amarrada con cuero
    c.px(29, 22, p("amber"))                                                     # glifo tallado
    c.rect(19, 18, 20, 20, p("iron_sh"))                                         # gatillo
    c.px(6, 13, p("rust")); c.px(14, 14, p("rust")); c.px(2, 12, p("rust"))
    im = c.done()
    if glow:
        from common import aura
        im = aura(im, glow, (255, 170, 60))
    return im


def mazo():
    c = cv32()
    c.line([(6, 29), (20, 11)], p("wood"), 3)
    c.line([(9, 25), (12, 21)], p("leather"), 3)
    c.poly([(14, 4), (26, 2), (30, 12), (18, 14)], p("iron_sh")); c.poly([(15, 5), (24, 3), (26, 8), (17, 9)], p("iron"))
    return c.done()


def astillas():
    c = cv32()
    c.ell(6, 14, 26, 30, p("leather")); c.rect(8, 14, 24, 17, p("wood_sh"))
    for (x, y, h) in ((10, 4, 12), (15, 2, 14), (20, 5, 11)):
        c.poly([(x, y), (x + 3, y + h), (x - 2, y + h)], p("obs")); c.line([(x, y), (x + 2, y + h - 2)], p("obs_hi"))
    return c.done()


def corazon_obsidiana(f=0):
    c = cv32()
    s = [0, 1, 1, 0][f]
    pts = [(16, 28 + s), (5 - s, 15), (5 - s, 9), (10, 4 - s), (16, 9), (22, 4 - s), (27 + s, 9), (27 + s, 15)]
    c.poly(pts, p("obs"))
    c.poly([(9, 8), (12, 6), (14, 10), (10, 13)], p("obs_hi"))
    c.line([(16, 12), (14, 17), (18, 20), (16, 24)], p("violet") if f % 2 == 0 else p("amber"))
    from common import aura
    return aura(c.done(), 1 + (f % 2), (150, 80, 220))


def fragmento_mural():
    c = cv32()
    c.poly([(4, 6), (26, 3), (29, 24), (8, 29), (2, 18)], p("clay_hi"))
    c.poly([(6, 8), (24, 5), (26, 22), (9, 26)], p("cream"))
    c.ell(12, 9, 20, 17, p("red")); c.ell(14, 11, 18, 15, p("yellow"))          # sol
    for a in range(0, 360, 45):
        x, y = 16 + 7 * math.cos(math.radians(a)), 13 + 7 * math.sin(math.radians(a))
        c.px(x, y, p("red"))
    c.rect(9, 20, 23, 21, p("blue"))
    return c.done()


def vida(f=0):
    c = cv32()
    s = [0, 1, 2, 1][f]
    c.poly([(16, 26 - s), (8, 16 - s), (8, 11 - s), (12, 8 - s), (16, 11 - s), (20, 8 - s), (24, 11 - s), (24, 16 - s)], p("red"))
    c.rect(10, 11 - s, 12, 13 - s, p("pink"))
    return c.done()


# ================================================================= PROYECTILES
def disparo(f):
    c = cv32()
    if f == 0:
        c.ell(0, 10, 14, 22, p("fire_hi")); c.ell(4, 12, 18, 20, p("fire"))
        for (x, y) in ((16, 11), (18, 16), (16, 21)):
            c.rect(x, y, x + 2, y + 1, p("amber_hi"))
    elif f == 1:
        for (x, y) in ((6, 9), (12, 14), (8, 19), (18, 12), (20, 18), (15, 22), (24, 15)):
            c.rect(x, y, x + 2, y + 1, p("amber_hi")); c.px(x - 1, y, p("fire"))
    else:
        for (x, y) in ((14, 7), (22, 13), (18, 20), (28, 11), (26, 21), (30, 16)):
            c.rect(x, y, x + 1, y, p("amber"))
    return c.done()


def astilla_proy():
    c = cv32(); c.poly([(4, 16), (26, 13), (30, 16), (26, 19)], p("obs")); c.line([(6, 16), (28, 15)], p("obs_hi"))
    return c.done()


def espina():
    c = cv32(); c.poly([(2, 16), (28, 14), (31, 16), (28, 18)], p("green")); c.line([(4, 16), (29, 15)], p("green_hi"))
    return c.done()


def laja(f):
    c = cv32(); r = f * 30
    pts = [(16 + 12 * math.cos(math.radians(a + r)), 16 + 6 * math.sin(math.radians(a + r))) for a in (0, 150, 210)]
    c.poly(pts, p("obs")); c.line([pts[0], pts[1]], p("obs_hi"))
    from common import aura
    return aura(c.done(), 1, (150, 80, 220))


# ================================================================= DECORACIÓN 32x32 (base en y=31)
def arbusto_seco():
    c = cv32()
    for (x0, x1, y1) in ((16, 6, 10), (16, 26, 8), (16, 12, 4), (16, 21, 3), (16, 3, 18), (16, 29, 17)):
        c.line([(x0, 31), (x1, y1)], p("dry"), 1)
    for (x, y) in ((6, 10), (26, 8), (12, 4), (21, 3), (3, 18), (29, 17), (9, 14), (23, 12)):
        c.line([(x, y), (x + 3, y - 2)], p("dry_sh"))
    c.ell(10, 24, 22, 31, p("dry_sh"))
    return c.done()


def arbusto_verde():
    c = cv32()
    c.ell(2, 14, 18, 31, p("green_sh")); c.ell(12, 10, 30, 31, p("green_sh")); c.ell(7, 6, 23, 26, p("green"))
    for (x, y) in ((10, 10), (16, 8), (20, 13), (8, 17), (24, 18), (14, 16)):
        c.rect(x, y, x + 2, y + 1, p("green_hi"))
    c.px(12, 20, p("red")); c.px(22, 12, p("red"))
    return c.done()


def maguey():
    c = cv32()
    leaves = [(16, 31, 2, 12), (16, 31, 8, 4), (16, 31, 14, 1), (16, 31, 20, 2), (16, 31, 26, 6), (16, 31, 30, 14)]
    for (x0, y0, x1, y1) in leaves:
        c.poly([(x0 - 3, y0), (x1, y1), (x0 + 3, y0)], p("agave"))
        c.line([(x0, y0 - 2), (x1, y1)], p("agave_hi"))
        c.px(x1, y1, p("black"))                                                   # púa
    c.ell(10, 25, 22, 31, p("agave_sh"))
    return c.done()


def nopal():
    c = cv32()
    c.ell(11, 14, 21, 31, p("green")); c.ell(3, 6, 13, 20, p("green")); c.ell(18, 2, 29, 17, p("green"))
    for (x, y) in ((14, 20), (17, 25), (6, 11), (9, 15), (22, 7), (25, 11)):
        c.px(x, y, p("cream"))
    c.ell(21, 0, 25, 4, p("pink")); c.ell(5, 4, 9, 8, p("red"))                   # tunas
    c.line([(13, 20), (13, 28)], p("green_sh"))
    return c.done()


def calavera():
    c = cv32()
    c.ell(7, 5, 25, 23, p("bone")); c.rect(10, 18, 22, 27, p("bone"))
    c.ell(10, 11, 15, 17, p("black")); c.ell(17, 11, 22, 17, p("black"))
    c.poly([(16, 18), (14, 21), (18, 21)], p("black"))
    for x in range(11, 22, 2):
        c.rect(x, 24, x, 27, p("bone_sh"))
    c.ell(9, 6, 15, 10, (248, 244, 232))
    c.line([(20, 6), (22, 10)], p("bone_sh"))
    return c.done()


def calavera_piedra():
    c = cv32()
    c.rect(4, 4, 28, 31, p("stone")); c.rect(4, 4, 28, 6, p("stone_hi")); c.rect(4, 28, 28, 31, p("stone_sh"))
    c.ell(8, 8, 24, 24, p("stone_hi")); c.rect(11, 20, 21, 26, p("stone_hi"))
    c.ell(10, 12, 15, 17, p("stone_sh")); c.ell(17, 12, 22, 17, p("stone_sh"))
    c.poly([(16, 18), (14, 21), (18, 21)], p("stone_sh"))
    for x in range(12, 21, 3):
        c.rect(x, 23, x, 26, p("stone_sh"))
    return c.done()


def huesos():
    c = cv32()
    for (a, b) in (((4, 28), (26, 20)), ((6, 20), (27, 29))):
        c.line([a, b], p("bone"), 3)
        for (x, y) in (a, b):
            c.ell(x - 2, y - 2, x + 2, y + 2, p("bone"))
    c.ell(18, 12, 28, 21, p("bone")); c.ell(20, 15, 22, 17, p("black")); c.ell(24, 15, 26, 17, p("black"))
    return c.done()


def vasija():
    c = cv32()
    c.ell(6, 10, 26, 31, p("clay")); c.rect(11, 5, 21, 11, p("clay")); c.rect(9, 4, 23, 6, p("clay_hi"))
    c.rect(7, 18, 25, 20, p("red_sh"));
    for x in range(8, 25, 4):
        c.rect(x, 21, x + 1, 23, p("cream"))
    c.ell(9, 12, 13, 22, p("clay_hi"))
    return c.done()


def olla_rota():
    c = cv32()
    c.poly([(4, 31), (6, 18), (12, 14), (16, 20), (20, 13), (26, 18), (28, 31)], p("clay"))
    c.poly([(8, 31), (9, 22), (14, 20), (14, 31)], p("clay_sh"))
    c.poly([(22, 27), (30, 26), (31, 31), (22, 31)], p("clay_hi"))
    c.rect(5, 23, 27, 24, p("red_sh"))
    return c.done()


def piedras():
    c = cv32()
    c.ell(2, 18, 18, 31, p("stone")); c.ell(14, 22, 30, 31, p("stone_sh")); c.ell(10, 12, 22, 24, p("stone_hi"))
    c.line([(6, 22), (10, 25)], p("stone_sh"))
    return c.done()


def cempasuchil():
    c = cv32()
    for (x, y, h) in ((8, 14, 17), (16, 8, 23), (24, 13, 18)):
        c.line([(x, 31), (x, y + 4)], p("green_sh"), 1)
        c.poly([(x, y + 12), (x - 4, y + 8), (x, y + 9)], p("green"))
        c.ell(x - 4, y - 4, x + 4, y + 4, p("marigold")); c.ell(x - 2, y - 2, x + 2, y + 2, p("marigold_sh"))
        for a in range(0, 360, 60):
            c.px(x + 4 * math.cos(math.radians(a)), y + 4 * math.sin(math.radians(a)), p("yellow"))
    return c.done()


def caja():
    c = cv32()
    c.rect(3, 8, 29, 31, p("wood")); c.rect(3, 8, 29, 10, p("wood_hi")); c.rect(3, 29, 29, 31, p("wood_sh"))
    c.line([(4, 11), (28, 28)], p("wood_sh"), 2); c.line([(4, 28), (28, 11)], p("wood_sh"), 2)
    c.rect(3, 8, 4, 31, p("wood_sh")); c.rect(28, 8, 29, 31, p("wood_sh"))
    return c.done()


def canasto():
    c = cv32()
    c.poly([(3, 12), (29, 12), (26, 31), (6, 31)], p("straw"))
    for y in range(14, 31, 4):
        c.line([(5, y), (27, y)], p("straw_sh"))
    for x in range(8, 26, 5):
        c.line([(x, 13), (x + 1, 30)], p("straw_sh"))
    c.arc([4, 0, 28, 24], 180, 360, p("wood"), 2)
    return c.done()


def estela():
    c = cv32()
    c.rect(8, 2, 24, 31, p("stone")); c.rect(8, 2, 24, 4, p("stone_hi")); c.rect(22, 2, 24, 31, p("stone_sh"))
    c.rect(11, 7, 20, 14, p("stone_sh")); c.ell(13, 8, 18, 13, p("stone_hi"))
    for y in (17, 21, 25):
        c.rect(11, y, 20, y + 1, p("stone_sh"))
    c.px(15, 10, p("amber"))
    return c.done()


def pua():
    c = cv32()
    for x in (2, 10, 18, 26):
        c.poly([(x, 31), (x + 2, 12), (x + 4, 31)], p("agave")); c.line([(x + 2, 13), (x + 2, 30)], p("agave_hi"))
        c.px(x + 2, 12, p("black"))
    c.rect(0, 29, 31, 31, p("dry_sh"))
    return c.done()


def bloque_tallable():
    c = cv32()
    c.rect(1, 1, 30, 30, p("clay_hi")); c.rect(1, 1, 30, 3, (220, 170, 120)); c.rect(1, 28, 30, 30, p("clay_sh"))
    c.line([(8, 6), (14, 14), (10, 20), (16, 26)], p("clay_sh")); c.line([(22, 8), (18, 14)], p("clay_sh"))
    c.rect(20, 19, 25, 24, p("clay")); c.px(22, 21, p("amber"))
    return c.done()


def antorcha(f):
    c = cv32()
    c.rect(14, 14, 17, 31, p("wood")); c.rect(12, 12, 19, 15, p("iron_sh"))
    flames = [[(15, 1), (10, 10), (15, 13), (21, 10)], [(17, 2), (11, 9), (15, 13), (20, 9)],
              [(14, 0), (10, 9), (16, 13), (21, 8)], [(16, 1), (11, 10), (16, 13), (20, 10)]][f]
    c.poly(flames, p("fire")); c.poly([(15, 6), (13, 10), (15, 12), (18, 10)], p("fire_hi"))
    from common import aura
    return aura(c.done(), 1, (255, 140, 50))


def brasero(f):
    c = cv32()
    c.rect(10, 22, 21, 31, p("stone_sh")); c.rect(4, 17, 27, 22, p("stone")); c.rect(4, 17, 27, 18, p("stone_hi"))
    for x in (7, 12, 17, 22):
        c.rect(x, 19, x + 1, 20, p("stone_sh"))
    flames = [[(16, 2), (8, 14), (16, 17), (24, 14)], [(14, 3), (9, 13), (16, 17), (23, 12)],
              [(17, 1), (8, 12), (16, 17), (24, 13)], [(15, 3), (9, 14), (16, 17), (23, 14)]][f]
    c.poly(flames, p("fire")); c.poly([(16, 8), (12, 14), (16, 16), (20, 14)], p("fire_hi"))
    from common import aura
    return aura(c.done(), 1, (255, 140, 50))


def brasero_apagado():
    c = cv32()
    c.rect(10, 22, 21, 31, p("stone_sh")); c.rect(4, 17, 27, 22, p("stone")); c.rect(4, 17, 27, 18, p("stone_hi"))
    for x in (7, 12, 17, 22):
        c.rect(x, 19, x + 1, 20, p("stone_sh"))
    c.ell(8, 13, 23, 19, p("black"))                       # carbones
    for (x, y) in ((11, 15), (15, 14), (19, 15)):
        c.rect(x, y, x + 1, y, p("clay_sh"))
    c.line([(14, 12), (13, 8), (15, 4)], (90, 84, 96))    # hilo de humo
    return c.done()


def ofrenda(f):
    """Calaverita de azúcar con vela."""
    c = cv32()
    c.ell(4, 12, 20, 28, p("cream")); c.rect(7, 24, 17, 30, p("cream"))
    c.ell(6, 16, 11, 21, p("blue")); c.ell(13, 16, 18, 21, p("pink"))
    c.px(12, 23, p("black")); c.rect(8, 27, 16, 27, p("purple"))
    c.ell(8, 12, 16, 15, p("yellow"))
    c.rect(23, 16, 27, 31, p("cream")); c.rect(24, 13, 25, 16, p("black"))
    fl = [[(24, 6), (22, 12), (25, 14), (27, 11)], [(25, 5), (22, 11), (25, 14), (27, 12)]][f % 2]
    c.poly(fl, p("fire")); c.px(25, 11, p("fire_hi"))
    return c.done()


# ================================================================= DECORACIÓN GRANDE 64x64
def tzompantli():
    c = Canvas(64, 64)
    for x in (4, 58):
        c.rect(x, 8, x + 3, 63, p("wood"))
    for y in (16, 34, 52):
        c.rect(2, y, 61, y + 1, p("wood_sh"))
        for x in range(9, 56, 9):
            c.ell(x - 4, y - 7, x + 4, y + 1, p("bone")); c.rect(x - 3, y, x + 3, y + 3, p("bone"))
            c.px(x - 2, y - 3, p("black")); c.px(x + 1, y - 3, p("black")); c.px(x - 1, y - 1, p("black"))
    c.rect(0, 6, 63, 8, p("wood"))
    return c.done()


def vagoneta():
    c = Canvas(64, 64)
    c.rect(0, 58, 63, 59, p("iron_sh"));
    for x in range(2, 64, 8):
        c.rect(x, 60, x + 4, 63, p("wood_sh"))
    c.poly([(6, 28), (58, 28), (54, 50), (10, 50)], p("iron")); c.rect(6, 28, 58, 31, p("iron_hi"))
    c.rect(8, 36, 56, 38, p("iron_sh")); c.px(12, 33, p("rust")); c.px(46, 42, p("rust")); c.px(30, 45, p("rust"))
    for (x, y, r) in ((20, 22, 6), (32, 20, 7), (44, 23, 5)):
        c.ell(x - r, y - r, x + r, y + r, p("stone"))
    for x in (18, 46):
        c.ell(x - 6, 46, x + 6, 58, p("black")); c.ell(x - 3, 49, x + 3, 55, p("iron_sh"))
    return c.done()


def mezquite():
    c = Canvas(64, 64)
    c.line([(30, 63), (32, 40), (26, 28), (18, 18)], p("wood_sh"), 4)
    c.line([(32, 40), (40, 26), (50, 18)], p("wood_sh"), 3)
    c.line([(28, 30), (34, 18)], p("wood_sh"), 2)
    for (x, y, rx, ry) in ((16, 14, 12, 7), (36, 10, 14, 8), (52, 16, 10, 6), (28, 20, 10, 5)):
        c.ell(x - rx, y - ry, x + rx, y + ry, p("green_sh"))
        c.ell(x - rx + 3, y - ry + 1, x + rx - 4, y + ry - 3, p("dry"))
    return c.done()


def papel_picado():
    c = Canvas(64, 64)
    c.line([(0, 6), (32, 12), (63, 6)], p("black"), 1)
    colors = ["pink", "yellow", "blue", "marigold", "purple", "green"]
    for i, x in enumerate(range(2, 60, 10)):
        y = 7 + int(5 * math.sin(math.pi * (x + 4) / 64))
        c.rect(x, y, x + 8, y + 14, p(colors[i % len(colors)]))
        c.rect(x + 3, y + 4, x + 5, y + 6, (0, 0, 0, 0))          # recorte
        c.rect(x + 2, y + 9, x + 6, y + 10, (0, 0, 0, 0))
        for zx in range(x, x + 9, 2):
            c.px(zx, y + 14, (0, 0, 0, 0))
    return c.done()


def build(out):
    save_sheet(out, "Armas", {
        "pico": [pico()], "pala": [pala()], "escopeta": [escopeta()], "mazo": [mazo()], "astillas": [astillas()],
        "escopeta_pickup": [escopeta(1), escopeta(2), escopeta(3), escopeta(2)],
        "corazon_obsidiana": [corazon_obsidiana(i) for i in range(4)],
        "fragmento_mural": [fragmento_mural()], "vida": [vida(i) for i in range(4)],
    }, 32, 32, 6)
    save_sheet(out, "Proyectiles", {
        "disparo": [disparo(0), disparo(1), disparo(2)], "astilla": [astilla_proy()], "espina": [espina()],
        "laja": [laja(0), laja(1), laja(2)],
    }, 32, 32, 6)
    save_sheet(out, "Decoracion", {
        "arbusto_seco": [arbusto_seco()], "arbusto_verde": [arbusto_verde()], "maguey": [maguey()], "nopal": [nopal()],
        "calavera": [calavera()], "calavera_piedra": [calavera_piedra()], "huesos": [huesos()], "vasija": [vasija()],
        "olla_rota": [olla_rota()], "piedras": [piedras()], "cempasuchil": [cempasuchil()], "caja": [caja()],
        "canasto": [canasto()], "estela": [estela()], "puas": [pua()], "bloque_tallable": [bloque_tallable()],
        "antorcha": [antorcha(i) for i in range(4)], "brasero": [brasero(i) for i in range(4)],
        "ofrenda": [ofrenda(0), ofrenda(1)],
        "brasero_apagado": [brasero_apagado()],
    }, 32, 32, 6)
    save_sheet(out, "DecoracionGrande", {
        "tzompantli": [tzompantli()], "vagoneta": [vagoneta()], "mezquite": [mezquite()], "papel_picado": [papel_picado()],
    }, 64, 64, 4)


if __name__ == "__main__":
    build(sys.argv[1] if len(sys.argv) > 1 else "out")
    print("ok")
