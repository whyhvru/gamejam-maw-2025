using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField] private InputRouter _inputRouter;
    [SerializeField] private EnemyManager _enemyManager;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _hitEnemyClip;
    [SerializeField] private AudioClip _missClip;

    private void OnEnable()
    {
        _inputRouter.OnAttack += HandleAttack;
    }

    private void OnDisable()
    {
        _inputRouter.OnAttack -= HandleAttack;
    }

    private void HandleAttack()
    {
        bool enemyIsPresent = EnemyExists();

        _audioSource.PlayOneShot(enemyIsPresent ? _hitEnemyClip : _missClip);
        OnAttack?.Invoke();
    }

    private bool EnemyExists()
    {
        return _enemyManager != null && _enemyManager.ActiveEnemyCount > 0;
    }

    public event System.Action OnAttack;
}