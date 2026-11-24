using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Editor-only, welded adjacency: render seams do not count as garment holes.
public static class QuaterniusOutfitChecks
{
    const string Output="evidence/M2/quaternius/repair";
    [Serializable] public class Result { public string garment; public int vertices,triangles,components,boundaryLoops,nonManifold,degenerate; public string failure; }
    [Serializable] class Report { public Result[] results; public string status; }
    public static Result Structure(Mesh mesh,string name,int components,int loops)
    {
        var r=new Result {garment=name,vertices=mesh.vertexCount,triangles=mesh.triangles.Length/3};
        var v=mesh.vertices;var t=mesh.triangles;var weld=new Dictionary<string,int>();var ids=new int[v.Length];
        for(int i=0;i<v.Length;i++)
        {
            if(!float.IsFinite(v[i].x)||!float.IsFinite(v[i].y)||!float.IsFinite(v[i].z))throw new Exception(name+" nonfinite vertex");
            string key=$"{Mathf.RoundToInt(v[i].x*100000)},{Mathf.RoundToInt(v[i].y*100000)},{Mathf.RoundToInt(v[i].z*100000)}";
            if(!weld.TryGetValue(key,out ids[i])){ids[i]=weld.Count;weld.Add(key,ids[i]);}
        }
        var edges=new Dictionary<(int,int),int>();var adjacency=new Dictionary<int,HashSet<int>>();
        Action<int,int> edge=(a,b)=>{a=ids[a];b=ids[b];var key=a<b?(a,b):(b,a);edges[key]=edges.TryGetValue(key,out int n)?n+1:1;
            if(!adjacency.ContainsKey(a))adjacency[a]=new HashSet<int>();if(!adjacency.ContainsKey(b))adjacency[b]=new HashSet<int>();adjacency[a].Add(b);adjacency[b].Add(a);};
        if(t.Length%3!=0)throw new Exception(name+" index count");
        for(int i=0;i<t.Length;i+=3)
        {
            if(t.Skip(i).Take(3).Any(x=>x<0||x>=v.Length))throw new Exception(name+" invalid index");
            if(Vector3.Cross(v[t[i+1]]-v[t[i]],v[t[i+2]]-v[t[i]]).magnitude<=1e-10f)r.degenerate++;
            edge(t[i],t[i+1]);edge(t[i+1],t[i+2]);edge(t[i+2],t[i]);
        }
        r.components=Count(adjacency);var boundary=new Dictionary<int,HashSet<int>>();
        foreach(var e in edges){if(e.Value>2)r.nonManifold++;if(e.Value!=1)continue;int a=e.Key.Item1,b=e.Key.Item2;
            if(!boundary.ContainsKey(a))boundary[a]=new HashSet<int>();if(!boundary.ContainsKey(b))boundary[b]=new HashSet<int>();boundary[a].Add(b);boundary[b].Add(a);}
        r.boundaryLoops=Count(boundary);
        if(boundary.Values.Any(x=>x.Count!=2))r.failure="boundary not closed loops";
        else if(r.degenerate!=0||r.nonManifold!=0||r.components!=components||r.boundaryLoops!=loops)r.failure=$"expected components={components}, loops={loops}; actual components={r.components}, loops={r.boundaryLoops}, nonManifold={r.nonManifold}, degenerate={r.degenerate}";
        return r;
    }
    static bool Ray(Vector3 origin,Vector3 direction,Vector3 a,Vector3 b,Vector3 c,out float distance)
    {
        distance=0;var e1=b-a;var e2=c-a;var p=Vector3.Cross(direction,e2);float det=Vector3.Dot(e1,p);if(Mathf.Abs(det)<1e-9f)return false;
        float inv=1/det;var s=origin-a;float u=Vector3.Dot(s,p)*inv;if(u<0||u>1)return false;var q=Vector3.Cross(s,e1);float v=Vector3.Dot(direction,q)*inv;if(v<0||u+v>1)return false;
        distance=Vector3.Dot(e2,q)*inv;return distance>1e-6f;
    }
    [Serializable] class CoverageResult {public string garment,predicate;public int samples,misses;public float maxDistance,bindError;public Vector3[] failures;}
    [Serializable] class CoverageReport {public CoverageResult[] results;public string status;}
    public static void Coverage()
    {
        Directory.CreateDirectory(Output);var results=new List<CoverageResult>();GameObject root=null;
        string suffix=Environment.GetEnvironmentVariable("OUTFIT_RUN")??DateTime.Now.ToString("yyyyMMdd_HHmmss");
        try
        {
            root=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/QuaterniusAudit/AthletePreview.prefab"));
            var body=root.GetComponentsInChildren<SkinnedMeshRenderer>().Single(x=>x.name=="SuperHero_Male");var bakedBody=new Mesh();body.BakeMesh(bakedBody);
            var points=bakedBody.vertices.Select(body.transform.TransformPoint).ToArray();var normals=bakedBody.normals.Select(body.transform.TransformDirection).Select(n=>n.normalized).ToArray();
            foreach(string name in new[]{"Jersey","Shorts","Shoes"})
            {
                var r=root.GetComponentsInChildren<SkinnedMeshRenderer>().Single(x=>x.name==name);var mesh=new Mesh();r.BakeMesh(mesh);var vertices=mesh.vertices.Select(r.transform.TransformPoint).ToArray();var triangles=mesh.triangles;
                var result=new CoverageResult{garment=name,predicate=name=="Jersey"?"torso:1.00<=y<=1.42,abs(x)<=.23; shoulder:1.42<y<=1.54,.12<=abs(x)<=.30":name=="Shorts"?".72<=y<=.96,abs(x)<=.26":"y<=.095 (entire foot)"};var failures=new List<Vector3>();
                for(int i=0;i<mesh.vertexCount;i++)result.bindError=Mathf.Max(result.bindError,Vector3.Distance(r.transform.TransformPoint(r.sharedMesh.vertices[i]),vertices[i]));
                for(int i=0;i<points.Length;i++)
                {
                    var p=points[i];float ax=Mathf.Abs(p.x);bool selected=name=="Jersey"?(p.y>=1&&p.y<=1.42f&&ax<=.23f)||(p.y>1.42f&&p.y<=1.54f&&ax>=.12f&&ax<=.30f):name=="Shorts"?p.y>=.72f&&p.y<=.96f&&ax<=.26f:p.y<=.095f;
                    if(!selected)continue;result.samples++;float nearest=float.PositiveInfinity;var origin=p-normals[i]*.002f;
                    for(int k=0;k<triangles.Length;k+=3)if(Ray(origin,normals[i],vertices[triangles[k]],vertices[triangles[k+1]],vertices[triangles[k+2]],out float distance))nearest=Mathf.Min(nearest,distance);
                    if(float.IsInfinity(nearest)||nearest<.002f||nearest>.042f){result.misses++;failures.Add(p);}else result.maxDistance=Mathf.Max(result.maxDistance,nearest-.002f);
                }
                result.failures=failures.ToArray();results.Add(result);UnityEngine.Object.DestroyImmediate(mesh);
            }
            UnityEngine.Object.DestroyImmediate(bakedBody);
            if(results.Any(r=>r.misses!=0||r.samples==0||r.bindError>.001f))throw new Exception("OUTFIT_CHECK_FAILED: coverage "+string.Join("; ",results.Select(r=>$"{r.garment} misses={r.misses}/{r.samples}, bind={r.bindError}")));
            Debug.Log("OUTFIT_COVERAGE_PASS; containment/pose not yet evaluated");
        }
        finally
        {
            File.WriteAllText(Output+"/coverage-"+suffix+".json",JsonUtility.ToJson(new CoverageReport{results=results.ToArray(),status=results.Count==3&&results.All(r=>r.misses==0&&r.samples>0&&r.bindError<=.001f)?"PASS_REST_RAYS_ONLY":"FAIL"},true));
            if(root!=null)UnityEngine.Object.DestroyImmediate(root);
        }
    }
    static int Count(Dictionary<int,HashSet<int>> graph)
    {
        int count=0;var seen=new HashSet<int>();foreach(int a in graph.Keys){if(!seen.Add(a))continue;count++;var q=new Queue<int>();q.Enqueue(a);while(q.Count>0)foreach(int b in graph[q.Dequeue()])if(seen.Add(b))q.Enqueue(b);}return count;
    }
    public static void Run()
    {
        Directory.CreateDirectory(Output);var results=new List<Result>();GameObject root=null;string status="FAIL";
        string suffix=Environment.GetEnvironmentVariable("OUTFIT_RUN")??DateTime.Now.ToString("yyyyMMdd_HHmmss");
        try
        {
            root=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/QuaterniusAudit/AthletePreview.prefab"));
            foreach(string name in new[]{"Jersey","Shorts","Shoes"})
            {
                var renderer=root.GetComponentsInChildren<SkinnedMeshRenderer>().Single(x=>x.name==name);
                var result=Structure(renderer.sharedMesh,name,name=="Shoes"?2:1,name=="Jersey"?4:name=="Shorts"?3:0);results.Add(result);
            }
            var failures=results.Where(x=>x.failure!=null).Select(x=>x.garment+": "+x.failure).ToArray();
            if(failures.Length>0)throw new Exception("OUTFIT_CHECK_FAILED: "+string.Join("; ",failures));
            status="STRUCTURE_PASS_ONLY";Debug.Log("OUTFIT_STRUCTURE_PASS; coverage and pose gate not yet evaluated");
        }
        finally
        {
            File.WriteAllText(Output+"/checks-"+suffix+".json",JsonUtility.ToJson(new Report{results=results.ToArray(),status=status},true));
            if(root!=null)UnityEngine.Object.DestroyImmediate(root);
        }
    }
}
