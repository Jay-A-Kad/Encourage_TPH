using UnityEngine;


[CreateAssetMenu(fileName = "WordList", menuName = "EncourageMan/Word List")]
public class WordList : ScriptableObject
{
    [Header("Word Pools")]
    public string[] easyWords = { "GO", "PUSH", "HYPE", "WIN", "HOLD", "YES", "RISE" };
    public string[] mediumWords = { "POWER", "STRONG", "HERO", "BRAVE", "FIRE" };
    public string[] hardWords = { "BELIEVE", "COURAGE", "INVINCIBLE", "ENCOURAGE" };

    [Header("Phase Unlock Timing (seconds from game start)")]
    public float mediumWordUnlockTime = 12f;
    public float hardWordUnlockTime = 22f;
}
