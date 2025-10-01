using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BreathManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource _loopBreathAudio;
    [SerializeField] private AudioSource _oneShotAudio;
    [SerializeField] private AudioClip _holdStartClip;
    [SerializeField] private AudioClip _holdEndClip;

    [Header("Death Handling")]
    [SerializeField] private DeathManager _deathManager;

    [Header("References")]
    [SerializeField] private InputRouter _inputRouter;

    private readonly float _breathFillSpeed = 0.025f;
    private readonly float _breathDrainSpeed = 0.05f;
    private readonly float _holdFillSpeed = 0.0166f;
    private readonly float _holdDrainSpeed = 0.0333f;
    private readonly int _hyperventilationPressCount = 7;
    private readonly float _hyperventilationInputWindow = 10f;
    public float BreathLevel { get; private set; } = 0f;
    public float HoldLevel { get; private set; } = 0f;
    public bool IsHolding { get; private set; }
    public bool IsWhistling { get; private set; }

    private readonly Queue<float> _breathToggleTimestamps = new();

    private void OnEnable()
    {
        if (_inputRouter != null)
        {
            _inputRouter.OnToggleBreath += OnToggleBreath;
            _inputRouter.OnWhistleStart += OnWhistleStart;
            _inputRouter.OnWhistleStop += OnWhistleStop;
            _deathManager.OnPlayerDeath += StopBreathing;
        }

        if (_loopBreathAudio != null)
        {
            _loopBreathAudio.loop = true;
            _loopBreathAudio.Play();
        }
    }

    private void OnDisable()
    {
        if (_inputRouter != null)
        {
            _inputRouter.OnToggleBreath -= OnToggleBreath;
            _inputRouter.OnWhistleStart -= OnWhistleStart;
            _inputRouter.OnWhistleStop -= OnWhistleStop;
            _deathManager.OnPlayerDeath -= StopBreathing;
        }

        if (_loopBreathAudio != null)
            _loopBreathAudio.Stop();
    }

    private void Update()
    {
        UpdateBreath(Time.deltaTime);
        UpdateHold(Time.deltaTime);
        CheckDeath();

        UpdateBreathingAudio();
    }

    private void OnToggleBreath()
    {
        if (!IsWhistling)
        {
            bool wasHolding = IsHolding;
            IsHolding = !IsHolding;

            if (IsHolding && !wasHolding)
                PlayOneShot(_holdStartClip);
            else if (!IsHolding && wasHolding)
                PlayOneShot(_holdEndClip);

            TrackHyperventilationInput();
        }
    }

    private void OnWhistleStart()
    {
        IsWhistling = true;
        IsHolding = true;
    }

    private void OnWhistleStop()
    {
        IsWhistling = false;
        IsHolding = false;
    }

    private void UpdateBreath(float dt)
    {
        if (IsHolding)
            BreathLevel -= _breathDrainSpeed * dt;
        else
            BreathLevel += _breathFillSpeed * dt;

        BreathLevel = Mathf.Clamp01(BreathLevel);
    }

    private void UpdateHold(float dt)
    {
        if (IsHolding)
            HoldLevel += _holdFillSpeed * dt;
        else
            HoldLevel -= _holdDrainSpeed * dt;

        HoldLevel = Mathf.Clamp01(HoldLevel);
    }

    private void TrackHyperventilationInput()
    {
        float now = Time.time;
        _breathToggleTimestamps.Enqueue(now);

        while (_breathToggleTimestamps.Count > 0 && now - _breathToggleTimestamps.Peek() > _hyperventilationInputWindow)
            _breathToggleTimestamps.Dequeue();

        if (_breathToggleTimestamps.Count >= _hyperventilationPressCount)
        {
            _deathManager?.Kill(DeathReasons.hyperventilation, DeathFactors.hyperFactorName);
            enabled = false;
        }
    }

    private void CheckDeath()
    {
        if (BreathLevel >= 1f)
        {
            _deathManager?.Kill(DeathReasons.toxicFog, DeathFactors.breathFactorName);
            enabled = false;
        }
        else if (HoldLevel >= 1f)
        {
            _deathManager?.Kill(DeathReasons.asphyxia, DeathFactors.breathFactorName);
            enabled = false;
        }
    }

    private void UpdateBreathingAudio()
    {
        if (_loopBreathAudio != null)
        {
            if (IsHolding || IsWhistling)
            {
                if (_loopBreathAudio.isPlaying)
                    _loopBreathAudio.Pause();
            }
            else
            {
                if (!_loopBreathAudio.isPlaying)
                    _loopBreathAudio.UnPause();
            }
        }
    }

    private void StopBreathing()
    {
        if (_loopBreathAudio != null)
            _loopBreathAudio.Stop();

        enabled = false;
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (_oneShotAudio != null && clip != null)
            _oneShotAudio.PlayOneShot(clip);
    }

    public void Respawn()
    {
        BreathLevel = 0f;
        HoldLevel = 0f;
        IsHolding = false;
        IsWhistling = false;
        _breathToggleTimestamps.Clear();
        enabled = true;

        if (_loopBreathAudio != null)
        {
            _loopBreathAudio.loop = true;
            _loopBreathAudio.Play();
        }
    }
}
