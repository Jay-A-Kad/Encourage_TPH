using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages word selection and evaluates keyboard input letter-by-letter.
/// Reads from WordList SO to determine which pool to use based on elapsed time.
/// Uses legacy Input.inputString so it works regardless of Input System mode.
///
/// Events fired (StageManager wires these to HypeMeterController and CrusherControllers):
///   onCorrectLetter  — a letter was typed correctly
///   onWordCompleted  — the full word was finished
///   onWrongKey       — an incorrect key was pressed
///
/// Events fired (WordDisplayUI subscribes directly):
///   onNewWord        — a new target word was chosen (string)
///   onTypingProgress — typed letter count changed (string word, int typedCount)
/// </summary>
public class TypingController : MonoBehaviour
{
    [Header("Config")]
    public WordList wordList;

    [Header("Events — consumed by HypeMeterController & CrusherControllers")]
    public UnityEvent onCorrectLetter;
    public UnityEvent onWordCompleted;
    public UnityEvent onWrongKey;

    [Header("Events — consumed by WordDisplayUI")]
    public UnityEvent<string>    onNewWord;
    public UnityEvent<string, int> onTypingProgress;

    public string CurrentWord  => _currentWord;
    public int    LettersTyped => _lettersTyped;

    private string _currentWord  = string.Empty;
    private int    _lettersTyped;
    private bool   _active;
    private float  _elapsedTime;

    // ── Lifecycle ────────────────────────────────────────────────────

    public void Initialize()
    {
        _active      = false;
        _elapsedTime = 0f;
        _currentWord = string.Empty;
        _lettersTyped = 0;
    }

    public void SetActive(bool active)
    {
        _active = active;
        if (active) PickNewWord();
    }

    // ── Update ───────────────────────────────────────────────────────

    private void Update()
    {
        if (!_active) return;

        _elapsedTime += Time.deltaTime;

        // Input.inputString returns characters typed this frame (legacy input, always available)
        foreach (char c in Input.inputString)
        {
            if (char.IsLetter(c))
                EvaluateInput(c);
        }
    }

    // ── Private helpers ──────────────────────────────────────────────

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
        string next   = _currentWord;
        int     tries = 0;

        // Avoid repeating the same word when the pool is large enough
        while (next == _currentWord && tries < 10)
        {
            next = pool[Random.Range(0, pool.Length)];
            tries++;
        }

        _currentWord  = next;
        _lettersTyped = 0;
        onNewWord?.Invoke(_currentWord);
        onTypingProgress?.Invoke(_currentWord, 0);
    }

    private string[] GetCurrentPool()
    {
        if (_elapsedTime >= wordList.hardWordUnlockTime  && wordList.hardWords.Length   > 0) return wordList.hardWords;
        if (_elapsedTime >= wordList.mediumWordUnlockTime && wordList.mediumWords.Length > 0) return wordList.mediumWords;
        return wordList.easyWords;
    }
}
