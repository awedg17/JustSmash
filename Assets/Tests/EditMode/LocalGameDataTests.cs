using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class LocalGameDataTests
{
    [Test] public void HostPreferencesStayInsideWorkspaceAndPreserveMalformedValue()
    {
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.LocalGameData")).FirstOrDefault(t => t != null);
        Assert.That(type, Is.Not.Null, "LocalGameData missing");
        string root = (string)type.GetProperty("Root").GetValue(null);
        string expected = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".hermes", "local-game-data"));
        Assert.That(root, Is.EqualTo(expected));
        string key = "test-" + Guid.NewGuid().ToString("N");
        string path = Path.Combine(root, "prefs", key + ".txt");
        object Call(string name, params object[] args) => type.GetMethod(name).Invoke(null, args);
        try
        {
            Call("SetFloat", key, .75f);
            Assert.That((float)Call("GetFloat", key, 0f), Is.EqualTo(.75f));
            Call("SetString", key, "malformed");
            Assert.That((float)Call("GetFloat", key, .5f), Is.EqualTo(.5f));
            Assert.That(File.ReadAllText(path), Is.EqualTo("malformed"));
            var error = Assert.Throws<TargetInvocationException>(() => Call("SetString", "../escape", "bad"));
            Assert.That(error.InnerException, Is.TypeOf<ArgumentException>());
        }
        finally
        {
            foreach (string suffix in new[] { "", ".tmp", ".bak" })
                if (File.Exists(path + suffix)) File.Delete(path + suffix);
        }
    }
}
