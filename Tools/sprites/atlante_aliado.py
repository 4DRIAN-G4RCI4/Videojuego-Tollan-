"""El Atlante guardián (aliado): misma figura que el jefe, pero sin corrupción y con grietas ámbar."""
import sys
from atlante_boss import draw, W, H
from common import save_sheet

ANIMS = {
    "sleep": [draw(ally=True, glow="b1")],
    "idle": [draw(ally=True, glow="amber_dim"), draw(ally=True, bob=1, glow="amber")],
}

if __name__ == "__main__":
    save_sheet(sys.argv[1] if len(sys.argv) > 1 else "out", "AtlanteAliado", ANIMS, W, H, 3)
    print("ok")
