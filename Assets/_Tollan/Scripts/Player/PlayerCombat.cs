using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Ataque cuerpo a cuerpo con el Pico de Tollan (se desbloquea con el Atlante).</summary>
[RequireComponent(typeof(PlayerController))]
public class PlayerCombat : MonoBehaviour
{
    [Tooltip("Se vuelve true cuando el Atlante entrega el Pico")]
    public bool hasPico = false;
    public int damage = 1;
    public float cooldown = 0.3f;
    public Vector2 hitboxSize = new Vector2(1.2f, 1.2f);
    public Vector2 hitboxOffset = new Vector2(0.85f, 0f);
    [Tooltip("Sprite que se muestra un instante al golpear (placeholder del swing)")]
    public GameObject swingVisual;

    PlayerController pc;
    float cd;

    void Awake()
    {
        pc = GetComponent<PlayerController>();
        if (swingVisual != null) swingVisual.SetActive(false);
    }

    void Update()
    {
        cd -= Time.deltaTime;
        if (!hasPico || pc.InputLocked || pc.IsDead) return;
        if (TollanInput.AttackPressed && cd <= 0f)
        {
            cd = cooldown;
            Attack();
        }
    }

    void Attack()
    {
        float dir = pc.FacingRight ? 1f : -1f;
        Vector2 center = (Vector2)transform.position + new Vector2(hitboxOffset.x * dir, hitboxOffset.y);
        var hits = Physics2D.OverlapBoxAll(center, hitboxSize, 0f);
        var done = new HashSet<IDamageable>();
        foreach (var h in hits)
        {
            if (h.transform.IsChildOf(transform)) continue; // no pegarse a sí mismo
            var target = h.GetComponentInParent<IDamageable>();
            if (target == null || done.Contains(target)) continue;
            done.Add(target);
            target.TakeDamage(damage, transform.position);
        }
        if (swingVisual != null) StartCoroutine(ShowSwing());
        if (TryGetComponent<PlayerAnimator>(out var anim)) anim.PlayOneShot("attack", 0.22f);
    }

    IEnumerator ShowSwing()
    {
        swingVisual.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        swingVisual.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        float dir = transform.localScale.x >= 0 ? 1f : -1f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + new Vector3(hitboxOffset.x * dir, hitboxOffset.y), hitboxSize);
    }
}
