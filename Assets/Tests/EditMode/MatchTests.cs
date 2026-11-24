using System;
using System.Linq;
using NUnit.Framework;

public class MatchTests
{
    Type Match => AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.SinglesMatch")).FirstOrDefault(t => t != null);
    [Test] public void DoublesServicePassesThroughAllFourPlayersAndSurvivesSave()
    {
        Assert.That(Match.GetField("Doubles"), Is.Not.Null, "No men's doubles service state");
        var m = Activator.CreateInstance(Match); Match.GetField("Doubles").SetValue(m, true);
        Action<int> point = side => Match.GetMethod("AwardPoint").Invoke(m, new object[] { side });
        Func<int> server = () => (int)Match.GetProperty("ServerPlayer").GetValue(m);
        Func<int> receiver = () => (int)Match.GetProperty("ReceiverPlayer").GetValue(m);
        Assert.That(server(), Is.EqualTo(0)); Assert.That(receiver(), Is.EqualTo(1));
        point(0); Assert.That(server(), Is.EqualTo(0), "Serving winner changes court, not server");
        Assert.That(receiver(), Is.EqualTo(3));
        point(1); Assert.That(server(), Is.EqualTo(3)); Assert.That(receiver(), Is.EqualTo(0));
        point(0); Assert.That(server(), Is.EqualTo(2)); Assert.That(receiver(), Is.EqualTo(1));
        point(1); Assert.That(server(), Is.EqualTo(1)); Assert.That(receiver(), Is.EqualTo(2));
        string json = UnityEngine.JsonUtility.ToJson(m); m = UnityEngine.JsonUtility.FromJson(json, Match);
        Assert.That(server(), Is.EqualTo(1));
        point(0); Assert.That(server(), Is.EqualTo(0)); Assert.That(receiver(), Is.EqualTo(3));
        for (int i = 0; i < 14; i++) point(0);
        Assert.That((int)Match.GetField("GameNumber").GetValue(m), Is.EqualTo(2));
        Assert.That(server(), Is.EqualTo(0));
        Assert.That(receiver() % 2, Is.EqualTo(1));
    }
    [Test] public void DecidingGameChangesEndsOnlyOnceAtEight()
    {
        var m = Activator.CreateInstance(Match);
        var game = Match.GetField("GameNumber"); game.SetValue(m, 3);
        for (int i = 0; i < 8; i++) Match.GetMethod("AwardPoint").Invoke(m, new object[] { 0 });
        Assert.That((bool)Match.GetField("EndsSwapped").GetValue(m), Is.True);
        string json = UnityEngine.JsonUtility.ToJson(m);
        var restored = UnityEngine.JsonUtility.FromJson(json, Match);
        Match.GetMethod("AwardPoint").Invoke(restored, new object[] { 0 });
        Assert.That((bool)Match.GetField("EndsSwapped").GetValue(restored), Is.True, "Save must retain deciding-end gate");
    }
    [Test] public void ChallengesOnlyConsumeFailedReviewsAndResetPerGame()
    {
        Assert.That(Match.GetMethod("Review"), Is.Not.Null, "Challenge review missing");
        var m = Activator.CreateInstance(Match);
        Func<bool, bool> review = success => (bool)Match.GetMethod("Review").Invoke(m, new object[] { 0, success });
        Assert.That(review(true), Is.True);
        Assert.That(review(false), Is.True);
        Assert.That(review(false), Is.True);
        Assert.That(review(false), Is.False);
        for (int i = 0; i < 15; i++) Match.GetMethod("AwardPoint").Invoke(m, new object[] { 0 });
        Assert.That(review(false), Is.True);
    }
    [Test] public void CompletedGamesKeepExactPointsAcrossResetAndSerialization()
    {
        var history = Match.GetField("GamePoints");
        Assert.That(history, Is.Not.Null, "Completed game scores are discarded on reset");
        foreach (bool doubles in new[] { false, true }) foreach (int winner in new[] { 0, 1 })
        {
            var m = Activator.CreateInstance(Match); Match.GetField("Doubles").SetValue(m, doubles);
            Action<int> point = side => Match.GetMethod("AwardPoint").Invoke(m, new object[] { side });
            var expected = new System.Collections.Generic.List<int>();
            foreach (var score in new[] { new[] { 16, 14 }, new[] { 20, 21 }, winner == 0 ? new[] { 15, 7 } : new[] { 7, 15 } })
            {
                int tied = Math.Min(score[0], score[1]);
                for (int i = 0; i < tied; i++) { point(0); point(1); }
                int side = score[0] > score[1] ? 0 : 1;
                for (int i = tied; i < score[side]; i++) point(side);
                expected.AddRange(score);
                Assert.That((int[])history.GetValue(m), Is.EqualTo(expected.ToArray()));
                string json = UnityEngine.JsonUtility.ToJson(m); m = UnityEngine.JsonUtility.FromJson(json, Match);
                Assert.That((int[])history.GetValue(m), Is.EqualTo(expected.ToArray()), "Save lost completed game scores");
            }
            Assert.That((int)Match.GetField("Winner").GetValue(m), Is.EqualTo(winner));
            Assert.Throws<System.Reflection.TargetInvocationException>(() => point(winner));
            Assert.That((int[])history.GetValue(m), Is.EqualTo(expected.ToArray()), "Rejected point appended history");
        }
    }
    [Test] public void Official15DeuceCapAndBestOfThree()
    {
        Assert.That(Match, Is.Not.Null, "M2 singles scoring missing");
        var m = Activator.CreateInstance(Match);
        Action<int> point = side => Match.GetMethod("AwardPoint").Invoke(m, new object[] { side });
        Func<int[]> games = () => (int[])Match.GetField("Games").GetValue(m);
        for (int i = 0; i < 14; i++) { point(0); point(1); }
        point(0);
        Assert.That(games()[0], Is.Zero, "15-14 must continue");
        point(0);
        Assert.That(games()[0], Is.EqualTo(1));
        Assert.That(((int[])Match.GetField("Points").GetValue(m))[0], Is.Zero);
        for (int i = 0; i < 20; i++) { point(0); point(1); }
        point(1);
        Assert.That(games()[1], Is.EqualTo(1), "21-20 cap");
        for (int i = 0; i < 15; i++) point(0);
        Assert.That((int)Match.GetField("Winner").GetValue(m), Is.Zero);
        var error = Assert.Throws<System.Reflection.TargetInvocationException>(() => point(1));
        Assert.That(error.InnerException, Is.TypeOf<InvalidOperationException>());
    }
}
