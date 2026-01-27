using System;
using UnityEngine;
using UnityEngine.Audio;

public class DeathManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Executors")]
    [SerializeField] private BreathManager _breathManager;
    [SerializeField] private HeartManager _heartManager;

    [Header("References")]
    [SerializeField] private DeathUI _deathUI;
    [SerializeField] private MadnessManager _madnessManager;

    private bool _isDead = false;

    public void Kill(string reason, string factor)
    {
        if (_isDead) return;
        _isDead = true;
        GameState.IsPlayerAlive = !_isDead;
        OnPlayerDeath?.Invoke();

        float madness = _madnessManager.IncreaseMadness(factor);
        _deathUI.ShowDeathScreen(reason, madness);
    }

    public void Respawn()
    {
        _isDead = false;
        GameState.IsPlayerAlive = !_isDead;
        _audioMixer.SetFloat("sfx", 0f);

        _breathManager.Respawn();
        _heartManager.Respawn();
    }

    public bool IsDead => _isDead;
    public Action OnPlayerDeath;
}
