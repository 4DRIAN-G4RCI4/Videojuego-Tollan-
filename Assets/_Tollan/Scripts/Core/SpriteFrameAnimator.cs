using UnityEngine;

/// <summary>Animación simple en loop para props (antorchas, braseros, ofrendas, potenciadores).</summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFrameAnimator : MonoBehaviour
{
    public Sprite[] frames;
    public float fps = 8f;
    public bool randomStart = true;

    SpriteRenderer sr;
    float t;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (randomStart) t = Random.value * 10f;
    }

    void Update()
    {
        if (frames == null || frames.Length == 0) return;
        t += Time.deltaTime;
        sr.sprite = frames[(int)(t * fps) % frames.Length];
    }
}
