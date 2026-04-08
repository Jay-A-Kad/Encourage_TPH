using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls EncourageMan's animation state machine and ragdoll death sequence.
///
/// Animator parameters expected (configure in man.controller to match):
///   int  "HypeState"       — 0=Weak, 1=Struggling, 2=Strong, 3=Invincible
///   bool "IsDead"          — true activates death blend
///   trigger "WordCompleted" — plays a flex/shout animation
///   trigger "Win"           — plays heroic end pose
///
/// Ragdoll setup:
///   Assign every Rigidbody in the ragdoll hierarchy to ragdollBodies[].
///   They should be kinematic at rest; this script enables them on death.
/// </summary>
[RequireComponent(typeof(Animator))]
public class EncourageManController : MonoBehaviour
{
    [Header("Ragdoll — assign all child Rigidbodies")]
    public Rigidbody[] ragdollBodies;

    [Header("Animator Parameter Names")]
    public string hypeStateParam      = "HypeState";
    public string isDeadParam         = "IsDead";
    public string wordCompletedTrigger = "WordCompleted";
    public string winTrigger          = "Win";

    [Header("Events")]
    public UnityEvent onDied;
    public UnityEvent onWon;

    private Animator _animator;
    private bool     _isDead;

    // ── Lifecycle ────────────────────────────────────────────────────

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        SetRagdollActive(false);
    }

    /// <summary>Reset to idle/ready state. Call before every game session.</summary>
    public void Initialize()
    {
        _isDead = false;
        SetRagdollActive(false);
        _animator.enabled = true;
        _animator.SetInteger(hypeStateParam, (int)HypeState.Struggling);
        _animator.SetBool(isDeadParam, false);
    }

    // ── State callbacks (wired by StageManager) ──────────────────────

    /// <summary>Wired to HypeMeterController.onStateChanged.</summary>
    public void OnHypeStateChanged(HypeState state)
    {
        if (_isDead) return;
        _animator.SetInteger(hypeStateParam, (int)state);
    }

    /// <summary>Wired to TypingController.onWordCompleted.</summary>
    public void OnWordCompleted()
    {
        if (_isDead) return;
        _animator.SetTrigger(wordCompletedTrigger);
    }

    // ── End states ───────────────────────────────────────────────────

    /// <summary>Switch to ragdoll — called by StageManager on lose.</summary>
    public void Die()
    {
        if (_isDead) return;
        _isDead = true;
        _animator.SetBool(isDeadParam, true);
        _animator.enabled = false;
        SetRagdollActive(true);
        onDied?.Invoke();
    }

    /// <summary>Trigger heroic win animation — called by StageManager on win.</summary>
    public void Win()
    {
        if (_isDead) return;
        _animator.SetInteger(hypeStateParam, (int)HypeState.Invincible);
        _animator.SetTrigger(winTrigger);
        onWon?.Invoke();
    }

    // ── Private helpers ──────────────────────────────────────────────

    private void SetRagdollActive(bool active)
    {
        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !active;
            if (rb.TryGetComponent<Collider>(out var col))
                col.enabled = active;
        }
    }
}
