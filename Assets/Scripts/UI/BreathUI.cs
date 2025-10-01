using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BreathUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BreathManager _breathManager;
    [SerializeField] private Image _breathBar;
    [SerializeField] private Image _holdBar;

    [Header("Prompt")]
    [SerializeField] private TextMeshProUGUI _holdBreathPrompt;

    private bool _hasShownPrompt = false;
    private bool _promptActive = false;

    private void Update()
    {
        _breathBar.fillAmount = _breathManager.BreathLevel;
        _holdBar.fillAmount = _breathManager.HoldLevel;

        ShowHoldPromptIfNeeded();
    }

    private void ShowHoldPromptIfNeeded()
    {
        if (_hasShownPrompt)
            return;

        if (!_promptActive && _breathManager.BreathLevel >= 0.8f && !_breathManager.IsHolding)
        {
            _holdBreathPrompt.enabled = true;
            _promptActive = true;
        }

        if (_promptActive && _breathManager.IsHolding)
        {
            _holdBreathPrompt.enabled = false;
            _hasShownPrompt = true;
            _promptActive = false;
        }
    }
}
