using UnityEngine;

public class EvilGnomeEnemy : Enemy
{
    private float _spawnTime;
    private float _timeToKill = 5f;
    private bool _isGone = false;

    public override void Init(EnemyManager manager,
    DeathManager deathManager,
    DodgeManager dodge,
    AttackManager attackManager,
    WhistleManager whistleManager,
    EarCoverManager earCoverManager,
    FlintManager flintManager)
    {
        base.Init(manager, deathManager, dodge, attackManager, whistleManager, earCoverManager, flintManager);
        _attackManager.OnAttack += TryKill;
        _dodgeManager.OnDodge += Kill;
        _flintManager.OnFlintUsed += Kill;
        _earCoverManager.OnCoverStarted += Kill;
        _whistleManager.OnWhistleStarted += Kill;
        _spawnTime = Time.time;
        _isGone = false;
    }

    public override void Tick(float deltaTime)
    {
        if (_isGone) return;

        if (Time.time - _spawnTime >= _timeToKill)
        {
            Kill();
            _isGone = true;
        }
    }

    private void Kill()
    {
        _deathManager.Kill(DeathReasons.gnomeEnemy, DeathFactors.enemyFactorName);
        _enemyManager.DespawnEnemy(this);
    }

    private void TryKill()
    {
        if (_isGone) return;

        _enemyManager.DespawnEnemy(this);
        _attackManager.OnAttack -= TryKill;
        _isGone = true;
    }

    private void OnDestroy()
    {
        _attackManager.OnAttack -= TryKill;
        _dodgeManager.OnDodge -= Kill;
        _flintManager.OnFlintUsed -= Kill;
        _earCoverManager.OnCoverStarted -= Kill;
        _whistleManager.OnWhistleStarted -= Kill;
    }
}