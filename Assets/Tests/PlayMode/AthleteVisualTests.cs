using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class AthleteVisualTests : PreferenceFixture
{
    [UnityTest] public IEnumerator BothAthletesUseSkinnedVisualWithoutReplacingGameplayRig()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var camera = Camera.main;
        var target = new RenderTexture(960,540,24); var previous = RenderTexture.active;
        try
        {
            camera.targetTexture=target; camera.Render(); RenderTexture.active=target;
            var image=new Texture2D(960,540,TextureFormat.RGB24,false);
            image.ReadPixels(new Rect(0,0,960,540),0,0); image.Apply();
            System.IO.File.WriteAllBytes(EvidenceDirectory + "/D045-integrated-court.png",image.EncodeToPNG());
            UnityEngine.Object.Destroy(image);
        }
        finally { camera.targetTexture=null; RenderTexture.active=previous; target.Release(); UnityEngine.Object.Destroy(target); }
        foreach (string name in new[]{"Player","Opponent"})
        {
            var athlete = GameObject.Find(name).transform;
            Assert.That(athlete.Find("Hips/Spine/Chest/ShoulderRight/ElbowRight/Racket"), Is.Not.Null, "Gameplay contact rig lost");
            var skins = athlete.GetComponentsInChildren<SkinnedMeshRenderer>();
            Assert.That(skins.Length, Is.GreaterThan(0), name + " NPC visual missing");
            foreach(var skin in skins)
            {
                Assert.That(skin.sharedMesh, Is.Not.Null);
                Assert.That(skin.bones.All(b => b != null), Is.True, "Missing skin bone");
                Assert.That(skin.bounds.size.y, Is.LessThan(3), "Exploded skin bounds");
            }
        }
    }
}
