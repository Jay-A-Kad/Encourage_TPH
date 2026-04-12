using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class StageManager : MonoBehaviour
{

    public enum StageState { Rules, Countdown, Active, Win, Lose }


    [Header("Gameplay Controllers")]
    public HypeMeterController hypeMeter;
    public TypingController typingController;
    public CrusherController crusherLeft;
    public CrusherController crusherRight;
    public EncourageManController encourageMan;
    public EncourageManPunchController punchController; // T2 only
    public GameAudioController gameAudio;

    [Header("UI Controllers")]
    public RulesPanelUI rulesPanel;
    public CountdownUI countdownUI;
    public ResultPanelUI resultPanel;

    [Header("Gameplay HUD is hidden until Space is pressed")]
    public GameObject[] gameplayHUD;

    [Header("Config")]
    public HypeConfig config;

    [Header("Optional Stage Events")]
    public UnityEvent onGameStarted;
    public UnityEvent onPlayerWon;
    public UnityEvent onPlayerLost;


    public float TimeRemaining => Mathf.Max(0f, config.stageDuration - _activeTimer);
    public bool IsActive => _state == StageState.Active;

    private StageState _state;
    private float _activeTimer;
    private bool _losePending;


    private void Start()
    {
        WireEvents();
        ResetControllers();
        EnterRules();
    }

    private void Update()
    {
        switch (_state)
        {
            case StageState.Rules:
                // Press Space to dismiss the rules panel and begin countdown
                if (Input.GetKeyDown(KeyCode.Space))
                    StartCoroutine(RunCountdown());
                break;

            case StageState.Active:
                _activeTimer += Time.deltaTime;
                if (_activeTimer >= config.stageDuration)
                    TriggerWin();
                else if (hypeMeter.NormalizedHype >= config.hypeWinThreshold)
                    TriggerWin();
                break;
        }
    }


    public void RetryGame()
    {
        StopAllCoroutines();
        ResetControllers();
        resultPanel.Hide();
        EnterRules();
    }


    private void WireEvents()
    {
        // Typing - Hype meter
        typingController.onCorrectLetter.AddListener(hypeMeter.OnCorrectLetter);
        typingController.onWordCompleted.AddListener(hypeMeter.OnWordCompleted);
        typingController.onWrongKey.AddListener(hypeMeter.OnWrongKey);

        
        if (crusherLeft != null)  typingController.onWordCompleted.AddListener(crusherLeft.TriggerWordBurst);
        if (crusherRight != null) typingController.onWordCompleted.AddListener(crusherRight.TriggerWordBurst);
        typingController.onWordCompleted.AddListener(encourageMan.OnWordCompleted);

        // Hype state = encourageman visuals
        hypeMeter.onStateChanged.AddListener(encourageMan.OnHypeStateChanged);

        // Lose conditions
        hypeMeter.onHypeDepleted.AddListener(OnLoseConditionMet);
        if (crusherLeft != null)  crusherLeft.onDeathZoneReached.AddListener(OnLoseConditionMet);
        if (crusherRight != null) crusherRight.onDeathZoneReached.AddListener(OnLoseConditionMet);
    }


    private void ResetControllers()
    {
        hypeMeter.Initialize();
        typingController.Initialize();
        if (crusherLeft != null)  crusherLeft.Initialize();
        if (crusherRight != null) crusherRight.Initialize();
        encourageMan.Initialize();
        gameAudio?.Initialize();
        _activeTimer = 0f;
        _losePending = false;
    }


    private void EnterRules()
    {
        _state = StageState.Rules;
        rulesPanel.Show();
        SetHUDVisible(false);
    }

    private IEnumerator RunCountdown()
    {
        _state = StageState.Countdown;
        rulesPanel.Hide();

        yield return countdownUI.PlayCountdown();

        BeginActivePlay();
    }

    private void BeginActivePlay()
    {
        _state = StageState.Active;
        _activeTimer = 0f;
        _losePending = false;

        hypeMeter.SetActive(true);
        typingController.SetActive(true);
        if (crusherLeft != null)  crusherLeft.SetActive(true);
        if (crusherRight != null) crusherRight.SetActive(true);
        punchController?.SetActive(true);
        gameAudio?.StartHeckler();
        SetHUDVisible(true);

        onGameStarted?.Invoke();
    }

    private void TriggerWin()
    {
        if (_state != StageState.Active) return;
        _state = StageState.Win;

        hypeMeter.SetActive(false);
        typingController.SetActive(false);
        if (crusherLeft != null)  crusherLeft.SetActive(false);
        if (crusherRight != null) crusherRight.SetActive(false);
        punchController?.SetActive(false);

        gameAudio?.StopHeckler();
        encourageMan.Win();

        typingController.GetTypingStats(out float wpm, out float accuracy);
        float hype = hypeMeter.NormalizedHype * 100f;
        resultPanel.ShowWin(wpm, accuracy, hype);
        onPlayerWon?.Invoke();
    }

    private void TriggerLose()
    {
        if (_state != StageState.Active) return;
        _state = StageState.Lose;

        hypeMeter.SetActive(false);
        typingController.SetActive(false);
        if (crusherLeft != null)  crusherLeft.SlamShut();
        if (crusherRight != null) crusherRight.SlamShut();
        punchController?.SetActive(false);

        gameAudio?.StopHeckler();
        encourageMan.Die();
        onPlayerLost?.Invoke();
        StartCoroutine(ShowLoseResultDelayed());
    }

    private IEnumerator ShowLoseResultDelayed()
    {
        yield return new WaitForSeconds(1f);
        typingController.GetTypingStats(out float wpm, out float accuracy);
        float hype = hypeMeter.NormalizedHype * 100f;
        resultPanel.ShowLose(wpm, accuracy, hype);
    }

    private void OnLoseConditionMet()
    {
        if (_losePending || _state != StageState.Active) return;
        _losePending = true;
        TriggerLose();
    }

    private void SetHUDVisible(bool visible)
    {
        foreach (GameObject go in gameplayHUD)
            if (go != null) go.SetActive(visible);
    }
}
