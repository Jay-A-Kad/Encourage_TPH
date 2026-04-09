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

        // Word completion - crusher bursts + encourageman flex
        typingController.onWordCompleted.AddListener(crusherLeft.TriggerWordBurst);
        typingController.onWordCompleted.AddListener(crusherRight.TriggerWordBurst);
        typingController.onWordCompleted.AddListener(encourageMan.OnWordCompleted);

        // Hype state = encourageman visuals
        hypeMeter.onStateChanged.AddListener(encourageMan.OnHypeStateChanged);

        // Lose conditions
        hypeMeter.onHypeDepleted.AddListener(OnLoseConditionMet);
        crusherLeft.onDeathZoneReached.AddListener(OnLoseConditionMet);
        crusherRight.onDeathZoneReached.AddListener(OnLoseConditionMet);
    }


    private void ResetControllers()
    {
        hypeMeter.Initialize();
        typingController.Initialize();
        crusherLeft.Initialize();
        crusherRight.Initialize();
        encourageMan.Initialize();
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
        crusherLeft.SetActive(true);
        crusherRight.SetActive(true);
        SetHUDVisible(true);

        onGameStarted?.Invoke();
    }

    private void TriggerWin()
    {
        if (_state != StageState.Active) return;
        _state = StageState.Win;

        hypeMeter.SetActive(false);
        typingController.SetActive(false);
        crusherLeft.SetActive(false);
        crusherRight.SetActive(false);

        encourageMan.Win();
        resultPanel.ShowWin();
        onPlayerWon?.Invoke();
    }

    private void TriggerLose()
    {
        if (_state != StageState.Active) return;
        _state = StageState.Lose;

        hypeMeter.SetActive(false);
        typingController.SetActive(false);
        crusherLeft.SlamShut();
        crusherRight.SlamShut();

        encourageMan.Die();
        resultPanel.ShowLose();
        onPlayerLost?.Invoke();
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
