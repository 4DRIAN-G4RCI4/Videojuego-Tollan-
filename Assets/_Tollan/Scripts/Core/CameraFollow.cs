using UnityEngine;

/// <summary>Cámara que sigue al jugador con suavizado y límites del nivel.</summary>
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector2 offset = new Vector2(2f, 1.5f);
    public float smoothTime = 0.15f;

    [Header("Límites")]
    public float minX = -1000f;
    public float maxX = 1000f;
    public float minY = -1000f;
    public float maxY = 1000f;

    Vector3 velocity;

    void LateUpdate()
    {
        if (target == null) return;
        float facing = Mathf.Sign(target.localScale.x);
        Vector3 desired = new Vector3(target.position.x + offset.x * facing, target.position.y + offset.y, transform.position.z);
        desired.x = Mathf.Clamp(desired.x, minX, maxX);
        desired.y = Mathf.Clamp(desired.y, minY, maxY);
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }
}
