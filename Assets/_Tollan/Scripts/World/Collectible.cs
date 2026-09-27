using UnityEngine;

/// <summary>Coleccionables: fragmentos de mural, Corazones de Obsidiana y curación.</summary>
[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    public enum Kind { MuralFragment, ObsidianHeart, Heal, Shotgun }

    public Kind kind = Kind.MuralFragment;
    public int amount = 1;
    [TextArea] public string pickupMessage;
    public float bobHeight = 0.15f;
    public float bobSpeed = 3f;

    Vector3 basePos;

    void Start() => basePos = transform.position;

    void Update()
    {
        transform.position = basePos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobHeight;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        var gm = GameManager.Instance;
        switch (kind)
        {
            case Kind.MuralFragment: gm?.AddMuralFragment(amount); break;
            case Kind.ObsidianHeart: gm?.AddObsidianHeart(amount); break;
            case Kind.Heal: if (player.TryGetComponent<Health>(out var h)) h.Heal(amount); break;
            case Kind.Shotgun: if (player.TryGetComponent<PlayerShotgun>(out var sg)) sg.AddAmmo(amount); break;
        }
        if (!string.IsNullOrEmpty(pickupMessage)) HUD.Instance?.ShowMessage(pickupMessage, 3f);
        Destroy(gameObject);
    }
}
