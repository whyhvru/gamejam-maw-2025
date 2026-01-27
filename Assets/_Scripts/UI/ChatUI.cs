using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChatUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _chatText;
    [SerializeField] private AudioClip _writingClip;
    [SerializeField] private AudioSource _audioSource;

    private readonly int _maxLines = 15;
    private readonly float _letterDelay = 0.02f;
    private readonly Queue<string> _chatQueue = new();
    private Coroutine _typingCoroutine;

    public void AddLine(string line)
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _typingCoroutine = StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(string line)
    {
        while (_chatQueue.Count >= _maxLines)
            _chatQueue.Dequeue();

        if (_writingClip != null && _audioSource != null)
            _audioSource.PlayOneShot(_writingClip, 1f);

        string visibleText = "";
        List<string> lines = new(_chatQueue) { "" };

        for (int i = 0; i < line.Length; i++)
        {
            visibleText += line[i];
            lines[^1] = visibleText;
            _chatText.text = string.Join("\n", lines);

            yield return new WaitForSeconds(_letterDelay);
        }

        _chatQueue.Enqueue(line);
        _chatText.text = string.Join("\n", _chatQueue);

        _typingCoroutine = null;
    }
}
