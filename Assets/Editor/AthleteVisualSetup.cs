using System.IO;
using UnityEditor;
using UnityEngine;

public static class AthleteVisualSetup
{
    public static void Run()
    {
        const string source="Assets/npc_casual_set_00/Prefabs/npc_csl_00_character_01m_01.prefab";
        Directory.CreateDirectory("Assets/Resources");
        var original=AssetDatabase.LoadAssetAtPath<GameObject>(source);
        if(original==null) throw new System.IO.FileNotFoundException(source);
        var instance=(GameObject)PrefabUtility.InstantiatePrefab(original);
        try
        {
            PrefabUtility.UnpackPrefabInstance(instance,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
            foreach(var lod in instance.GetComponentsInChildren<LODGroup>(true)) Object.DestroyImmediate(lod);
            foreach(var skin in instance.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if(!skin.name.EndsWith("lod0")) { Object.DestroyImmediate(skin.gameObject); continue; }
                skin.updateWhenOffscreen=true;
            }
            PrefabUtility.SaveAsPrefabAsset(instance,"Assets/Resources/AthleteNPC.prefab");
            AssetDatabase.SaveAssets();
            Debug.Log("AthleteNPC prefab prepared; original mesh/material assets preserved");
        }
        finally { Object.DestroyImmediate(instance); }
    }
}
