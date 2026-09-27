using UnityEngine;

/// <summary>
/// Cambia la animación del jugador según su estado: idle, walk, jump, fall.
/// Los ataques/disparos llaman a PlayOneShot("attack") desde PlayerCombat.
/// Los estados del Animator se llaman igual que las animaciones (los crea Tools > Tollan > 3).
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    public float walkThreshold = 0.3f;

    PlayerController pc;
    Rigidbody2D rb;
    string current;
    float lockUntil;

    void Awake()
    {
        pc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    /// <summary>Vuelve a reproducir el estado actual (tras cambiar de Animator Controller).</summary>
    public void ForceRefresh() => current = null;

    public void PlayOneShot(string state, float duration)
    {
        Play(state, true);
        lockUntil = Time.time + duration;
    }

    void Update()
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;
        if (Time.time < lockUntil) return;

        string s;
        if (!pc.Grounded) s = rb.linearVelocity.y > 0.1f ? "jump" : "fall";
        else s = Mathf.Abs(rb.linearVelocity.x) > walkThreshold ? "walk" : "idle";
        Play(s, false);
    }

    void Play(string state, bool restart)
    {
        if (!restart && state == current) return;
        current = state;
        animator.Play(state, 0, 0f);
    }
}
