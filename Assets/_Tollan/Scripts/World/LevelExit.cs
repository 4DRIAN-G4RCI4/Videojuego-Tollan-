using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Final del nivel: muestra mensaje y carga la siguiente escena (si existe en Build Settings).</summary>
[RequireComponent(typeof(Collider2D))]
public class LevelExit : MonoBehaviour
{
    [TextArea] public string message = "¡Nivel completado!";
    [Tooltip("Nombre de la siguiente escena (déjalo vacío si aún no existe)")]
    public string nextScene = "";
    public float delay = 3f;

    bool done;

    void OnTriggerEnter2D(Collider2D other)
    {
        var pc = other.GetComponentInParent<PlayerController>();
        if (pc == null || done) return;
        done = true;
        pc.InputLocked = true;
        HUD.Instance?.ShowMessage(message, 999f);
        if (!string.IsNullOrEmpty(nextScene) && Application.CanStreamedLevelBeLoaded(nextScene))
            StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(nextScene);
    }
}
