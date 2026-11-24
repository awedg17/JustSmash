using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

public class OpponentProfileTests
{
    [Test] public void DifficultyChangesPerceptionNotMovementCapacity()
    {
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetType("JustSmash.OpponentProfile")).FirstOrDefault(t => t != null);
        Assert.That(type, Is.Not.Null, "OpponentProfile missing");
        object Profile(string level) => type.GetMethod("For").Invoke(null, new object[] { level });
        float Value(object p, string field) => (float)type.GetField(field).GetValue(p);
        var easy = Profile("Easy"); var normal = Profile("Normal"); var hard = Profile("Hard");
        Assert.That(Value(easy, "Reaction"), Is.GreaterThan(Value(normal, "Reaction")));
        Assert.That(Value(normal, "Reaction"), Is.GreaterThan(Value(hard, "Reaction")));
        Assert.That(Value(easy, "AimError"), Is.GreaterThan(Value(hard, "AimError")));
        Assert.That((float)type.GetField("MovementSpeed").GetRawConstantValue(), Is.EqualTo(4.5f));
        var error = Assert.Throws<TargetInvocationException>(() => Profile("Impossible"));
        Assert.That(error.InnerException, Is.TypeOf<ArgumentException>());
    }
}
