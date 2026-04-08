using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays the current target word with per-letter color feedback.
/// Self-wires to TypingController in Awake — no external wiring needed.
///
/// - Typed letters turn typedColor (green)
/// - Pending letters remain pendingColor (white)
/// - A wrong key triggers a brief red flash on the whole word
/// </summary>
public class WordDisplayUI : MonoBehaviour
{
    [Header("Source")]
    public TypingController typingController;

    [Header("UI References")]
    public TextMeshProUGUI wordText;

    [Header("Colors")]
    public Color typedColor   = Color.green;
    public Color pendingColor = Color.white;
    public Color wrongFlash   = Color.red;

    [Header("Wrong Key Flash")]
    public float flashDuration = 0.15f;

    private string _word        = string.Empty;
    private int    _typedCount;
    private float  _flashTimer;

    // ── Lifecycle ────────────────────────────────────────────────────

    private void Awake()
    {
        if (typingController == null) return;
        typingController.onNewWord       .AddListener(OnNewWord);
        typingController.onTypingProgress.AddListener(OnTypingProgress);
        typingController.onWrongKey      .AddListener(OnWrongKey);
    }

    private void OnDestroy()
    {
        if (typingController == null) return;
        typingController.onNewWord       .RemoveListener(OnNewWord);
        typingController.onTypingProgress.RemoveListener(OnTypingProgress);
        typingController.onWrongKey      .RemoveListener(OnWrongKey);
    }

    // ── Update ───────────────────────────────────────────────────────

    private void Update()
    {
        if (_flashTimer <= 0f) return;

        _flashTimer -= Time.deltaTime;
        if (_flashTimer <= 0f)
            RenderWord(_word, _typedCount);   // restore normal colors after flash
    }

    // ── Callbacks ────────────────────────────────────────────────────

    private void OnNewWord(string word)
    {
        _word       = word;
        _typedCount = 0;
        RenderWord(word, 0);
    }

    private void OnTypingProgress(string word, int typedCount)
    {
        _word       = word;
        _typedCount = typedCount;
        if (_flashTimer <= 0f)          // don't overwrite an active flash
            RenderWord(word, typedCount);
    }

    private void OnWrongKey()
    {
        _flashTimer = flashDuration;
        if (wordText != null)
            wordText.color = wrongFlash;
    }

    // ── Rendering ────────────────────────────────────────────────────

    private void RenderWord(string word, int typedCount)
    {
        if (wordText == null || word.Length == 0) return;

        wordText.color = Color.white;   // reset base color (rich text overrides per char)

        var sb = new StringBuilder(word.Length * 30);
        for (int i = 0; i < word.Length; i++)
        {
            Color  c   = i < typedCount ? typedColor : pendingColor;
            string hex = ColorUtility.ToHtmlStringRGB(c);
            sb.Append($"<color=#{hex}>{word[i]}</color>");
        }
        wordText.text = sb.ToString();
    }
}
