using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ArenaAtmosphereTests : PreferenceFixture
{
    [UnityTest]
    public IEnumerator IndoorFinishKeepsDecorationOutsidePlayWithoutExtraLights()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot");
        yield return null; // ArenaPart removes primitive colliders at frame end.
        var objects = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        var lining = objects.Where(t => t.name == "HallWallLining").ToArray();
        Assert.That(lining.Length, Is.EqualTo(3));
        foreach (var part in lining)
        {
            Assert.That(part.GetComponent<Collider>(), Is.Null);
            Assert.That(Mathf.Abs(part.position.x) > 9 || part.position.z > 15, Is.True,
                "Wall finish must stay outside court/runoff");
            Assert.That(part.GetComponent<Renderer>().enabled, Is.True);
        }
        Assert.That(objects.Count(t => t.name == "CeilingLampHousing"), Is.EqualTo(10));
        var lamps = objects.Where(t => t.name == "CeilingLamp").ToArray();
        Assert.That(lamps.Length, Is.EqualTo(10));
        foreach (var lamp in lamps)
        {
            Assert.That(lamp.GetComponent<Collider>(), Is.Null);
            var material = lamp.GetComponent<Renderer>().sharedMaterial;
            Assert.That(material.name, Is.EqualTo("Ceiling diffuser"));
            Assert.That(material.shader.name, Is.EqualTo("Sprites/Default").Or.EqualTo("UI/Default"));
            Assert.That(material.color.maxColorComponent, Is.LessThanOrEqualTo(.8f));
        }
        var lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        Assert.That(lights.Length, Is.EqualTo(1));
        Assert.That(lights[0].type, Is.EqualTo(LightType.Directional));
        Assert.That(GameObject.Find("Court").transform.localScale, Is.EqualTo(new Vector3(8, .1f, 16)));
        Assert.That(GameObject.Find("NetTape"), Is.Not.Null);
        Assert.That(GameObject.Find("NetMesh"), Is.Not.Null);
        Assert.That(GameObject.Find("Umpire"), Is.Not.Null);
        Assert.That(GameObject.Find("ServiceJudge"), Is.Not.Null);
        Assert.That(objects.Any(t => t.name.Contains("Spectator") || t.name.Contains("Bleacher")), Is.False);
    }
}
