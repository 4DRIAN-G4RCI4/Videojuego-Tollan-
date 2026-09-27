using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>Hace "respirar" un sprite y/o una luz 2D (grietas ámbar, ojos, fuego).</summary>
public class GlowPulse : MonoBehaviour
{
    public float speed = 2f;
    [Range(0f, 1f)] public float minAlpha = 0.35f;
    public float lightMin = 0.4f;
    public float lightMax = 1.2f;
    public bool flicker = false;   // fuego: parpadeo irregular

    SpriteRenderer sr;
    Light2D light2D;
    float seed;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        light2D = GetComponent<Light2D>();
        seed = Random.value * 10f;
    }

    void Update()
    {
        float t = flicker
            ? Mathf.PerlinNoise(seed, Time.time * speed * 3f)
            : (Mathf.Sin((Time.time + seed) * speed) + 1f) * 0.5f;

        if (sr != null)
        {
            var c = sr.color;
            c.a = Mathf.Lerp(minAlpha, 1f, t);
            sr.color = c;
        }
        if (light2D != null)
            light2D.intensity = Mathf.Lerp(lightMin, lightMax, t);
    }
}
