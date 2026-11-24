using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
public class ShuttleTests
{
    Type Court => AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
    [Test] public void HitGradesHaveDistinctApprovedColors()
    {
        var method = Court.GetMethod("GradeColor");
        Assert.That(method, Is.Not.Null, "Hit feedback requires grade colors");
        var colors = new System.Collections.Generic.HashSet<Color>();
        foreach (string grade in new[] { "Excellent", "Good", "Fair", "Poor" })
            colors.Add((Color)method.Invoke(null, new object[] { grade + " | Clear" }));
        Assert.That(colors.Count, Is.EqualTo(4));
        var poor = (Color)method.Invoke(null, new object[] { "Poor | miss" });
        Assert.That(poor.r, Is.GreaterThan(poor.g));
    }
    [Test] public void AnalogChoosesSixSectorsRatherThanContinuousTargets()
    {
        var aim = Court.GetMethod("CourtPoint");
        var a = (Vector3)aim.Invoke(null, new object[] { new Vector2(.2f, .8f), .5f });
        var b = (Vector3)aim.Invoke(null, new object[] { new Vector2(.9f, .6f), .5f });
        Assert.That(a, Is.EqualTo(b), "Same back-right sector must have same target");
        var zones = new System.Collections.Generic.HashSet<Vector3>();
        foreach (float x in new[] { -.8f, .8f }) foreach (float y in new[] { -.8f, 0, .8f })
            zones.Add((Vector3)aim.Invoke(null, new object[] { new Vector2(x, y), .5f }));
        Assert.That(zones.Count, Is.EqualTo(6));
    }
    [Test] public void LaunchSolvesDragFlightToAnalogTarget()
    {
        var solve = Court.GetMethod("LaunchVelocity");
        Assert.That(solve, Is.Not.Null, "Drag-aware aim solver missing");
        foreach (float x in new[] { -2.8f, 0, 2.8f }) foreach (float z in new[] { 1.2f, 3.5f, 6.4f })
        {
            var start = new Vector3(.3f, 2.1f, -3);
            var target = new Vector3(x, .04f, z);
            var v = (Vector3)solve.Invoke(null, new object[] { start, target, 1.1f });
            var p = start;
            for (int i = 0; i < 55; i++) { v = (Vector3)Court.GetMethod("StepVelocity").Invoke(null, new object[] { v, .02f }); p += v * .02f; }
            Assert.That(Vector3.Distance(p, target), Is.LessThan(.08f), "Target " + target + " actual " + p);
        }
    }
    [Test] public void ElbowPreservesBoneLengthsAtReachableContact()
    {
        var solve = Court.GetMethod("ElbowPosition");
        Assert.That(solve, Is.Not.Null, "Two-bone IK missing");
        var shoulder = Vector3.zero; var hand = new Vector3(.6f, .3f, 0);
        var elbow = (Vector3)solve.Invoke(null, new object[] { shoulder, hand, .4f, .4f });
        Assert.That(Vector3.Distance(shoulder, elbow), Is.EqualTo(.4f).Within(.001f));
        Assert.That(Vector3.Distance(hand, elbow), Is.EqualTo(.4f).Within(.001f));
    }
    [Test] public void CourtBoundariesAndNonlinearDragAreExplicit()
    {
        var inside = Court.GetMethod("InCourt");
        Assert.That(inside.Invoke(null, new object[] { new Vector3(2.59f, 0, 6.7f), false }), Is.EqualTo(true));
        Assert.That(inside.Invoke(null, new object[] { new Vector3(2.6f, 0, 0), false }), Is.EqualTo(false));
        var step = Court.GetMethod("StepVelocity");
        var slow = (Vector3)step.Invoke(null, new object[] { Vector3.forward * 10, .02f });
        var fast = (Vector3)step.Invoke(null, new object[] { Vector3.forward * 20, .02f });
        Assert.That(20 - fast.z, Is.GreaterThan(2 * (10 - slow.z)));
    }
}
