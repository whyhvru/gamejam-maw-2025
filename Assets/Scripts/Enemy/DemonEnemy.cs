using UnityEngine;

public class DemonEnemy : Enemy
{
    [SerializeField] private AudioSource _audioSource;

    private readonly float _reactionWindow = 1.2f;
    private float _spawnTime;

    private bool _reacted = false;

    public override void Init(EnemyManager manager,
    DeathManager deathManager,
    DodgeManager dodgeManager,
    AttackManager attackManager,
    WhistleManager whistleManager,
    EarCoverManager earCoverManager,
    FlintManager flintManager)
    {
        base.Init(manager, deathManager, dodgeManager, attackManager, whistleManager, earCoverManager, flintManager);
        _spawnTime = Time.time;
        _dodgeManager.OnDodge += TryDodge;
        _flintManager.OnFlintUsed += Kill;
        _attackManager.OnAttack += Kill;
        _earCoverManager.OnCoverStarted += Kill;
        _whistleManager.OnWhistleStarted += Kill;

        _audioSource.Play();
    }

    private void TryDodge()
    {
        if (Time.time - _spawnTime <= _reactionWindow)
        {
            _enemyManager.DespawnEnemy(this);
            _reacted = true;
        }
    }

    public override void Tick(float deltaTime)
    {
        if (!_reacted && Time.time - _spawnTime > _reactionWindow)
        {
            _dodgeManager.OnDodge -= TryDodge;
            Kill();
        }
    }

    private void Kill()
    {
        _deathManager.Kill(DeathReasons.demonEnemy, DeathFactors.enemyFactorName);
        _enemyManager.DespawnEnemy(this);
    }

    private void OnDestroy()
    {
        _dodgeManager.OnDodge -= TryDodge;
        _flintManager.OnFlintUsed -= Kill;
        _attackManager.OnAttack -= Kill;
        _earCoverManager.OnCoverStarted -= Kill;
        _whistleManager.OnWhistleStarted -= Kill;
    }
}
