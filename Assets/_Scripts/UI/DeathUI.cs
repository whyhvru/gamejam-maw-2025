using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private DeathManager _deathManager;
    [SerializeField] private CanvasGroup _blackFade;
    [SerializeField] private RectTransform _moveableBlock;
    [SerializeField] private TMP_Text _madnessText;
    [SerializeField] private TMP_Text _madnessLabelText;
    [SerializeField] private TMP_Text _reasonText;
    [SerializeField] private AudioSource _deathAudioSource;
    [SerializeField] private AudioClip _deathClip;

    private readonly float[] _tickDelays = new float[4] { 0.8f, 2f, 3.2f, 4.4f };
    private readonly float _deathScreenDuration = 5f;
    private readonly float _fadeDuration = 1f;
    private readonly float _slideDuration = 1f;
    private Coroutine _currentRoutine;
    private Vector2 _centerPos = new(0f, 0f);
    private Vector2 _bottomPos = new(0f, -474f);
    private float _previousMadness = 0f;

    private void Awake()
    {
        _blackFade.alpha = 0f;
        _moveableBlock.anchoredPosition = _bottomPos;

        _madnessText.enabled = false;
        _madnessLabelText.enabled = false;
        _reasonText.enabled = false;
    }

    public void ShowDeathScreen(string reason, float madnessNormalized)
    {
        if (_currentRoutine != null)
            StopCoroutine(_currentRoutine);

        _currentRoutine = StartCoroutine(AnimateDeath(reason, madnessNormalized));
    }

    private IEnumerator AnimateDeath(string reason, float targetMadness)
    {
        float startTime = Time.time;

        _blackFade.alpha = 1f;
        _moveableBlock.anchoredPosition = _centerPos;
        _madnessLabelText.enabled = true;
        _madnessText.enabled = true;
        _reasonText.enabled = true;
        _reasonText.text = $"Причина смерти: {reason}";

        _deathAudioSource.clip = _deathClip;
        _deathAudioSource.Play();

        float madnessStep = (targetMadness - _previousMadness) / _tickDelays.Length;
        float currentMadness = _previousMadness;

        for (int i = 0; i < _tickDelays.Length; i++)
        {
            yield return new WaitForSeconds(_tickDelays[i] - (i > 0 ? _tickDelays[i - 1] : 0));
            currentMadness += madnessStep;
            _madnessText.text = $"{Mathf.RoundToInt(currentMadness)}%";
        }

        if (currentMadness >= 100f)
        {
            SceneManager.LoadScene(3);
        }

        _previousMadness = targetMadness;

        while (Time.time - startTime < _deathScreenDuration)
            yield return null;

        float fadeTimer = 0f;
        while (fadeTimer < _fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float t = fadeTimer / _fadeDuration;
            _blackFade.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        _madnessLabelText.enabled = false;
        _reasonText.enabled = false;

        float slideTimer = 0f;
        Vector2 start = _centerPos;
        while (slideTimer < _slideDuration)
        {
            slideTimer += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, slideTimer / _slideDuration);
            _moveableBlock.anchoredPosition = Vector2.Lerp(start, _bottomPos, t);
            yield return null;
        }

        _moveableBlock.anchoredPosition = _bottomPos;
        _deathManager.Respawn();
        _currentRoutine = null;
    }
}
