"""Corta cada *_sheet.png según su *_sheet.txt y exporta frames PNG + un GIF por animación."""
import os, sys, glob
from PIL import Image

BG = (26, 20, 48, 255)
FPS = {"idle": 6, "walk": 10, "fly": 10, "float": 6, "attack": 14, "slam": 8, "throw": 8, "charge": 8,
       "grab": 8, "ring": 8, "swarm": 6, "jump": 8, "fall": 8, "hurt": 8, "block": 6, "phase3": 6, "dive": 8}
ES = {"idle": "quieto", "walk": "caminar", "jump": "salto", "fall": "caida", "attack": "ataque", "hurt": "dano",
      "charge": "embestida", "fly": "vuelo", "dive": "picada", "block": "bloqueo", "slam": "golpe_suelo",
      "throw": "lanza_obsidiana", "phase3": "fase3", "shoot": "disparo", "grab": "agarre", "ring": "campanazo", "float": "flotar", "swarm": "enjambre"}


def read_manifest(path):
    lines = open(path).read().split("\n")
    fw, fh = [int(v) for v in lines[0].split()[1].split("x")]
    anims = []
    for l in lines[1:]:
        if not l.strip():
            continue
        name, *rest = l.split()
        d = dict(p.split("=") for p in rest)
        anims.append((name, int(d["row"]), int(d["frames"])))
    return fw, fh, anims


def export(src_dir, dst_dir):
    for man in sorted(glob.glob(os.path.join(src_dir, "*_sheet.txt"))):
        char = os.path.basename(man)[:-len("_sheet.txt")]
        sheet = Image.open(os.path.join(src_dir, f"{char}_sheet.png"))
        fw, fh, anims = read_manifest(man)
        scale = max(2, 384 // max(fw, fh))
        for anim, row, n in anims:
            folder = os.path.join(dst_dir, "Frames", char)
            os.makedirs(folder, exist_ok=True)
            frames = []
            for i in range(n):
                f = sheet.crop((i * fw, row * fh, i * fw + fw, row * fh + fh))
                f.save(os.path.join(folder, f"{char}_{anim}_{i}.png"))
                frames.append(f)
            gif_dir = os.path.join(dst_dir, "GIFs", char)
            os.makedirs(gif_dir, exist_ok=True)
            seq = frames if n > 1 else frames * 2
            big = [Image.alpha_composite(Image.new("RGBA", (fw, fh), BG), f).resize((fw * scale, fh * scale), Image.NEAREST) for f in seq]
            dur = int(1000 / FPS.get(anim, 8))
            big[0].save(os.path.join(gif_dir, f"{char}_{ES.get(anim, anim)}.gif"), save_all=True,
                        append_images=big[1:], duration=dur, loop=0, disposal=2)
        print("ok", char, [a for a, _, _ in anims])


if __name__ == "__main__":
    export(sys.argv[1], sys.argv[2])
