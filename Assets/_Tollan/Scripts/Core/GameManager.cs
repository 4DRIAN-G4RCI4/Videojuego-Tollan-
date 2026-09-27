using System;
using UnityEngine;

/// <summary>Estado de la partida: checkpoint actual y coleccionables.</summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Checkpoint (brasero)")]
    public Vector3 checkpoint;

    [Header("Progreso")]
    public int obsidianHearts;          // 0..4
    public int muralFragments;          // 0..12
    public const int TotalHearts = 4;
    public const int TotalMurals = 12;

    public event Action OnStatsChanged;

    void Awake()
    {
        Instance = this;
    }

    public void SetCheckpoint(Vector3 position) => checkpoint = position;

    public void AddObsidianHeart(int amount = 1)
    {
        obsidianHearts = Mathf.Clamp(obsidianHearts + amount, 0, TotalHearts);
        OnStatsChanged?.Invoke();
    }

    public void AddMuralFragment(int amount = 1)
    {
        muralFragments = Mathf.Clamp(muralFragments + amount, 0, TotalMurals);
        OnStatsChanged?.Invoke();
    }
}
