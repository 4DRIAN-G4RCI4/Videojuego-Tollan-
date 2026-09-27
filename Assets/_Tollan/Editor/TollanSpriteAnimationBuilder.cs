using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.U2D.Sprites;
using UnityEngine;

/// <summary>
/// Tools > Tollan > 3. Importar sprites y crear animaciones
/// Lee cada  Art/**/<Nombre>_sheet.png  + su  <Nombre>_sheet.txt  (manifiesto),
/// corta la hoja en sprites, crea un AnimationClip por animación y un Animator Controller por personaje
/// en  Assets/_Tollan/Animations/<Nombre>/
/// </summary>
public static class TollanSpriteAnimationBuilder
{
    const string ArtRoot = "Assets/_Tollan/Art";
    const string AnimRoot = "Assets/_Tollan/Animations";

    static readonly Dictionary<string, float> Fps = new Dictionary<string, float>
    {
        { "idle", 6 }, { "walk", 10 }, { "fly", 10 }, { "float", 6 }, { "attack", 14 }, { "slam", 8 }, { "throw", 8 },
        { "charge", 8 }, { "grab", 8 }, { "ring", 8 }, { "swarm", 6 }, { "jump", 8 }, { "fall", 8 }, { "hurt", 8 },
        { "block", 6 }, { "phase3", 6 }, { "dive", 8 }, { "shoot", 12 }, { "disparo", 14 },
        { "antorcha", 8 }, { "brasero", 8 }, { "ofrenda", 4 }, { "corazon_obsidiana", 6 }, { "vida", 6 }, { "escopeta_pickup", 8 }, { "laja", 12 },
    };

    static readonly HashSet<string> Looping = new HashSet<string> { "idle", "walk", "fly", "float", "phase3", "swarm",
        "antorcha", "brasero", "ofrenda", "corazon_obsidiana", "vida", "escopeta_pickup", "laja" };

    struct AnimDef { public string name; public int row; public int frames; }

