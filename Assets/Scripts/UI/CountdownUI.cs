using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays the 3 – 2 – 1 – HYPE! countdown sequence.
/// StageManager yields on PlayCountdown() — the coroutine completes when the
/// sequence finishes so the manager knows exactly when to begin active play.
/// </summary>
public class CountdownUI : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI countdownText;

    [Header("Timing")]
    [Tooltip("Duration each digit is shown (seconds)")]
    public float digitDuration = 0.8f;
    [Tooltip("Duration the HYPE! text is shown (seconds)")]
    public float hypeDuration  = 0.8f;

    [Header("Scale Animation")]
    public float startScale = 2.0f;
    public float endScale   = 1.0f;

    // ── Public API ───────────────────────────────────────────────────

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    /// <summary>
    /// Runs the full countdown. Yield on this in StageManager.
    /// Example:  yield return countdownUI.PlayCountdown();
    /// </summary>
    public IEnumerator PlayCountdown()
    {
        Show();
        yield return ShowStep("3",     digitDuration);
        yield return ShowStep("2",     digitDuration);
        yield return ShowStep("1",     digitDuration);
        yield return ShowStep("HYPE!", hypeDuration);
        Hide();
    }

    // ── Private ──────────────────────────────────────────────────────

    private IEnumerator ShowStep(string text, float duration)
    {
        if (countdownText != null)
        {
            countdownText.text = text;
            countdownText.transform.localScale = Vector3.one * startScale;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            if (countdownText != null)
                countdownText.transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);
            yield return null;
        }
    }
}
