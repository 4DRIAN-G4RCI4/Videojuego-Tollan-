using System.Collections;
using UnityEngine;

/// <summary>Bloque tallable: se rompe a golpes de pico y suelta escombros.</summary>
public class BreakableBlock : MonoBehaviour, IDamageable
{
    public int hits = 2;

    SpriteRenderer sr;
    Color baseColor;
    Vector3 basePos;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) baseColor = sr.color;
        basePos = transform.position;
    }

    public void TakeDamage(int amount, Vector2 from)
    {
        hits -= amount;
        if (hits <= 0) { Break(); return; }
        StopAllCoroutines();
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        if (sr != null) sr.color = Color.white;
        for (int i = 0; i < 6; i++)
        {
            transform.position = basePos + (Vector3)Random.insideUnitCircle * 0.06f;
            yield return new WaitForSeconds(0.02f);
        }
        transform.position = basePos;
        if (sr != null) sr.color = baseColor * 0.85f;
        baseColor = sr != null ? sr.color : baseColor;
    }

    void Break()
    {
        if (sr != null)
        {
            for (int i = 0; i < 5; i++)
            {
                var d = new GameObject("Escombro");
                d.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.3f;
                d.transform.localScale = Vector3.one * Random.Range(0.2f, 0.35f);
                var dsr = d.AddComponent<SpriteRenderer>();
                dsr.sprite = sr.sprite;
                dsr.color = baseColor;
                dsr.sortingOrder = sr.sortingOrder + 1;
                var rb = d.AddComponent<Rigidbody2D>();
                rb.gravityScale = 3f;
                rb.linearVelocity = new Vector2(Random.Range(-4f, 4f), Random.Range(3f, 7f));
                rb.angularVelocity = Random.Range(-360f, 360f);
                Destroy(d, 1.2f);
            }
        }
        Destroy(gameObject);
    }
}
