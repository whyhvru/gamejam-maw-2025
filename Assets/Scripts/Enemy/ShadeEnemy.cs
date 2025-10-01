using UnityEngine;

public class ShadeEnemy : Enemy
{
    private float _exposureTimer = 0f;
    private bool _flintTriggered = false;
    private float _flintStartTime;

    private const float _timeToKill = 4f;
    private const float _flintDespawnDelay = 0.75f;

    public override void Init(EnemyManager manager,
    DeathManager deathManager,
    DodgeManager dodge,
    AttackManager attackManager,
    WhistleManager whistleManager,
    EarCoverManager earCoverManager,
    FlintManager flintManager)
    {
        base.Init(manager, deathManager, dodge, attackManager, whistleManager, earCoverManager, flintManager);
        _flintManager.OnFlintUsed += OnFlintUsed;
        _dodgeManager.OnDodge += Kill;
        _attackManager.OnAttack += Kill;
        _earCoverManager.OnCoverStarted += Kill;
        _whistleManager.OnWhistleStarted += Kill;
    }

    private void OnFlintUsed()
    {
        _flintTriggered = true;
        _flintStartTime = Time.time;
    }

    public override void Tick(float deltaTime)
    {
        if (_flintTriggered && Time.time - _flintStartTime >= _flintDespawnDelay)
        {
            _enemyManager.DespawnEnemy(this);
            return;
        }

        _exposureTimer += deltaTime;
        if (_exposureTimer >= _timeToKill)
        {
            Kill();
        }
    }

    private void Kill()
    {
        _deathManager.Kill(DeathReasons.shadeEnemy, DeathFactors.enemyFactorName);
        _enemyManager.DespawnEnemy(this);
    }

    private void OnDestroy()
    {
        _flintManager.OnFlintUsed -= OnFlintUsed;
        _dodgeManager.OnDodge -= Kill;
        _attackManager.OnAttack -= Kill;
        _earCoverManager.OnCoverStarted -= Kill;
        _whistleManager.OnWhistleStarted -= Kill;
    }
}
