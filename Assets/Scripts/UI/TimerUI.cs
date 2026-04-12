using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [Header("References")]
    public StageManager stageManager;
    public TextMeshProUGUI timerText;

    [Header("Format")]
    public string prefix = "TIME: ";
    public bool showDecimal = false;

    private void Update()
    {
        if (stageManager == null || timerText == null) return;

        if (!stageManager.IsActive)
            return;

        float t = stageManager.TimeRemaining;
        timerText.text = showDecimal
            ? $"{prefix}{t:F1}s"
            : $"{prefix}{Mathf.CeilToInt(t)}s";
    }
}
