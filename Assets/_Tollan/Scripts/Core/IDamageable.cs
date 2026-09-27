using UnityEngine;

/// <summary>Todo lo que puede recibir un golpe (jugador, enemigos, bloques).</summary>
public interface IDamageable
{
    void TakeDamage(int amount, Vector2 from);
}
