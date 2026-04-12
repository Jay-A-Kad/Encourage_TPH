using UnityEngine;


[CreateAssetMenu(fileName = "WordList", menuName = "EncourageMan/Word List")]
public class WordList : ScriptableObject
{
    [Header("Word Pools")]
    public string[] easyWords = { "YOU GOT THIS", "PUSH HARDER", "RISE UP NOW", "GO FOR IT", "STAY STRONG", "DO NOT STOP", "KEEP GOING" };
    public string[] mediumWords = { "YOU CAN DO THIS", "NEVER GIVE UP NOW", "KEEP PUSHING FORWARD", "UNSTOPPABLE", "DETERMINATION", "PERSEVERANCE", "OUTSTANDING" };
    public string[] hardWords = {
        "BELIEVE IN YOUR POWER",
        "YOU ARE TRULY INVINCIBLE",
        "GIVE IT EVERYTHING NOW",
        "NOTHING CAN STOP YOU NOW",
        "PUSH THROUGH THE PAIN TODAY",
        "YOUR STRENGTH DEFINES YOU NOW",
        "RISE ABOVE EVERY CHALLENGE",
        "EXTRAORDINARILY",
        "WHOLEHEARTEDNESS",
        "UNCONQUERABLE",
        "INDESTRUCTIBLE",
        "UNBREAKABLE",
        "INSURMOUNTABLE",
        "PERSEVERINGLY",
        "OVERCOMEABILITY",
        "UNSTOPPABLY"
    };

    [Header("Phase Unlock Timing (seconds from game start)")]
    public float mediumWordUnlockTime = 12f;
    public float hardWordUnlockTime = 22f;
}
