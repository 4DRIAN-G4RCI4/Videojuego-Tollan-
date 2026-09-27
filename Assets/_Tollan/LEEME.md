# TOLLAN – Prototipo

## Cómo generar el Nivel 0
1. Menú **Tools > Tollan > 2. Generar Nivel 0 (prototipo)**
2. Se abre la escena `Scenes/Nivel0_TallerDeCantera` → **Play**

## Controles
| Acción | Teclado | Control |
|---|---|---|
| Mover | A / D o flechas | Stick / cruceta |
| Saltar (mantener = más alto) | Espacio / Z / W / ↑ | A (sur) |
| Atacar con el pico | X / J / Clic | X (oeste) |
| Interactuar / avanzar diálogo | E / Enter | Y (norte) |

## Estructura
- `Scripts/Core` – input, vida, GameManager, cámara, parallax
- `Scripts/Player` – movimiento y combate de Ixtli
- `Scripts/World` – púas, bloques tallables, braseros, coleccionables, diálogos, salida
- `Scripts/Enemies` – enemigo patrullero base y muñeco de práctica
- `Scripts/UI` – HUD y cuadro de diálogo
- `Editor` – generador de niveles (menú Tools > Tollan)
- `Art/Placeholder` – cuadros de color temporales; se reemplazan por los sprites finales

## Cambiar placeholders por arte final
Selecciona el objeto (ej. `Ixtli`) y en **Sprite Renderer** cambia *Sprite*, pon *Draw Mode = Simple* y *Color = blanco*.
Importa los sprites con **Pixels Per Unit = 32**, **Filter Mode = Point**, **Compression = None**.
