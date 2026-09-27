using UnityEngine;

/// <summary>
/// Zona de tutorial: al entrar muestra un cartel con la explicación (arriba de la pantalla)
/// y lo oculta al salir. No detiene el juego.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TutorialHint : MonoBehaviour
{
    [TextArea(2, 4)] public string text;
    [Tooltip("Solo se muestra si el jugador ya tiene el Pico")]
    public bool requirePico = false;
    [Tooltip("Solo se muestra si ya se desbloqueó el cambio de personaje")]
    public bool requireSwitch = false;
    [Tooltip("Solo se muestra si el jugador tiene tiros de escopeta")]
    public bool requireShotgun = false;

    void OnTriggerEnter2D(Collider2D other) => TryShow(other);
    void OnTriggerStay2D(Collider2D other) => TryShow(other);   // por si se cumple la condición estando dentro

    void TryShow(Collider2D other)
    {
        var pc = other.GetComponentInParent<PlayerController>();
        if (pc == null || HUD.Instance == null || HUD.Instance.IsShowingHint(this)) return;
        if (requirePico && (!pc.TryGetComponent<PlayerCombat>(out var combat) || !combat.hasPico)) return;
        if (requireSwitch && (!pc.TryGetComponent<PlayerCharacterSwitch>(out var sw) || !sw.unlocked)) return;
        if (requireShotgun && (!pc.TryGetComponent<PlayerShotgun>(out var sg) || sg.ammo <= 0)) return;
        HUD.Instance.ShowHint(text, this);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == null || HUD.Instance == null) return;
        HUD.Instance.HideHint(this);
    }
}
