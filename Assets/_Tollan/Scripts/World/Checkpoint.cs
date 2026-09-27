using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>Brasero: guarda el avance y cura al jugador al encenderlo.</summary>
[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    public SpriteRenderer fire;
    public Light2D fireLight;
    [Tooltip("Visual del brasero apagado (se oculta al encenderlo)")]
    public GameObject unlitVisual;
    public bool litAtStart = false;
    public Vector3 respawnOffset = new Vector3(0f, 1f, 0f);

    bool lit;

    void Start() => SetLit(litAtStart);

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        GameManager.Instance?.SetCheckpoint(transform.position + respawnOffset);
        if (player.TryGetComponent<Health>(out var h)) h.Heal(99);

        if (!lit)
        {
            SetLit(true);
            HUD.Instance?.ShowMessage("Brasero encendido · Avance guardado", 2f);
        }
    }

    void SetLit(bool value)
    {
        lit = value;
        if (fire != null) fire.enabled = value;
        if (fireLight != null) fireLight.enabled = value;
        if (unlitVisual != null) unlitVisual.SetActive(!value);
    }
}
