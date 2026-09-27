using System.Collections;
using UnityEngine;

/// <summary>Música de fondo en loop con fade in.  (Tecla M: silenciar / activar)</summary>
[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 0.45f;
    public float fadeIn = 2f;

    AudioSource src;
    bool muted;

    void Start()
    {
        src = GetComponent<AudioSource>();
        if (clip == null) return;
        src.clip = clip;
        src.loop = true;
        src.playOnAwake = false;
        src.volume = 0f;
        src.Play();
        StartCoroutine(Fade(0f, volume, fadeIn));
    }

    void Update()
    {
        var k = UnityEngine.InputSystem.Keyboard.current;
        if (k != null && k.mKey.wasPressedThisFrame)
        {
            muted = !muted;
            src.mute = muted;
            HUD.Instance?.ShowMessage(muted ? "Música: OFF  [M]" : "Música: ON  [M]", 1.2f);
        }
    }

    public void FadeOut(float time) => StartCoroutine(Fade(src.volume, 0f, time));

    IEnumerator Fade(float from, float to, float time)
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            src.volume = Mathf.Lerp(from, to, t / time);
            yield return null;
        }
        src.volume = to;
    }
}
