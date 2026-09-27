using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>
/// Menú  Tools > Tollan  :
///   1. Crear carpetas del proyecto
///   2. Generar Nivel 0 (prototipo)  -> arma la escena jugable con placeholders
/// Los placeholders (cuadros de color) se reemplazan después por los sprites finales.
/// </summary>
public static class TollanLevel0Builder
{
    const string Root = "Assets/_Tollan";
    const string ScenePath = Root + "/Scenes/Nivel0_TallerDeCantera.unity";
    const string PlaceholderDir = Root + "/Art/Placeholder";

    static Sprite square, triangle, circle;

    // Paleta (propuesta: Taller / Ruinas = ocres, café, azul noche)
    static readonly Color Tierra = Hex("6b4a32");
    static readonly Color TierraTop = Hex("8a6a4a");
    static readonly Color Piedra = Hex("7d7266");
    static readonly Color PiedraOscura = Hex("3b3440");
    static readonly Color Madera = Hex("a0703c");
    static readonly Color Ambar = Hex("ffa53a");
    static readonly Color Basalto = Hex("4a4a58");
    static readonly Color Noche = Hex("1a1430");

    // ------------------------------------------------------------------ Carpetas
    [MenuItem("Tools/Tollan/1. Crear carpetas del proyecto")]
    public static void CreateFolders()
    {
        string[] folders =
        {
            "Art/Characters", "Art/Enemies", "Art/Bosses", "Art/Tilesets", "Art/Backgrounds",
            "Art/Props", "Art/UI", "Art/VFX", "Art/Placeholder",
            "Audio/Music", "Audio/SFX", "Animations", "Materials", "Prefabs", "Scenes", "Video"
        };
        foreach (var f in folders) EnsureFolder(Root + "/" + f);
        AssetDatabase.Refresh();
        Debug.Log("[Tollan] Carpetas listas en " + Root);
    }

