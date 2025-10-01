using UnityEngine;

public class PillManager : MonoBehaviour
{
    [SerializeField] private InputRouter _inputRouter;
    [SerializeField] private HeartManager _heartManager;
    [SerializeField] private DeathManager _deathManager;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _gulpClip;

    private const int _pillOverdoseLimit = 3;
    private const float _overdoseWindow = 60f;
    private const float _safeThresholdBPM = 130f;

    private int _pillsTaken = 0;
    private float _pillTimer = 0f;

    private void OnEnable()
    {
        _inputRouter.OnTakeSedative += TakePill;
    }

    private void OnDisable()
    {
        _inputRouter.OnTakeSedative -= TakePill;
    }

    private void Update()
    {
        if (_pillsTaken > 0)
        {
            _pillTimer += Time.deltaTime;
            if (_pillTimer > _overdoseWindow)
            {
                _pillTimer = 0f;
                _pillsTaken = 0;
            }
        }
    }

    public void TakePill()
    {
        if (_heartManager.CurrentBPM < _safeThresholdBPM)
        {
            _deathManager.Kill(DeathReasons.infarct, DeathFactors.heartFactorName);
            return;
        }

        _pillsTaken++;
        _pillTimer = 0f;

        if (_pillsTaken > _pillOverdoseLimit)
        {
            _deathManager.Kill(DeathReasons.overdose, DeathFactors.heartFactorName);
            return;
        }

        _heartManager.ReduceHeartRate();

        if (_audioSource != null && _gulpClip != null)
        {
            _audioSource.PlayOneShot(_gulpClip);
        }
    }
}
