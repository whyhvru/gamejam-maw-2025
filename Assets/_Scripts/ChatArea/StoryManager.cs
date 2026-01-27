using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    private const float LineDelay = 3f;
    private const int EndSceneBuildIndex = 4;

    [Header("Executors")]
    [SerializeField] private DeathManager _deathManager;

    [Header("References")]
    [SerializeField] private ChatUI _chatUI;
    [SerializeField] private InputRouter _inputRouter;

    [Header("Settings")]
    [SerializeField] private AudioSource _walkAudioSource;

    private StoryData _storyData;
    private Coroutine _storyCoroutine;
    private int _currentIndex;
    private float _nextLineTime;
    private bool _isPausedByEnemy = false;

    private List<IStoryEventReceiver> _eventReceivers = new();

    private void Awake()
    {
        LoadStory();

        _eventReceivers.AddRange(FindObjectsOfType<MonoBehaviour>(true).OfType<IStoryEventReceiver>());
    }

    private void LoadStory()
    {
        TextAsset json = Resources.Load<TextAsset>("Story/story");
        _storyData = JsonUtility.FromJson<StoryData>(json.text);
    }

    private void OnEnable()
    {
        _inputRouter.OnStartWalk += StartStory;
        _inputRouter.OnStopWalk += StopStory;
        _deathManager.OnPlayerDeath += StopStory;
    }

    private void OnDisable()
    {
        _inputRouter.OnStartWalk -= StartStory;
        _inputRouter.OnStopWalk -= StopStory;
        _deathManager.OnPlayerDeath -= StopStory;
    }

    public void StartStory()
    {
        if (!GameState.IsPlayerAlive || _isPausedByEnemy)
        {
            return;
        }

        _storyCoroutine ??= StartCoroutine(PlayStory());

        if (_walkAudioSource != null && !_walkAudioSource.isPlaying)
        {
            _walkAudioSource.Play();
        }
    }

    public void StopStory()
    {
        if (_storyCoroutine != null)
        {
            StopCoroutine(_storyCoroutine);
            _storyCoroutine = null;
        }

        if (_walkAudioSource != null && _walkAudioSource.isPlaying)
        {
            _walkAudioSource.Stop();
        }
    }

    private IEnumerator PlayStory()
    {
        while (_currentIndex < _storyData.lines.Count)
        {
            while (Time.time < _nextLineTime)
                yield return null;

            StoryLine line = _storyData.lines[_currentIndex];

            if (!string.IsNullOrEmpty(line.@event))
            {
                foreach (var receiver in _eventReceivers)
                    receiver.OnStoryEvent(line.@event);
            }

            string text = string.IsNullOrEmpty(line.author)
                        ? $"{line.text}\n"
                        : $"{line.author}: {line.text}\n";

            _chatUI.AddLine(text);

            _currentIndex++;
            _nextLineTime = Time.time + LineDelay;
        }

        SceneManager.LoadScene(EndSceneBuildIndex);
    }

    public void PauseStory()
    {
        if (_storyCoroutine != null)
        {
            StopCoroutine(_storyCoroutine);
            _storyCoroutine = null;
        }

        _isPausedByEnemy = true;

        if (_walkAudioSource != null && _walkAudioSource.isPlaying)
        {
            _walkAudioSource.Stop();
        }
    }

    public void ResumeStory()
    {
        _isPausedByEnemy = false;
    }
}
