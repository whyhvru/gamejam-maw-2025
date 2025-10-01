using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlintUI : MonoBehaviour
{
    [SerializeField] private FlintManager _flintManager;
    [SerializeField] private Image _fillBar;

    private readonly float _fillSpeed = 4f;
    private Coroutine _animCoroutine;

    private void OnEnable()
    {
        _flintManager.OnFlintUsed += HandleFlintUsed;
        _flintManager.OnFlintExpired += HandleFlintExpired;
    }

    private void OnDisable()
    {
        _flintManager.OnFlintUsed -= HandleFlintUsed;
        _flintManager.OnFlintExpired -= HandleFlintExpired;
    }

    private void HandleFlintUsed()
    {
        if (_animCoroutine != null) StopCoroutine(_animCoroutine);
        _fillBar.fillOrigin = (int)Image.Origin360.Top;
        _animCoroutine = StartCoroutine(AnimateFill(1f));
    }

    private void HandleFlintExpired()
    {
        if (_animCoroutine != null) StopCoroutine(_animCoroutine);
        _fillBar.fillOrigin = (int)Image.Origin360.Bottom;
        _animCoroutine = StartCoroutine(AnimateFill(0f));
    }

    private IEnumerator AnimateFill(float target)
    {
        while (!Mathf.Approximately(_fillBar.fillAmount, target))
        {
            _fillBar.fillAmount = Mathf.MoveTowards(_fillBar.fillAmount, target, Time.deltaTime * _fillSpeed);
            yield return null;
        }
    }
}