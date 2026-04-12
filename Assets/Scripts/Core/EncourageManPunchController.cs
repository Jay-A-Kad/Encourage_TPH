using UnityEngine;

public class EncourageManPunchController : MonoBehaviour
{
    [Header("References")]
    public TypingController typingController;
    public Animator animator;
    public BoulderController boulder;
    public GameAudioController gameAudio;

    [Header("Punch Origin")]
    public Transform punchOrigin;

    [Header("Animator Trigger Names")]
    public string normalPunchTrigger = "NormalPunch";
    public string hookPunchTrigger   = "HookPunch";

    [Header("Hook Punch Streak")]
    public int hookPunchStreakThreshold = 3;


    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (typingController != null)
            typingController.onStreakChanged.AddListener(OnStreakChanged);
    }

    private void OnDestroy()
    {
        if (typingController != null)
            typingController.onStreakChanged.RemoveListener(OnStreakChanged);
    }

    private bool _active;

    public void SetActive(bool active) => _active = active;

    private void OnStreakChanged(int streak)
    {
        // streak broken by wrong key
        if (!_active) return;
        if (streak == 0) return; 

        bool isHook = (streak % hookPunchStreakThreshold == 0);
        Punch(isHook);
    }


    private void Punch(bool hook)
    {
        string trigger = hook ? hookPunchTrigger : normalPunchTrigger;

        if (animator != null)
        {
            animator.ResetTrigger(normalPunchTrigger);
            animator.ResetTrigger(hookPunchTrigger);
            animator.SetTrigger(trigger);
        }

        if (boulder != null)
        {
            Vector3 origin = punchOrigin != null ? punchOrigin.position : transform.position;
            boulder.ReceivePunch(hook, origin);
        }

        if (gameAudio != null)
        {
            if (hook) gameAudio.PlayHookPunch();
            else      gameAudio.PlayNormalPunch();
        }
    }
}
