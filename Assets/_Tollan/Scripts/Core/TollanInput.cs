using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Entrada centralizada (teclado + control). Usa el Input System nuevo.
/// Mover: A/D o flechas · Saltar: Espacio/Z/W/↑ · Atacar: X/J/Clic · Interactuar: E/Enter
/// </summary>
public static class TollanInput
{
    const float Deadzone = 0.3f;

    public static float Horizontal
    {
        get
        {
            float x = 0f;
            var k = Keyboard.current;
            if (k != null)
            {
                if (k.aKey.isPressed || k.leftArrowKey.isPressed) x -= 1f;
                if (k.dKey.isPressed || k.rightArrowKey.isPressed) x += 1f;
            }
            var g = Gamepad.current;
            if (g != null)
            {
                float gx = g.leftStick.x.ReadValue();
                if (Mathf.Abs(gx) > Deadzone) x = Mathf.Sign(gx);
                if (g.dpad.left.isPressed) x = -1f;
                if (g.dpad.right.isPressed) x = 1f;
            }
            return Mathf.Clamp(x, -1f, 1f);
        }
    }

    public static bool JumpPressed
    {
        get
        {
            var k = Keyboard.current; var g = Gamepad.current;
            return (k != null && (k.spaceKey.wasPressedThisFrame || k.zKey.wasPressedThisFrame ||
                                  k.wKey.wasPressedThisFrame || k.upArrowKey.wasPressedThisFrame))
                || (g != null && g.buttonSouth.wasPressedThisFrame);
        }
    }

    public static bool JumpHeld
    {
        get
        {
            var k = Keyboard.current; var g = Gamepad.current;
            return (k != null && (k.spaceKey.isPressed || k.zKey.isPressed || k.wKey.isPressed || k.upArrowKey.isPressed))
                || (g != null && g.buttonSouth.isPressed);
        }
    }

    public static bool AttackPressed
    {
        get
        {
            var k = Keyboard.current; var g = Gamepad.current; var m = Mouse.current;
            return (k != null && (k.xKey.wasPressedThisFrame || k.jKey.wasPressedThisFrame))
                || (m != null && m.leftButton.wasPressedThisFrame)
                || (g != null && g.buttonWest.wasPressedThisFrame);
        }
    }

    /// <summary>Disparar escopeta: K / Clic derecho / botón B (este).</summary>
    public static bool ShootPressed
    {
        get
        {
            var k = Keyboard.current; var g = Gamepad.current; var m = Mouse.current;
            return (k != null && k.kKey.wasPressedThisFrame)
                || (m != null && m.rightButton.wasPressedThisFrame)
                || (g != null && g.buttonEast.wasPressedThisFrame);
        }
    }

    /// <summary>Cambiar de personaje: C / bumper derecho.</summary>
    public static bool SwitchPressed
    {
        get
        {
            var k = Keyboard.current; var g = Gamepad.current;
            return (k != null && k.cKey.wasPressedThisFrame) || (g != null && g.rightShoulder.wasPressedThisFrame);
        }
    }

    public static bool InteractPressed
    {
        get
        {
            var k = Keyboard.current; var g = Gamepad.current;
            return (k != null && (k.eKey.wasPressedThisFrame || k.enterKey.wasPressedThisFrame))
                || (g != null && g.buttonNorth.wasPressedThisFrame);
        }
    }
}
