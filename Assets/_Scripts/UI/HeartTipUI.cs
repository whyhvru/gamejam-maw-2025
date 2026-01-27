using UnityEngine;
using TMPro;

public class HeartTipUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tipText;

    private float _displayDuration = 3f;
    private float _timer;
    private bool _isVisible;

    private void Update()
    {
        if (_isVisible)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _tipText.enabled = false;
                _isVisible = false;
            }
        }
    }

    public void ShowTip()
    {
        _tipText.enabled = true;
        _timer = _displayDuration;
        _isVisible = true;
    }
}