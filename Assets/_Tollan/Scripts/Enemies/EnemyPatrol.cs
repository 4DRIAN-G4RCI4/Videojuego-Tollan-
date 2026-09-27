using System.Collections;
using UnityEngine;

/// <summary>
/// Enemigo básico (Tlacuachillo de Barro, Minero Olvidado...): camina, se voltea en paredes
/// y orillas, daña al tocar y muere a golpes.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Health))]
public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    public int contactDamage = 1;
    public bool startMovingRight = false;
    public Vector2 knockback = new Vector2(4f, 3f);

    Rigidbody2D rb;
    Collider2D col;
    Health health;
    SpriteRenderer sr;
    Color baseColor;
    int dir;
    float stun;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        health = GetComponent<Health>();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) baseColor = sr.color;
        dir = startMovingRight ? 1 : -1;
        rb.freezeRotation = true;
        health.OnDamaged += OnHit;
        health.OnDied += () => Destroy(gameObject, 0.05f);
    }

    void FixedUpdate()
    {
        if (stun > 0f) { stun -= Time.fixedDeltaTime; return; }

        Bounds b = col.bounds;
        Vector2 front = new Vector2(dir > 0 ? b.max.x + 0.05f : b.min.x - 0.05f, b.center.y);
        Vector2 footFront = new Vector2(front.x, b.min.y - 0.1f);

        bool wall = Solid(Physics2D.OverlapPointAll(front));
        bool ground = Solid(Physics2D.OverlapPointAll(footFront));
        if (wall || !ground) dir = -dir;

        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
        var s = transform.localScale;
        s.x = Mathf.Abs(s.x) * dir;
        transform.localScale = s;
    }

    bool Solid(Collider2D[] hits)
    {
        foreach (var h in hits)
        {
            if (h == col || h.isTrigger) continue;
            if (h.GetComponentInParent<PlayerController>() != null) continue;
            return true;
        }
        return false;
    }

    void OnCollisionStay2D(Collision2D c)
    {
        if (c.collider.GetComponentInParent<PlayerController>() == null) return;
        var h = c.collider.GetComponentInParent<Health>();
        if (h != null) h.TakeDamage(contactDamage, transform.position);
    }

    void OnHit(Vector2 from)
    {
        float d = Mathf.Sign(transform.position.x - from.x);
        rb.linearVelocity = new Vector2(d * knockback.x, knockback.y);
        stun = 0.25f;
        StopAllCoroutines();
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        if (sr == null) yield break;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.08f);
        sr.color = baseColor;
    }
}
