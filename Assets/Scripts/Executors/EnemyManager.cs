using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Managers for init")]
    [SerializeField] private WhistleManager _whistleManager;
    [SerializeField] private DeathManager _deathManager;
    [SerializeField] private FlintManager _flintManager;
    [SerializeField] private EarCoverManager _earCoverManager;
    [SerializeField] private AttackManager _attackManager;
    [SerializeField] private DodgeManager _dodgeManager;

    [Header("Settings")]
    [SerializeField] private VisualEventManager _visualEventManager;
    [SerializeField] private List<EnemyEntry> _enemyEntries;
    [SerializeField] private StoryManager _storyManager;
    [SerializeField] private EnemyTipUI _enemyTipUI;

    private Dictionary<string, Enemy> _enemyPrefabMap;
    private readonly List<Enemy> _activeEnemies = new();
    private readonly HashSet<string> _shownEnemyTips = new();

    public int ActiveEnemyCount => _activeEnemies.Count;

    private void Awake()
    {
        _enemyPrefabMap = new Dictionary<string, Enemy>();
        foreach (var entry in _enemyEntries)
        {
            if (entry != null && entry.Prefab != null && !_enemyPrefabMap.ContainsKey(entry.Type))
            {
                _enemyPrefabMap.Add(entry.Type, entry.Prefab);
            }
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            _activeEnemies[i].Tick(dt);
        }
    }

    public void SpawnEnemy(string Type)
    {
        if (!_enemyPrefabMap.TryGetValue(Type, out var prefab))
        {
            return;
        }

        Enemy instance = Instantiate(prefab);
        _activeEnemies.Add(instance);

        if (!_shownEnemyTips.Contains(Type))
        {
            _shownEnemyTips.Add(Type);
            ShowTipForEnemy(Type);
        }

        if (instance is ShadeEnemy shade)
        {
            shade.Init(this, _deathManager, _dodgeManager, _attackManager, _whistleManager, _earCoverManager, _flintManager);
        }
        if (instance is GhostEnemy ghost)
        {
            ghost.Init(this, _deathManager, _dodgeManager, _attackManager, _whistleManager, _earCoverManager, _flintManager);
        }
        if (instance is WolfEnemy wolf)
        {
            wolf.Init(this, _deathManager, _dodgeManager, _attackManager, _whistleManager, _earCoverManager, _flintManager);
        }
        if (instance is EvilGnomeEnemy gnome)
        {
            gnome.Init(this, _deathManager, _dodgeManager, _attackManager, _whistleManager, _earCoverManager, _flintManager);
        }
        if (instance is DemonEnemy demon)
        {
            demon.Init(this, _deathManager, _dodgeManager, _attackManager, _whistleManager, _earCoverManager, _flintManager);
        }

        _storyManager?.PauseStory();
    }

    public void DespawnEnemy(Enemy enemy)
    {
        if (_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Remove(enemy);
            _visualEventManager.RestorePreviousVisual();
            Destroy(enemy.gameObject);

            if (_activeEnemies.Count == 0)
            {
                _storyManager?.ResumeStory();
            }
        }
    }

    private void ShowTipForEnemy(string type)
    {
        switch (type)
        {
            case "Ghost":
                _enemyTipUI?.ShowTip("Зажмите уши (удерживайте Q), чтобы не сойти с ума.");
                break;
            case "Wolf":
                _enemyTipUI?.ShowTip("Отпугните волка свистом (удерживайте E).");
                break;
            case "Shade":
                _enemyTipUI?.ShowTip("Зажгите огонь (F), чтобы прогнать тень.");
                break;
            case "Demon":
                _enemyTipUI?.ShowTip("Отступите назад (S), вам не прогнать демона.");
                break;
            case "Gnome":
                _enemyTipUI?.ShowTip("Ударьте гнома (A), надоедлевого монстра.");
                break;
            default:
                break;
        }
    }
}

[System.Serializable]
public class EnemyEntry
{
    public string Type;
    public Enemy Prefab;
}