    static void EnsureFolder(string path)
    {
        var parts = path.Split('/');
        string cur = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = cur + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(cur, parts[i]);
            cur = next;
        }
    }

    // ------------------------------------------------------------------ Nivel 0
    [MenuItem("Tools/Tollan/2. Generar Nivel 0 (prototipo)")]
    public static void BuildLevel0()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Tollan", "Primero detén el Play (botón ▶ de arriba) y vuelve a intentarlo.", "OK");
            return;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (File.Exists(ScenePath) &&
            !EditorUtility.DisplayDialog("Tollan", "Ya existe el Nivel 0. ¿Reemplazarlo?", "Sí, reemplazar", "Cancelar"))
            return;

        CreateFolders();
        square = ShapeSprite("square.png", (x, y) => true);
        triangle = ShapeSprite("triangle.png", (x, y) => Mathf.Abs(x - 15.5f) <= (31 - y) * 0.5f + 0.5f);
        circle = ShapeSprite("circle.png", (x, y) => (x - 15.5f) * (x - 15.5f) + (y - 15.5f) * (y - 15.5f) <= 16f * 16f);
        var noFriction = NoFrictionMaterial();
        LoadArt();
        if (S("Ixtli_idle_0") == null)
            Debug.LogWarning("[Tollan] No encontré los sprites cortados. Corre primero  Tools > Tollan > 3. Importar sprites y crear animaciones  (se usarán cuadros de color).");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // --- Managers y luz
        var gm = new GameObject("GameManager").AddComponent<GameManager>();

        var globalLight = new GameObject("Luz Global 2D").AddComponent<Light2D>();
        globalLight.lightType = Light2D.LightType.Global;
        globalLight.color = Hex("b8c0ff");
        globalLight.intensity = 0.8f;

        // --- Cámara
        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5.625f;                // 360 px / 32 ppu / 2
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Noche;
        camGO.AddComponent<AudioListener>();
        camGO.AddComponent<UniversalAdditionalCameraData>();
        var ppc = camGO.AddComponent<PixelPerfectCamera>();
        ppc.assetsPPU = 32;
        ppc.refResolutionX = 640;
        ppc.refResolutionY = 360;

        // --- Fondo con parallax
        BuildBackground();

        // --- Nivel
        var level = new GameObject("Nivel").transform;
        BuildLevelGeometry(level);

        // --- Jugador
        var player = BuildPlayer(new Vector2(-3f, 1f), noFriction);
        gm.checkpoint = player.transform.position;

        camGO.transform.position = new Vector3(3.5f, 3f, -10f);
        var follow = camGO.AddComponent<CameraFollow>();
        follow.target = player.transform;
        follow.minX = 3.5f;   // pared izquierda (-6.5) + medio ancho de cámara (10)
        follow.maxX = 84.5f;  // pared derecha (94.5) - 10
        follow.minY = 3f;
        follow.maxY = 12f;

        // --- Música de fondo
        var music = AssetDatabase.LoadAssetAtPath<AudioClip>(Root + "/Audio/Music/Musica_Tutorial.ogg");
        if (music != null)
        {
            var mGO = new GameObject("Musica");
            mGO.AddComponent<AudioSource>();
            mGO.AddComponent<MusicPlayer>().clip = music;
        }

        // --- UI
        BuildUI(player.GetComponent<Health>());

        // --- Guardar
        EditorSceneManager.SaveScene(scene, ScenePath);
        var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        scenes.RemoveAll(s => s.path == ScenePath);
        scenes.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();

        Selection.activeGameObject = player;
        Debug.Log("[Tollan] Nivel 0 generado: " + ScenePath + "  -> dale Play");
    }

    // ------------------------------------------------------------------ Fondo
    static void BuildBackground()
    {
        var bg = new GameObject("Fondo").transform;

        var cielo = ImportSingle(Root + "/Art/ThirdParty/GandalfHardcore/Fondo_Cielo.png");
        var montanas = ImportSingle(Root + "/Art/ThirdParty/GandalfHardcore/Fondo_Montanas.png");
        if (cielo != null)
        {
            var l = Layer("Capa_Cielo", bg, 1f, 1f);
            var go = SpriteObj("Cielo", new Vector2(35f, 4.5f), cielo, l, -70);
            var sr = go.GetComponent<SpriteRenderer>();
            sr.drawMode = SpriteDrawMode.Tiled; sr.size = new Vector2(160f, cielo.bounds.size.y);
            sr.color = new Color(0.32f, 0.26f, 0.62f);
        }
        if (montanas != null)
        {
            var l = Layer("Capa_Montanas", bg, 0.88f, 0.7f);
            var go = SpriteObj("Montanas", new Vector2(35f, 0.2f), montanas, l, -45);
            var sr = go.GetComponent<SpriteRenderer>();
            sr.drawMode = SpriteDrawMode.Tiled; sr.size = new Vector2(160f, montanas.bounds.size.y);
            sr.color = new Color(0.40f, 0.30f, 0.52f);
        }

        var stars = Layer("Capa_Estrellas", bg, 0.95f, 0.9f);
        var rnd = new System.Random(7);
        for (int i = 0; i < 40; i++)
        {
            float x = (float)rnd.NextDouble() * 110f - 10f;
            float y = 4.5f + (float)rnd.NextDouble() * 4f;
            float s = 0.08f + (float)rnd.NextDouble() * 0.1f;
            var st = SpriteObj("Estrella", new Vector2(x, y), circle, stars, -60);
            st.transform.localScale = new Vector3(s, s, 1f);
            st.GetComponent<SpriteRenderer>().color = Hex("fff3d6");
            var gp = st.AddComponent<GlowPulse>();
            gp.speed = 1f + (float)rnd.NextDouble() * 2f;
            gp.minAlpha = 0.2f;
        }

        var moonLayer = Layer("Capa_Luna", bg, 0.97f, 0.9f);
        foreach (var (n, size, c, order) in new[] {
            ("Halo", 4.4f, new Color(0.97f, 0.85f, 0.63f, 0.10f), -56),
            ("Luna", 2.4f, Hex("f7e2b8"), -55) })
        {
            var m = SpriteObj(n, new Vector2(-1.5f, 6.4f), circle, moonLayer, order);
            m.transform.localScale = new Vector3(size, size, 1f);
            m.GetComponent<SpriteRenderer>().color = c;
        }
        var crater = SpriteObj("Crater", new Vector2(-1.9f, 6.6f), circle, moonLayer, -54);
        crater.transform.localScale = new Vector3(0.45f, 0.45f, 1f);
        crater.GetComponent<SpriteRenderer>().color = Hex("e6c88e");

        if (montanas == null)
        {
            var far = Layer("Capa_Cerros", bg, 0.8f, 0.6f);
            float[] heights = { 4, 6, 5, 7, 4.5f, 6.5f, 5, 7.5f, 4, 6, 5.5f, 7, 4.5f, 6 };
            for (int i = 0; i < heights.Length; i++)
                Box("Cerro", new Vector2(-10f + i * 9f, heights[i] * 0.5f - 1f), new Vector2(10f, heights[i]), Hex("3d1c38"), far, false, -40);
        }

        var mid = Layer("Capa_Ruinas", bg, 0.5f, 0.3f);
        for (int i = 0; i < 16; i++)
        {
            if (i % 2 == 1) continue;
            float h = 2f + (i * 37 % 4);
            float cx = -8f + i * 6.5f;
            Box("Columna", new Vector2(cx, h * 0.5f), new Vector2(0.7f, h), Hex("241428"), mid, false, -30);
            Box("Capitel", new Vector2(cx, h + 0.15f), new Vector2(1.1f, 0.3f), Hex("241428"), mid, false, -30);
            if (i % 4 == 0)
            {
                Box("Columna", new Vector2(cx + 2.6f, h * 0.5f), new Vector2(0.7f, h), Hex("241428"), mid, false, -30);
                Box("Dintel", new Vector2(cx + 1.3f, h + 0.45f), new Vector2(3.6f, 0.4f), Hex("241428"), mid, false, -30);
            }
        }
    }

    static Transform Layer(string name, Transform parent, float fx, float fy)
    {
        var t = new GameObject(name).transform;
        t.SetParent(parent, false);
        var p = t.gameObject.AddComponent<ParallaxLayer>();
        p.factorX = fx;
        p.factorY = fy;
        return t;
    }

    // ------------------------------------------------------------------ Geometría
    static void BuildLevelGeometry(Transform level)
    {
        // Suelos (top en y = 0). Huecos: 26-29 y 60-63
        if (S("FloorTiles1_6_1") != null)
        {
            BuildTileGround(level);
        }
        else
        {
            Ground("Suelo_1", -6.5f, 26f, level);
            Ground("Suelo_2", 29f, 60f, level);
            Ground("Suelo_3", 63f, 94.5f, level);
            Box("Pared_Izq", new Vector2(-7f, 5f), new Vector2(1f, 12f), PiedraOscura, level, true, 5);
            Box("Pared_Der", new Vector2(95f, 5f), new Vector2(1f, 12f), PiedraOscura, level, true, 5);
        }
        PlaceDecor(level);
        PlaceHints(level);

        // Abuelo Nabor (diálogo inicial automático)
        GameObject abuelo;
        if (S("AbueloNabor_idle_0") != null)
        {
            abuelo = SpriteObj("Abuelo Nabor", new Vector2(1.5f, 0f), S("AbueloNabor_idle_0"), level, 8);
            abuelo.GetComponent<SpriteRenderer>().flipX = true;          // mira hacia Ixtli
            AddAnimator(abuelo, "AbueloNabor");
            TriggerZone(abuelo, new Vector2(3f, 2.5f), new Vector2(0f, 1.25f));
        }
        else
        {
            abuelo = Box("Abuelo Nabor", new Vector2(1.5f, 0.7f), new Vector2(0.9f, 1.4f), Hex("c9b89a"), level, false, 8);
            Box("Sombrero", new Vector2(0f, 0.8f), new Vector2(1.3f, 0.25f), Hex("d9c07a"), abuelo.transform, false, 9);
            Box("Mazo", new Vector2(0.55f, 0.3f), new Vector2(0.2f, 1.2f), Madera, abuelo.transform, false, 9);
            TriggerZone(abuelo, new Vector2(3f, 2.5f), new Vector2(0f, 0.5f));
        }
        var dt = abuelo.AddComponent<DialogueTrigger>();
        dt.autoStart = true;
        dt.lines = new[]
        {
            new DialogueLine("Abuelo Nabor", "Ixtli, ya es tarde... termina de limpiar la base del Atlante y nos vamos a casa."),
            new DialogueLine("Abuelo Nabor", "Fíjate en los letreros de arriba: te dicen cómo moverte y qué hace cada cosa."),
            new DialogueLine("Abuelo Nabor", "Y ojo con los huecos de la cantera, mija. Las estelas cuentan cosas del pasado."),
            new DialogueLine("Ixtli", "Sí, abuelo. Ahorita voy."),
        };

        // Obstáculo pequeño y andamios (plataformas de un solo sentido)
        if (S("Decoracion_caja_0") != null)
        {
            var caja = SpriteObj("Caja", new Vector2(6.5f, 0f), S("Decoracion_caja_0"), level, 4);
            var cc = caja.AddComponent<BoxCollider2D>(); cc.size = new Vector2(0.85f, 0.75f); cc.offset = new Vector2(0f, 0.375f);
        }
        else Box("Bloque_Piedra", new Vector2(6.5f, 0.5f), new Vector2(1f, 1f), Piedra, level, true, 4);
        Platform("Andamio_1", new Vector2(10f, 1.8f), 3f, level);
        Platform("Andamio_2", new Vector2(13.5f, 3.2f), 3f, level);
        Platform("Andamio_3", new Vector2(17.5f, 4.5f), 3f, level);
        Platform("Andamio_4", new Vector2(21.5f, 3f), 3f, level);
        Scaffold(new Vector2(10f, 1.8f), level);
        Scaffold(new Vector2(13.5f, 3.2f), level);
        Scaffold(new Vector2(17.5f, 4.5f), level);
        Scaffold(new Vector2(21.5f, 3f), level);

        // Fragmento de mural arriba del andamio más alto
        GameObject mural;
        if (S("Armas_fragmento_mural_0") != null)
        {
            mural = SpriteObj("Fragmento_Mural_1", new Vector2(17.5f, 5.1f), S("Armas_fragmento_mural_0"), level, 12);
            TriggerZone(mural, new Vector2(1f, 1f), new Vector2(0f, 0.5f));
        }
        else
        {
            mural = Box("Fragmento_Mural_1", new Vector2(17.5f, 5.6f), new Vector2(0.6f, 0.6f), Hex("e8c79a"), level, false, 12);
            mural.transform.rotation = Quaternion.Euler(0, 0, 45);
            TriggerZone(mural, new Vector2(1f, 1f), Vector2.zero);
        }
        var col = mural.AddComponent<Collectible>();
        col.kind = Collectible.Kind.MuralFragment;
        col.pickupMessage = "Fragmento de mural 1/12:  «Tollan, la ciudad del sol.»";

        // Brasero 1 (después del primer hueco)
        Brasero("Brasero_1", 31f, level);

        // Estela con pista
        GameObject estela;
        if (S("Decoracion_estela_0") != null)
        {
            estela = new GameObject("Estela");
            estela.transform.SetParent(level, false);
            estela.transform.localPosition = new Vector3(34f, 0f, 0f);
            var ev = SpriteObj("Visual", Vector2.zero, S("Decoracion_estela_0"), estela.transform, 3);
            ev.transform.localScale = new Vector3(1.8f, 1.8f, 1f);
            TriggerZone(estela, new Vector2(2.4f, 2.5f), new Vector2(0f, 1.25f));
        }
        else
        {
            estela = Box("Estela", new Vector2(34f, 1f), new Vector2(0.9f, 2f), Piedra, level, false, 3);
            Box("Glifo", new Vector2(0f, 0.3f), new Vector2(0.5f, 0.5f), PiedraOscura, estela.transform, false, 4);
            TriggerZone(estela, new Vector2(2.4f, 2.5f), Vector2.zero);
        }
        var dEst = estela.AddComponent<DialogueTrigger>();
        dEst.oneShot = false;
        dEst.promptIcon = PromptIcon(estela.transform, S("Decoracion_estela_0") != null ? 2.4f : 1.6f);
        dEst.lines = new[]
        {
            new DialogueLine("Estela", "«Donde arde el brasero, la memoria se guarda. Si caes, volverás a su fuego.»"),
        };

        // Púas
        Spikes("Puas_1", new Vector2(38f, 0.4f), 3f, level);

        // Atlante (entrega el Pico)
        BuildAtlante(new Vector2(46f, 0f), level);

        // Muro de bloques tallables (hay que romperlo con el pico)
        Box("Columna_Techo", new Vector2(51f, 7.5f), new Vector2(1.2f, 9f), PiedraOscura, level, true, 5);
        for (int i = 0; i < 3; i++)
        {
            GameObject b;
            if (S("Decoracion_bloque_tallable_0") != null)
            {
                b = SpriteObj("Bloque_Tallable_" + (i + 1), new Vector2(51f, i), S("Decoracion_bloque_tallable_0"), level, 6);
                var bc = b.AddComponent<BoxCollider2D>(); bc.size = Vector2.one; bc.offset = new Vector2(0f, 0.5f);
            }
            else
            {
                b = Box("Bloque_Tallable_" + (i + 1), new Vector2(51f, 0.5f + i), new Vector2(1f, 1f), Hex("b08a5a"), level, true, 6);
                Box("Grieta", new Vector2(0.1f, 0.05f), new Vector2(0.5f, 0.12f), Hex("6b4a32"), b.transform, false, 7);
            }
            b.AddComponent<BreakableBlock>().hits = 2;
        }

        // Muñeco de práctica
        GameObject dummy;
        if (S("MunecoPractica_idle_0") != null)
        {
            dummy = SpriteObj("Muneco_Practica", new Vector2(56f, 0f), S("MunecoPractica_idle_0"), level, 6);
            var dc = dummy.AddComponent<BoxCollider2D>(); dc.size = new Vector2(0.7f, 1.4f); dc.offset = new Vector2(0f, 0.7f);
        }
        else
        {
            dummy = Box("Muneco_Practica", new Vector2(56f, 0.7f), new Vector2(0.8f, 1.4f), Hex("c9a36b"), level, true, 6);
            Box("Cabeza", new Vector2(0f, 0.95f), new Vector2(0.6f, 0.5f), Hex("d9b37b"), dummy.transform, false, 7);
        }
        var dh = dummy.AddComponent<Health>();
        dh.maxHealth = 5;
        dh.invulnerableTime = 0.05f;
        dummy.AddComponent<TrainingDummy>();

        // Brasero 2 (después del segundo hueco) y tramo final
        Brasero("Brasero_2", 65f, level);
        BuildItzcoatlNPC(new Vector2(67.2f, 0f), level);
        Spikes("Puas_2", new Vector2(69.5f, 0.4f), 2f, level);
        BuildShotgunSection(level);
        Platform("Andamio_5", new Vector2(69.5f, 2.6f), 3f, level);
        Scaffold(new Vector2(69.5f, 2.6f), level);

        // Salida (arco)
        var exit = new GameObject("Salida_Arco");
        exit.transform.SetParent(level, false);
        exit.transform.localPosition = new Vector3(91f, 0f, 0f);
        Box("Pilar_Izq", new Vector2(-1.2f, 1.75f), new Vector2(0.6f, 3.5f), Piedra, exit.transform, false, 2);
        Box("Pilar_Der", new Vector2(1.2f, 1.75f), new Vector2(0.6f, 3.5f), Piedra, exit.transform, false, 2);
        Box("Dintel", new Vector2(0f, 3.8f), new Vector2(3.2f, 0.6f), Piedra, exit.transform, false, 2);
        var exitCol = exit.AddComponent<BoxCollider2D>();
        exitCol.isTrigger = true;
        exitCol.size = new Vector2(1.8f, 3f);
        exitCol.offset = new Vector2(0f, 1.5f);
        var le = exit.AddComponent<LevelExit>();
        le.message = "¡Tutorial completado!\nRumbo del Oriente: El Sendero de Maguey";
        le.nextScene = "Nivel1_SenderoDeMaguey";
    }

    static void Ground(string name, float fromX, float toX, Transform parent)
    {
        float w = toX - fromX;
        var g = Box(name, new Vector2(fromX + w * 0.5f, -1.5f), new Vector2(w, 3f), Tierra, parent, true, 0);
        Box("Borde", new Vector2(0f, 1.35f), new Vector2(w, 0.3f), TierraTop, g.transform, false, 1);
    }

    static void Platform(string name, Vector2 center, float width, Transform parent)
    {
        var p = Box(name, center, new Vector2(width, 0.35f), Madera, parent, true, 3);
        p.GetComponent<BoxCollider2D>().usedByEffector = true;
        var eff = p.AddComponent<PlatformEffector2D>();
        eff.useOneWay = true;
        eff.surfaceArc = 160f;
    }

    static void Scaffold(Vector2 top, Transform parent)
    {
        float h = top.y;
        if (h <= 0.2f) return;
        Box("Poste", new Vector2(top.x - 1.2f, h * 0.5f), new Vector2(0.15f, h), Hex("7a5530"), parent, false, 2);
        Box("Poste", new Vector2(top.x + 1.2f, h * 0.5f), new Vector2(0.15f, h), Hex("7a5530"), parent, false, 2);
    }

    static void Spikes(string name, Vector2 center, float width, Transform parent)
    {
        GameObject s;
        BoxCollider2D c;
        if (S("Decoracion_puas_0") != null)
        {
            s = SpriteObj(name, new Vector2(center.x, 0f), S("Decoracion_puas_0"), parent, 4);
            var sr = s.GetComponent<SpriteRenderer>();
            sr.drawMode = SpriteDrawMode.Tiled; sr.size = new Vector2(width, 1f);
            c = s.AddComponent<BoxCollider2D>();
            c.isTrigger = true; c.size = new Vector2(width * 0.9f, 0.45f); c.offset = new Vector2(0f, 0.25f);
        }
        else
        {
            s = Box(name, center, new Vector2(width, 0.8f), Hex("9fa36b"), parent, false, 4, triangle);
            c = s.AddComponent<BoxCollider2D>();
            c.isTrigger = true;
            c.size = new Vector2(width * 0.95f, 0.45f);
            c.offset = new Vector2(0f, -0.15f);
        }
        s.AddComponent<Hazard>().damage = 1;
    }

    static void Brasero(string name, float x, Transform parent)
    {
        var root = new GameObject(name);
        root.transform.SetParent(parent, false);
        root.transform.localPosition = new Vector3(x, 0f, 0f);
        GameObject fire, unlit = null;
        var lit = Animated("Brasero_Encendido", Vector2.zero, "Decoracion_brasero", 4, 8f, root.transform, 5);
        if (lit != null)
        {
            lit.transform.localScale = new Vector3(1.6f, 1.6f, 1f);
            fire = lit;
            var apagado = S("Decoracion_brasero_apagado_0");
            unlit = SpriteObj("Brasero_Apagado", Vector2.zero, apagado != null ? apagado : S("Decoracion_brasero_0"), root.transform, 4);
            unlit.transform.localScale = new Vector3(1.6f, 1.6f, 1f);
            if (apagado == null) unlit.GetComponent<SpriteRenderer>().color = new Color(0.45f, 0.45f, 0.5f);
        }
        else
        {
            Box("Base", new Vector2(0f, 0.3f), new Vector2(0.9f, 0.6f), PiedraOscura, root.transform, false, 4);
            Box("Cuenco", new Vector2(0f, 0.7f), new Vector2(1.2f, 0.25f), Piedra, root.transform, false, 4);
            fire = Box("Fuego", new Vector2(0f, 1.15f), new Vector2(0.55f, 0.65f), Ambar, root.transform, false, 5, triangle);
            fire.AddComponent<GlowPulse>().flicker = true;
        }

        var lightGO = new GameObject("Luz");
        lightGO.transform.SetParent(root.transform, false);
        lightGO.transform.localPosition = new Vector3(0f, 1.2f, 0f);
        var l = lightGO.AddComponent<Light2D>();
        l.lightType = Light2D.LightType.Point;
        l.color = Ambar;
        l.intensity = 1.2f;
        l.pointLightInnerRadius = 0.5f;
        l.pointLightOuterRadius = 4.5f;
        var lp = lightGO.AddComponent<GlowPulse>();
        lp.flicker = true;
        lp.lightMin = 0.9f;
        lp.lightMax = 1.4f;

        var bc = root.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(1.6f, 3f);
        bc.offset = new Vector2(0f, 1.5f);
        var cp = root.AddComponent<Checkpoint>();
        cp.fire = fire.GetComponent<SpriteRenderer>();
        cp.fireLight = l;
        cp.unlitVisual = unlit;
    }

    static void BuildAtlante(Vector2 feet, Transform parent)
    {
        var root = new GameObject("Atlante");
        root.transform.SetParent(parent, false);
        root.transform.localPosition = feet;
        var t = root.transform;
        const int o = -5; // detrás del jugador

        Animator atlAnim = null;
        bool atlSprite = S("AtlanteAliado_sleep_0") != null;
        if (atlSprite)
        {
            var body = SpriteObj("Cuerpo", Vector2.zero, S("AtlanteAliado_sleep_0"), t, o);
            AddAnimator(body, "AtlanteAliado");
            atlAnim = body.GetComponent<Animator>();
        }
        else
        {
            Box("Pedestal", new Vector2(0f, 0.3f), new Vector2(3.4f, 0.6f), PiedraOscura, t, false, o);
            Box("Pierna_Izq", new Vector2(-0.55f, 1.6f), new Vector2(0.8f, 2f), Basalto, t, false, o);
            Box("Pierna_Der", new Vector2(0.55f, 1.6f), new Vector2(0.8f, 2f), Basalto, t, false, o);
            Box("Cinturon", new Vector2(0f, 2.75f), new Vector2(2.3f, 0.4f), Hex("5a5a68"), t, false, o + 1);
            Box("Torso", new Vector2(0f, 3.9f), new Vector2(2.4f, 2f), Basalto, t, false, o);
            Box("Pectoral_Mariposa", new Vector2(0f, 4.0f), new Vector2(1.3f, 0.8f), Hex("6a6a7a"), t, false, o + 1);
            Box("Brazo_Izq", new Vector2(-1.5f, 3.7f), new Vector2(0.6f, 2f), Basalto, t, false, o);
            Box("Brazo_Der", new Vector2(1.5f, 3.7f), new Vector2(0.6f, 2f), Basalto, t, false, o);
            Box("Lanzadardos", new Vector2(1.9f, 3.9f), new Vector2(0.2f, 3f), Hex("5a4a3a"), t, false, o - 1);
            Box("Cabeza", new Vector2(0f, 5.5f), new Vector2(1.4f, 1.2f), Basalto, t, false, o);
            Box("Tocado", new Vector2(0f, 6.35f), new Vector2(2f, 0.7f), Hex("5a5a68"), t, false, o);
            for (int i = 0; i < 5; i++)
                Box("Pluma", new Vector2(-0.8f + i * 0.4f, 6.95f), new Vector2(0.25f, 0.6f), Hex("5a5a68"), t, false, o);

            // Ojos y grietas ámbar que "respiran"
            foreach (var x in new[] { -0.3f, 0.3f })
            {
                var eye = Box("Ojo", new Vector2(x, 5.55f), new Vector2(0.22f, 0.12f), Ambar, t, false, o + 2);
                eye.AddComponent<GlowPulse>().speed = 1.5f;
            }
            var cracks = new[] { new Vector3(-0.5f, 4.3f, 30f), new Vector3(0.6f, 3.5f, -25f), new Vector3(-0.4f, 1.3f, 60f), new Vector3(1.4f, 3.2f, 80f) };
            foreach (var c in cracks)
            {
                var cr = Box("Grieta_Ambar", new Vector2(c.x, c.y), new Vector2(0.7f, 0.08f), Ambar, t, false, o + 2);
                cr.transform.localRotation = Quaternion.Euler(0, 0, c.z);
                cr.AddComponent<GlowPulse>().speed = 1.2f;
            }
        }

        // Tezcatl: aparece y escapa al terminar el diálogo
        GameObject tez;
        if (S("Tezcatl_float_0") != null)
            tez = Animated("Tezcatl_Escapa", new Vector2(2.6f, 3.2f), "Tezcatl_float", 2, 4f, t, 15);
        else
            tez = Box("Tezcatl_Escapa", new Vector2(2.6f, 4.2f), new Vector2(1f, 2f), Hex("16121e"), t, false, 15);
        tez.AddComponent<TezcatlCameo>();
        tez.SetActive(false);

        var lightGO = new GameObject("Luz_Atlante");
        lightGO.transform.SetParent(t, false);
        lightGO.transform.localPosition = new Vector3(0f, 4f, 0f);
        var l = lightGO.AddComponent<Light2D>();
        l.lightType = Light2D.LightType.Point;
        l.color = Ambar;
        l.pointLightOuterRadius = 6f;
        var lp = lightGO.AddComponent<GlowPulse>();
        lp.speed = 1.2f;
        lp.lightMin = 0.3f;
        lp.lightMax = 0.9f;

        var bc = root.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(5f, 4f);
        bc.offset = new Vector2(0f, 2f);
        var dt = root.AddComponent<DialogueTrigger>();
        dt.grantsPico = true;
        dt.promptIcon = PromptIcon(t, atlSprite ? 6.6f : 7.8f);
        dt.wakeAnimator = atlAnim;
        dt.activateOnFinish = tez;
        dt.lines = new[]
        {
            new DialogueLine("Ixtli", "¿Qué...? La piedra... está respirando."),
            new DialogueLine("Atlante", "...Hija de la cantera. Mi sueño se rompió... y con él despertó Tezcatl."),
            new DialogueLine("Atlante", "Devora la memoria del valle. Mi poder está partido en cuatro Corazones de Obsidiana."),
            new DialogueLine("Atlante", "Toma el Pico de Tollan. Rompe la piedra que te cierra el paso."),
            new DialogueLine("Tezcatl", "...Por fin libre. El valle olvidará a sus muertos... y tú también, niña."),
            new DialogueLine("Ixtli", "¡¿Qué fue eso?!  ...Abuelo, ¿estás viendo esto?"),
        };
    }

    // ------------------------------------------------------------------ Itzcóatl (se une a Ixtli)
    static void BuildItzcoatlNPC(Vector2 feet, Transform parent)
    {
        GameObject npc;
        if (S("Itzcoatl_idle_0") != null)
        {
            npc = SpriteObj("Itzcoatl_NPC", feet, S("Itzcoatl_idle_0"), parent, 8);
            npc.GetComponent<SpriteRenderer>().flipX = true;              // mira hacia Ixtli
            AddAnimator(npc, "Itzcoatl");
        }
        else
        {
            npc = Box("Itzcoatl_NPC", feet + new Vector2(0f, 0.75f), new Vector2(0.8f, 1.5f), Hex("2e9682"), parent, false, 8);
        }
        TriggerZone(npc, new Vector2(2.6f, 2.5f), new Vector2(0f, 1.25f));
        var dt = npc.AddComponent<DialogueTrigger>();
        dt.autoStart = true;
        dt.unlocksSwitch = true;
        dt.deactivateOnFinish = npc;
        dt.lines = new[]
        {
            new DialogueLine("Itzcóatl", "¡Ixtli! Se oyó un estruendo en todo el taller... ¿qué pasó?"),
            new DialogueLine("Ixtli", "El Atlante despertó. Y algo oscuro salió de la piedra: Tezcatl. Se lleva la memoria del valle."),
            new DialogueLine("Itzcóatl", "Entonces no vas sola. Traje la pala de la cantera."),
            new DialogueLine("Ixtli", "Vamos. El Atlante dijo que hay que ir a los cuatro rumbos."),
        };
    }

    // ------------------------------------------------------------------ Tramo de la escopeta
    static void BuildShotgunSection(Transform level)
    {
        // Potenciador: Escopeta del Abuelo (3 tiros)
        GameObject gun = Animated("Escopeta_Potenciador", new Vector2(73f, 0.4f), "Armas_escopeta_pickup", 4, 8f, level, 12);
        if (gun == null) gun = Box("Escopeta_Potenciador", new Vector2(73f, 0.8f), new Vector2(0.9f, 0.4f), Ambar, level, false, 12);
        gun.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
        TriggerZone(gun, new Vector2(1f, 1f), new Vector2(0f, 0.4f));
        var col = gun.AddComponent<Collectible>();
        col.kind = Collectible.Kind.Shotgun;
        col.amount = 3;
        col.pickupMessage = "¡ESCOPETA DEL ABUELO!  3 tiros.  Dispara con  K  o  Clic derecho";
        var gl = new GameObject("Brillo");
        gl.transform.SetParent(gun.transform, false);
        gl.transform.localPosition = new Vector3(0f, 0.4f, 0f);
        var l = gl.AddComponent<Light2D>();
        l.lightType = Light2D.LightType.Point; l.color = Ambar; l.intensity = 0.8f; l.pointLightOuterRadius = 2.2f;
        gl.AddComponent<GlowPulse>().speed = 3f;

        // Muro de obsidiana (2 bloques, 6 golpes de pico cada uno = 1 escopetazo)
        Box("Columna_Techo_2", new Vector2(78f, 6.5f), new Vector2(1.2f, 9f), PiedraOscura, level, true, 5);
        for (int i = 0; i < 2; i++)
        {
            GameObject b;
            if (S("Decoracion_bloque_tallable_0") != null)
            {
                b = SpriteObj("Bloque_Obsidiana_" + (i + 1), new Vector2(78f, i), S("Decoracion_bloque_tallable_0"), level, 6);
                var bc = b.AddComponent<BoxCollider2D>(); bc.size = Vector2.one; bc.offset = new Vector2(0f, 0.5f);
                b.GetComponent<SpriteRenderer>().color = new Color(0.35f, 0.28f, 0.48f);
            }
            else b = Box("Bloque_Obsidiana_" + (i + 1), new Vector2(78f, 0.5f + i), Vector2.one, Hex("2a2238"), level, true, 6);
            b.AddComponent<BreakableBlock>().hits = 6;
        }

        // Cajas para probar la escopeta
        foreach (var x in new[] { 82.2f, 83.2f, 82.7f })
        {
            float y = x == 82.7f ? 0.75f : 0f;
            GameObject c;
            if (S("Decoracion_caja_0") != null)
            {
                c = SpriteObj("Caja_Rompible", new Vector2(x, y), S("Decoracion_caja_0"), level, 5);
                var cc = c.AddComponent<BoxCollider2D>(); cc.size = new Vector2(0.85f, 0.75f); cc.offset = new Vector2(0f, 0.375f);
            }
            else c = Box("Caja_Rompible", new Vector2(x, y + 0.4f), new Vector2(0.85f, 0.75f), Madera, level, true, 5);
            c.AddComponent<BreakableBlock>().hits = 3;
        }
    }

    // ------------------------------------------------------------------ Tutorial
    static void PlaceHints(Transform level)
    {
        var root = new GameObject("Tutorial").transform;
        root.SetParent(level, false);

        void Hint(float x, float width, string text, bool pico = false, bool swap = false, bool shotgun = false)
        {
            var go = new GameObject("Pista");
            go.transform.SetParent(root, false);
            go.transform.localPosition = new Vector3(x, 3f, 0f);
            var c = go.AddComponent<BoxCollider2D>();
            c.isTrigger = true; c.size = new Vector2(width, 8f);
            var h = go.AddComponent<TutorialHint>();
            h.text = text; h.requirePico = pico; h.requireSwitch = swap; h.requireShotgun = shotgun;
        }

        Hint(-4.5f, 4.5f, "MOVERTE:  ← →  o  A / D\nSALTAR:  ESPACIO  (mantenlo presionado para saltar más alto)");
        Hint(4.5f, 3f, "Arriba a la izquierda está tu VIDA.  Arriba a la derecha:\nCorazones de Obsidiana (0/4) y Murales encontrados (0/12).");
        Hint(9f, 3f, "ANDAMIOS: se atraviesan desde abajo.  Salta para subirte a ellos.");
        Hint(14.8f, 4f, "Allá arriba brilla un FRAGMENTO DE MURAL.  Hay 12 escondidos:\ncada uno revela un pedazo de la historia de Tollan.");
        Hint(24f, 3f, "¡HUECO!  Si caes, regresas al último BRASERO que encendiste.");
        Hint(31f, 2.5f, "BRASERO: al tocarlo se enciende, GUARDA tu avance y te CURA toda la vida.");
        Hint(34f, 2.5f, "ESTELAS:  presiona  E  para leerlas.  También sirve para hablar con la gente.");
        Hint(37.5f, 3.5f, "PÚAS DE MAGUEY: te quitan un corazón y te empujan.  ¡Sáltalas!");
        Hint(43.5f, 4f, "Acércate al ATLANTE y presiona  E.");
        Hint(48.5f, 3f, "PICO DE TOLLAN:  ataca con  X,  J  o  Clic izquierdo.", pico: true);
        Hint(50.5f, 1.5f, "BLOQUES TALLABLES: rómpelos a picazos (2 golpes cada uno).", pico: true);
        Hint(55f, 3.5f, "MUÑECO DE PRÁCTICA: golpéalo para practicar tus ataques.", pico: true);
        Hint(59.5f, 2f, "Otro hueco...  toma carrera y salta al final de la orilla.");
        Hint(69f, 3f, "CAMBIAR DE PERSONAJE:  tecla  C.\nIxtli pelea con el Pico, Itzcóatl con la Pala.", swap: true);
        Hint(72.5f, 3f, "ESCOPETA DEL ABUELO: es un POTENCIADOR.  Recógela: trae 3 tiros.");
        Hint(75.8f, 3.5f, "DISPARAR:  tecla  K  o  Clic derecho  (control: botón B).\nTus tiros se ven arriba a la izquierda. ¡Son pocos, úsalos bien!", shotgun: true);
        Hint(78f, 1f, "BLOQUE DE OBSIDIANA: casi no se rompe a picazos...  ¡dale un ESCOPETAZO!");
        Hint(83f, 4f, "Las cajas también se rompen.  Contra los JEFES la escopeta hace MUCHO daño:\nsáltales a la cabeza y remata con un disparo.");
        Hint(89.5f, 3f, "Cruza el ARCO para terminar el tutorial.");
    }

    // ------------------------------------------------------------------ Jugador
    static GameObject BuildPlayer(Vector2 pos, PhysicsMaterial2D mat)
    {
        GameObject p;
        bool realArt = S("Ixtli_idle_0") != null;
        if (realArt)
        {
            p = new GameObject("Ixtli");
            p.transform.position = pos;
            var visual = SpriteObj("Visual", new Vector2(0f, -0.725f), S("Ixtli_idle_0"), p.transform, 10);   // pies al fondo del collider
            AddAnimator(visual, "Ixtli");
        }
        else
        {
            p = Box("Ixtli", pos, new Vector2(0.8f, 1.5f), Hex("d8433c"), null, false, 10);
            Box("Trenza", new Vector2(-0.35f, 0.45f), new Vector2(0.2f, 0.45f), Hex("1b1410"), p.transform, false, 11);
            Box("Ojo", new Vector2(0.2f, 0.45f), new Vector2(0.14f, 0.14f), Hex("1b1410"), p.transform, false, 11);
            Box("Delantal", new Vector2(0.05f, -0.15f), new Vector2(0.6f, 0.5f), Hex("8a5a36"), p.transform, false, 11);
        }
        p.tag = "Player";

        var rb = p.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3.5f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var col = p.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.7f, 1.45f);
        col.sharedMaterial = mat;

        var h = p.AddComponent<Health>();
        h.maxHealth = 3;
        h.invulnerableTime = 1f;
        p.AddComponent<PlayerController>();
        var combat = p.AddComponent<PlayerCombat>();
        var shotgun = p.AddComponent<PlayerShotgun>();
        shotgun.blastFrames = Frames("Proyectiles_disparo", 3);
        if (realArt)
        {
            p.AddComponent<PlayerAnimator>();
            var ctrlIx = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>($"{Root}/Animations/Ixtli/Ixtli.controller");
            var ctrlIt = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>($"{Root}/Animations/Itzcoatl/Itzcoatl.controller");
            var sw = p.AddComponent<PlayerCharacterSwitch>();
            if (ctrlIx != null && ctrlIt != null) sw.controllers = new[] { ctrlIx, ctrlIt };
        }
        else
        {
            var swing = Box("Golpe_Pico", new Vector2(0.85f, 0f), new Vector2(1.1f, 1.1f), new Color(1f, 0.65f, 0.23f, 0.55f), p.transform, false, 12);
            combat.swingVisual = swing;
            swing.SetActive(false);
        }
        return p;
    }

    // ------------------------------------------------------------------ Arte real
    static Dictionary<string, Sprite> art = new Dictionary<string, Sprite>();

    static void LoadArt()
    {
        art.Clear();
        foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { Root + "/Art" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            foreach (var o in AssetDatabase.LoadAllAssetRepresentationsAtPath(path))
                if (o is Sprite sp && !art.ContainsKey(sp.name)) art[sp.name] = sp;
        }
    }

    static Sprite S(string name) => art.TryGetValue(name, out var sp) ? sp : null;

    static Sprite[] Frames(string prefix, int count)
    {
        var list = new List<Sprite>();
        for (int i = 0; i < count; i++) { var sp = S(prefix + "_" + i); if (sp != null) list.Add(sp); }
        return list.ToArray();
    }

    static GameObject SpriteObj(string name, Vector2 localPos, Sprite sprite, Transform parent, int order)
    {
        var go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = order;
        return go;
    }

    static void AddAnimator(GameObject go, string character)
    {
        var ctrl = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>($"{Root}/Animations/{character}/{character}.controller");
        if (ctrl == null) return;
        go.AddComponent<Animator>().runtimeAnimatorController = ctrl;
    }

    static GameObject Animated(string name, Vector2 pos, string prefix, int frames, float fps, Transform parent, int order)
    {
        var f = Frames(prefix, frames);
        if (f.Length == 0) return null;
        var go = SpriteObj(name, pos, f[0], parent, order);
        var a = go.AddComponent<SpriteFrameAnimator>();
        a.frames = f; a.fps = fps;
        return go;
    }

    /// <summary>Importa un PNG suelto (fondos) como sprite pixel art de 32 PPU.</summary>
    static Sprite ImportSingle(string path)
    {
        if (!File.Exists(path)) return null;
        var imp = (TextureImporter)AssetImporter.GetAtPath(path);
        if (imp == null) return null;
        var st = new TextureImporterSettings();
        imp.ReadTextureSettings(st);
        if (imp.textureType != TextureImporterType.Sprite || imp.spritePixelsPerUnit != 32 || st.spriteMeshType != SpriteMeshType.FullRect)
        {
            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.ReadTextureSettings(st);
            st.spriteMeshType = SpriteMeshType.FullRect;
            imp.SetTextureSettings(st);
            imp.spritePixelsPerUnit = 32;
            imp.filterMode = FilterMode.Point;
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.mipmapEnabled = false;
            imp.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    // ------------------------------------------------------------------ Suelo con tiles (GandalfHardcore, fila otoñal)
    static Tile MakeTile(string spriteName)
    {
        var sp = S(spriteName);
        if (sp == null) return null;
        EnsureFolder(Root + "/Tiles");
        string path = $"{Root}/Tiles/{spriteName}.asset";
        var t = AssetDatabase.LoadAssetAtPath<Tile>(path);
        if (t == null) { t = ScriptableObject.CreateInstance<Tile>(); AssetDatabase.CreateAsset(t, path); }
        t.sprite = sp;
        t.colliderType = Tile.ColliderType.Grid;
        EditorUtility.SetDirty(t);
        return t;
    }

    static void BuildTileGround(Transform level)
    {
        Tile TL = MakeTile("FloorTiles1_6_0"), TM = MakeTile("FloorTiles1_6_1"), TR = MakeTile("FloorTiles1_6_2");
        Tile L = MakeTile("FloorTiles1_7_0"), F = MakeTile("FloorTiles1_7_1"), R = MakeTile("FloorTiles1_7_2");

        var grid = new GameObject("Grid").AddComponent<Grid>();
        grid.transform.SetParent(level, false);
        var tmGO = new GameObject("Suelo");
        tmGO.transform.SetParent(grid.transform, false);
        var tm = tmGO.AddComponent<Tilemap>();
        tmGO.AddComponent<TilemapRenderer>().sortingOrder = 0;
        var rb = tmGO.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        var tc = tmGO.AddComponent<TilemapCollider2D>();
        tc.compositeOperation = Collider2D.CompositeOperation.Merge;
        tmGO.AddComponent<CompositeCollider2D>();

        void Segment(int a, int b)
        {
            for (int x = a; x < b; x++)
            {
                tm.SetTile(new Vector3Int(x, -1, 0), x == a ? TL : x == b - 1 ? TR : TM);
                for (int y = -2; y >= -5; y--)
                    tm.SetTile(new Vector3Int(x, y, 0), x == a ? L : x == b - 1 ? R : F);
            }
        }
        Segment(-7, 26);   // hueco 26-29
        Segment(29, 60);   // hueco 60-63
        Segment(63, 95);
        for (int y = -5; y <= 12; y++)   // paredes
        {
            tm.SetTile(new Vector3Int(-8, y, 0), F);
            tm.SetTile(new Vector3Int(95, y, 0), F);
        }
    }

    // ------------------------------------------------------------------ Decoración
    static void PlaceDecor(Transform level)
    {
        if (S("Decoracion_maguey_0") == null) return;
        var deco = new GameObject("Decoracion").transform;
        deco.SetParent(level, false);

        void Put(string item, float x, int order = 2, float scale = 1f, bool flip = false)
        {
            var sp = S(item);
            if (sp == null) return;
            var go = SpriteObj(item.Replace("Decoracion_", "").Replace("_0", ""), new Vector2(x, 0f), sp, deco, order);
            go.transform.localScale = new Vector3(scale, scale, 1f);
            go.GetComponent<SpriteRenderer>().flipX = flip;
        }

        foreach (var x in new[] { -4.5f, 8.5f, 30.5f, 42.5f, 67.5f, 86.5f }) Put("Decoracion_maguey_0", x);
        foreach (var x in new[] { 4.3f, 24.2f, 61.5f }) Put("Decoracion_nopal_0", x, 2, 1f, x > 20f);
        foreach (var x in new[] { 12.3f, 36.2f, 57.6f, 72.4f }) Put("Decoracion_arbusto_seco_0", x, 1);
        foreach (var x in new[] { 2.8f, 57.2f }) Put("Decoracion_vasija_0", x, 3, 0.8f);
        Put("Decoracion_olla_rota_0", 48.8f, 3, 0.8f);
        foreach (var x in new[] { 3.4f, 33.1f, 70.2f }) Put("Decoracion_piedras_0", x, 3, 0.8f);
        foreach (var x in new[] { 24.9f, 54.3f }) Put("Decoracion_calavera_0", x, 3, 0.6f);
        foreach (var x in new[] { 40.2f, 64.4f, 74.6f, 89.2f }) Put("Decoracion_cempasuchil_0", x, 3);
        Put("DecoracionGrande_mezquite_0", 19.5f, -3, 1.2f);
        Put("DecoracionGrande_mezquite_0", 58.5f, -3, 1.4f, true);
        Put("Decoracion_huesos_0", 27.5f, 3, 0.8f);

        // antorchas con luz
        foreach (var x in new[] { -1.5f, 15.5f, 28.5f, 44f, 53f, 66.5f, 81f, 88f, 93.5f })
        {
            var t = Animated("Antorcha", new Vector2(x, 0f), "Decoracion_antorcha", 4, 8f, deco, 2);
            if (t == null) break;
            t.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            var lGO = new GameObject("Luz");
            lGO.transform.SetParent(t.transform, false);
            lGO.transform.localPosition = new Vector3(0f, 0.8f, 0f);
            var l = lGO.AddComponent<Light2D>();
            l.lightType = Light2D.LightType.Point;
            l.color = Ambar; l.intensity = 0.9f;
            l.pointLightInnerRadius = 0.3f; l.pointLightOuterRadius = 3.5f;
            var gp = lGO.AddComponent<GlowPulse>(); gp.flicker = true; gp.lightMin = 0.7f; gp.lightMax = 1.1f;
        }

        // ofrenda junto al Atlante
        Animated("Ofrenda", new Vector2(43.2f, 0f), "Decoracion_ofrenda", 2, 4f, deco, 3);
    }

    // ------------------------------------------------------------------ UI
    static void BuildUI(Health playerHealth)
    {
        var canvasGO = new GameObject("UI");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        var hud = canvasGO.AddComponent<HUD>();
        hud.playerHealth = playerHealth;
        hud.hearts = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            var rt = Rect(canvasGO.transform, "Corazon_" + (i + 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(28 + i * 42, -28), new Vector2(32, 32));
            var img = rt.gameObject.AddComponent<Image>();
            var heart = S("Armas_vida_0");
            if (heart != null)
            {
                img.sprite = heart; img.preserveAspect = true;
                rt.sizeDelta = new Vector2(44, 44);
                hud.heartFull = Color.white;
                hud.heartEmpty = new Color(0.25f, 0.2f, 0.25f, 0.8f);
                img.color = Color.white;
            }
            else
            {
                img.color = hud.heartFull;
                rt.localRotation = Quaternion.Euler(0, 0, 45);
            }
            hud.hearts[i] = img;
        }
        hud.statsText = MakeText(canvasGO.transform, "Contadores", "", 22, TextAnchor.UpperRight, Hex("f3e6cf"),
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(-24, -20), new Vector2(520, 70));
        hud.messageText = MakeText(canvasGO.transform, "Mensaje", "", 28, TextAnchor.MiddleCenter, Hex("ffd48a"),
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -90), new Vector2(1100, 100));
        hud.messageText.gameObject.AddComponent<Outline>().effectColor = new Color(0, 0, 0, 0.8f);

        var ammoRT = Rect(canvasGO.transform, "Tiros_Escopeta", new Vector2(0, 1), new Vector2(0, 1), new Vector2(18, -78), new Vector2(240, 44));
        ammoRT.gameObject.AddComponent<Image>().color = new Color(0.08f, 0.05f, 0.12f, 0.75f);
        var gunIcon = Rect(ammoRT, "Icono", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(4, 0), new Vector2(64, 40));
        var gunImg = gunIcon.gameObject.AddComponent<Image>(); gunImg.preserveAspect = true;
        if (S("Armas_escopeta_0") != null) gunImg.sprite = S("Armas_escopeta_0"); else gunImg.color = Ambar;
        hud.ammoText = MakeText(ammoRT, "Texto", "x 0", 22, TextAnchor.MiddleLeft, Hex("ffd48a"),
            new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(76, 0), new Vector2(160, 40));
        hud.ammoPanel = ammoRT.gameObject;

        // Cuadro de diálogo
        var panelRT = Rect(canvasGO.transform, "Dialogo", new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 24), new Vector2(1100, 180));
        var panelImg = panelRT.gameObject.AddComponent<Image>();
        panelImg.color = new Color(0.08f, 0.05f, 0.12f, 0.93f);
        var outline = panelRT.gameObject.AddComponent<Outline>();
        outline.effectColor = Hex("c98a3e");
        outline.effectDistance = new Vector2(3, -3);

        var portraitRT = Rect(panelRT, "Retrato", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(24, 0), new Vector2(128, 128));
        var portrait = portraitRT.gameObject.AddComponent<Image>();
        var speaker = MakeText(panelRT, "Nombre", "", 26, TextAnchor.UpperLeft, Hex("f2b04c"),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(176, -18), new Vector2(880, 36));
        speaker.fontStyle = FontStyle.Bold;
        var body = MakeText(panelRT, "Texto", "", 24, TextAnchor.UpperLeft, Hex("f3e6cf"),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(176, -58), new Vector2(880, 110));
        MakeText(panelRT, "Continuar", "[E] >>", 18, TextAnchor.LowerRight, Hex("cdb89b"),
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(-18, 12), new Vector2(200, 30));

        // Panel de pistas del tutorial (arriba al centro)
        var hintRT = Rect(canvasGO.transform, "Pista_Tutorial", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -150), new Vector2(860, 84));
        hintRT.gameObject.AddComponent<Image>().color = new Color(0.08f, 0.05f, 0.12f, 0.88f);
        var hOut = hintRT.gameObject.AddComponent<Outline>(); hOut.effectColor = Hex("c98a3e"); hOut.effectDistance = new Vector2(2, -2);
        var hintIcon = Rect(hintRT, "Icono", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(14, 0), new Vector2(56, 56));
        var hintImg = hintIcon.gameObject.AddComponent<Image>(); hintImg.preserveAspect = true;
        var estelaPortrait = S("Retratos_estela_0");
        if (estelaPortrait != null) hintImg.sprite = estelaPortrait; else hintImg.color = Hex("c98a3e");
        hud.hintText = MakeText(hintRT, "Texto", "", 22, TextAnchor.MiddleLeft, Hex("f3e6cf"),
            new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(84, 0), new Vector2(760, 76));
        hud.hintPanel = hintRT.gameObject;
        hintRT.gameObject.SetActive(false);

        var dui = canvasGO.AddComponent<DialogueUI>();
        dui.panel = panelRT.gameObject;
        dui.speakerText = speaker;
        dui.bodyText = body;
        dui.portrait = portrait;
        portrait.preserveAspect = true;
        foreach (var (who, key) in new[] { ("Ixtli", "ixtli"), ("Itzcóatl", "itzcoatl"), ("Abuelo Nabor", "abuelo"),
                                           ("Atlante", "atlante"), ("Tezcatl", "tezcatl"), ("Estela", "estela") })
        {
            var face = S("Retratos_" + key + "_0");
            if (face != null) dui.portraits.Add(new SpeakerPortrait { speaker = who, sprite = face });
        }
        panelRT.gameObject.SetActive(false);
    }

    static RectTransform Rect(Transform parent, string name, Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rt = (RectTransform)go.transform;
        rt.SetParent(parent, false);
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return rt;
    }

    static Text MakeText(Transform parent, string name, string content, int size, TextAnchor align, Color color,
                         Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 sizeDelta)
    {
        var rt = Rect(parent, name, anchor, pivot, pos, sizeDelta);
        var t = rt.gameObject.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text = content;
        t.fontSize = size;
        t.alignment = align;
        t.color = color;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        return t;
    }

    // ------------------------------------------------------------------ Helpers
    static GameObject Box(string name, Vector2 localPos, Vector2 size, Color color, Transform parent,
                          bool solid, int order, Sprite sprite = null)
    {
        var go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite != null ? sprite : square;
        sr.color = color;
        sr.sortingOrder = order;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = size;
        if (solid) go.AddComponent<BoxCollider2D>().size = size;
        return go;
    }

    static BoxCollider2D TriggerZone(GameObject go, Vector2 size, Vector2 offset)
    {
        var c = go.AddComponent<BoxCollider2D>();
        c.isTrigger = true;
        c.size = size;
        c.offset = offset;
        return c;
    }

    static GameObject PromptIcon(Transform parent, float height)
    {
        var icon = Box("Icono_E", new Vector2(0f, height), new Vector2(0.35f, 0.35f), Hex("ffd48a"), parent, false, 20);
        icon.transform.localRotation = Quaternion.Euler(0, 0, 45);
        return icon;
    }

    static Sprite ShapeSprite(string file, Func<int, int, bool> inside)
    {
        string path = PlaceholderDir + "/" + file;
        if (!File.Exists(path))
        {
            var tex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            var px = new Color32[32 * 32];
            for (int y = 0; y < 32; y++)
                for (int x = 0; x < 32; x++)
                    px[y * 32 + x] = inside(x, y) ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 0);
            tex.SetPixels32(px);
            tex.Apply();
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);
        }

        var imp = (TextureImporter)AssetImporter.GetAtPath(path);
        var settings = new TextureImporterSettings();
        imp.ReadTextureSettings(settings);
        bool dirty = imp.textureType != TextureImporterType.Sprite || imp.spritePixelsPerUnit != 32 ||
                     imp.filterMode != FilterMode.Point || settings.spriteMeshType != SpriteMeshType.FullRect;
        if (dirty)
        {
            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            imp.SetTextureSettings(settings);
            imp.spritePixelsPerUnit = 32;
            imp.filterMode = FilterMode.Point;
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.mipmapEnabled = false;
            imp.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static PhysicsMaterial2D NoFrictionMaterial()
    {
        string path = Root + "/Materials/SinFriccion.physicsMaterial2D";
        var mat = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(path);
        if (mat != null) return mat;
        mat = new PhysicsMaterial2D("SinFriccion") { friction = 0f, bounciness = 0f };
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out var c);
        return c;
    }
}
