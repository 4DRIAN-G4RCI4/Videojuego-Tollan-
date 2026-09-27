using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Corazones de vida, contador de Corazones de Obsidiana / murales y mensajes en pantalla.</summary>
public class HUD : MonoBehaviour
{
    public static HUD Instance { get; private set; }

    public Health playerHealth;
    public Image[] hearts;
    public Text statsText;
    public Text messageText;
    [Header("Escopeta")]
    public GameObject ammoPanel;
    public Text ammoText;
    [Header("Pistas del tutorial")]
    public GameObject hintPanel;
    public Text hintText;
    public Color heartFull = new Color(0.85f, 0.26f, 0.24f);
    public Color heartEmpty = new Color(0.23f, 0.16f, 0.16f, 0.8f);

    Coroutine msgRoutine;
    Object hintOwner;

    void Awake() => Instance = this;

    void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.OnChanged += (cur, max) => RefreshHearts(cur);
            RefreshHearts(playerHealth.Current);
        }
        if (GameManager.Instance != null) GameManager.Instance.OnStatsChanged += RefreshStats;
        RefreshStats();
        if (messageText != null) messageText.text = "";
        SetAmmo(0);
    }

    void RefreshHearts(int current)
    {
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++)
            if (hearts[i] != null) hearts[i].color = i < current ? heartFull : heartEmpty;
    }

    void RefreshStats()
    {
        if (statsText == null) return;
        var gm = GameManager.Instance;
        int h = gm != null ? gm.obsidianHearts : 0;
        int m = gm != null ? gm.muralFragments : 0;
        statsText.text = $"Corazones de Obsidiana {h}/{GameManager.TotalHearts}\nMurales {m}/{GameManager.TotalMurals}";
    }

    public void SetAmmo(int ammo)
    {
        if (ammoPanel != null) ammoPanel.SetActive(ammo > 0);
        if (ammoText != null) ammoText.text = "x " + ammo + "   [K]";
    }

    // ---------------- pistas del tutorial
    public bool IsShowingHint(Object owner) => hintOwner == owner;

    public void ShowHint(string text, Object owner)
    {
        hintOwner = owner;
        if (hintText != null) hintText.text = text;
    }

    public void HideHint(Object owner)
    {
        if (hintOwner == owner) hintOwner = null;
    }

    void Update()
    {
        if (hintPanel == null) return;
        bool dialogOpen = DialogueUI.Instance != null && DialogueUI.Instance.IsOpen;
        bool show = hintOwner != null && !dialogOpen;
        if (hintPanel.activeSelf != show) hintPanel.SetActive(show);
    }

    public void ShowMessage(string msg, float seconds)
    {
        if (messageText == null) return;
        if (msgRoutine != null) StopCoroutine(msgRoutine);
        msgRoutine = StartCoroutine(Msg(msg, seconds));
    }

    IEnumerator Msg(string msg, float seconds)
    {
        messageText.text = msg;
        yield return new WaitForSeconds(seconds);
        messageText.text = "";
    }
}
