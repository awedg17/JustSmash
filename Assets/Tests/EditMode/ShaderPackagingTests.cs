using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class ShaderPackagingTests
{
    [Test] public void RuntimeMaterialShadersSurvivePlayerBuild()
    {
        var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
        var included = settings.FindProperty("m_AlwaysIncludedShaders");
        foreach (var name in new[] { "Standard", "Sprites/Default" })
        {
            var shader = Shader.Find(name);
            Assert.That(shader, Is.Not.Null, name + " unavailable in Editor");
            bool retained = false;
            for (int i = 0; i < included.arraySize; i++)
                retained |= included.GetArrayElementAtIndex(i).objectReferenceValue == shader;
            Assert.That(retained, Is.True, name + " must be retained for runtime-created materials");
        }
    }
}
