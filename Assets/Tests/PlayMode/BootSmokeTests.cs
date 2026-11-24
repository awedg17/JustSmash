using System;
using System.Linq;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class BootSmokeTests : PreferenceFixture
{
    [UnityTest] public IEnumerator BootRunsPausesAndReloads()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot");
        yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.BootDiagnostics")).First(t => t != null);
        var boot = UnityEngine.Object.FindFirstObjectByType(type);
        Assert.That(boot, Is.Not.Null);
        Assert.That(Application.targetFrameRate, Is.EqualTo(30));
        Assert.That(Time.fixedDeltaTime, Is.EqualTo(.02f).Within(.0001f));
        type.GetMethod("SetPaused").Invoke(boot, new object[] { true });
        Assert.That(Time.timeScale, Is.Zero);
        type.GetMethod("SetPaused").Invoke(boot, new object[] { false });
        Assert.That(Time.timeScale, Is.EqualTo(1));
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot");
        yield return null;
        Assert.That(UnityEngine.Object.FindObjectsByType(type, FindObjectsSortMode.None).Length, Is.EqualTo(1));
        LogAssert.NoUnexpectedReceived();
    }
}
