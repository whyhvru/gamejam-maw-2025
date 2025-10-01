using System;
using UnityEngine;

public class DodgeManager : MonoBehaviour
{
    [SerializeField] private InputRouter _inputRouter;

    private void OnEnable()
    {
        _inputRouter.OnRetreat += OnRetreat;
    }

    private void OnDisable()
    {
        _inputRouter.OnRetreat -= OnRetreat;
    }

    private void OnRetreat()
    {
        OnDodge?.Invoke();
    }

    public Action OnDodge;
}