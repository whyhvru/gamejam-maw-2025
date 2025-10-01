using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HeartManager : MonoBehaviour
{
    [SerializeField] private InputRouter _inputRouter;
    [SerializeField] private AudioSource _heartbeatAudio;
    [SerializeField] private AudioSource _oneshotAudioSource;
    [SerializeField] private AudioClip _ekgBeepClip;
    [SerializeField] private GameObject _heartbeatVisualBlock;
    [SerializeField] private Image _heartbeatFillImage;
    [SerializeField] private TMP_Text _bpmText;
    [SerializeField] private HeartTipUI _heartTipUI;

    private readonly float _visualDuration = 5f;
    private readonly float _minBPM = 60f;
    private readonly float _maxBPM = 150f;
    private readonly float _timeToMax = 180f;
    public float CurrentBPM { get; private set; }
    public bool IsHigh => CurrentBPM >= 145f;
    private bool _hasShownHighBpmTip = false;

    private float _timer = 0f;
    private Coroutine _visualCoroutine;

    private void OnEnable()
    {
        _inputRouter.OnHeartbeatVisual += OnHeartbeatVisual;
    }

    private void OnDisable()
    {
        _inputRouter.OnHeartbeatVisual -= OnHeartbeatVisual;
    }

    private void Start()
    {
        Respawn();
    }

    public void Respawn()
    {
        CurrentBPM = _minBPM;
        _heartbeatAudio.loop = true;
        _heartbeatAudio.Stop();

        if (_heartbeatVisualBlock != null)
            _heartbeatVisualBlock.SetActive(false);
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        float t = Mathf.Clamp01(_timer / _timeToMax);
        CurrentBPM = Mathf.Lerp(_minBPM, _maxBPM, t);

        if (IsHigh && !_heartbeatAudio.isPlaying)
        {
            _heartbeatAudio.Play();

            if (!_hasShownHighBpmTip)
            {
                _hasShownHighBpmTip = true;
                _heartTipUI.ShowTip();
            }
        }
        else if (!IsHigh && _heartbeatAudio.isPlaying)
        {
            _heartbeatAudio.Stop();
        }
    }

    public void ReduceHeartRate()
    {
        CurrentBPM = _minBPM;
        _timer = 0f;
    }

    private void OnHeartbeatVisual()
    {
        if (_visualCoroutine != null)
            StopCoroutine(_visualCoroutine);

        _visualCoroutine = StartCoroutine(ShowHeartbeatVisual());
    }

    private IEnumerator ShowHeartbeatVisual()
    {
        _heartbeatVisualBlock.SetActive(true);

        if (_ekgBeepClip != null)
            _oneshotAudioSource.PlayOneShot(_ekgBeepClip);

        _heartbeatFillImage.fillMethod = Image.FillMethod.Horizontal;
        _heartbeatFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        _heartbeatFillImage.fillAmount = 0f;

        float halfDuration = _visualDuration / 2f;
        float fullDuration = _visualDuration;
        float timer = 0f;

        float bpmUpdateTimer = 0f;
        int bpmUpdates = 0;

        while (timer < fullDuration)
        {
            timer += Time.deltaTime;
            bpmUpdateTimer += Time.deltaTime;

            if (bpmUpdates < 2 && bpmUpdateTimer >= 1f)
            {
                bpmUpdateTimer = 0f;
                bpmUpdates++;

                int offset = Random.Range(-2, 3); // -2, -1, 0, 1, 2
                int fluctuatedBPM = Mathf.Clamp(Mathf.RoundToInt(CurrentBPM) + offset, Mathf.RoundToInt(_minBPM), Mathf.RoundToInt(_maxBPM));
                _bpmText.text = fluctuatedBPM.ToString();
            }

            if (timer < halfDuration)
            {
                _heartbeatFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                _heartbeatFillImage.fillAmount = Mathf.Lerp(0f, 1f, timer / halfDuration);
            }
            else
            {
                float t = (timer - halfDuration) / halfDuration;
                _heartbeatFillImage.fillOrigin = (int)Image.OriginHorizontal.Right;
                _heartbeatFillImage.fillAmount = Mathf.Lerp(1f, 0f, t);
            }

            yield return null;
        }

        _heartbeatVisualBlock.SetActive(false);
        _visualCoroutine = null;
    }
}