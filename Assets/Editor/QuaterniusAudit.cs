using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class QuaterniusAudit
{
    public static void Run()
    {
        const string path="Assets/QuaterniusAudit/Superhero_Male_FullBody.fbx";
        var importer=(ModelImporter)AssetImporter.GetAtPath(path);
        importer.isReadable=true; importer.SaveAndReimport();
        var model=AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var instance=UnityEngine.Object.Instantiate(model);
        Directory.CreateDirectory("evidence/M2/quaternius");
        try
        {
            using(var report=new StreamWriter("evidence/M2/quaternius/unity-audit.txt"))
            {
                foreach(var t in instance.GetComponentsInChildren<Transform>()) report.WriteLine($"bone {t.name} world={t.position} local={t.localPosition}");
                foreach(var r in instance.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    report.WriteLine($"mesh {r.name} vertices={r.sharedMesh.vertexCount} triangles={r.sharedMesh.triangles.Length/3} bones={r.bones.Length} submeshes={r.sharedMesh.subMeshCount} bounds={r.bounds}");
                    var mat=new Material(Shader.Find("Standard")); mat.color=r.name.ToLower().Contains("eye")?Color.black:new Color(.62f,.39f,.25f);
                    r.sharedMaterials=Enumerable.Repeat(mat,r.sharedMesh.subMeshCount).ToArray();
                }
            }
            Capture(instance,"base");
        }
        finally { UnityEngine.Object.DestroyImmediate(instance); }
    }
    public static void Capture(GameObject instance,string prefix,string outputDirectory="evidence/M2/quaternius")
    
    {
        Directory.CreateDirectory(outputDirectory);
        var cameraObject=new GameObject("AuditCamera"); var lightObject=new GameObject("AuditLight");
        var camera=cameraObject.AddComponent<Camera>(); var light=lightObject.AddComponent<Light>();
        light.type=LightType.Directional; light.intensity=1.2f; light.transform.rotation=Quaternion.Euler(25,-30,0);
        var bounds=instance.GetComponentsInChildren<Renderer>().Select(r=>r.bounds).Aggregate((a,b)=>{a.Encapsulate(b);return a;});
        camera.clearFlags=CameraClearFlags.SolidColor; camera.backgroundColor=new Color(.16f,.18f,.2f);
        camera.orthographic=true; camera.aspect=.8f; camera.orthographicSize=Mathf.Max(bounds.size.y*.6f,bounds.size.x*.6f/camera.aspect);
        var target=new RenderTexture(800,1000,24); var previous=RenderTexture.active;
        try
        {
            foreach(var view in new[]{"front","back","side","front-left","front-right"})
            {
                var direction=view=="front"?Vector3.back:view=="back"?Vector3.forward:view=="front-left"?new Vector3(1,0,-1).normalized:view=="front-right"?new Vector3(-1,0,-1).normalized:Vector3.right;
                camera.transform.position=bounds.center+direction*4; camera.transform.LookAt(bounds.center);
                camera.targetTexture=target; camera.Render(); RenderTexture.active=target;
                var image=new Texture2D(800,1000,TextureFormat.RGB24,false);
                image.ReadPixels(new Rect(0,0,800,1000),0,0); image.Apply();
                File.WriteAllBytes($"{outputDirectory}/{prefix}-{view}.png",image.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(image);
            }
        }
        finally {camera.targetTexture=null;RenderTexture.active=previous;target.Release();UnityEngine.Object.DestroyImmediate(target);UnityEngine.Object.DestroyImmediate(cameraObject);UnityEngine.Object.DestroyImmediate(lightObject);}
    }
}
