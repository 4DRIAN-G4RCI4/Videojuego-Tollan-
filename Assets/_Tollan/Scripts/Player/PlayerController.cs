using System.Collections;
using UnityEngine;

/// <summary>
/// Movimiento de plataformas estilo Mario: aceleración, coyote time, jump buffer,
/// salto variable (soltar = salto corto), caída más rápida y knockback al recibir daño.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Health))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 7f;
    public float acceleration = 60f;
    public float deceleration = 70f;

    [Header("Salto")]
    public float jumpForce = 13f;
    public float coyoteTime = 0.1f;
    public float jumpBuffer = 0.12f;
    [Range(0f, 1f)] public float jumpCutMultiplier = 0.5f;
    public float fallGravityMultiplier = 1.6f;
    public float maxFallSpeed = 18f;

    [Header("Daño")]
    public Vector2 knockback = new Vector2(6f, 7f);
    public float stunTime = 0.25f;
    public float deathY = -12f;
    public float respawnDelay = 0.8f;

    public bool FacingRight { get; private set; } = true;
    public bool Grounded { get; private set; }
    public bool IsDead { get; private set; }
    /// <summary>True durante diálogos/cinemáticas: el jugador no se mueve.</summary>
    public bool InputLocked { get; set; }

    Rigidbody2D rb;
    BoxCollider2D col;
    Health health;
    SpriteRenderer sr;
    float coyoteCounter, bufferCounter, stunCounter, baseGravity;
    bool jumping;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        health = GetComponent<Health>();
        sr = GetComponentInChildren<SpriteRenderer>(); // el sprite puede estar en un hijo "Visual"
        baseGravity = rb.gravityScale;
        health.OnDamaged += OnDamaged;
        health.OnDied += OnDied;
    }

    void Update()
    {
        if (IsDead) return;

        Grounded = CheckGround();
        coyoteCounter = Grounded ? coyoteTime : coyoteCounter - Time.deltaTime;

        if (!InputLocked && TollanInput.JumpPressed) bufferCounter = jumpBuffer;
        else bufferCounter -= Time.deltaTime;

        // Saltar
        if (bufferCounter > 0f && coyoteCounter > 0f && stunCounter <= 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            bufferCounter = 0f;
            coyoteCounter = 0f;
            jumping = true;
        }

        // Salto variable: si sueltas el botón mientras subes, corta el salto (una vez)
        if (jumping && rb.linearVelocity.y <= 0f) jumping = false;
        if (jumping && !TollanInput.JumpHeld)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            jumping = false;
        }

        // Caída al vacío
        if (transform.position.y < deathY) health.Kill();

        // Parpadeo mientras es invulnerable
        if (sr != null) sr.enabled = !health.IsInvulnerable || Mathf.Repeat(Time.time * 16f, 2f) > 1f;
    }

    void FixedUpdate()
    {
        if (IsDead) return;
        if (stunCounter > 0f) { stunCounter -= Time.fixedDeltaTime; return; }

        float input = InputLocked ? 0f : TollanInput.Horizontal;
        float target = input * moveSpeed;
        float rate = Mathf.Abs(target) > 0.01f ? acceleration : deceleration;
        float vx = Mathf.MoveTowards(rb.linearVelocity.x, target, rate * Time.fixedDeltaTime);
        float vy = rb.linearVelocity.y;

        rb.gravityScale = vy < 0f ? baseGravity * fallGravityMultiplier : baseGravity;
        vy = Mathf.Max(vy, -maxFallSpeed);
        rb.linearVelocity = new Vector2(vx, vy);

        if (input > 0.01f) Face(true);
        else if (input < -0.01f) Face(false);
    }

    void Face(bool right)
    {
        if (FacingRight == right) return;
        FacingRight = right;
        var s = transform.localScale;
        s.x = Mathf.Abs(s.x) * (right ? 1f : -1f);
        transform.localScale = s;
    }

    bool CheckGround()
    {
        if (rb.linearVelocity.y > 0.1f) return false; // subiendo (ej. atravesando andamio)
        Bounds b = col.bounds;
        Vector2 point = new Vector2(b.center.x, b.min.y);
        var hits = Physics2D.OverlapBoxAll(point, new Vector2(b.size.x * 0.9f, 0.1f), 0f);
        foreach (var h in hits)
        {
            if (h == col || h.isTrigger || h.attachedRigidbody == rb) continue;
            return true;
        }
        return false;
    }

    void OnDamaged(Vector2 from)
    {
        if (health.IsDead) return;
        float dir = Mathf.Sign(transform.position.x - from.x);
        if (dir == 0f) dir = FacingRight ? -1f : 1f;
        rb.linearVelocity = new Vector2(dir * knockback.x, knockback.y);
        stunCounter = stunTime;
        jumping = false;
    }

    void OnDied()
    {
        if (IsDead) return;
        IsDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        if (sr != null) sr.enabled = false;
        yield return new WaitForSeconds(respawnDelay);
        var gm = GameManager.Instance;
        transform.position = gm != null ? gm.checkpoint : Vector3.zero;
        health.ResetHealth();
        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;
        stunCounter = 0f;
        IsDead = false;
        if (sr != null) sr.enabled = true;
    }
}
