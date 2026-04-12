using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CrowdBakedAnimator : MonoBehaviour
{
    public Mesh[] frames;
    public float fps = 15f;
    public float speed = 1f;
    public bool loop = true;
    public bool randomStartFrame = true;

    [Header("Debug")]
    public bool debugMode = false;

    private MeshFilter _meshFilter;
    private float _startOffset;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();

      
        var smrs = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (smrs.Length > 0)
        {
            foreach (var smr in smrs) smr.enabled = false;
            
        }

        if (frames == null || frames.Length == 0)
        {
            
            return;
        }


        if (frames[0] != null)
        {
            int need = frames[0].subMeshCount;
            var mr = GetComponent<MeshRenderer>();
            if (mr != null && mr.sharedMaterials.Length != need)
                Debug.LogWarning($"[CrowdBakedAnimator] '{name}': MeshRenderer has {mr.sharedMaterials.Length} material(s) " +
                                 $"but mesh has {need} submesh(es). Assign {need} material(s) in the order logged by the baker.", this);
        }

        if (debugMode)
        {
        
            var unique = new HashSet<Mesh>();
            foreach (var f in frames) if (f != null) unique.Add(f);
            Debug.Log($"[CrowdBakedAnimator] '{name}': {frames.Length} frame slot(s), {unique.Count} unique mesh(es). " +
                      (unique.Count <= 1 ? "WARNING — all frames are the same mesh. Re-bake the animation." : "Frames look distinct, animation should play."), this);
        }

        if (randomStartFrame)
            _startOffset = Random.Range(0, frames.Length);
    }

    private void Update()
    {
        if (_meshFilter == null || frames == null || frames.Length == 0)
            return;

        float t = Time.time * fps * speed + _startOffset;
        int frameIndex = loop
            ? ((int)t % frames.Length)
            : Mathf.Min((int)t, frames.Length - 1);

        if (frames[frameIndex] == null) return;

        _meshFilter.sharedMesh = frames[frameIndex];

        if (debugMode && Time.frameCount % 30 == 0)
            Debug.Log($"[CrowdBakedAnimator] '{name}': playing frame {frameIndex}/{frames.Length} — {frames[frameIndex].name}", this);
    }
}
