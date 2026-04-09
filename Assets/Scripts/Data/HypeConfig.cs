using UnityEngine;


[CreateAssetMenu(fileName = "HypeConfig", menuName = "EncourageMan/Hype Config")]
public class HypeConfig : ScriptableObject
{
    [Header("Hype Meter")]
    [Range(0f, 100f)] public float startValue = 40f;
    public float maxValue = 100f;
    public float correctLetterGain = 2f;
    public float completedWordGain = 8f;
    public float wrongKeyPenalty = 4f;
    public float passiveDecayPerSecond = 5f;

    [Header("Hype Thresholds (0–100)")]
    [Range(0f, 100f)] public float weakThreshold = 25f;
    [Range(0f, 100f)] public float strugglingThreshold = 50f;
    [Range(0f, 100f)] public float strongThreshold = 75f;

    [Header("Crusher Movement")]
    [Tooltip("Closing speed  when hype is 0")]
    public float baseCloseSpeed = 1.5f;
    [Tooltip("Push-back speed at full invincible hype")]
    public float maxPushBackSpeed = 0.5f;
    [Tooltip("Starting distance of each crusher from scene center")]
    public float startDistance = 8f;
    [Tooltip("Crusher is pushed back to this distance on full hype ")]
    public float winDistance = 10f;
    [Tooltip("Crusher closer than this triggers death")]
    public float deathDistance = 1.5f;

    [Header("Word Completion Burst")]
    public float wordBurstDuration = 0.4f;
    [Tooltip("Speed multiplier applied during the burst window")]
    public float wordBurstSpeedMultiplier = 2.5f;

    [Header("Stage")]
    public float stageDuration = 30f;
}
