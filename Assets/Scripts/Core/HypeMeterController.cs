using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Tracks the Hype Meter value and broadcasts changes via UnityEvents.
/// Responsible only for hype value logic — no UI, no movement.
///
/// Wiring (done in StageManager.WireEvents):
///   TypingController.onCorrectLetter → OnCorrectLetter()
///   TypingController.onWordCompleted → OnWordCompleted()
///   TypingController.onWrongKey      → OnWrongKey()
///
/// Wiring (done in Inspector on UI components):
///   onNormalizedHypeChanged → HypeMeterUI.OnNormalizedHypeChanged(float)
///   onStateChanged          → HypeMeterUI.OnHypeStateChanged(HypeState)
/// </summary>
public class HypeMeterController : MonoBehaviour
{
    [Header("Config")]
    public HypeConfig config;

    [Header("Events — broadcast to listeners")]
    /// <summary>Fires whenever hype changes. Value is 0–1 (normalized). Ideal for UI sliders.</summary>
    public UnityEvent<float> onNormalizedHypeChanged;
    /// <summary>Fires when the hype tier (Weak/Struggling/Strong/Invincible) changes.</summary>
    public UnityEvent<HypeState> onStateChanged;
    /// <summary>Fires once when hype reaches exactly 0.</summary>
    public UnityEvent onHypeDepleted;

    public float CurrentHype    => _hype;
    public float NormalizedHype => _hype / config.maxValue;
    public HypeState CurrentState => _state;

    private float     _hype;
    private HypeState _state;
    private bool      _active;

    // ── Lifecycle ────────────────────────────────────────────────────

    /// <summary>Resets hype to the configured start value. Call before every game session.</summary>
    public void Initialize()
    {
        _hype   = config.startValue;
        _active = false;
        _state  = EvaluateState();
        onNormalizedHypeChanged?.Invoke(NormalizedHype);
        onStateChanged?.Invoke(_state);
    }

    /// <summary>Enable/disable passive decay and gain processing.</summary>
    public void SetActive(bool active) => _active = active;

    // ── Input callbacks (wired by StageManager) ──────────────────────

    public void OnCorrectLetter() => ModifyHype( config.correctLetterGain);
    public void OnWordCompleted() => ModifyHype( config.completedWordGain);
    public void OnWrongKey()      => ModifyHype(-config.wrongKeyPenalty);

    // ── Internal ─────────────────────────────────────────────────────

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
        if (_hype <= config.weakThreshold)       return HypeState.Weak;
        if (_hype <= config.strugglingThreshold) return HypeState.Struggling;
        if (_hype <= config.strongThreshold)     return HypeState.Strong;
        return HypeState.Invincible;
    }
}
