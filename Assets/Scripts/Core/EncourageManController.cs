using UnityEngine;
using UnityEngine.Events;


//TODO: add animation once done with game 

[RequireComponent(typeof(Animator))]
public class EncourageManController : MonoBehaviour
{
    [Header("child Rigidbodies")]
    public Rigidbody[] ragdollBodies;

    [Header("Animator Parameter Names")]
    public string hypeStateParam = "HypeState";
    public string isDeadParam = "IsDead";
    public string wordCompletedTrigger = "WordCompleted";
    public string winTrigger = "Win";

    [Header("Events")]
    public UnityEvent onDied;
    public UnityEvent onWon;

    private Animator _animator;
    private bool _isDead;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        SetRagdollActive(false);
    }

    public void Initialize()
    {
        _isDead = false;
        SetRagdollActive(false);
        _animator.enabled = true;
        _animator.SetInteger(hypeStateParam, (int)HypeState.Struggling);
        _animator.SetBool(isDeadParam, false);
    }


    public void OnHypeStateChanged(HypeState state)
    {
        if (_isDead) return;
        _animator.SetInteger(hypeStateParam, (int)state);
    }


    public void OnWordCompleted()
    {
        if (_isDead) return;
        _animator.SetTrigger(wordCompletedTrigger);
    }


    public void Die()
    {
        if (_isDead) return;
        _isDead = true;
        _animator.SetBool(isDeadParam, true);
        _animator.enabled = false;
        SetRagdollActive(true);
        onDied?.Invoke();
    }


    public void Win()
    {
        if (_isDead) return;
        _animator.SetInteger(hypeStateParam, (int)HypeState.Invincible);
        _animator.SetTrigger(winTrigger);
        onWon?.Invoke();
    }



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
