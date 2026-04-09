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

    private Vector3[] _ragdollLocalPositions;
    private Quaternion[] _ragdollLocalRotations;


    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _ragdollLocalPositions = new Vector3[ragdollBodies.Length];
        _ragdollLocalRotations = new Quaternion[ragdollBodies.Length];
        for (int i = 0; i < ragdollBodies.Length; i++)
        {
            _ragdollLocalPositions[i] = ragdollBodies[i].transform.localPosition;
            _ragdollLocalRotations[i] = ragdollBodies[i].transform.localRotation;
        }

        SetRagdollActive(false);
    }

    public void Initialize()
    {
        _isDead = false;
        SetRagdollActive(false);

        for (int i = 0; i < ragdollBodies.Length; i++)
        {
            ragdollBodies[i].transform.localPosition = _ragdollLocalPositions[i];
            ragdollBodies[i].transform.localRotation = _ragdollLocalRotations[i];
        }

        _animator.enabled = true;
        _animator.Rebind();
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
            rb.collisionDetectionMode = active
                ? CollisionDetectionMode.Continuous
                : CollisionDetectionMode.Discrete;
            if (rb.TryGetComponent<Collider>(out var col))
                col.enabled = active;
        }
    }
}
