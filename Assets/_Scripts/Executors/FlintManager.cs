using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FlintManager : MonoBehaviour
{
    [SerializeField] private InputRouter _inputRouter;
    [SerializeField] private AudioClip[] _flintSounds;
    [SerializeField] private Volume _volume;

    private AudioSource _audioSource;
    private Coroutine _flintCoroutine;
    private Bloom _bloom;
    private readonly float _bloomFlashIntensity = 2f;
    private readonly float _bloomFadeDuration = 0.5f;
    private float _initialBloomIntensity;

    public bool IsOn { get; private set; }
    public float ElapsedOnTime { get; private set; }

    public event Action OnFlintUsed;
    public event Action OnFlintExpired;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (_volume != null && _volume.profile.TryGet(out _bloom))
        {
            _initialBloomIntensity = _bloom.intensity.value;
        }
    }

    private void OnEnable()
    {
        _inputRouter.OnUseFlint += UseFlint;
    }

    private void OnDisable()
    {
        _inputRouter.OnUseFlint -= UseFlint;
    }

    public void UseFlint()
    {
        if (IsOn) return;

        IsOn = true;
        OnFlintUsed?.Invoke();

        PlayRandomFlintSound();
        TriggerBloomFlash();

        if (_flintCoroutine != null)
            StopCoroutine(_flintCoroutine);

        _flintCoroutine = StartCoroutine(FlintLifetimeRoutine());
    }

    private void PlayRandomFlintSound()
    {
        if (_flintSounds == null || _flintSounds.Length == 0 || _audioSource == null) return;

        var clip = _flintSounds[UnityEngine.Random.Range(0, _flintSounds.Length)];
        _audioSource.PlayOneShot(clip);
    }

    private void TriggerBloomFlash()
    {
        if (_bloom == null) return;
        StopCoroutine(nameof(BloomFlashRoutine));
        StartCoroutine(BloomFlashRoutine());
    }

    private IEnumerator BloomFlashRoutine()
    {
        _bloom.intensity.value = _bloomFlashIntensity;

        float elapsed = 0f;
        while (elapsed < _bloomFadeDuration)
        {
            _bloom.intensity.value = Mathf.Lerp(_bloomFlashIntensity, _initialBloomIntensity, elapsed / _bloomFadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _bloom.intensity.value = _initialBloomIntensity;
    }

    private IEnumerator FlintLifetimeRoutine()
    {
        ElapsedOnTime = 0f;
        float duration = 0.5f;

        while (ElapsedOnTime < duration)
        {
            ElapsedOnTime += Time.deltaTime;
            yield return null;
        }

        IsOn = false;
        OnFlintExpired?.Invoke();
    }
}
