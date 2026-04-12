#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class CrowdAnimationBaker : EditorWindow
{
    public GameObject sourcePrefab;
    public AnimationClip clip;
    public int sampleRate = 15;
    public string outputFolder = "Assets/BakedCrowd";

    [MenuItem("Tools/Crowd Animation Baker")]
    public static void ShowWindow()
    {
        GetWindow<CrowdAnimationBaker>("Crowd Baker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Bake Skinned Animation to Mesh Frames", EditorStyles.boldLabel);

        sourcePrefab = (GameObject)EditorGUILayout.ObjectField("Source Prefab", sourcePrefab, typeof(GameObject), false);
        clip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", clip, typeof(AnimationClip), false);
        sampleRate = EditorGUILayout.IntField("Sample Rate", sampleRate);
        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

        if (GUILayout.Button("Bake"))
        {
            Bake();
        }
    }

    private void Bake()
    {
        if (sourcePrefab == null || clip == null)
        {
            Debug.LogError("Assign both Source Prefab and Animation Clip.");
            return;
        }

        // Instantiate into the scene so AnimationMode can drive the bones
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(sourcePrefab);
        if (instance == null) instance = Instantiate(sourcePrefab);
        instance.name = sourcePrefab.name + "_BAKE_TEMP";

        SkinnedMeshRenderer[] smrs = instance.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (smrs.Length == 0)
        {
            Debug.LogError("No SkinnedMeshRenderers found in prefab.");
            DestroyImmediate(instance);
            return;
        }

        // Print material order — assign these to the MeshRenderer in this exact order
        Debug.Log($"[CrowdBaker] Found {smrs.Length} SMR(s). Assign materials to MeshRenderer in this order:");
        int matSlot = 0;
        for (int s = 0; s < smrs.Length; s++)
            foreach (var mat in smrs[s].sharedMaterials)
                Debug.Log($"  Slot {matSlot++}: '{smrs[s].name}' → {mat?.name}");

        EnsureFolder(outputFolder);

        int frameCount = Mathf.CeilToInt(clip.length * sampleRate);
        float dt = 1f / sampleRate;

  
        AnimationMode.StartAnimationMode();

        for (int i = 0; i < frameCount; i++)
        {
            float t = Mathf.Min(i * dt, clip.length);
            AnimationMode.SampleAnimationClip(instance, clip, t);

            CombineInstance[] combine = new CombineInstance[smrs.Length];
            for (int s = 0; s < smrs.Length; s++)
            {
                Mesh part = new Mesh();
                smrs[s].BakeMesh(part);
                combine[s].mesh = part;
                combine[s].transform = instance.transform.worldToLocalMatrix
                                       * smrs[s].transform.localToWorldMatrix;
            }

            Mesh combined = new Mesh();
            combined.name = $"{clip.name}_frame_{i:D4}";
            combined.CombineMeshes(combine, false, true); // false = keep one submesh per SMR

            foreach (var ci in combine)
                DestroyImmediate(ci.mesh);

            AssetDatabase.CreateAsset(combined, $"{outputFolder}/{combined.name}.asset");
        }

        AnimationMode.StopAnimationMode();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        DestroyImmediate(instance);

        Debug.Log($"[CrowdBaker] Done! {frameCount} frames baked. Each mesh has {smrs.Length} submesh(es) — MeshRenderer needs {matSlot} material slot(s).");
    }

    private void EnsureFolder(string folder)
    {
        if (AssetDatabase.IsValidFolder(folder)) return;
        string[] parts = folder.Replace("Assets/", "").Split('/');
        string current = "Assets";
        foreach (var part in parts)
        {
            if (!AssetDatabase.IsValidFolder(current + "/" + part))
                AssetDatabase.CreateFolder(current, part);
            current += "/" + part;
        }
    }
}
#endif