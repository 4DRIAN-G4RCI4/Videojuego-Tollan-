using UnityEngine;

/// <summary>Cambia entre Ixtli e Itzcóatl (tecla C / bumper derecho) una vez desbloqueado.</summary>
public class PlayerCharacterSwitch : MonoBehaviour
{
    public bool unlocked = false;
    public RuntimeAnimatorController[] controllers;
    public string[] names = { "Ixtli", "Itzcóatl" };
    [Tooltip("Daño y espera entre golpes de cada arma: Pico (Ixtli) / Pala (Itzcóatl)")]
    public int[] damage = { 1, 2 };
    public float[] cooldown = { 0.3f, 0.5f };
    public Vector2[] hitbox = { new Vector2(1.2f, 1.2f), new Vector2(1.5f, 1.3f) };

    int index;
    Animator animator;
    PlayerAnimator playerAnimator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    public string CurrentName => names != null && index < names.Length ? names[index] : "";

    void Update()
    {
        if (!unlocked || controllers == null || controllers.Length < 2 || animator == null) return;
        if (TryGetComponent<PlayerController>(out var pc) && pc.InputLocked) return;
        if (TollanInput.SwitchPressed) Switch();
    }

    public void Switch()
    {
        index = (index + 1) % controllers.Length;
        animator.runtimeAnimatorController = controllers[index];
        if (playerAnimator != null) playerAnimator.ForceRefresh();
        if (TryGetComponent<PlayerCombat>(out var combat))
        {
            if (index < damage.Length) combat.damage = damage[index];
            if (index < cooldown.Length) combat.cooldown = cooldown[index];
            if (index < hitbox.Length) combat.hitboxSize = hitbox[index];
        }
        HUD.Instance?.ShowMessage("Ahora juegas con " + CurrentName, 1.5f);
    }
}
