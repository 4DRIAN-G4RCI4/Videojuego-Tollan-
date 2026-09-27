using UnityEngine;

/// <summary>
/// Zona de diálogo (NPC, estela, Atlante). Se activa sola al entrar (autoStart)
/// o al presionar E. Puede entregar el Pico de Tollan al terminar.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour
{
    public DialogueLine[] lines;
    public bool autoStart = false;
    public bool oneShot = true;
    [Tooltip("Al terminar el diálogo el jugador obtiene el Pico (ataque)")]
    public bool grantsPico = false;
    [Tooltip("Icono que aparece cuando puedes presionar E")]
    public GameObject promptIcon;
    [Header("Al empezar / terminar")]
    [Tooltip("Animator que cambia de estado al empezar a hablar (ej. el Atlante despierta)")]
    public Animator wakeAnimator;
    public string wakeState = "idle";
    [Tooltip("Desbloquea el cambio de personaje (tecla C)")]
    public bool unlocksSwitch = false;
    public GameObject activateOnFinish;
    public GameObject deactivateOnFinish;

    PlayerController playerInside;
    bool used;
    Vector3 promptBase;

    void Start()
    {
        if (promptIcon != null)
        {
            promptBase = promptIcon.transform.localPosition;
            promptIcon.SetActive(false);
        }
    }

    bool CanStart => !(oneShot && used) && DialogueUI.Instance != null && !DialogueUI.Instance.IsOpen
                     && DialogueUI.Instance.LastClosedFrame != Time.frameCount;

    void Update()
    {
        if (promptIcon != null)
        {
            bool show = playerInside != null && !autoStart && CanStart;
            if (promptIcon.activeSelf != show) promptIcon.SetActive(show);
            if (show) promptIcon.transform.localPosition = promptBase + Vector3.up * Mathf.Sin(Time.time * 4f) * 0.1f;
        }

        if (playerInside != null && !autoStart && CanStart && TollanInput.InteractPressed)
            Begin();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var pc = other.GetComponentInParent<PlayerController>();
        if (pc == null) return;
        playerInside = pc;
        if (autoStart && CanStart) Begin();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == playerInside) playerInside = null;
    }

    void Begin()
    {
        used = true;
        var player = playerInside;
        if (wakeAnimator != null) wakeAnimator.Play(wakeState, 0, 0f);
        DialogueUI.Instance.Show(lines, () =>
        {
            if (grantsPico && player != null)
            {
                var combat = player.GetComponent<PlayerCombat>();
                if (combat != null) combat.hasPico = true;
                HUD.Instance?.ShowMessage("¡Obtuviste el PICO DE TOLLAN!  Ataca con X / J / Clic", 4f);
            }
            if (unlocksSwitch && player != null && player.TryGetComponent<PlayerCharacterSwitch>(out var sw))
            {
                sw.unlocked = true;
                HUD.Instance?.ShowMessage("¡Itzcóatl se une!  Presiona C para cambiar de personaje", 4f);
            }
            if (activateOnFinish != null) activateOnFinish.SetActive(true);
            if (deactivateOnFinish != null) deactivateOnFinish.SetActive(false);
        });
    }
}
