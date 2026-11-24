using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class ShotIntentTests
{
    static Type Kind => AppDomain.CurrentDomain.GetAssemblies()
        .Select(a => a.GetType("JustSmash.ShotIntent")).FirstOrDefault(t => t != null);

    static object Create()
    {
        Assert.That(Kind, Is.Not.Null, "ShotIntent missing");
        return Activator.CreateInstance(Kind);
    }

    static object Call(object intent, string name, params object[] args) =>
        Kind.GetMethod(name).Invoke(intent, args);

    static bool Take(object intent, float now, bool reachable, out int shot)
    {
        object[] args = { now, reachable, -1 };
        bool result = (bool)Call(intent, "TryTake", args);
        shot = (int)args[2];
        return result;
    }

    [Test] public void EarlyInputFiresOnceWhenReachable()
    {
        var intent = Create();
        Call(intent, "Queue", 0, Vector2.left, .5f, 1f);
        Assert.That(Take(intent, 1.04f, false, out _), Is.False);
        Assert.That(Take(intent, 1.12f, true, out int shot), Is.True);
        Assert.That(shot, Is.EqualTo(0));
        Assert.That(Take(intent, 1.13f, true, out _), Is.False);
    }

    [Test] public void ExpiredInputNeverBecomesALateHit()
    {
        var intent = Create();
        Call(intent, "Queue", 2, Vector2.right, .5f, 1f);
        Assert.That(Take(intent, 1.161f, true, out _), Is.False);
        Assert.That((bool)Kind.GetProperty("Pending").GetValue(intent), Is.False);
    }

    [Test] public void NewerIntentWinsAndCancelRemovesIt()
    {
        var intent = Create();
        Call(intent, "Queue", 0, Vector2.left, .5f, 1f);
        Call(intent, "Queue", 4, Vector2.right, .8f, 1.01f);
        Assert.That(Take(intent, 1.05f, true, out int shot), Is.True);
        Assert.That(shot, Is.EqualTo(4));
        Assert.That((Vector2)Kind.GetProperty("Aim").GetValue(intent), Is.EqualTo(Vector2.right));
        Call(intent, "Queue", 1, Vector2.zero, .5f, 2f);
        Call(intent, "Clear");
        Assert.That(Take(intent, 2.01f, true, out _), Is.False);
    }

    [Test] public void InvalidAimCannotReplacePendingInput()
    {
        var intent = Create();
        Call(intent, "Queue", 0, Vector2.left, .5f, 1f);
        var error = Assert.Throws<TargetInvocationException>(() =>
            Call(intent, "Queue", 1, new Vector2(float.NaN, 0), .5f, 1.01f));
        Assert.That(error.InnerException, Is.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(Take(intent, 1.05f, true, out int shot), Is.True);
        Assert.That(shot, Is.EqualTo(0));
    }
}
