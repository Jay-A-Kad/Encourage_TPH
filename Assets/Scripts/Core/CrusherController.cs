using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls a single hydraulic crusher wall.
/// This component is fully reusable — assign one instance to the left crusher
/// (Side.Left) and an identical instance to the right crusher (Side.Right).
///
/// Movement logic:
///   Hype 0 %      → closing at baseCloseSpeed
///   Hype 75 %     → nearly stopped
///   Hype 76–100 % → pushed back toward winDistance
///   Word completed → brief burst that adds extra push-back
///
/// The crusher tracks a single float _distance (units from scene center) and
/// applies it as a signed X position so the same code works for both sides.
/// </summary>
public class CrusherController : MonoBehaviour
{
    public enum Side { Left, Right }

    [Header("Config")]
    public HypeConfig config;

    [Header("Setup")]
    public Side side;

    [Header("References")]
    public HypeMeterController hypeMeter;

    [Header("Events")]
    /// <summary>Fires once when the crusher crosses the deathDistance threshold.</summary>
    public UnityEvent onDeathZoneReached;

    public float DistanceFromCenter => _distance;

    private float   _distance;
    private bool    _active;
    private float   _burstTimer;
    private bool    _deathFired;
    private Vector3 _originWorldPos;  // world Y and Z are locked; only X changes

    // ── Lifecycle ────────────────────────────────────────────────────

    /// <summary>Reset crusher to starting position. Call before every game session.</summary>
    public void Initialize()
    {
        _originWorldPos = transform.position;   // capture Y and Z from editor placement
        _distance       = config.startDistance;
        _active         = false;
        _burstTimer     = 0f;
        _deathFired     = false;
        ApplyPosition();
    }

    public void SetActive(bool active) => _active = active;

    /// <summary>
    /// Trigger the short push-back burst awarded when the player completes a word.
    /// Wired via StageManager from TypingController.onWordCompleted.
    /// </summary>
    public void TriggerWordBurst()
    {
        _burstTimer = config.wordBurstDuration;
    }

    /// <summary>
    /// Instantly snap the crusher to the death position (used in the lose sequence).
    /// </summary>
    public void SlamShut()
    {
        _active   = false;
        _distance = config.deathDistance;
        ApplyPosition();
    }

    // ── Update ───────────────────────────────────────────────────────

    private void Update()
    {
        if (!_active) return;

        float hype  = hypeMeter.NormalizedHype;
        float speed = ComputeClosingSpeed(hype);  // positive = closing, negative = backing off

        if (_burstTimer > 0f)
        {
            _burstTimer -= Time.deltaTime;
            // Burst subtracts from closing speed (pushes crusher outward)
            speed -= config.maxPushBackSpeed * config.wordBurstSpeedMultiplier;
        }

        // _distance decreasing = crusher closing in
        _distance -= speed * Time.deltaTime;
        _distance  = Mathf.Clamp(_distance, config.deathDistance, config.winDistance);

        ApplyPosition();

        if (!_deathFired && _distance <= config.deathDistance + 0.05f)
        {
            _deathFired = true;
            onDeathZoneReached?.Invoke();
        }
    }

    // ── Private helpers ──────────────────────────────────────────────

    /// <summary>
    /// Maps normalized hype (0–1) to a closing speed.
    ///   0.00 – 0.75 → lerp from baseCloseSpeed down to 0  (still closing)
    ///   0.75 – 1.00 → lerp from 0 down to -maxPushBackSpeed  (backing off)
    /// </summary>
    private float ComputeClosingSpeed(float normalizedHype)
    {
        const float invincibleThreshold = 0.75f;

        if (normalizedHype < invincibleThreshold)
            return Mathf.Lerp(config.baseCloseSpeed, 0f, normalizedHype / invincibleThreshold);

        float t = (normalizedHype - invincibleThreshold) / (1f - invincibleThreshold);
        return Mathf.Lerp(0f, -config.maxPushBackSpeed, t);
    }

    private void ApplyPosition()
    {
        // Move along world X only — Y and Z stay locked to editor placement.
        // Using world position avoids the FBX rotation causing movement along Z.
        float signedX      = side == Side.Left ? -_distance : _distance;
        transform.position = new Vector3(signedX, _originWorldPos.y, _originWorldPos.z);
    }
}
