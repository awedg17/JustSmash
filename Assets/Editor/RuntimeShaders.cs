using System;
using UnityEditor;
using UnityEngine;

public static class RuntimeShaders
{
    public static void Retain()
    {
        var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
        var included = settings.FindProperty("m_AlwaysIncludedShaders");
        foreach (var name in new[] { "Standard", "Sprites/Default" })
        {
            var shader = Shader.Find(name);
            if (shader == null) throw new InvalidOperationException("Required shader missing: " + name);
            bool found = false;
            for (int i = 0; i < included.arraySize; i++) found |= included.GetArrayElementAtIndex(i).objectReferenceValue == shader;
            if (!found)
            {
                included.InsertArrayElementAtIndex(included.arraySize);
                included.GetArrayElementAtIndex(included.arraySize - 1).objectReferenceValue = shader;
                Debug.Log("[RuntimeShaders] Added to AlwaysIncluded: " + name);
            }
        }
        settings.ApplyModifiedPropertiesWithoutUndo();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}