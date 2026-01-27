using UnityEngine;
using TMPro;

public class EnemyTipUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tipText;
    [SerializeField] private float _fadeOutDelay = 5f;

    private float _timer;
    private bool _isShowing;

    private void Update()
    {
        if (_isShowing)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _tipText.enabled = false;
                _isShowing = false;
            }
        }
    }

    public void ShowTip(string text)
    {
        _tipText.text = text;
        _tipText.enabled = true;
        _timer = _fadeOutDelay;
        _isShowing = true;
    }
}