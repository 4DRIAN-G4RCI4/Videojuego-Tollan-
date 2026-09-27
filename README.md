# TOLLAN: El Despertar de los Atlantes

Videojuego 2D de plataformas · Desarrollo de Videojuegos · UTTT

## Abrir el proyecto
1. Instala **Unity 6000.3.24f1** (misma versión exacta) desde Unity Hub.
2. Clona este repo (GitHub Desktop → *File → Clone repository*).
3. Unity Hub → **Add → Add project from disk** → elige la carpeta clonada.
   La primera vez tarda unos minutos (Unity regenera `Library` y baja los paquetes).
4. Abre la escena `Assets/_Tollan/Scenes/Nivel0_TallerDeCantera` y dale **Play**.

Si faltan sprites o animaciones: **Tools → Tollan → 3. Importar sprites y crear animaciones** y luego
**Tools → Tollan → 2. Generar Nivel 0**.

## IDE
Unity → **Edit → Preferences → External Tools → External Script Editor** (Visual Studio / VS Code / Rider)
y doble clic a cualquier script. El `.sln` se genera solo.

## Controles
| Acción | Teclado | Control |
|---|---|---|
| Mover | A/D o flechas | Stick / cruceta |
| Saltar (mantener = más alto) | Espacio / Z / W / ↑ | A |
| Atacar (pico / pala) | X / J / Clic izq. | X |
| Escopeta | K / Clic der. | B |
| Interactuar | E / Enter | Y |
| Cambiar personaje | C | RB |
| Silenciar música | M | — |

## Estructura
- `Assets/_Tollan/Scripts` – código del juego
- `Assets/_Tollan/Editor` – generadores (menú **Tools → Tollan**)
- `Assets/_Tollan/Art` – sprites (propios + `ThirdParty`)
- `Tools/sprites` – scripts de Python que generan el pixel art

## Reglas del equipo
- **No editen la misma escena al mismo tiempo.**
- Antes de trabajar: *Pull*. Al terminar: *Commit + Push*.
- Suban siempre los archivos `.meta`.

## Licencias
Ver `Assets/_Tollan/CREDITOS.md`. **Este repositorio debe ser privado** (licencia de GandalfHardcore).
