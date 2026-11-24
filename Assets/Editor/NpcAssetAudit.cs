using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class NpcAssetAudit
{
    public static void Run()
    {
        const string root = "Assets/npc_casual_set_00";
        Directory.CreateDirectory("evidence/M2");
        using (var report = new StreamWriter("evidence/M2/D045-npc-audit.txt"))
        {
            foreach (string path in AssetDatabase.FindAssets("t:GameObject", new[]{root + "/Prefabs"})
                .Select(AssetDatabase.GUIDToAssetPath).Where(p => p.EndsWith(".prefab") && !p.Contains("Prefabs_Parts")))
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var renderers = prefab.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                var animators = prefab.GetComponentsInChildren<Animator>(true);
                report.WriteLine(path + " | skins=" + renderers.Length + " | animators=" + animators.Length);
                foreach (var a in animators)
                    report.WriteLine(" avatar=" + (a.avatar == null ? "none" : a.avatar.name + " valid=" + a.avatar.isValid + " human=" + a.avatar.isHuman));
                foreach (var r in renderers)
                    report.WriteLine(" mesh=" + r.name + " vertices=" + (r.sharedMesh == null ? 0 : r.sharedMesh.vertexCount) + " bones=" + r.bones.Length + " materials=" + string.Join(",", r.sharedMaterials.Select(m => m == null ? "MISSING" : m.name + ":" + m.shader.name)));
                if (Path.GetFileName(path) == "npc_csl_00_character_01m_01.prefab")
                {
                    foreach (var t in prefab.GetComponentsInChildren<Transform>(true)) report.WriteLine(" bone=" + t.name);
                    var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    try
                    {
                        var cameraObject = new GameObject("AuditCamera");
                        var lightObject = new GameObject("AuditLight");
                        var camera = cameraObject.AddComponent<Camera>();
                        var light = lightObject.AddComponent<Light>(); light.type = LightType.Directional;
                        light.transform.rotation = Quaternion.Euler(35,-25,0);
                        var bounds = new Bounds(instance.transform.position, Vector3.zero);
                        foreach(var r in instance.GetComponentsInChildren<Renderer>()) bounds.Encapsulate(r.bounds);
                        report.WriteLine(" bounds=" + bounds);
                        camera.transform.position = bounds.center + new Vector3(0,0,-Mathf.Max(3,bounds.size.y*1.5f));
                        camera.transform.LookAt(bounds.center);
                        camera.backgroundColor = new Color(.15f,.15f,.15f); camera.clearFlags = CameraClearFlags.SolidColor;
                        var target = new RenderTexture(800,1000,24); var previous = RenderTexture.active;
                        try
                        {
                            camera.targetTexture = target; camera.Render(); RenderTexture.active = target;
                            var image = new Texture2D(800,1000,TextureFormat.RGB24,false);
                            image.ReadPixels(new Rect(0,0,800,1000),0,0); image.Apply();
                            File.WriteAllBytes("evidence/M2/D045-npc-preview.png",image.EncodeToPNG());
                            UnityEngine.Object.DestroyImmediate(image);
                        }
                        finally { RenderTexture.active = previous; camera.targetTexture = null; target.Release(); UnityEngine.Object.DestroyImmediate(target); UnityEngine.Object.DestroyImmediate(cameraObject); UnityEngine.Object.DestroyImmediate(lightObject); }
                    }
                    finally { UnityEngine.Object.DestroyImmediate(instance); }
                }
            }
        }
        Debug.Log("NPC asset audit complete: evidence/M2/D045-npc-audit.txt");
    }
}
