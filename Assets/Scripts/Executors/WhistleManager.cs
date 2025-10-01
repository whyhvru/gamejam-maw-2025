using System;
using UnityEngine;

public class WhistleManager : MonoBehaviour
{
    [SerializeField] private InputRouter _inputRouter;

    private AudioSource _whistleAudioSource;

    private void Awake()
    {
        _whistleAudioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _inputRouter.OnWhistleStart += OnWhistleStart;
        _inputRouter.OnWhistleStop += OnWhistleStop;
    }

    private void OnDisable()
    {
        _inputRouter.OnWhistleStart -= OnWhistleStart;
        _inputRouter.OnWhistleStop -= OnWhistleStop;
    }

    private void OnWhistleStart()
    {
        _whistleAudioSource.Play();
        OnWhistleStarted?.Invoke();
    }

    private void OnWhistleStop()
    {
        _whistleAudioSource.Stop();
        OnWhistleStopped?.Invoke();
    }

    public Action OnWhistleStarted;
    public Action OnWhistleStopped;
}