using UnityEngine;

/// <summary>Daña al jugador al tocarlo (púas de maguey, trampas). Funciona con trigger o colisión.</summary>
public class Hazard : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerStay2D(Collider2D other) => TryHurt(other);
    void OnCollisionStay2D(Collision2D c) => TryHurt(c.collider);

    void TryHurt(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == null) return;
        var h = other.GetComponentInParent<Health>();
        if (h != null) h.TakeDamage(damage, transform.position);
    }
}
