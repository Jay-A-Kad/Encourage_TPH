using UnityEngine;

public class CrowdBobAnim : MonoBehaviour
{
    [Header("Bob")]
    public float bobHeight = 0.15f;
    [Tooltip("Bobs per second")]
    public float bobFrequency = 1.8f;

    [Header("Sway")]
    public bool enableSway = true;
    [Tooltip("How far sways")]
    public float swayAmount = 0.05f;
    [Tooltip("Sway cycles per second")]
    public float swayFrequency = 0.9f;

    [Header("Randomisation")]
    public bool randomisePhase = true;
    [Tooltip("Randomise speed")]
    [Range(0f, 0.4f)] public float speedVariance = 0.2f;

    private Vector3 _originLocalPos;
    private float _phase;
    private float _speedMultiplier;

    private void Start()
    {
        _originLocalPos = transform.localPosition;

        _phase = randomisePhase ? Random.Range(0f, Mathf.PI * 2f) : 0f;
        _speedMultiplier = 1f + Random.Range(-speedVariance, speedVariance);
    }

    private void Update()
    {
        float t = Time.time * _speedMultiplier;

        float bobOffset = Mathf.Sin(t * bobFrequency * Mathf.PI * 2f + _phase) * bobHeight;
        float swayOffset = enableSway
            ? Mathf.Sin(t * swayFrequency * Mathf.PI * 2f + _phase * 0.5f) * swayAmount
            : 0f;

        transform.localPosition = _originLocalPos + new Vector3(swayOffset, bobOffset, 0f);
    }
}
