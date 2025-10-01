using UnityEngine;

public class GhostEnemy : Enemy
{
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _screamClip;

    private float _lifeTime = 18.5f;
    private float _uncoveredLimit = 9f;

    private float _lifeTimer = 0f;
    private float _uncoveredTimer = 0f;

    private bool _isCovered = false;

    public override void Init(EnemyManager manager,
    DeathManager deathManager,
    DodgeManager dodge,
    AttackManager attackManager,
    WhistleManager whistleManager,
    EarCoverManager earCoverManager,
    FlintManager flintManager)
    {
        base.Init(manager, deathManager, dodge, attackManager, whistleManager, earCoverManager, flintManager);
        _dodgeManager.OnDodge += Kill;
        _flintManager.OnFlintUsed += Kill;
        _attackManager.OnAttack += Kill;
        _whistleManager.OnWhistleStarted += Kill;
        _earCoverManager.OnCoverStarted += Kill;
        _earCoverManager.OnCoverStopped += OnCoverStopped;
    }

    private void OnCoverStarted()
    {
        _isCovered = true;
    }

    private void OnCoverStopped()
    {
        _isCovered = false;
    }

    public override void Tick(float deltaTime)
    {
        _lifeTimer += deltaTime;

        if (!_isCovered)
        {
            _uncoveredTimer += deltaTime;

            if (!_audioSource.isPlaying && _screamClip != null)
            {
                _audioSource.clip = _screamClip;
                _audioSource.loop = true;
                _audioSource.Play();
            }

            if (_uncoveredTimer >= _uncoveredLimit)
            {
                Kill();
            }
        }
        else
        {
            if (_audioSource.isPlaying)
                _audioSource.Stop();
        }

        if (_lifeTimer >= _lifeTime)
        {
            _enemyManager.DespawnEnemy(this);
        }
    }

    private void Kill()
    {
        _deathManager.Kill(DeathReasons.ghostEnemy, DeathFactors.enemyFactorName);
        _enemyManager.DespawnEnemy(this);
    }

    private void OnDestroy()
    {
        _dodgeManager.OnDodge -= Kill;
        _flintManager.OnFlintUsed -= Kill;
        _attackManager.OnAttack -= Kill;
        _whistleManager.OnWhistleStarted -= Kill;
        _earCoverManager.OnCoverStarted -= Kill;
        _earCoverManager.OnCoverStopped -= OnCoverStopped;
    }
}
