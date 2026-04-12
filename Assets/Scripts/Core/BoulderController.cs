using System.Collections.Generic;
using UnityEngine;


public class BoulderController : MonoBehaviour
{
    [Header("Fracture Pieces")]
    public Rigidbody[] pieces;

    [Header("Contact Zone")]
    public float contactRadius = 1.5f;
    [Tooltip("Max pieces knocked")]
    public int piecesPerNormalPunch = 3;
    [Tooltip("Max pieces knocked per Hook Punch")]
    public int piecesPerHookPunch = 7;

    [Header("Force")]
    public float normalPunchVelocity = 4f;
    public float hookPunchVelocity   = 8f;
    [Tooltip("Upward explosion")]
    [Range(0f, 3f)] public float upwardModifier = 0.5f;
    private HashSet<Rigidbody> _loosePieces = new HashSet<Rigidbody>();

    private void Awake()
    {
        if (pieces == null || pieces.Length == 0)
            pieces = GetComponentsInChildren<Rigidbody>();

        foreach (var rb in pieces)
            if (rb != null) rb.isKinematic = true;
    }

    public void ReceivePunch(bool isHook, Vector3 punchOrigin)
    {
        int   maxPieces = isHook ? piecesPerHookPunch   : piecesPerNormalPunch;
        float velocity  = isHook ? hookPunchVelocity    : normalPunchVelocity;

        var candidates = new List<(float dist, Rigidbody rb)>();
        foreach (var rb in pieces)
        {
            if (rb == null || _loosePieces.Contains(rb)) continue;
            float dist = Vector3.Distance(rb.position, punchOrigin);
            if (dist <= contactRadius)
                candidates.Add((dist, rb));
        }

        candidates.Sort((a, b) => a.dist.CompareTo(b.dist));

        int count = Mathf.Min(maxPieces, candidates.Count);
        for (int i = 0; i < count; i++)
        {
            Rigidbody rb = candidates[i].rb;
            _loosePieces.Add(rb);
            rb.isKinematic = false;
            rb.AddExplosionForce(velocity, punchOrigin, contactRadius, upwardModifier, ForceMode.VelocityChange);
        }
    }

    public void ResetBoulder()
    {
        _loosePieces.Clear();
        foreach (var rb in pieces)
        {
            if (rb == null) continue;
            rb.isKinematic = true;
            rb.linearVelocity  = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
