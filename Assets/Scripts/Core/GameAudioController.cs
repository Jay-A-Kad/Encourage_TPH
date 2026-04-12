using UnityEngine;


public class GameAudioController : MonoBehaviour
{
    [Header("Source")]
    public TypingController typingController;

    [Header("Correct Word Sounds (plays on every completed word)")]
    public AudioClip[] correctWordSounds;

    [Header("Wrong Key Sounds")]
    public AudioClip[] wrongKeySounds;
    [Tooltip("Minimum seconds between wrong-key sound triggers to avoid spam")]
    public float wrongKeyCooldown = 0.25f;

    [Header("Streak Sounds (plays when consecutive streak >= 2)")]
    public AudioClip[] streakSounds;

    [Header("Punch Sounds (T2 Boulder Trial)")]
    public AudioClip[] normalPunchSounds;
    public AudioClip[] hookPunchSounds;

    [Header("Crowd Heckler Sounds")]
    public AudioClip[] hecklerSounds;
    public int maxHecklerCount = 3;
    public float hecklerMinInterval = 8f;
    public float hecklerMaxInterval = 18f;


    private AudioSource _audioSource;
    private bool _hecklerActive;
    private int _hecklerCount;
    private float _hecklerTimer;
    private float _nextHecklerTime;
    private float _wrongKeyCooldownTimer;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        if (typingController != null)
        {
            typingController.onWordCompleted.AddListener(OnWordCompleted);
            typingController.onWrongKey.AddListener(OnWrongKey);
            typingController.onStreakChanged.AddListener(OnStreakChanged);
        }
    }

    private void OnDestroy()
    {
        if (typingController != null)
        {
            typingController.onWordCompleted.RemoveListener(OnWordCompleted);
            typingController.onWrongKey.RemoveListener(OnWrongKey);
            typingController.onStreakChanged.RemoveListener(OnStreakChanged);
        }
    }

    private void Update()
    {
        if (_wrongKeyCooldownTimer > 0f)
            _wrongKeyCooldownTimer -= Time.deltaTime;

        if (!_hecklerActive || _hecklerCount >= maxHecklerCount) return;

        _hecklerTimer += Time.deltaTime;
        if (_hecklerTimer >= _nextHecklerTime)
        {
            PlayRandom(hecklerSounds);
            _hecklerCount++;
            _hecklerTimer = 0f;
            _nextHecklerTime = Random.Range(hecklerMinInterval, hecklerMaxInterval);
        }
    }


    public void Initialize()
    {
        _hecklerActive = false;
        _hecklerCount = 0;
        _hecklerTimer = 0f;
        _wrongKeyCooldownTimer = 0f;
    }

    public void StartHeckler()
    {
        _hecklerActive = true;
        _hecklerCount = 0;
        _hecklerTimer = 0f;
        _nextHecklerTime = Random.Range(hecklerMinInterval, hecklerMaxInterval);
    }

    public void StopHeckler()
    {
        _hecklerActive = false;
    }

    public void PlayNormalPunch() => PlayRandom(normalPunchSounds);
    public void PlayHookPunch()   => PlayRandom(hookPunchSounds);


    private void OnWordCompleted()
    {
        PlayRandom(correctWordSounds);
    }

    private void OnWrongKey()
    {
        if (_wrongKeyCooldownTimer > 0f) return;
        PlayRandom(wrongKeySounds);
        _wrongKeyCooldownTimer = wrongKeyCooldown;
    }

    private void OnStreakChanged(int streak)
    {
        // Only play streak sound when an active streak is building 
        if (streak < 2) return;
        PlayRandom(streakSounds);
    }


    private void PlayRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip != null)
            _audioSource.PlayOneShot(clip);
    }
}
