using TMPro;
using UnityEngine;

public class QuitUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _quitBlock;

    private float _showTimer = 0f;
    private bool _isShown = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowQuitBlockForSeconds(2f);
        }

        if (_isShown)
        {
            _showTimer -= Time.deltaTime;
            if (_showTimer <= 0f)
            {
                _quitBlock.enabled = false;
                _isShown = false;
            }
        }
    }

    private void ShowQuitBlockForSeconds(float seconds)
    {
        _quitBlock.enabled = true;
        _showTimer = seconds;
        _isShown = true;
    }
}
