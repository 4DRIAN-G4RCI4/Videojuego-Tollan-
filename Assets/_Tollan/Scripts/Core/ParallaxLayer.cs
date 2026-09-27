using UnityEngine;

/// <summary>
/// Capa de fondo con parallax. factor 1 = se mueve igual que la cámara (se ve "lejos"),
/// factor 0 = fija en el mundo.
/// </summary>
[DefaultExecutionOrder(100)]
public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 1f)] public float factorX = 0.5f;
    [Range(0f, 1f)] public float factorY = 0.2f;

    Transform cam;
    Vector3 startPos, camStart;

    void Start()
    {
        if (Camera.main == null) { enabled = false; return; }
        cam = Camera.main.transform;
        startPos = transform.position;
        camStart = cam.position;
    }

    void LateUpdate()
    {
        Vector3 d = cam.position - camStart;
        transform.position = new Vector3(startPos.x + d.x * factorX, startPos.y + d.y * factorY, startPos.z);
    }
}
