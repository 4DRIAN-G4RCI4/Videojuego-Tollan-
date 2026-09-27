using System;
using UnityEngine;

/// <summary>Vida genérica con invulnerabilidad temporal tras recibir daño.</summary>
public class Health : MonoBehaviour, IDamageable
{
    public int maxHealth = 3;
    [Tooltip("Segundos sin recibir daño después de un golpe")]
    public float invulnerableTime = 0f;

    public int Current { get; private set; }
    public bool IsDead => Current <= 0;
    public bool IsInvulnerable => invTimer > 0f;

    public event Action<int, int> OnChanged;   // (actual, máximo)
    public event Action<Vector2> OnDamaged;    // desde dónde vino el golpe
    public event Action OnDied;

    float invTimer;

    void Awake() => Current = maxHealth;

    void Update()
    {
        if (invTimer > 0f) invTimer -= Time.deltaTime;
    }

    public void TakeDamage(int amount, Vector2 from)
    {
        if (IsDead || invTimer > 0f || amount <= 0) return;
        Current = Mathf.Max(0, Current - amount);
        invTimer = invulnerableTime;
        OnChanged?.Invoke(Current, maxHealth);
        OnDamaged?.Invoke(from);
        if (Current == 0) OnDied?.Invoke();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        Current = Mathf.Min(maxHealth, Current + amount);
        OnChanged?.Invoke(Current, maxHealth);
    }

    /// <summary>Muerte directa (caídas al vacío), ignora invulnerabilidad.</summary>
    public void Kill()
    {
        if (IsDead) return;
        Current = 0;
        OnChanged?.Invoke(Current, maxHealth);
        OnDied?.Invoke();
    }

    public void ResetHealth()
    {
        Current = maxHealth;
        invTimer = 0f;
        OnChanged?.Invoke(Current, maxHealth);
    }
}
