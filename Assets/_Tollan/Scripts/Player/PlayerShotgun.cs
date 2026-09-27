using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Escopeta del Abuelo (potenciador): pocos tiros, mucho daño en un cono frente al jugador.
/// Disparar: K / Clic derecho / botón B del control.
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class PlayerShotgun : MonoBehaviour
{
    public int ammo = 0;
    public int maxAmmo = 5;
    public int damage = 6;
    public float cooldown = 0.6f;
    public float range = 4.5f;
    public float height = 1.6f;
    public float recoil = 4f;
    [Tooltip("Frames del fogonazo (Proyectiles_disparo_0..2)")]
    public Sprite[] blastFrames;

    public System.Action<int> OnAmmoChanged;

    PlayerController pc;
    Rigidbody2D rb;
    float cd;

    void Awake()
    {
        pc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start() => OnAmmoChanged?.Invoke(ammo);

    public void AddAmmo(int amount)
    {
        ammo = Mathf.Clamp(ammo + amount, 0, maxAmmo);
        OnAmmoChanged?.Invoke(ammo);
        HUD.Instance?.SetAmmo(ammo);
    }

    void Update()
    {
        cd -= Time.deltaTime;
        if (pc.InputLocked || pc.IsDead || ammo <= 0 || cd > 0f) return;
        if (TollanInput.ShootPressed) Fire();
    }

    void Fire()
    {
        cd = cooldown;
        ammo--;
        OnAmmoChanged?.Invoke(ammo);
        HUD.Instance?.SetAmmo(ammo);

        float dir = pc.FacingRight ? 1f : -1f;
        Vector2 origin = (Vector2)transform.position + new Vector2(0.6f * dir, 0.1f);
        Vector2 center = origin + new Vector2(range * 0.5f * dir, 0f);

        var done = new HashSet<IDamageable>();
        foreach (var h in Physics2D.OverlapBoxAll(center, new Vector2(range, height), 0f))
        {
            if (h.transform.IsChildOf(transform)) continue;
            var target = h.GetComponentInParent<IDamageable>();
            if (target == null || done.Contains(target)) continue;
            done.Add(target);
            target.TakeDamage(damage, transform.position);
        }

        rb.linearVelocity = new Vector2(-dir * recoil, rb.linearVelocity.y + 1.5f);   // retroceso
        if (TryGetComponent<PlayerAnimator>(out var anim)) anim.PlayOneShot("shoot", 0.3f);
        if (blastFrames != null && blastFrames.Length > 0) StartCoroutine(Blast(origin, dir));
        if (Camera.main != null) StartCoroutine(Shake(Camera.main.transform, 0.15f, 0.12f));
        if (ammo == 0) HUD.Instance?.ShowMessage("La escopeta se quedó sin tiros", 1.5f);
    }

    IEnumerator Blast(Vector2 origin, float dir)
    {
        var go = new GameObject("Fogonazo");
        go.transform.position = origin + new Vector2(0.5f * dir, 0f);
        go.transform.localScale = new Vector3(dir * 1.6f, 1.6f, 1f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 30;
        for (int i = 0; i < blastFrames.Length; i++)
        {
            sr.sprite = blastFrames[i];
            go.transform.position += new Vector3(0.7f * dir, 0f, 0f);
            yield return new WaitForSeconds(0.06f);
        }
        Destroy(go);
    }

    IEnumerator Shake(Transform cam, float time, float strength)
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            cam.position += (Vector3)(Random.insideUnitCircle * strength);
            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        float dir = transform.localScale.x >= 0 ? 1f : -1f;
        Gizmos.color = new Color(1f, 0.6f, 0.2f);
        Gizmos.DrawWireCube(transform.position + new Vector3((0.6f + range * 0.5f) * dir, 0.1f), new Vector3(range, height));
    }
}
