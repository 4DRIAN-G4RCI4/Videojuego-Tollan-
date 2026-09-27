using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SpeakerPortrait
{
    public string speaker;
    public Sprite sprite;
}

/// <summary>Cuadro de diálogo con retrato y efecto máquina de escribir. Bloquea al jugador mientras está abierto.</summary>
public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    public GameObject panel;
    public Text speakerText;
    public Text bodyText;
    public Image portrait;
    public float charsPerSecond = 45f;
    [Tooltip("Retrato por nombre de quien habla")]
    public System.Collections.Generic.List<SpeakerPortrait> portraits = new System.Collections.Generic.List<SpeakerPortrait>();

    public bool IsOpen => panel != null && panel.activeSelf;
    public int LastClosedFrame { get; private set; } = -1;

    DialogueLine[] lines;
    int index;
    Action onDone;
    int openedFrame;
    bool typing;
    Coroutine typeRoutine;
    PlayerController player;

    void Awake()
    {
        Instance = this;
        if (panel != null) panel.SetActive(false);
    }

    public void Show(DialogueLine[] newLines, Action done = null)
    {
        if (newLines == null || newLines.Length == 0) { done?.Invoke(); return; }
        lines = newLines;
        index = 0;
        onDone = done;
        openedFrame = Time.frameCount;
        panel.SetActive(true);
        player = FindFirstObjectByType<PlayerController>();
        if (player != null) player.InputLocked = true;
        ShowLine();
    }

    void Update()
    {
        if (!IsOpen || Time.frameCount == openedFrame) return;
        if (TollanInput.InteractPressed || TollanInput.JumpPressed || TollanInput.AttackPressed)
        {
            if (typing) FinishTyping();
            else Next();
        }
    }

    void ShowLine()
    {
        var line = lines[index];
        speakerText.text = line.speaker;
        if (portrait != null)
        {
            var sp = portraits.Find(p => p.speaker == line.speaker);
            if (sp != null && sp.sprite != null) { portrait.sprite = sp.sprite; portrait.color = Color.white; }
            else { portrait.sprite = null; portrait.color = PortraitColor(line.speaker); }
        }
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        typeRoutine = StartCoroutine(Type(line.text));
    }

    IEnumerator Type(string text)
    {
        typing = true;
        bodyText.text = "";
        float delay = 1f / Mathf.Max(1f, charsPerSecond);
        foreach (char c in text)
        {
            bodyText.text += c;
            yield return new WaitForSeconds(delay);
        }
        typing = false;
    }

    void FinishTyping()
    {
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        bodyText.text = lines[index].text;
        typing = false;
    }

    void Next()
    {
        index++;
        if (index < lines.Length) { ShowLine(); return; }
        panel.SetActive(false);
        LastClosedFrame = Time.frameCount;
        StartCoroutine(UnlockNextFrame());
        var cb = onDone;
        onDone = null;
        cb?.Invoke();
    }

    IEnumerator UnlockNextFrame()
    {
        yield return null;
        if (player != null) player.InputLocked = false;
    }

    /// <summary>Color del retrato placeholder según quién habla (luego se cambia por el sprite 64x64).</summary>
    static Color PortraitColor(string speaker)
    {
        switch (speaker)
        {
            case "Ixtli": return new Color(0.85f, 0.26f, 0.24f);
            case "Abuelo Nabor": return new Color(0.79f, 0.72f, 0.6f);
            case "Atlante": return new Color(0.95f, 0.65f, 0.25f);
            case "Tezcatl": return new Color(0.1f, 0.08f, 0.12f);
            default: return new Color(0.4f, 0.4f, 0.45f);
        }
    }
}
