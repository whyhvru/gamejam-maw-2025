using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected EnemyManager _enemyManager;
    protected DeathManager _deathManager;
    protected DodgeManager _dodgeManager;
    protected AttackManager _attackManager;
    protected WhistleManager _whistleManager;
    protected EarCoverManager _earCoverManager;
    protected FlintManager _flintManager;

    [SerializeField] private string _id;
    public string Id => _id;

    public virtual void Init(EnemyManager manager,
    DeathManager deathManager,
    DodgeManager dodgeManager,
    AttackManager attackManager,
    WhistleManager whistleManager,
    EarCoverManager earCoverManager,
    FlintManager flintManager)
    {
        _enemyManager = manager;
        _deathManager = deathManager;
        _dodgeManager = dodgeManager;
        _attackManager = attackManager;
        _whistleManager = whistleManager;
        _earCoverManager = earCoverManager;
        _flintManager = flintManager;
    }

    public virtual void Tick(float deltaTime) { }
}
