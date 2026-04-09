using TMPro;
using UnityEngine;


public class StreakUI : MonoBehaviour
{
    [Header("References")]
    public TypingController typingController;
    public TextMeshProUGUI streakText;


    private void Awake()
    {
        if (typingController != null)
            typingController.onStreakChanged.AddListener(OnStreakChanged);

        SetVisible(false);
    }

    private void OnDestroy()
    {
        if (typingController != null)
            typingController.onStreakChanged.RemoveListener(OnStreakChanged);
    }


    private void OnStreakChanged(int streak)
    {

        int multiplier = streak - 1;

        if (multiplier <= 0)
        {
            SetVisible(false);
            return;
        }

        if (streakText != null)
            streakText.text = $"{multiplier}x";

        SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
        if (streakText != null)
            streakText.gameObject.SetActive(visible);
    }
}
