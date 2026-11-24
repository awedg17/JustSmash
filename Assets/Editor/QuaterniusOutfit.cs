using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Original ring-built garments; licensed body supplies fit reference and nearest skin weights only.
public static class QuaterniusOutfit
{
    static readonly List<Vector3> vertices=new List<Vector3>();
    static readonly List<int> triangles=new List<int>();
    static readonly List<BoneWeight> weights=new List<BoneWeight>();
    static SkinnedMeshRenderer body;
    static Vector3[] bodyWorld;
    static float Cross(Vector2 a,Vector2 b)=>a.x*b.y-a.y*b.x;
    // Original concave front/back panels. Perimeter joins share vertices, not overlapping tubes.
    static void Panels(Vector2[] outline,float[] depth,int[] holes)
    {
        int n=outline.Length;
        for(int side=0;side<2;side++)for(int i=0;i<n;i++)vertices.Add(new Vector3(outline[i].x,outline[i].y,.04f+(side==0?-depth[i]:depth[i])));
        var polygon=Enumerable.Range(0,n).ToList();float area=0;for(int i=0;i<n;i++)area+=Cross(outline[i],outline[(i+1)%n]);if(area<0)polygon.Reverse();
        while(polygon.Count>3)
        {
            bool found=false;
            for(int j=0;j<polygon.Count;j++)
            {
                int a=polygon[(j+polygon.Count-1)%polygon.Count],b=polygon[j],c=polygon[(j+1)%polygon.Count];
                if(Cross(outline[b]-outline[a],outline[c]-outline[b])<=1e-8f)continue;
                if(polygon.Any(k=>k!=a&&k!=b&&k!=c&&Cross(outline[b]-outline[a],outline[k]-outline[a])>=0&&Cross(outline[c]-outline[b],outline[k]-outline[b])>=0&&Cross(outline[a]-outline[c],outline[k]-outline[c])>=0))continue;
                triangles.AddRange(new[]{a,c,b,a+n,b+n,c+n});polygon.RemoveAt(j);found=true;break;
            }
            if(!found)throw new Exception("Panel triangulation failed");
        }
        triangles.AddRange(new[]{polygon[0],polygon[2],polygon[1],polygon[0]+n,polygon[1]+n,polygon[2]+n});
        for(int i=0;i<n;i++)if(!holes.Contains(i))Quad(i,(i+1)%n,(i+1)%n+n,i+n);
    }
    static void Quad(int a,int b,int c,int d){triangles.AddRange(new[]{a,b,c,a,c,d});}
    static T Save<T>(T candidate,string path) where T:UnityEngine.Object
    {
        var existing=AssetDatabase.LoadAssetAtPath<T>(path);if(existing==null){AssetDatabase.CreateAsset(candidate,path);return candidate;}
        EditorUtility.CopySerialized(candidate,existing);EditorUtility.SetDirty(existing);UnityEngine.Object.DestroyImmediate(candidate);return existing;
    }
    static Mesh Candidate(string name)
    {
        if(name!="Shoes")
        {
            // Refine original panels, then fit depth against the licensed body surface.
            for(int pass=0;pass<3;pass++)
            {
                var old=triangles.ToArray();triangles.Clear();var midpoints=new Dictionary<(int,int),int>();
                Func<int,int,int> mid=(a,b)=>{var key=a<b?(a,b):(b,a);if(midpoints.TryGetValue(key,out int index))return index;index=vertices.Count;vertices.Add((vertices[a]+vertices[b])*.5f);midpoints[key]=index;return index;};
                for(int i=0;i<old.Length;i+=3){int a=old[i],b=old[i+1],c=old[i+2],ab=mid(a,b),bc=mid(b,c),ca=mid(c,a);triangles.AddRange(new[]{a,ab,ca,ab,b,bc,ca,bc,c,ab,bc,ca});}
            }
            var fitNormals=new Vector3[vertices.Count];for(int k=0;k<triangles.Count;k+=3){int a=triangles[k],b=triangles[k+1],c=triangles[k+2];var normal=Vector3.Cross(vertices[b]-vertices[a],vertices[c]-vertices[a]).normalized;fitNormals[a]+=normal;fitNormals[b]+=normal;fitNormals[c]+=normal;}
            var bt=body.sharedMesh.triangles;
            for(int i=0;i<vertices.Count;i++)
            {
                var p=vertices[i];float near=float.PositiveInfinity,far=float.NegativeInfinity;
                for(int k=0;k<bt.Length;k+=3)
                {
                    var a=bodyWorld[bt[k]];var b=bodyWorld[bt[k+1]];var c=bodyWorld[bt[k+2]];
                    if(p.x<Mathf.Min(a.x,b.x,c.x)||p.x>Mathf.Max(a.x,b.x,c.x)||p.y<Mathf.Min(a.y,b.y,c.y)||p.y>Mathf.Max(a.y,b.y,c.y))continue;
                    var ab=new Vector2(b.x-a.x,b.y-a.y);var ac=new Vector2(c.x-a.x,c.y-a.y);var ap=new Vector2(p.x-a.x,p.y-a.y);float det=Cross(ab,ac);if(Mathf.Abs(det)<1e-10f)continue;
                    float u=Cross(ap,ac)/det,v=Cross(ab,ap)/det;if(u<0||v<0||u+v>1)continue;float z=a.z+u*(b.z-a.z)+v*(c.z-a.z);near=Mathf.Min(near,z);far=Mathf.Max(far,z);
                }
                if(!float.IsInfinity(near)){float target=p.z<.04f?Mathf.Min(p.z,near-.012f):Mathf.Max(p.z,far+.012f);p.z=Mathf.Lerp(p.z,target,Mathf.Abs(fitNormals[i].normalized.z));}vertices[i]=p;
            }
        }
        foreach(var world in vertices)
        {
            int nearest=0;float distance=float.MaxValue;
            for(int k=0;k<bodyWorld.Length;k++){if(Mathf.Abs(world.x)>.02f&&world.x*bodyWorld[k].x<0)continue;float d=(bodyWorld[k]-world).sqrMagnitude;if(d<distance){distance=d;nearest=k;}}
            if(name=="Shoes")
            {
                int bone=Array.FindIndex(body.bones,b=>b.name==(world.x>0?"foot_l":"foot_r"));if(bone<0)throw new Exception("Foot bone missing");weights.Add(new BoneWeight{boneIndex0=bone,weight0=1});
            }
            else weights.Add(body.sharedMesh.boneWeights[nearest]);
        }
        var mesh=new Mesh{name=name};mesh.vertices=vertices.Select(body.transform.InverseTransformPoint).ToArray();mesh.SetTriangles(triangles,0);mesh.boneWeights=weights.ToArray();mesh.bindposes=body.bones.Select(b=>b.worldToLocalMatrix*body.transform.localToWorldMatrix).ToArray();mesh.RecalculateNormals();mesh.RecalculateBounds();
        var result=QuaterniusOutfitChecks.Structure(mesh,name,name=="Shoes"?2:1,name=="Jersey"?4:name=="Shorts"?3:0);
        vertices.Clear();triangles.Clear();weights.Clear();if(result.failure!=null)throw new Exception("Candidate "+name+": "+result.failure);return mesh;
    }
    static void SaveGarment(GameObject root,Mesh candidate,Color color)
    {
        string name=candidate.name;var mesh=Save(candidate,$"Assets/QuaterniusAudit/{name}.asset");
        var mat=new Material(Shader.Find("Standard")){name=name,color=color};mat.SetFloat("_Glossiness",.12f);mat=Save(mat,$"Assets/QuaterniusAudit/{name}.mat");
        var go=new GameObject(name);go.transform.SetParent(body.transform.parent,false);go.transform.localPosition=body.transform.localPosition;go.transform.localRotation=body.transform.localRotation;go.transform.localScale=body.transform.localScale;
        var skin=go.AddComponent<SkinnedMeshRenderer>();skin.sharedMesh=mesh;skin.sharedMaterial=mat;skin.bones=body.bones;skin.rootBone=body.rootBone;skin.updateWhenOffscreen=true;
        var baked=new Mesh();skin.BakeMesh(baked);float error=0;for(int i=0;i<mesh.vertexCount;i++)error=Mathf.Max(error,Vector3.Distance(mesh.vertices[i],baked.vertices[i]));
        File.AppendAllText("evidence/M2/quaternius/repair/bind-candidate.txt",$"{name}: displacement={error}; world={skin.bounds}; local={mesh.bounds}; scale={go.transform.lossyScale}\n");UnityEngine.Object.DestroyImmediate(baked);
    }
    public static void CaptureBase()
    {
        var root=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/QuaterniusAudit/Superhero_Male_FullBody.fbx"));
        try{QuaterniusAudit.Capture(root,"base","evidence/M2/quaternius/repair");}finally{vertices.Clear();triangles.Clear();weights.Clear();UnityEngine.Object.DestroyImmediate(root);}
    }
    public static void Run()
    {
        vertices.Clear();triangles.Clear();weights.Clear();
        var root=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/QuaterniusAudit/Superhero_Male_FullBody.fbx"));
        try
        {
            body=root.GetComponentsInChildren<SkinnedMeshRenderer>().Single(r=>r.name=="SuperHero_Male");
            bodyWorld=body.sharedMesh.vertices.Select(v=>body.transform.TransformPoint(v)).ToArray();
            Panels(new[]{new Vector2(-.20f,.93f),new Vector2(.20f,.93f),new Vector2(.235f,1.32f),new Vector2(.37f,1.35f),new Vector2(.37f,1.55f),new Vector2(.21f,1.53f),new Vector2(.085f,1.53f),new Vector2(.06f,1.51f),new Vector2(-.06f,1.51f),new Vector2(-.085f,1.53f),new Vector2(-.21f,1.53f),new Vector2(-.37f,1.55f),new Vector2(-.37f,1.35f),new Vector2(-.235f,1.32f)},new[]{.15f,.15f,.17f,.105f,.105f,.14f,.09f,.09f,.09f,.09f,.14f,.105f,.105f,.17f},new[]{0,3,6,7,8,11});
            var jersey=Candidate("Jersey");
            Panels(new[]{new Vector2(-.20f,1.0f),new Vector2(-.26f,.86f),new Vector2(-.25f,.66f),new Vector2(-.035f,.66f),new Vector2(-.02f,.79f),new Vector2(.02f,.79f),new Vector2(.035f,.66f),new Vector2(.25f,.66f),new Vector2(.26f,.86f),new Vector2(.20f,1.0f)},new[]{.17f,.18f,.14f,.14f,.12f,.12f,.14f,.14f,.18f,.17f},new[]{2,6,9});
            var shorts=Candidate("Shorts");
            foreach(float sign in new[]{-1f,1f})
            {
                var foot=bodyWorld.Where(p=>p.y<=.095f&&p.x*sign>0).ToArray();if(foot.Length==0)throw new Exception("Foot ROI empty");
                float minX=foot.Min(p=>p.x)-.008f,maxX=foot.Max(p=>p.x)+.008f,minY=foot.Min(p=>p.y)-.015f,maxY=foot.Max(p=>p.y)+.012f,minZ=foot.Min(p=>p.z)-.012f,maxZ=foot.Max(p=>p.z)+.012f;
                int start=vertices.Count;const int sides=16,rings=5;
                for(int z=0;z<rings;z++)for(int j=0;j<sides;j++)
                {
                    float angle=j*Mathf.PI*2/sides,scale=z==0||z==rings-1?.96f:1;
                    vertices.Add(new Vector3((minX+maxX)/2+Mathf.Sign(Mathf.Cos(angle))*Mathf.Pow(Mathf.Abs(Mathf.Cos(angle)),.3f)*(maxX-minX)/2*scale,(minY+maxY)/2+Mathf.Sign(Mathf.Sin(angle))*Mathf.Pow(Mathf.Abs(Mathf.Sin(angle)),.3f)*(maxY-minY)/2,Mathf.Lerp(minZ,maxZ,z/(float)(rings-1))));
                }
                for(int z=0;z<rings-1;z++)for(int j=0;j<sides;j++)Quad(start+z*sides+j,start+z*sides+(j+1)%sides,start+(z+1)*sides+(j+1)%sides,start+(z+1)*sides+j);
                int front=vertices.Count;vertices.Add(new Vector3((minX+maxX)/2,(minY+maxY)/2,minZ));int back=vertices.Count;vertices.Add(new Vector3((minX+maxX)/2,(minY+maxY)/2,maxZ));
                for(int j=0;j<sides;j++)triangles.AddRange(new[]{front,start+(j+1)%sides,start+j,back,start+(rings-1)*sides+j,start+(rings-1)*sides+(j+1)%sides});
            }
            var shoes=Candidate("Shoes");
            foreach(var r in root.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                string n=r==body?"Skin":r.name=="Eyes"?"Eyes":"Brows";
                var mat=new Material(Shader.Find("Standard")){name=n,color=n=="Skin"?new Color(.57f,.33f,.20f):n=="Eyes"?new Color(.85f,.83f,.76f):new Color(.04f,.025f,.02f)};
                mat.SetFloat("_Glossiness",.12f);r.sharedMaterial=Save(mat,$"Assets/QuaterniusAudit/{n}.mat");
            }
            SaveGarment(root,jersey,new Color(.04f,.34f,.72f));SaveGarment(root,shorts,new Color(.025f,.035f,.065f));SaveGarment(root,shoes,new Color(.85f,.88f,.91f));
            AssetDatabase.SaveAssets();
            PrefabUtility.SaveAsPrefabAsset(root,"Assets/QuaterniusAudit/AthletePreview.prefab");
            QuaterniusAudit.Capture(root,"candidate-3","evidence/M2/quaternius/repair");
            File.WriteAllText("evidence/M2/quaternius/outfit-counts.txt",string.Join("\n",root.GetComponentsInChildren<SkinnedMeshRenderer>().Select(r=>$"{r.name}: vertices={r.sharedMesh.vertexCount} triangles={r.sharedMesh.triangles.Length/3} bones={r.bones.Length} materials={r.sharedMaterials.Length}")));
        }
        finally{vertices.Clear();triangles.Clear();weights.Clear();UnityEngine.Object.DestroyImmediate(root);}
    }
}
