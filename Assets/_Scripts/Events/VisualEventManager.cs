using System.Globalization;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VisualEventManager : MonoBehaviour, IStoryEventReceiver
{
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private Image _visualWindow;
    [SerializeField] private Sprite[] _visuals;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioClip[] _audioClips;

    private Sprite _previousVisual;

    public void OnStoryEvent(string eventTag)
    {
        if (string.IsNullOrEmpty(eventTag)) return;

        var commands = eventTag.Split(',');

        foreach (var rawCommand in commands)
        {
            var trimmed = rawCommand.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            var parts = trimmed.Split(':');
            var command = parts[0].Trim();
            var argument = parts.Length > 1 ? parts[1] : "";

            switch (command)
            {
                case "ShowVisual": ShowVisual(argument); break;
                case "SpawnEnemy": SpawnEnemy(argument); break;
                case "ShakeVisual": StartCoroutine(ShakeEffect()); break;
                case "PlaySFX":
                    var pair = argument.Split('-');
                    var sfxname = pair[0];
                    var volumeStr = pair.Length > 1 ? pair[1] : "1.0";
                    PlaySFX(sfxname, volumeStr);
                    break;
            }
        }
    }

    private void ShowVisual(string id)
    {
        var sprite = _visuals.FirstOrDefault(s => s.name == id);

        if (_visualWindow.sprite != null)
        {
            _previousVisual = _visualWindow.sprite;
        }

        _visualWindow.sprite = sprite;
    }

    public void RestorePreviousVisual()
    {
        if (_previousVisual != null)
        {
            _visualWindow.sprite = _previousVisual;
        }
    }

    private void SpawnEnemy(string Type)
    {
        _enemyManager.SpawnEnemy(Type);
    }

    private IEnumerator ShakeEffect()
    {
        var originalPos = _visualWindow.rectTransform.localPosition;
        float duration = 0.75f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            _visualWindow.rectTransform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * 5f;
            elapsed += Time.deltaTime;
            yield return null;
        }

        _visualWindow.rectTransform.localPosition = originalPos;
    }

    private void PlaySFX(string clipName, string volumeStr)
    {
        var clip = _audioClips.FirstOrDefault(c => c != null && c.name == clipName);
        if (clip == null)
        {
            return;
        }
        if (!float.TryParse(volumeStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float volume))
        {
            return;
        }

        _sfxSource.PlayOneShot(clip, volume);
    }
}
