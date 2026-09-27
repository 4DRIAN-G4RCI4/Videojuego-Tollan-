using System.Collections;
using UnityEngine;

/// <summary>
/// Pequeña escena: Tezcatl aparece junto al Atlante, se ríe y escapa volando hacia el cielo.
/// Se activa al terminar el diálogo del Atlante (DialogueTrigger.activateOnFinish).
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class TezcatlCameo : MonoBehaviour
{
    public Vector2 flyDirection = new Vector2(1.2f, 1f);
    public float flySpeed = 4f;
    public float lifetime = 3.2f;
    [TextArea] public string message = "Tezcatl escapó hacia los cuatro rumbos del valle...";

    SpriteRenderer sr;

    void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        var c = sr.color;
        for (float t = 0; t < 0.6f; t += Time.deltaTime)         // aparece
        {
            sr.color = new Color(c.r, c.g, c.b, t / 0.6f);
            yield return null;
        }
        yield return new WaitForSeconds(0.6f);

        Vector3 start = transform.position;
        Vector3 dir = ((Vector3)flyDirection).normalized;
        for (float t = 0; t < lifetime; t += Time.deltaTime)     // escapa ondulando
        {
            transform.position = start + dir * flySpeed * t + Vector3.up * Mathf.Sin(t * 8f) * 0.25f;
            sr.color = new Color(c.r, c.g, c.b, 1f - t / lifetime);
            yield return null;
        }
        HUD.Instance?.ShowMessage(message, 3.5f);   // después del mensaje del Pico
        Destroy(gameObject);
    }
}
