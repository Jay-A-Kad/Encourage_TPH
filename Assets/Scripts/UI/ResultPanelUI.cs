using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ResultPanelUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI trialsText;
    public Button retryButton;
    public Button nextRoundButton;
    public StageManager stageManager;

    [Header("Win Text")]
    public string winTitle = "STAGE CLEAR!";
    public string winSubtitle = "EncourageMan is unstoppable!";

    [Header("Lose Text")]
    public string loseTitle = "CRUSHED...";
    public string loseSubtitle = "Try again! EncourageMan believes in you!";

    [Header("Next Round")]
    public string nextSceneName = "T2";

    [Header("Trials")]
    public int currentRound = 1;
    public int totalRounds = 2;


    private void Awake()
    {
        gameObject.SetActive(false);
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);
        if (nextRoundButton != null)
            nextRoundButton.onClick.AddListener(OnNextRoundClicked);
    }


    public void ShowWin(float wpm, float accuracy, float hype)
    {
        SetText(winTitle, winSubtitle);
        if (statsText != null)
            statsText.text = $"SPEED: {wpm:F0} WPM  |  ACCURACY: {accuracy:F0}%  |  HYPE: {hype:F0}/100";
        if (trialsText != null)
            trialsText.text = $"Trials Completed {currentRound}/{totalRounds}";
        if (retryButton != null) retryButton.gameObject.SetActive(false);
        if (nextRoundButton != null) nextRoundButton.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void ShowLose(float wpm, float accuracy, float hype)
    {
        SetText(loseTitle, loseSubtitle);
        if (statsText != null)
            statsText.text = $"SPEED: {wpm:F0} WPM  |  ACCURACY: {accuracy:F0}%  |  HYPE: {hype:F0}/100";
        if (retryButton != null) retryButton.gameObject.SetActive(true);
        if (nextRoundButton != null) nextRoundButton.gameObject.SetActive(false);
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

    private void OnNextRoundClicked()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
