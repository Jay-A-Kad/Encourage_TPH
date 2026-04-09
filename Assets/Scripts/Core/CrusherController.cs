using UnityEngine;
using UnityEngine.Events;


/*hype crusher logic
if 0 it will close at base speed
if 75% it will nearly stop
if 76> push back to win dist
if word correct it will push with burst

based on a distance it will apply force in one direction */
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
    public UnityEvent onDeathZoneReached;

    public float DistanceFromCenter => _distance;

    private float _distance;
    private bool _active;
    private float _burstTimer;
    private bool _deathFired;
    private Vector3 _originWorldPos;


    public void Initialize()
    {
        _originWorldPos = transform.position;
        _distance = config.startDistance;
        _active = false;
        _burstTimer = 0f;
        _deathFired = false;
        ApplyPosition();
    }

    public void SetActive(bool active) => _active = active;


    public void TriggerWordBurst()
    {
        _burstTimer = config.wordBurstDuration;
    }


    // Instantly snap the crusher to the death position

    public void SlamShut()
    {
        _active = false;
        _distance = config.deathDistance;
        ApplyPosition();
    }


    private void Update()
    {
        if (!_active) return;

        float hype = hypeMeter.NormalizedHype;
        float speed = ComputeClosingSpeed(hype);

        if (_burstTimer > 0f)
        {
            _burstTimer -= Time.deltaTime;
            // Burst subtracts from closing speed 
            speed -= config.maxPushBackSpeed * config.wordBurstSpeedMultiplier;
        }


        _distance -= speed * Time.deltaTime;
        _distance = Mathf.Clamp(_distance, config.deathDistance, config.winDistance);

        ApplyPosition();

        if (!_deathFired && _distance <= config.deathDistance + 0.05f)
        {
            _deathFired = true;
            onDeathZoneReached?.Invoke();
        }
    }


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
        float signedX = side == Side.Left ? -_distance : _distance;
        transform.position = new Vector3(signedX, _originWorldPos.y, _originWorldPos.z);
    }
}
