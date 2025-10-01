using System;
using UnityEngine;
using UnityEngine.Audio;

public class EarCoverManager : MonoBehaviour
{
    [SerializeField] private InputRouter _inputRouter;
    [SerializeField] private AudioMixer _audioMixer;

    public bool IsCovering { get; private set; }

    private void OnEnable()
    {
        _inputRouter.OnHoldEars += OnHoldEars;
        _inputRouter.OnReleaseEars += OnReleaseEars;
    }

    private void OnDisable()
    {
        _inputRouter.OnHoldEars -= OnHoldEars;
        _inputRouter.OnReleaseEars -= OnReleaseEars;
    }

    private void OnHoldEars()
    {
        IsCovering = true;
        OnCoverStarted?.Invoke();
        _audioMixer.SetFloat("sfx", -6f);
    }

    private void OnReleaseEars()
    {
        IsCovering = false;
        OnCoverStopped?.Invoke();
        _audioMixer.SetFloat("sfx", 0f);
    }

    public event Action OnCoverStarted;
    public event Action OnCoverStopped;
}