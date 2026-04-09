using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ResultPanelUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public Button retryButton;
    public StageManager stageManager;

    [Header("Win Text")]
    public string winTitle = "STAGE CLEAR!";
    public string winSubtitle = "EncourageMan is unstoppable!";

    [Header("Lose Text")]
    public string loseTitle = "CRUSHED...";
    public string loseSubtitle = "Try again! EncourageMan believes in you!";


    private void Awake()
    {
        gameObject.SetActive(false);
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);
    }


    public void ShowWin()
    {
        SetText(winTitle, winSubtitle);
        gameObject.SetActive(true);
    }

    public void ShowLose()
    {
        SetText(loseTitle, loseSubtitle);
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);


    private void SetText(string title, string subtitle)
    {
        if (titleText != null) titleText.text = title;
        if (subtitleText != null) subtitleText.text = subtitle;
    }

    private void OnRetryClicked()
    {
        if (stageManager != null)
            stageManager.RetryGame();
    }
}
