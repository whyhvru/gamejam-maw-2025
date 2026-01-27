using UnityEngine;

public class WolfEnemy : Enemy
{
    private float _spawnTime;
    private bool _isWhistling = false;
    private float _whistleDuration = 0f;
    private bool _repelled = false;

    private const float _killDelay = 7f;
    private const float _requiredWhistleDuration = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _howlClip;
    [SerializeField] private AudioClip _whineClip;

    public override void Init(EnemyManager manager,
    DeathManager deathManager,
    DodgeManager dodge,
    AttackManager attackManager,
    WhistleManager whistleManager,
    EarCoverManager earCoverManager,
    FlintManager flintManager)
    {
        base.Init(manager, deathManager, dodge, attackManager, whistleManager, earCoverManager, flintManager);
        _spawnTime = Time.time;

        if (_audioSource != null && _howlClip != null)
        {
            _audioSource.PlayOneShot(_howlClip);
        }

        _dodgeManager.OnDodge += Kill;
        _flintManager.OnFlintUsed += Kill;
        _attackManager.OnAttack += Kill;
        _earCoverManager.OnCoverStarted += Kill;
        _whistleManager.OnWhistleStarted += OnWhistleStart;
        _whistleManager.OnWhistleStopped += OnWhistleStop;
    }

    public override void Tick(float deltaTime)
    {
        if (_repelled)
            return;

        if (_isWhistling)
        {
            _whistleDuration += deltaTime;
            if (_whistleDuration >= _requiredWhistleDuration)
            {
                RepelWolf();
            }
        }

        if (Time.time - _spawnTime >= _killDelay && !_repelled)
        {
            _whistleManager.OnWhistleStarted -= OnWhistleStart;
            _whistleManager.OnWhistleStopped -= OnWhistleStop;
            Kill();
        }
    }

    private void RepelWolf()
    {
        _repelled = true;

        if (_whineClip != null)
        {
            _audioSource.Stop();
            _audioSource.PlayOneShot(_whineClip);
        }

        _enemyManager.DespawnEnemy(this);
        _whistleManager.OnWhistleStarted -= OnWhistleStart;
        _whistleManager.OnWhistleStopped -= OnWhistleStop;
    }

    private void Kill()
    {
        _deathManager.Kill(DeathReasons.wolfEnemy, DeathFactors.enemyFactorName);
        _enemyManager.DespawnEnemy(this);
    }

    private void OnWhistleStart()
    {
        _isWhistling = true;
        _whistleDuration = 0f;
    }

    private void OnWhistleStop()
    {
        _isWhistling = false;
        _whistleDuration = 0f;
    }

    private void OnDestroy()
    {
        _dodgeManager.OnDodge -= Kill;
        _flintManager.OnFlintUsed -= Kill;
        _attackManager.OnAttack -= Kill;
        _earCoverManager.OnCoverStarted -= Kill;
        _whistleManager.OnWhistleStarted -= OnWhistleStart;
        _whistleManager.OnWhistleStopped -= OnWhistleStop;
    }
}
