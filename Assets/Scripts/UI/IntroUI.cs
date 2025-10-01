using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroSequence : MonoBehaviour
{
    [SerializeField] private TMP_Text _introText;
    [SerializeField] private string[] _lines;

    private readonly float _letterDelay = 0.1f;
    private readonly float _timeBeforeLoad = 2f;
    private readonly int _nextSceneIndex = 1;

    private void Start()
    {
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        _introText.text = "";
        for (int i = 0; i < _lines.Length; i++)
        {
            string line = _lines[i];
            foreach (var c in line)
            {
                _introText.text += c;
                yield return new WaitForSeconds(_letterDelay);
            }

            if (i < _lines.Length - 1)
            {
                yield return new WaitForSeconds(1f);
                _introText.text += "\n\n";
            }
        }

        yield return new WaitForSeconds(_timeBeforeLoad);
        SceneManager.LoadScene(_nextSceneIndex);
    }
}