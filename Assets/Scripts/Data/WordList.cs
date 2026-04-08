using UnityEngine;

/// <summary>
/// ScriptableObject that holds the word pools for each difficulty phase.
/// Create via: Assets > Create > EncourageMan > Word List
/// </summary>
[CreateAssetMenu(fileName = "WordList", menuName = "EncourageMan/Word List")]
public class WordList : ScriptableObject
{
    [Header("Word Pools")]
    public string[] easyWords   = { "GO", "PUSH", "HYPE", "WIN", "HOLD", "YES", "RISE" };
    public string[] mediumWords = { "POWER", "STRONG", "HERO", "BRAVE", "FIRE" };
    public string[] hardWords   = { "BELIEVE", "COURAGE", "INVINCIBLE", "ENCOURAGE" };

    [Header("Phase Unlock Timing (seconds from game start)")]
    [Tooltip("Seconds elapsed before medium words begin appearing")]
    public float mediumWordUnlockTime = 12f;
    [Tooltip("Seconds elapsed before hard words begin appearing")]
    public float hardWordUnlockTime = 22f;
}
