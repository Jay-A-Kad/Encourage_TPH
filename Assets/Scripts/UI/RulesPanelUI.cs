using UnityEngine;

/// <summary>
/// Shows and hides the full-screen rules panel.
/// Attach this script to the Rules Panel root GameObject.
/// StageManager calls Show() on Start and Hide() when the player presses Space.
/// </summary>
public class RulesPanelUI : MonoBehaviour
{
    [Tooltip("Optional CanvasGroup for alpha fading. If null the panel is simply enabled/disabled.")]
    public CanvasGroup canvasGroup;

    public void Show()
    {
        gameObject.SetActive(true);
        if (canvasGroup != null)
        {
            canvasGroup.alpha          = 1f;
            canvasGroup.interactable   = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void Hide()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha          = 0f;
            canvasGroup.interactable   = false;
            canvasGroup.blocksRaycasts = false;
        }
        gameObject.SetActive(false);
    }
}
