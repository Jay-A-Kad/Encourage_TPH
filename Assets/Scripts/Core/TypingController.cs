using UnityEngine;
using UnityEngine.Events;


public class TypingController : MonoBehaviour
{
    [Header("Config")]
    public WordList wordList;

    [Header("Events — consumed by HypeMeterController & CrusherControllers")]
    public UnityEvent onCorrectLetter;
    public UnityEvent onWordCompleted;
    public UnityEvent onWrongKey;

    [Header("Events — consumed by WordDisplayUI")]
    public UnityEvent<string> onNewWord;
    public UnityEvent<string, int> onTypingProgress;

    public string CurrentWord => _currentWord;
    public int LettersTyped => _lettersTyped;

    private string _currentWord = string.Empty;
    private int _lettersTyped;
    private bool _active;
    private float _elapsedTime;


    public void Initialize()
    {
        _active = false;
        _elapsedTime = 0f;
        _currentWord = string.Empty;
        _lettersTyped = 0;
    }

    public void SetActive(bool active)
    {
        _active = active;
        if (active) PickNewWord();
    }


    private void Update()
    {
        if (!_active) return;

        _elapsedTime += Time.deltaTime;


        foreach (char c in Input.inputString)
        {
            if (char.IsLetter(c))
                EvaluateInput(c);
        }
    }


    private void EvaluateInput(char key)
    {
        if (_currentWord.Length == 0) return;

        char expected = _currentWord[_lettersTyped];
        if (char.ToUpper(key) == char.ToUpper(expected))
        {
            _lettersTyped++;
            onCorrectLetter?.Invoke();
            onTypingProgress?.Invoke(_currentWord, _lettersTyped);

            if (_lettersTyped >= _currentWord.Length)
            {
                onWordCompleted?.Invoke();
                PickNewWord();
            }
        }
        else
        {
            onWrongKey?.Invoke();
        }
    }

    private void PickNewWord()
    {
        string[] pool = GetCurrentPool();
        string next = _currentWord;
        int tries = 0;


        while (next == _currentWord && tries < 10)
        {
            next = pool[Random.Range(0, pool.Length)];
            tries++;
        }

        _currentWord = next;
        _lettersTyped = 0;
        onNewWord?.Invoke(_currentWord);
        onTypingProgress?.Invoke(_currentWord, 0);
    }

    private string[] GetCurrentPool()
    {
        if (_elapsedTime >= wordList.hardWordUnlockTime && wordList.hardWords.Length > 0) return wordList.hardWords;
        if (_elapsedTime >= wordList.mediumWordUnlockTime && wordList.mediumWords.Length > 0) return wordList.mediumWords;
        return wordList.easyWords;
    }
}