    [MenuItem("Tools/Tollan/3. Importar sprites y crear animaciones")]
    public static void BuildAll()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Tollan", "Primero detén el Play.", "OK");
            return;
        }

        var manifests = Directory.GetFiles(ArtRoot, "*_sheet.txt", SearchOption.AllDirectories);
        if (manifests.Length == 0)
        {
            EditorUtility.DisplayDialog("Tollan", "No encontré manifiestos *_sheet.txt en " + ArtRoot, "OK");
            return;
        }

        int count = 0;
        foreach (var raw in manifests)
        {
            string manifest = raw.Replace('\\', '/');
            string png = manifest.Substring(0, manifest.Length - 4) + ".png";
            if (!File.Exists(png)) { Debug.LogWarning("[Tollan] Falta la hoja " + png); continue; }
            string character = Path.GetFileName(png).Replace("_sheet.png", "");
            EditorUtility.DisplayProgressBar("Tollan", "Animando " + character, (float)count / manifests.Length);
            try
            {
                Build(character, png, manifest);
                count++;
            }
            catch (System.Exception e)
            {
                Debug.LogError("[Tollan] Error con " + character + ": " + e);
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[Tollan] {count} personajes con animaciones en {AnimRoot}");
    }

    static void Build(string character, string pngPath, string manifestPath)
    {
        // ---- leer manifiesto
        var lines = File.ReadAllLines(manifestPath);
        var size = lines[0].Split(' ')[1].Split('x');
        int fw = int.Parse(size[0]), fh = int.Parse(size[1]);
        // "frame 32x32 grid"  -> tileset: solo se corta en celdas <Nombre>_<fila>_<col>, sin animaciones
        bool grid = lines[0].Contains("grid");
        var anims = new List<AnimDef>();
        for (int i = 1; i < lines.Length; i++)
        {
            var l = lines[i].Trim();
            if (l.Length == 0) continue;
            var parts = l.Split(' ');
            var def = new AnimDef { name = parts[0] };
            foreach (var p in parts)
            {
                if (p.StartsWith("row=")) def.row = int.Parse(p.Substring(4));
                if (p.StartsWith("frames=")) def.frames = int.Parse(p.Substring(7));
            }
            anims.Add(def);
        }

        // ---- configurar importador (pixel art)
        var importer = (TextureImporter)AssetImporter.GetAtPath(pngPath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 32;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        var tis = new TextureImporterSettings();
        importer.ReadTextureSettings(tis);
        tis.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(tis);
        importer.SaveAndReimport();

        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(pngPath);
        int texH = tex.height;

        // ---- cortar la hoja (pivote abajo al centro = los pies)
        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var provider = factory.GetSpriteEditorDataProviderFromObject(importer);
        provider.InitSpriteEditorDataProvider();

        var rects = new List<SpriteRect>();
        var pairs = new List<SpriteNameFileIdPair>();
        if (grid)
        {
            int cols = tex.width / fw, rows = texH / fh;
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    var tr = new SpriteRect
                    {
                        name = $"{character}_{r}_{c}",
                        rect = new Rect(c * fw, texH - (r + 1) * fh, fw, fh),
                        alignment = SpriteAlignment.Center,
                        pivot = new Vector2(0.5f, 0.5f),
                        spriteID = GUID.Generate(),
                    };
                    rects.Add(tr);
                    pairs.Add(new SpriteNameFileIdPair(tr.name, tr.spriteID));
                }
            provider.SetSpriteRects(rects.ToArray());
            provider.GetDataProvider<ISpriteNameFileIdDataProvider>()?.SetNameFileIdPairs(pairs);
            provider.Apply();
            importer.SaveAndReimport();
            return;
        }
        foreach (var a in anims)
        {
            for (int f = 0; f < a.frames; f++)
            {
                var sr = new SpriteRect
                {
                    name = $"{character}_{a.name}_{f}",
                    rect = new Rect(f * fw, texH - (a.row + 1) * fh, fw, fh),   // Unity cuenta Y desde abajo
                    alignment = SpriteAlignment.BottomCenter,
                    pivot = new Vector2(0.5f, 0f),
                    spriteID = GUID.Generate(),
                };
                rects.Add(sr);
                pairs.Add(new SpriteNameFileIdPair(sr.name, sr.spriteID));
            }
        }
        provider.SetSpriteRects(rects.ToArray());
        var nameProvider = provider.GetDataProvider<ISpriteNameFileIdDataProvider>();
        nameProvider?.SetNameFileIdPairs(pairs);
        provider.Apply();
        importer.SaveAndReimport();

        var sprites = new Dictionary<string, Sprite>();
        foreach (var obj in AssetDatabase.LoadAllAssetRepresentationsAtPath(pngPath))
            if (obj is Sprite s) sprites[s.name] = s;

        // ---- clips
        string folder = $"{AnimRoot}/{character}";
        EnsureFolder(folder);
        var clips = new List<AnimationClip>();
        foreach (var a in anims)
        {
            float fps = Fps.TryGetValue(a.name, out var v) ? v : 8f;
            var clip = new AnimationClip { frameRate = fps, name = $"{character}_{a.name}" };
            var keys = new ObjectReferenceKeyframe[a.frames + 1];
            for (int f = 0; f < a.frames; f++)
                keys[f] = new ObjectReferenceKeyframe { time = f / fps, value = sprites[$"{character}_{a.name}_{f}"] };
            // repite el último frame para que dure lo mismo que los demás
            keys[a.frames] = new ObjectReferenceKeyframe { time = a.frames / fps, value = sprites[$"{character}_{a.name}_{a.frames - 1}"] };
            var binding = new EditorCurveBinding { type = typeof(SpriteRenderer), path = "", propertyName = "m_Sprite" };
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);

            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = Looping.Contains(a.name);
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            string clipPath = $"{folder}/{character}_{a.name}.anim";
            AssetDatabase.DeleteAsset(clipPath);
            AssetDatabase.CreateAsset(clip, clipPath);
            clips.Add(clip);
        }

        // ---- Animator Controller: un estado por animación (se llaman igual: idle, walk, attack...)
        string ctrlPath = $"{folder}/{character}.controller";
        AssetDatabase.DeleteAsset(ctrlPath);
        var ctrl = AnimatorController.CreateAnimatorControllerAtPath(ctrlPath);
        var sm = ctrl.layers[0].stateMachine;
        for (int i = 0; i < clips.Count; i++)
        {
            var state = sm.AddState(anims[i].name, new Vector3(260, 60 * i, 0));
            state.motion = clips[i];
            if (i == 0) sm.defaultState = state;
        }
        EditorUtility.SetDirty(ctrl);
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
}
