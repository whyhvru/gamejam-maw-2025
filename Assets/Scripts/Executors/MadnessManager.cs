using System.Collections.Generic;
using UnityEngine;

public class MadnessManager : MonoBehaviour
{
    [Range(0f, 100f)]
    [SerializeField] private float _currentMadness = 0f;

    private Dictionary<string, float> _factorDictionary;
    public float CurrentMadness => _currentMadness;

    private void Awake()
    {
        _factorDictionary = new Dictionary<string, float>
        {
            { DeathFactors.enemyFactorName, DeathFactors.enemyFactor},
            { DeathFactors.breathFactorName, DeathFactors.breathFactor},
            { DeathFactors.hyperFactorName, DeathFactors.hyperFactor},
            { DeathFactors.stumbleFactorName, DeathFactors.stumbleFactor},
            { DeathFactors.longwalkFactorName, DeathFactors.longwalkFactor},
            { DeathFactors.heartFactorName, DeathFactors.heartFactor},
        };
    }

    public float IncreaseMadness(string factor)
    {
        if (_factorDictionary.TryGetValue(factor, out float value))
        {
            _currentMadness = Mathf.Min(_currentMadness + value, 100f);
        }
        else
        {
            _currentMadness = Mathf.Min(_currentMadness + 10f, 100f);
        }

        return _currentMadness;
    }

    public void ResetMadness()
    {
        _currentMadness = 0f;
    }
}