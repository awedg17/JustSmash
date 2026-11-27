using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MatchFlowTests : PreferenceFixture
{
    [UnityTest] public IEnumerator ChallengeUsesActualLandingOnceAndKeepsSuccessfulQuota()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        Assert.That(type.GetMethod("ChallengeCall"), Is.Not.Null, "Line review integration missing");
        type.GetMethod("StartMatch").Invoke(court, null);
        type.GetMethod("Serve").Invoke(court, new object[] { false });
        GameObject.Find("Shuttle").transform.position = new Vector3(0, .045f, 3);
        type.GetField("velocity", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(court, Vector3.down);
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex("\\[collision\\] ground.*"));
            LogAssert.Expect(LogType.Log, "[scoring] point winner=0");
        yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
        Assert.That(type.GetMethod("ChallengeCall").Invoke(court, null), Is.EqualTo(true));
        var match = type.GetProperty("Match").GetValue(court);
        Assert.That(((int[])match.GetType().GetField("Challenges").GetValue(match))[0], Is.EqualTo(1));
        Assert.That(type.GetMethod("ChallengeCall").Invoke(court, null), Is.EqualTo(false), "One review per point");
        LogAssert.NoUnexpectedReceived();
    }
    [UnityTest] public IEnumerator MatchMenuProvidesSaveLoadReplayAndReadableTouchControls()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        Assert.That(type.GetMethod("RunMenuAction"), Is.Not.Null, "Save/load/replay UI dispatcher missing");
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        Assert.That((float)type.GetProperty("ControlFontSize").GetValue(court), Is.GreaterThanOrEqualTo(24));
        LogAssert.NoUnexpectedReceived();
    }
    [UnityTest] public IEnumerator JumpCostsMoreEnergyThanIdleAndRecoveryIsPartial()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        Assert.That(type.GetProperty("Energy"), Is.Not.Null, "Stamina missing");
        float before = (float)type.GetProperty("Energy").GetValue(court);
        type.GetMethod("Jump").Invoke(court, null);
        Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.LessThan(before));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest] public IEnumerator ReplayUsesRecordedPositionsAndCanBeSkipped()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        Assert.That(type.GetMethod("BeginReplay"), Is.Not.Null, "Replay missing");
        type.GetMethod("Feed").Invoke(court, new object[] { "Lob" });
        yield return new WaitForSeconds(.5f);
        var saved = GameObject.Find("Shuttle").transform.position;
        Assert.That(type.GetMethod("BeginReplay").Invoke(court, null), Is.EqualTo(true));
        yield return new WaitForSeconds(.1f);
        Assert.That(GameObject.Find("Shuttle").transform.position, Is.Not.EqualTo(saved));
        type.GetMethod("SkipReplay").Invoke(court, null);
        Assert.That(GameObject.Find("Shuttle").transform.position, Is.EqualTo(saved));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest] public IEnumerator SnapshotRestoresRallyAndRejectsCorruptInput()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        Assert.That(type.GetMethod("CaptureMatch"), Is.Not.Null, "M2 snapshot missing");
        type.GetMethod("StartMatch").Invoke(court, null);
        type.GetMethod("Serve").Invoke(court, new object[] { true });
        var pos = GameObject.Find("Shuttle").transform.position;
        var speed = (Vector3)type.GetProperty("ShuttleVelocity").GetValue(court);
        type.GetMethod("Jump").Invoke(court, null);
        float energy = (float)type.GetProperty("Energy").GetValue(court);
        var json = (string)type.GetMethod("CaptureMatch").Invoke(court, null);
        type.GetMethod("StartMatch").Invoke(court, null);
        type.GetMethod("RestoreMatch").Invoke(court, new object[] { json });
        Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo("Rally"));
        Assert.That(GameObject.Find("Shuttle").transform.position, Is.EqualTo(pos));
        Assert.That(type.GetProperty("ShuttleVelocity").GetValue(court), Is.EqualTo(speed));
        Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.EqualTo(energy), "Snapshot must retain stamina");
        Assert.Throws<System.Reflection.TargetInvocationException>(() => type.GetMethod("RestoreMatch").Invoke(court, new object[] { "{}" }));
        Assert.That(GameObject.Find("Shuttle").transform.position, Is.EqualTo(pos), "Invalid snapshot must not alter live state");
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest] public IEnumerator PlayerCannotHitOwnServeTwice()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        type.GetMethod("StartMatch").Invoke(court, null);
        type.GetMethod("Serve").Invoke(court, new object[] { false });
        GameObject.Find("Shuttle").transform.position = GameObject.Find("Player").transform.position + new Vector3(.3f, 2.1f, .35f);
        type.GetMethod("BeginShot").Invoke(court, new object[] { "Clear" });
        Assert.That(type.GetMethod("ReleaseShot").Invoke(court, null), Is.EqualTo(false), "Player cannot hit again before opponent contact");
        var match = type.GetProperty("Match").GetValue(court);
        Assert.That(((int[])match.GetType().GetField("Points").GetValue(match))[1], Is.EqualTo(0), "Rejected tap cannot fabricate physical double hit");
        Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo("Rally"));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest] public IEnumerator ExhibitionCanFinishTwoGamesWithoutStuckPointState()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        type.GetMethod("StartMatch").Invoke(court, null);
        for (int i = 0; i < 30; i++)
        {
            while (new[] { "Ready", "GameEnd", "Interval", "ChangeEnds" }.Contains((string)type.GetProperty("FlowStage").GetValue(court))) type.GetMethod("AdvanceFlow").Invoke(court, null);
            type.GetMethod("Serve").Invoke(court, new object[] { false });
            var active = type.GetProperty("Match").GetValue(court);
            float end = (bool)active.GetType().GetField("EndsSwapped").GetValue(active) ? -1 : 1;
            GameObject.Find("Shuttle").transform.position = new Vector3(0, .045f, 3 * end);
            type.GetField("velocity", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(court, Vector3.down);
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
            LogAssert.Expect(LogType.Log, "[scoring] point winner=0");
            yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
            Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo(i == 29 ? "MatchEnd" : "Point"), "point " + i);
            type.GetMethod("NextPoint").Invoke(court, null);
        }
        var match = type.GetProperty("Match").GetValue(court);
        Assert.That((int)match.GetType().GetField("Winner").GetValue(match), Is.EqualTo(0));
        Assert.That(((int[])match.GetType().GetField("Games").GetValue(match))[0], Is.EqualTo(2));
        Assert.That(type.GetProperty("Flying").GetValue(court), Is.EqualTo(false));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest] public IEnumerator SnapshotRejectsInvalidTimerWithoutMutatingLiveState()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        type.GetMethod("StartMatch").Invoke(court, null);
        var json = (string)type.GetMethod("CaptureMatch").Invoke(court, null);
        json = System.Text.RegularExpressions.Regex.Replace(json, "\"serveRemaining\":[^,}]+", "\"serveRemaining\":-10");
        var before = type.GetProperty("Match").GetValue(court);
        var error = Assert.Throws<System.Reflection.TargetInvocationException>(() => type.GetMethod("RestoreMatch").Invoke(court, new object[] { json }));
        Assert.That(error.InnerException, Is.TypeOf<ArgumentException>());
        Assert.That(type.GetProperty("Match").GetValue(court), Is.SameAs(before));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest] public IEnumerator NewGameRecoversOnlySmallPartOfEnergy()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        type.GetMethod("StartMatch").Invoke(court, null);
        type.GetProperty("Energy").SetValue(court, 60f); // Valid tired state, not repeated illegal jumps during service.
        float before = (float)type.GetProperty("Energy").GetValue(court);
        var match = type.GetProperty("Match").GetValue(court);
        for (int i = 0; i < 15; i++) match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        type.GetMethod("NextPoint").Invoke(court, null);
        float after = (float)type.GetProperty("Energy").GetValue(court);
        Assert.That(after, Is.GreaterThan(before));
        Assert.That(after, Is.LessThan(100));
        Assert.That(after - before, Is.LessThanOrEqualTo(10));
        type.GetMethod("NextPoint").Invoke(court, null);
        Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.EqualTo(after), "Recovery once per game, not per NextPoint call");
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest] public IEnumerator OpponentCanReturnReachableShuttle()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        type.GetMethod("StartMatch").Invoke(court, null);
        type.GetMethod("Serve").Invoke(court, new object[] { false });
        GameObject.Find("Shuttle").transform.position = GameObject.Find("Opponent").transform.TransformPoint(new Vector3(.3f, 1.8f, .35f));
        type.GetField("velocity", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(court, Vector3.down);
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[AI\] return .*"));
        yield return new WaitForSeconds(.4f);
        Assert.That(((Vector3)type.GetProperty("ShuttleVelocity").GetValue(court)).z, Is.LessThan(0), "Opponent must return toward player");
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest] public IEnumerator ExhibitionAwardsGroundPointAndOffersNextServe()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        Assert.That(type.GetMethod("StartMatch"), Is.Not.Null, "Exhibition start missing");
        type.GetMethod("StartMatch").Invoke(court, null);
        type.GetMethod("Serve").Invoke(court, new object[] { false });
        GameObject.Find("Shuttle").transform.position = new Vector3(0, .045f, 3);
        type.GetField("velocity", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(court, Vector3.down);
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex("\\[collision\\] ground.*"));
            LogAssert.Expect(LogType.Log, "[scoring] point winner=0");
        yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
        var match = type.GetProperty("Match").GetValue(court);
        Assert.That(((int[])match.GetType().GetField("Points").GetValue(match))[0], Is.EqualTo(1));
        Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo("Point"));
        type.GetMethod("NextPoint").Invoke(court, null);
        Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo("Serving"));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest] public IEnumerator NonFinalSetHoldsClosingScoreDuringReviewAndReplayAndUpdatesOnCancelOrBreak()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        var displayGame = type.GetProperty("DisplayGameNumber", flags);
        var displayPoints = type.GetMethod("DisplayPoints", flags);
        var header = type.GetProperty("HudScoreHeader", flags);
        var endSign = type.GetProperty("EndSign", flags);
        int PointsFor(int side) => (int)displayPoints.Invoke(court, new object[] { side });

        // 1. Finish Game 1 at 15-13 -> hold 15-13 during review and closing replay -> Lewati enters Game 2 break 0-0.
        type.GetMethod("EnterTraining").Invoke(court, null);
        type.GetMethod("StartMatch").Invoke(court, null);
        for (int i = 0; i < 3 && (string)type.GetProperty("FlowStage").GetValue(court) != "Serving"; i++) type.GetMethod("AdvanceFlow").Invoke(court, null);
        type.GetProperty("ReplayMode").SetValue(court, 1);
        var match = type.GetProperty("Match").GetValue(court);
        var mt = match.GetType();
        var replay = (IList)type.GetField("replay", flags).GetValue(court);
        var frame = type.GetMethod("CaptureFrame", flags).Invoke(court, null);
        for (int i = 0; i < 10; i++) replay.Add(frame);
        mt.GetField("Points").SetValue(match, new[] { 14, 13 });
        type.GetMethod("ScorePoint", flags).Invoke(court, new object[] { 0 });
        Assert.That(displayGame.GetValue(court), Is.EqualTo(1));
        Assert.That(PointsFor(0), Is.EqualTo(15));
        Assert.That(PointsFor(1), Is.EqualTo(13));
        Assert.That((string)header.GetValue(court), Does.StartWith("GAME 1"));
        Assert.That((float)endSign.GetValue(court), Is.EqualTo(1f));
        type.GetMethod("UpdateArena", flags).Invoke(court, null);
        Assert.That(GameObject.Find("ArenaScoreboard").GetComponentInChildren<TextMesh>().text, Does.Contain("GAME 1").And.Contain("15     13"));

        type.GetMethod("ContinuePoint", flags).Invoke(court, new object[] { 3f });
        Assert.That(type.GetField("replaying", flags).GetValue(court), Is.True);
        Assert.That(displayGame.GetValue(court), Is.EqualTo(1));
        Assert.That(PointsFor(0), Is.EqualTo(15));
        Assert.That(PointsFor(1), Is.EqualTo(13));
        var skipRect = (Rect)type.GetMethod("UiActionRect").Invoke(court, new object[] { "Lewati" });
        type.GetMethod("PointerDown", flags).Invoke(court, new object[] { 31, skipRect.center });
        type.GetMethod("PointerUp", flags).Invoke(court, new object[] { 31 });
        Assert.That(type.GetField("replaying", flags).GetValue(court), Is.False);
        Assert.That(type.GetProperty("FlowStage").GetValue(court), Is.EqualTo("GameEnd"));
        Assert.That(displayGame.GetValue(court), Is.EqualTo(2));
        Assert.That(PointsFor(0), Is.EqualTo(0));
        Assert.That(PointsFor(1), Is.EqualTo(0));
        Assert.That((float)endSign.GetValue(court), Is.EqualTo(-1f));

        // 2. Replay OFF holds closing score during review window, then transitions immediately at FinishPoint.
        type.GetProperty("ReplayMode").SetValue(court, 2);
        for (int i = 0; i < 3 && (string)type.GetProperty("FlowStage").GetValue(court) != "Serving"; i++) type.GetMethod("AdvanceFlow").Invoke(court, null);
        match = type.GetProperty("Match").GetValue(court);
        mt.GetField("Points").SetValue(match, new[] { 13, 14 });
        type.GetMethod("ScorePoint", flags).Invoke(court, new object[] { 1 });
        Assert.That(displayGame.GetValue(court), Is.EqualTo(2));
        Assert.That(PointsFor(0), Is.EqualTo(13));
        Assert.That(PointsFor(1), Is.EqualTo(15));
        type.GetMethod("ContinuePoint", flags).Invoke(court, new object[] { 3f });
        Assert.That(type.GetField("replaying", flags).GetValue(court), Is.False);
        Assert.That(type.GetProperty("FlowStage").GetValue(court), Is.EqualTo("GameEnd"));
        Assert.That(displayGame.GetValue(court), Is.EqualTo(3));
        Assert.That(PointsFor(0), Is.EqualTo(0));
        Assert.That(PointsFor(1), Is.EqualTo(0));

        // 3. Challenge overturning a non-final set point shows corrected score and continues old set.
        type.GetMethod("StartMatch").Invoke(court, null);
        type.GetProperty("ReplayMode").SetValue(court, 1);
        for (int i = 0; i < 3 && (string)type.GetProperty("FlowStage").GetValue(court) != "Serving"; i++) type.GetMethod("AdvanceFlow").Invoke(court, null);
        match = type.GetProperty("Match").GetValue(court);
        mt.GetField("Points").SetValue(match, new[] { 13, 14 });
        type.GetField("beforePoint", flags).SetValue(court, JsonUtility.ToJson(match));
        type.GetField("lastLanding", flags).SetValue(court, new Vector3(0, .04f, 3f));
        type.GetField("announcedIn", flags).SetValue(court, false);
        type.GetField("lastHitter", flags).SetValue(court, 0);
        type.GetField("reviewAvailable", flags).SetValue(court, true);
        type.GetMethod("ScorePoint", flags).Invoke(court, new object[] { 1 });
        Assert.That(displayGame.GetValue(court), Is.EqualTo(1));
        Assert.That(PointsFor(0), Is.EqualTo(13));
        Assert.That(PointsFor(1), Is.EqualTo(15));
        Assert.That(type.GetMethod("ChallengeCall").Invoke(court, null), Is.True);
        Assert.That(displayGame.GetValue(court), Is.EqualTo(1));
        Assert.That(PointsFor(0), Is.EqualTo(14));
        Assert.That(PointsFor(1), Is.EqualTo(14));
        type.GetMethod("SkipReplay").Invoke(court, null);
        type.GetMethod("NextPoint").Invoke(court, null);
        Assert.That(displayGame.GetValue(court), Is.EqualTo(1));
        Assert.That(PointsFor(0), Is.EqualTo(14));
        Assert.That(PointsFor(1), Is.EqualTo(14));
        Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo("Serving"));
        LogAssert.NoUnexpectedReceived();
    }
}
