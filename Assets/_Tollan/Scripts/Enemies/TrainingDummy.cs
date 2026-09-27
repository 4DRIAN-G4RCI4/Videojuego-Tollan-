using System.Collections;
using UnityEngine;

/// <summary>Muñeco de práctica del tutorial: recibe golpes y nunca muere.</summary>
[RequireComponent(typeof(Health))]
public class TrainingDummy : MonoBehaviour
{
    Health health;
    SpriteRenderer sr;
    Color baseColor;
    Quaternion baseRot;

    void Awake()
    {
        health = GetComponent<Health>();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) baseColor = sr.color;
        baseRot = transform.rotation;
        health.OnDamaged += from => { StopAllCoroutines(); StartCoroutine(Hit(from)); };
        health.OnDied += () => health.ResetHealth();
    }

    IEnumerator Hit(Vector2 from)
    {
        float dir = Mathf.Sign(transform.position.x - from.x);
        if (sr != null) sr.color = Color.white;
        transform.rotation = Quaternion.Euler(0, 0, -12f * dir);
        yield return new WaitForSeconds(0.08f);
        if (sr != null) sr.color = baseColor;
        yield return new WaitForSeconds(0.08f);
        transform.rotation = baseRot;
    }
}
