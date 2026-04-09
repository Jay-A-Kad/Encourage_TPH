using UnityEngine;
using UnityEngine.Events;


public class HypeMeterController : MonoBehaviour
{
    [Header("Config")]
    public HypeConfig config;

    [Header("Events — broadcast to listeners")]

    public UnityEvent<float> onNormalizedHypeChanged;

    public UnityEvent<HypeState> onStateChanged;

    public UnityEvent onHypeDepleted;

    public float CurrentHype => _hype;
    public float NormalizedHype => _hype / config.maxValue;
    public HypeState CurrentState => _state;

    private float _hype;
    private HypeState _state;
    private bool _active;

    public void Initialize()
    {
        _hype = config.startValue;
        _active = false;
        _state = EvaluateState();
        onNormalizedHypeChanged?.Invoke(NormalizedHype);
        onStateChanged?.Invoke(_state);
    }


    public void SetActive(bool active) => _active = active;



    public void OnCorrectLetter() => ModifyHype(config.correctLetterGain);
    public void OnWordCompleted() => ModifyHype(config.completedWordGain);
    public void OnWrongKey() => ModifyHype(-config.wrongKeyPenalty);



    private void Update()
    {
        if (!_active) return;
        ModifyHype(-config.passiveDecayPerSecond * Time.deltaTime);
    }

    private void ModifyHype(float delta)
    {
        if (!_active) return;

        float before = _hype;
        _hype = Mathf.Clamp(_hype + delta, 0f, config.maxValue);

        if (Mathf.Approximately(_hype, before)) return;

        onNormalizedHypeChanged?.Invoke(NormalizedHype);
        CheckStateChange();

        if (_hype <= 0f)
            onHypeDepleted?.Invoke();
    }

    private void CheckStateChange()
    {
        HypeState newState = EvaluateState();
        if (newState == _state) return;
        _state = newState;
        onStateChanged?.Invoke(_state);
    }

    private HypeState EvaluateState()
    {
        if (_hype <= config.weakThreshold) return HypeState.Weak;
        if (_hype <= config.strugglingThreshold) return HypeState.Struggling;
        if (_hype <= config.strongThreshold) return HypeState.Strong;
        return HypeState.Invincible;
    }
}
