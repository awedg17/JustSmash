using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MobileOpponentTests : PreferenceFixture
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    Type type; UnityEngine.Object court;
    object Call(string name, params object[] args) => type.GetMethod(name).Invoke(court, args);
    void Set(string name, object value) => type.GetField(name, Hidden).SetValue(court, value);
    IEnumerator Boot()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        ((Behaviour)court).enabled = false;
    }
    [UnityTest] public IEnumerator DoublesSeparationUsesMovementBudgetAndKeepsHumanAuthority()
    {
        yield return Boot(); Set("doublesSelected", true);
        var rows = new System.Collections.Generic.List<string> { "end,energy,side,owner,boundary,firstStep,velocityStep,maxStep,separation,humanMoved" };
        var checks = new System.Collections.Generic.List<(float first, float recorded, float maximum, float cap, float distance, float human, string label)>();
        foreach (bool swapped in new[] { false, true }) foreach (float energy in new[] { 100f, 30f })
        foreach (int side in new[] { 0, 1 }) foreach (bool secondOwns in new[] { false, true })
        foreach (Vector2 boundary in new[] { Vector2.zero, Vector2.left, Vector2.right, Vector2.down, Vector2.up, new Vector2(-1, -1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(1, 1) })
        {
            Call("ConfigureOpponent", "Normal", 193); Call("StartMatch");
            var match = type.GetProperty("Match").GetValue(court); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            Set("flowStage", null); Call("Serve", true); Set("serviceFlight", false); Set("lastHitter", 1 - side); Set("lastPlayer", 1 - side);
            float end = swapped ? -1 : 1, cap = 4.5f * Mathf.Lerp(.7f, 1, Mathf.Clamp01(energy / 75));
            Transform Actor(int id) => (Transform)type.GetMethod("Actor", Hidden).Invoke(court, new object[] { id });
            for (int id = 0; id < 4; id++) Actor(id).position = (Vector3)type.GetMethod("FormationTarget", Hidden).Invoke(court, new object[] { id });
            int owner = side + (secondOwns ? 2 : 0), yielding = side == 0 ? 2 : owner ^ 2;
            var anchor = Actor(owner).position; anchor.x = boundary.x * 3.2f;
            float courtSide = (side == 0 ? -1 : 1) * end;
            anchor.z = (boundary.y < 0 ? .5f : boundary.y > 0 ? 7 : 3.6f) * courtSide;
            Actor(side).position = Actor(side + 2).position = anchor;
            Actor(yielding).position += new Vector3(boundary.x, 0, boundary.y * courtSide) * .1f;
            var human = Actor(0).position;
            type.GetProperty("OpponentEnergy").SetValue(court, energy); Set("partnerEnergy", energy); Set("secondEnergy", energy);
            GameObject.Find("Shuttle").transform.position = new Vector3(0, 20, (side == 0 ? -3 : 3) * end); Set("velocity", Vector3.up);
            Set(side == 0 ? "nearOwner" : "farOwner", owner);
            float stamp = (float)type.GetField(side == 0 ? "aiContactAt" : "swingAt", Hidden).GetValue(court); Set("teamObserved", stamp);
            var targets = (Vector3[])type.GetField("teamTargets", Hidden).GetValue(court);
            var thinks = (double[])type.GetField("teamThink", Hidden).GetValue(court); var refresh = (double[])type.GetField("teamRefresh", Hidden).GetValue(court);
            for (int id = 0; id < 4; id++) { targets[id] = Actor(id).position; thinks[id] = 0; refresh[id] = double.MaxValue; }
            var motion = (Vector3[])type.GetField("athleteVelocity", Hidden).GetValue(court);
            var rng = type.GetField("opponentRandom", Hidden).GetValue(court); float first = 0, recorded = 0, maximum = 0;
            for (int tick = 0; tick < 40; tick++)
            {
                Vector3 before = Actor(yielding).position;
                type.GetMethod("UpdateDoublesAt", Hidden).Invoke(court, new object[] { Time.timeAsDouble + tick * (double)Time.fixedDeltaTime });
                float step = Vector3.Distance(before, Actor(yielding).position); maximum = Mathf.Max(maximum, step);
                if (tick == 0) { first = step; recorded = motion[yielding].magnitude * Time.fixedDeltaTime; }
            }
            float separation = Vector3.Distance(Actor(side).position, Actor(side + 2).position), humanMoved = Vector3.Distance(human, Actor(0).position);
            string label = end + "/" + energy + "/" + side + "/" + owner + "/" + boundary;
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", end, energy, side, owner, boundary.x + "/" + boundary.y, first, recorded, maximum, separation, humanMoved));
            checks.Add((first, recorded, maximum, cap, separation, humanMoved, label));
            Assert.That(type.GetField("opponentRandom", Hidden).GetValue(court), Is.EqualTo(rng));
            Assert.That(type.GetField(side == 0 ? "nearOwner" : "farOwner", Hidden).GetValue(court), Is.EqualTo(owner));
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/doubles-separation.csv", rows);
        Assert.That(checks.Count, Is.EqualTo(144));
        foreach (var check in checks)
        {
            Assert.That(check.first, Is.EqualTo(check.recorded).Within(.00001f), check.label + " correction bypasses recorded momentum");
            Assert.That(check.first, Is.LessThanOrEqualTo(check.cap * 10 * Time.fixedDeltaTime * Time.fixedDeltaTime + .00001f), check.label + " first step exceeds acceleration");
            Assert.That(check.maximum, Is.LessThanOrEqualTo(check.cap * Time.fixedDeltaTime + .00001f), check.label + " exceeds speed cap");
            Assert.That(check.distance, Is.GreaterThanOrEqualTo(.55f), check.label + " persistent overlap");
            Assert.That(check.human, Is.Zero, check.label + " bot moved Manual human");
        }
    }
    [UnityTest] public IEnumerator SinglesRecoveryUsesSameFatigueAsChasingAtBothEnds()
    {
        yield return Boot();
        var opponent = GameObject.Find("Opponent").transform;
        var rows = new System.Collections.Generic.List<string> { "difficulty,end,energy,phase,moved,expected" };
        var checks = new System.Collections.Generic.List<(float moved, float expected, string label)>();
        foreach (string difficulty in new[] { "Easy", "Normal", "Hard" }) foreach (bool swapped in new[] { false, true })
        foreach (float energy in new[] { 100f, 30f }) foreach (bool recovering in new[] { false, true })
        {
            Call("ConfigureOpponent", difficulty, 193); Call("StartMatch");
            var match = type.GetProperty("Match").GetValue(court);
            match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint"); Call("AdvanceFlow"); Call("Serve", true);
            float end = swapped ? -1 : 1;
            var start = new Vector3(1, 0, 5.8f * end); opponent.position = start;
            GameObject.Find("Shuttle").transform.position = new Vector3(0, 8, -3 * end); Set("velocity", Vector3.up);
            type.GetProperty("OpponentEnergy").SetValue(court, energy);
            Set("lastHitter", recovering ? 1 : 0);
            Set("observedSwing", type.GetField("swingAt", Hidden).GetValue(court));
            Set("opponentThinkAt", Time.time - 1); Set("opponentRefreshAt", float.MaxValue);
            Set("opponentTarget", new Vector3(0, 0, 3.8f * end));
            var rng = type.GetField("opponentRandom", Hidden).GetValue(court);
            type.GetMethod("UpdateOpponent", Hidden).Invoke(court, null);
            float moved = Vector3.Distance(start, opponent.position);
            float expected = 4.5f * (10 * Time.fixedDeltaTime) * Time.fixedDeltaTime * Mathf.Lerp(.7f, 1, Mathf.Clamp01(energy / 100 / .75f));
            string label = difficulty + "/" + end + "/" + energy + "/" + (recovering ? "recovery" : "chase");
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5}", difficulty, end, energy, recovering ? "recovery" : "chase", moved, expected));
            checks.Add((moved, expected, label));
            Assert.That(type.GetProperty("OpponentEnergy").GetValue(court), Is.EqualTo(energy), "Movement must not spend energy");
            Assert.That(type.GetField("opponentRandom", Hidden).GetValue(court), Is.EqualTo(rng));
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/fatigue-movement.csv", rows);
        Assert.That(checks.Count, Is.EqualTo(24));
        foreach (var check in checks) Assert.That(check.moved, Is.EqualTo(check.expected).Within(.00001f), check.label);
    }
    [UnityTest] public IEnumerator SinglesAndDoublesBotsShareAccelerationAndFatigueAtBothEnds()
    {
        yield return Boot();
        var rows = new System.Collections.Generic.List<string> { "doubles,difficulty,end,energy,actor,tick,moved,expected" };
        var checks = new System.Collections.Generic.List<(float moved, float expected, string label)>();
        foreach (bool doubles in new[] { false, true }) foreach (string difficulty in new[] { "Easy", "Normal", "Hard" })
        foreach (bool swapped in new[] { false, true }) foreach (float energy in new[] { 100f, 30f })
        {
            Set("doublesSelected", doubles); Call("ConfigureOpponent", difficulty, 193); Call("StartMatch");
            var match = type.GetProperty("Match").GetValue(court); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            Set("flowStage", null); Call("Serve", true); Set("serviceFlight", false); Set("lastHitter", 0); Set("lastPlayer", 0);
            float end = swapped ? -1 : 1, cap = 4.5f * Mathf.Lerp(.7f, 1, Mathf.Clamp01(energy / 75));
            GameObject.Find("Player").transform.position = new Vector3(2.5f * end, 0, -6 * end);
            GameObject.Find("Shuttle").transform.position = new Vector3(0, 20, -3 * end); Set("velocity", Vector3.up);
            type.GetProperty("OpponentEnergy").SetValue(court, energy); Set("partnerEnergy", energy); Set("secondEnergy", energy);
            float stamp = (float)type.GetField("swingAt", Hidden).GetValue(court); Set("observedSwing", stamp); Set("teamObserved", stamp);
            Set("opponentThinkAt", Time.time - 1); Set("opponentRefreshAt", float.MaxValue);
            var targets = (Vector3[])type.GetField("teamTargets", Hidden).GetValue(court);
            var thinks = (double[])type.GetField("teamThink", Hidden).GetValue(court); var refresh = (double[])type.GetField("teamRefresh", Hidden).GetValue(court);
            for (int id = 1; id < (doubles ? 4 : 2); id++)
            {
                var target = doubles ? (Vector3)type.GetMethod("FormationTarget", Hidden).Invoke(court, new object[] { id }) : new Vector3(0, 0, 3.8f * end);
                targets[id] = target; thinks[id] = Time.timeAsDouble - 1; refresh[id] = double.MaxValue;
                var actor = (Transform)type.GetMethod("Actor", Hidden).Invoke(court, new object[] { id }); actor.position = target + Vector3.right * 1.2f * end;
                if (id == 1) Set("opponentTarget", target);
            }
            Set("farOwner", 1);
            var rng = type.GetField("opponentRandom", Hidden).GetValue(court);
            for (int tick = 0; tick < 10; tick++)
            {
                var before = new Vector3[4];
                for (int id = 1; id < (doubles ? 4 : 2); id++) before[id] = ((Transform)type.GetMethod("Actor", Hidden).Invoke(court, new object[] { id })).position;
                type.GetMethod("UpdateOpponent", Hidden).Invoke(court, null);
                for (int id = 1; id < (doubles ? 4 : 2); id++)
                {
                    float moved = Vector3.Distance(before[id], ((Transform)type.GetMethod("Actor", Hidden).Invoke(court, new object[] { id })).position);
                    float expected = cap * Mathf.Min(1, (tick + 1) * 10 * Time.fixedDeltaTime) * Time.fixedDeltaTime;
                    string label = doubles + "/" + difficulty + "/" + end + "/" + energy + "/" + id + "/" + tick;
                    rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6},{7}", doubles,difficulty,end,energy,id,tick,moved,expected));
                    checks.Add((moved, expected, label));
                }
            }
            Assert.That(type.GetField("opponentRandom", Hidden).GetValue(court), Is.EqualTo(rng));
            Assert.That(type.GetProperty("OpponentEnergy").GetValue(court), Is.EqualTo(energy));
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/bot-acceleration.csv", rows);
        Assert.That(checks.Count, Is.EqualTo(480));
        foreach (var check in checks) Assert.That(check.moved, Is.EqualTo(check.expected).Within(.00001f), check.label);
    }
    [UnityTest] public IEnumerator ReactionWaitBrakesExistingMomentumWithoutReadingFlight()
    {
        yield return Boot(); var actor = GameObject.Find("Opponent").transform;
        foreach (bool swapped in new[] { false, true }) foreach (float energy in new[] { 100f, 30f })
        {
            Call("ConfigureOpponent", "Normal", 193); Call("StartMatch"); var match = type.GetProperty("Match").GetValue(court);
            match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint"); Set("flowStage", null); Call("Serve", true);
            Set("serviceFlight", false); Set("lastHitter", 1); float end = swapped ? -1 : 1;
            actor.position = new Vector3(2 * end, 0, 3.8f * end); type.GetProperty("OpponentEnergy").SetValue(court, energy);
            GameObject.Find("Shuttle").transform.position = new Vector3(0, 20, -3 * end); Set("velocity", Vector3.up);
            var update = type.GetMethod("UpdateOpponent", Hidden); for (int i = 0; i < 6; i++) update.Invoke(court, null);
            Set("lastHitter", 0); Set("observedSwing", -100f); var rng = type.GetField("opponentRandom", Hidden).GetValue(court);
            Vector3 before = actor.position; update.Invoke(court, null);
            float step = Vector3.Distance(before, actor.position), cap = 4.5f * Mathf.Lerp(.7f, 1, Mathf.Clamp01(energy / 75));
            Assert.That(step, Is.InRange(.001f, cap * Time.fixedDeltaTime - .001f), "Reaction wait snaps moving AI to rest");
            Assert.That((actor.position.x - before.x) * end, Is.LessThan(0), "Reaction wait must only brake previous direction");
            for (int i = 0; i < 10; i++) update.Invoke(court, null);
            var momentum = (Vector3[])type.GetField("athleteVelocity", Hidden).GetValue(court);
            Assert.That(momentum[1], Is.EqualTo(Vector3.zero));
            Assert.That(type.GetField("opponentRandom", Hidden).GetValue(court), Is.EqualTo(rng));
        }
    }
    [UnityTest] public IEnumerator SinglesUseNetOnlyForReachableForecourtSpaceAndPreserveEasy()
    {
        yield return Boot();
        var rows = new System.Collections.Generic.List<string> { "difficulty,end,defenderDepth,actorDepth,height,expected,actual" };
        var errors = new System.Collections.Generic.List<string>();
        foreach (string difficulty in new[] { "Easy", "Normal", "Hard" }) foreach (bool swapped in new[] { false, true })
        foreach (float defenderDepth in new[] { 2f, 5.5f }) foreach (float actorDepth in new[] { 1.2f, 5.5f }) foreach (float height in new[] { .7f, 1.1f })
        {
            Call("ConfigureOpponent", difficulty, 193); Call("StartMatch"); var match = type.GetProperty("Match").GetValue(court);
            match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            Set("flowStage", null); Call("Serve", true); Set("serviceFlight", false); Set("lastHitter", 0);
            float end = swapped ? -1 : 1;
            var actor = GameObject.Find("Opponent").transform; actor.position = new Vector3(.5f * end, 0, actorDepth * end);
            GameObject.Find("Player").transform.position = new Vector3(-1.5f * end, 0, -defenderDepth * end);
            var shuttle = GameObject.Find("Shuttle").transform; shuttle.position = actor.TransformPoint(new Vector3(.3f, height, .35f)); Set("velocity", Vector3.down);
            Set("observedSwing", type.GetField("swingAt", Hidden).GetValue(court)); Set("opponentThinkAt", Time.time - 1);
            Set("opponentRefreshAt", float.MaxValue); Set("opponentTarget", actor.position); Set("aiContactAt", Time.time - 1);
            float energy = (float)type.GetProperty("OpponentEnergy").GetValue(court); Vector3 before = actor.position;
            type.GetMethod("UpdateOpponent", Hidden).Invoke(court, null);
            Assert.That(actor.position, Is.EqualTo(before)); Assert.That((int)type.GetField("lastHitter", Hidden).GetValue(court), Is.EqualTo(1));
            Assert.That((float)type.GetProperty("OpponentEnergy").GetValue(court), Is.EqualTo(energy - 1));
            string actual = (string)type.GetField("opponentStroke", Hidden).GetValue(court);
            string expected = difficulty != "Easy" && actorDepth < 2 && defenderDepth > 4 && height >= 1 ? "Net" : "Lift";
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6}", difficulty,end,defenderDepth,actorDepth,height,expected,actual));
            if (actual != expected) errors.Add(rows[rows.Count - 1]);
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/ai-net-selection.csv", rows); Assert.That(rows.Count - 1, Is.EqualTo(48));
        Assert.That(errors, Is.Empty, "Contextual shot mismatch: " + string.Join("; ", errors));
    }
    [UnityTest] public IEnumerator SinglesNetChoiceProducesLowLegalFlightWithoutReachRescue()
    {
        yield return Boot();
        var rows = new System.Collections.Generic.List<string> { "difficulty,seed,end,actorX,depth,height,apex,clearance,landingDepth,predictionError" };
        var errors = new System.Collections.Generic.List<string>();
        var actor = GameObject.Find("Opponent").transform; var shuttle = GameObject.Find("Shuttle").transform;
        var marker = (Transform)type.GetField("landingShadow", Hidden).GetValue(court); var tick = type.GetMethod("FixedUpdate", Hidden);
        foreach (string difficulty in new[] { "Normal", "Hard" }) foreach (int seed in new[] { 71, 193 }) foreach (bool swapped in new[] { false, true })
        foreach (float x in new[] { -2f, 0f, 2f }) foreach (float depth in new[] { .9f, 1.2f, 1.9f }) foreach (float height in new[] { 1f, 1.2f, 1.44f })
        {
            Call("ConfigureOpponent", difficulty, seed); Call("StartMatch"); var match = type.GetProperty("Match").GetValue(court);
            match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint"); Set("flowStage", null); Call("Serve", true);
            Set("serviceFlight", false); Set("lastHitter", 0); Set("assisted", false); Call("SetAnalog", Vector2.zero);
            float end = swapped ? -1 : 1; actor.position = new Vector3(x * end, 0, depth * end);
            GameObject.Find("Player").transform.position = new Vector3(-1.5f * end, 0, -5.5f * end);
            shuttle.position = actor.TransformPoint(new Vector3(.3f, height, .35f)); Set("velocity", Vector3.down);
            Set("observedSwing", type.GetField("swingAt", Hidden).GetValue(court)); Set("opponentThinkAt", Time.time - 1);
            Set("opponentRefreshAt", float.MaxValue); Set("opponentTarget", actor.position); Set("aiContactAt", Time.time - 1);
            var beforeActor = actor.position; type.GetMethod("UpdateOpponent", Hidden).Invoke(court, null);
            Assert.That(actor.position, Is.EqualTo(beforeActor)); Assert.That(type.GetField("opponentStroke", Hidden).GetValue(court), Is.EqualTo("Net"));
            var predicted = marker.position; type.GetProperty("MatchState").SetValue(court, "Training");
            float apex = height, clearance = float.NaN; int ticks = 0;
            while ((bool)type.GetProperty("Flying").GetValue(court) && ticks++ < 200)
            {
                var before = shuttle.position; tick.Invoke(court, null); var after = shuttle.position; apex = Mathf.Max(apex, after.y);
                if (before.z * end > 0 && after.z * end <= 0)
                {
                    var crossing = Vector3.Lerp(before, after, -before.z / (after.z - before.z));
                    clearance = crossing.y - (1.524f + .026f * Mathf.Pow(crossing.x / 3.05f, 2));
                }
            }
            float landing = -shuttle.position.z * end, error = Vector2.Distance(new Vector2(predicted.x, predicted.z), new Vector2(shuttle.position.x, shuttle.position.z));
            string row = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", difficulty,seed,end,x,depth,height,apex,clearance,landing,error);
            rows.Add(row);
            if ((bool)type.GetProperty("Flying").GetValue(court) || !float.IsFinite(clearance) || clearance <= .02f || clearance >= .7f || apex > 2.3f || landing <= 0 || landing >= 2 || Mathf.Abs(shuttle.position.x) > 2.59f || error >= .0001f) errors.Add(row);
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/ai-net-flight.csv", rows); Assert.That(rows.Count - 1, Is.EqualTo(216));
        Assert.That(errors, Is.Empty, "AI Net envelope: " + string.Join("; ", errors));
    }
    [UnityTest] public IEnumerator BotDecisionsIgnoreUncommittedHumanInputAcrossFormatsAndEnds()
    {
        yield return Boot();
        var rows = new System.Collections.Generic.List<string> { "doubles,difficulty,end,depth,phase,inputVariants" };
        foreach (bool doubles in new[] { false, true }) foreach (string difficulty in new[] { "Easy", "Normal", "Hard" })
        foreach (bool swapped in new[] { false, true }) foreach (float depth in new[] { 1.2f, 3f }) foreach (int phase in new[] { 0, 1, 2 })
        {
            Vector3[] expectedPositions = null, expectedTargets = null; Vector3 expectedFlight = Vector3.zero;
            uint expectedRandom = 0; int expectedHitter = -1; string expectedShot = null;
            foreach (int variant in new[] { 0, 1, 2 })
            {
                Set("doublesSelected", doubles); Call("ConfigureOpponent", difficulty, 193); Call("StartMatch");
                var match = type.GetProperty("Match").GetValue(court); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
                Set("flowStage", null); Call("Serve", true); Set("serviceFlight", false); Set("lastHitter", 0); Set("lastPlayer", 0);
                float end = swapped ? -1 : 1;
                var opponent = GameObject.Find("Opponent").transform; opponent.position = new Vector3(1, 0, depth * end);
                GameObject.Find("Player").transform.position = new Vector3(-1, 0, -5 * end);
                if (doubles) GameObject.Find("OpponentPartner").transform.position = new Vector3(-2.5f, 0, 6 * end);
                var shuttle = GameObject.Find("Shuttle").transform;
                shuttle.position = opponent.TransformPoint(new Vector3(.3f, phase == 2 ? 1.2f : 4, .35f)); Set("velocity", Vector3.down);
                float stamp = (float)type.GetField("swingAt", Hidden).GetValue(court);
                Set("observedSwing", stamp); Set("teamObserved", stamp); Set("opponentTarget", opponent.position);
                Set("opponentThinkAt", phase == 0 ? Time.time + 1 : Time.time - 1);
                Set("opponentRefreshAt", phase == 1 ? Time.time - 1 : float.MaxValue); Set("aiContactAt", Time.time - 1);
                var think = (double[])type.GetField("teamThink", Hidden).GetValue(court);
                var refresh = (double[])type.GetField("teamRefresh", Hidden).GetValue(court);
                var targets = (Vector3[])type.GetField("teamTargets", Hidden).GetValue(court);
                for (int id = 1; id < 4; id++) { think[id] = phase == 0 ? Time.timeAsDouble + 1 : Time.timeAsDouble - 1; refresh[id] = phase == 1 ? Time.timeAsDouble - 1 : double.MaxValue; targets[id] = opponent.position; }
                Set("farOwner", 1);
                Call("SetAnalog", variant == 0 ? Vector2.left : variant == 1 ? Vector2.right : Vector2.down);
                Set("shotAim", variant == 0 ? Vector2.one : -Vector2.one); Set("stroke", variant == 0 ? "Smash" : variant == 1 ? "Net" : "Lift");
                Set("jumpHeld", variant == 1); Set("charging", variant == 2);
                type.GetMethod("UpdateOpponent", Hidden).Invoke(court, null);
                var positions = Enumerable.Range(1, doubles ? 3 : 1).Select(id => ((Transform)type.GetMethod("Actor", Hidden).Invoke(court, new object[] { id })).position).ToArray();
                var observed = doubles ? (Vector3[])targets.Clone() : new[] { (Vector3)type.GetField("opponentTarget", Hidden).GetValue(court) };
                var flight = (Vector3)type.GetProperty("ShuttleVelocity").GetValue(court); uint rng = (uint)type.GetField("opponentRandom", Hidden).GetValue(court);
                int hitter = (int)type.GetField("lastHitter", Hidden).GetValue(court); string shot = (string)type.GetField("opponentStroke", Hidden).GetValue(court);
                if (phase < 2) Assert.That(hitter, Is.Zero, "Waiting or unreachable ball cannot be hit");
                else Assert.That(hitter, Is.EqualTo(1), "Fixture must exercise legal real return");
                if (variant == 0) { expectedPositions = positions; expectedTargets = observed; expectedFlight = flight; expectedRandom = rng; expectedHitter = hitter; expectedShot = shot; }
                else
                {
                    Assert.That(positions, Is.EqualTo(expectedPositions), "Input changed AI movement"); Assert.That(observed, Is.EqualTo(expectedTargets), "Input changed observed target");
                    Assert.That(flight, Is.EqualTo(expectedFlight)); Assert.That(rng, Is.EqualTo(expectedRandom)); Assert.That(hitter, Is.EqualTo(expectedHitter)); Assert.That(shot, Is.EqualTo(expectedShot));
                }
            }
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},3", doubles,difficulty,swapped ? -1 : 1,depth,phase));
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/ai-input-independence.csv", rows); Assert.That(rows.Count - 1, Is.EqualTo(72));
    }
    [UnityTest] public IEnumerator DifficultyDelaysReactionAndSeedControlsRealReturn()
    {
        Vector3 first = Vector3.zero;
        foreach (string difficulty in new[] { "Easy", "Normal", "Hard", "Normal" })
        {
            yield return Boot();
            Assert.That(type.GetMethod("ConfigureOpponent"), Is.Not.Null, "Difficulty is only a UI label");
            Call("ConfigureOpponent", difficulty, 193);
            Call("StartMatch"); Call("Serve", true);
            var opponent = GameObject.Find("Opponent").transform;
            GameObject.Find("Shuttle").transform.position = opponent.TransformPoint(new Vector3(.3f, 1.8f, .35f));
            Set("velocity", Vector3.down);
            var start = opponent.position;
            type.GetMethod("UpdateOpponent", Hidden).Invoke(court, null);
            Assert.That(opponent.position, Is.EqualTo(start), difficulty + " read future shot before reaction delay");
            Assert.That((int)type.GetField("lastHitter", Hidden).GetValue(court), Is.Zero);
            yield return new WaitForSeconds(.31f);
            type.GetMethod("UpdateOpponent", Hidden).Invoke(court, null);
            Assert.That(Vector3.Distance(opponent.position, start), Is.LessThanOrEqualTo(4.5f * Time.fixedDeltaTime + .001f));
            Assert.That((int)type.GetField("lastHitter", Hidden).GetValue(court), Is.EqualTo(1), difficulty + " failed reachable descending return");
            var velocity = (Vector3)type.GetProperty("ShuttleVelocity").GetValue(court);
            Assert.That(velocity.z, Is.LessThan(0));
            if (difficulty == "Normal")
            {
                if (first == Vector3.zero) first = velocity;
                else Assert.That(velocity, Is.EqualTo(first), "Same seed/state must produce same decision");
            }
        }
    }
    [UnityTest] public IEnumerator AiPredictionUsesActualNetDeflectionInBothDirections()
    {
        yield return Boot();
        var shuttle = GameObject.Find("Shuttle").transform;
        var opponent = GameObject.Find("Opponent").transform;
        var profileType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.OpponentProfile")).First(t => t != null);
        var profile = profileType.GetMethod("For").Invoke(null, new object[] { "Easy" });
        var rows = new System.Collections.Generic.List<string> { "end,clearance,singlesExpectedZ,singlesPredictedZ,teamExpectedZ,teamPredictedZ" };
        foreach (bool swapped in new[] { false, true }) foreach (float height in new[] { 1f, 2f })
        {
            Call("ConfigureOpponent", "Easy", 71); Call("StartMatch");
            var match = type.GetProperty("Match").GetValue(court); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            float end = swapped ? -1 : 1;
            var start = new Vector3(1, height, -.2f * end); var launch = new Vector3(0, 1, 8 * end);
            shuttle.position = start; Set("velocity", launch); Set("flying", true); Set("flowStage", "Ready");
            type.GetProperty("MatchState").SetValue(court, "Training");
            type.GetMethod("ObserveFlight", Hidden).Invoke(court, new[] { profile });
            var singles = (Vector3)type.GetField("opponentTarget", Hidden).GetValue(court);
            var team = (Vector3)type.GetMethod("PredictTeamContact", Hidden).Invoke(court, new object[] { 1 });
            var expectedSingles = start; var expectedTeam = start; bool singleDone = false, teamDone = false;
            for (int i = 0; i < 100 && (bool)type.GetProperty("Flying").GetValue(court); i++)
            {
                type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
                var point = shuttle.position; var velocity = (Vector3)type.GetProperty("ShuttleVelocity").GetValue(court);
                if (i < 65 && !singleDone && point.z * end >= .35f)
                {
                    expectedSingles = point;
                    float distance = Vector2.Distance(new Vector2(opponent.position.x, opponent.position.z), new Vector2(point.x, point.z));
                    singleDone = velocity.y < 0 && point.y < 2.2f && point.y > .6f && distance < 4.5f * (i + 1) * Time.fixedDeltaTime + .5f;
                }
                if (!teamDone && point.z * end >= .25f) { expectedTeam = point; teamDone = velocity.y < 0 && point.y <= 1.8f; }
            }
            expectedSingles -= opponent.right * .30f + opponent.forward * .35f;
            expectedSingles.z = Mathf.Clamp(expectedSingles.z * end, .35f, 7.1f) * end;
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5}", end,height,expectedSingles.z,singles.z,expectedTeam.z,team.z));
            System.IO.File.WriteAllLines(EvidenceDirectory + "/ai-net-prediction.csv", rows);
            Assert.That(singles.z, Is.EqualTo(expectedSingles.z).Within(.0001f), "Singles must not chase a fictitious flight through net");
            Assert.That(team.z, Is.EqualTo(expectedTeam.z).Within(.0001f), "Doubles must use same net deflection as actual flight");
        }
    }
    [UnityTest] public IEnumerator ReachableSinglesReturnIgnoresPriorImpossibleCandidates()
    {
        yield return Boot();
        foreach (string difficulty in new[] { "Easy", "Normal", "Hard" }) foreach (bool swapped in new[] { false, true })
        foreach (float height in new[] { 1f, 2.1f })
        {
            Vector3 expectedVelocity = Vector3.zero; string expectedShot = null; uint expectedRandom = 0;
            foreach (bool priorImpossible in new[] { false, true })
            {
                Call("ConfigureOpponent", difficulty, 71); Call("StartMatch");
                var match = type.GetProperty("Match").GetValue(court); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
                Set("flowStage", null); Set("flying", true); Set("lastHitter", 0); Set("serviceFlight", false);
                type.GetProperty("MatchState").SetValue(court, "Rally");
                Set("observedSwing", type.GetField("swingAt", Hidden).GetValue(court));
                Set("opponentThinkAt", Time.time - 1); Set("opponentRefreshAt", Time.time + 1); Set("aiContactAt", Time.time - 1);
                var root = GameObject.Find("Opponent").transform; var origin = root.position; Set("opponentTarget", origin);
                var shuttle = GameObject.Find("Shuttle").transform;
                if (priorImpossible)
                {
                    shuttle.position = root.TransformPoint(new Vector3(.3f, 3.65f, .35f)); Set("velocity", Vector3.down);
                    uint before = (uint)type.GetField("opponentRandom", Hidden).GetValue(court);
                    type.GetMethod("UpdateOpponent", Hidden).Invoke(court, null);
                    Assert.That((int)type.GetField("lastHitter", Hidden).GetValue(court), Is.Zero);
                    Assert.That((uint)type.GetField("opponentRandom", Hidden).GetValue(court), Is.EqualTo(before), difficulty + " impossible singles contact consumed RNG");
                    root.position = origin;
                }
                shuttle.position = root.TransformPoint(new Vector3(.3f, height, .35f)); Set("velocity", Vector3.down);
                float instant = Time.time; type.GetMethod("UpdateOpponent", Hidden).Invoke(court, null);
                Assert.That((int)type.GetField("lastHitter", Hidden).GetValue(court), Is.EqualTo(1)); Assert.That(Time.time, Is.EqualTo(instant));
                var velocity = (Vector3)type.GetProperty("ShuttleVelocity").GetValue(court);
                string shot = (string)type.GetField("opponentStroke", Hidden).GetValue(court);
                uint random = (uint)type.GetField("opponentRandom", Hidden).GetValue(court);
                if (!priorImpossible) { expectedVelocity = velocity; expectedShot = shot; expectedRandom = random; }
                else { Assert.That(velocity, Is.EqualTo(expectedVelocity)); Assert.That(shot, Is.EqualTo(expectedShot)); Assert.That(random, Is.EqualTo(expectedRandom)); }
            }
        }
    }
    [UnityTest] public IEnumerator UnreachableDoublesCandidateDoesNotConsumeShotRandomness()
    {
        yield return Boot(); Call("ConfigureDiscipline", true); Call("ConfigureOpponent", "Easy", 71);
        Call("StartMatch"); Call("Serve", true);
        var root = GameObject.Find("Opponent").transform; root.position = new Vector3(1.2f, 0, 2.7f);
        GameObject.Find("OpponentPartner").transform.position = new Vector3(-2.7f, 0, 6);
        Set("serviceFlight", false); Set("farOwner", 1);
        Set("teamObserved", type.GetField("swingAt", Hidden).GetValue(court));
        var think = (double[])type.GetField("teamThink", Hidden).GetValue(court);
        var refresh = (double[])type.GetField("teamRefresh", Hidden).GetValue(court);
        var targets = (Vector3[])type.GetField("teamTargets", Hidden).GetValue(court);
        for (int id = 1; id < 4; id++) { think[id] = Time.time - 1; refresh[id] = Time.time + 1; }
        targets[1] = root.position;
        var shuttle = GameObject.Find("Shuttle").transform;
        shuttle.position = root.TransformPoint(new Vector3(.3f, 3.64183021f, .25f)); Set("velocity", Vector3.down);
        var visual = root.Find("AthleteVisual").GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
        Assert.That(visual.GetType().GetMethod("CanReach").Invoke(visual, new object[] { shuttle.position, "Clear" }), Is.EqualTo(false));
        uint before = (uint)type.GetField("opponentRandom", Hidden).GetValue(court);
        float contactTime = (float)type.GetField("aiContactAt", Hidden).GetValue(court);
        type.GetMethod("UpdateDoubles", Hidden).Invoke(court, null);
        uint after = (uint)type.GetField("opponentRandom", Hidden).GetValue(court);
        TestContext.WriteLine("Impossible contact random=" + before + " -> " + after);
        Assert.That((int)type.GetField("lastPlayer", Hidden).GetValue(court), Is.EqualTo(0));
        Assert.That((float)type.GetField("aiContactAt", Hidden).GetValue(court), Is.EqualTo(contactTime));
        Assert.That(after, Is.EqualTo(before), "An impossible contact must not consume the next shot decision");
    }
    [UnityTest] public IEnumerator ReachableDoublesReturnIgnoresPriorImpossibleCandidates()
    {
        yield return Boot(); Call("ConfigureDiscipline", true);
        int cases = 0;
        foreach (string difficulty in new[] { "Easy", "Normal", "Hard" })
        foreach (int id in new[] { 1, 2, 3 })
        foreach (bool swapped in new[] { false, true })
        foreach (float height in new[] { 1f, 2.1f })
        {
            Vector3 expectedVelocity = Vector3.zero; string expectedShot = null; uint expectedRandom = 0;
            foreach (bool priorImpossible in new[] { false, true })
            {
                Call("ConfigureOpponent", difficulty, 71); Call("StartMatch");
                var match = type.GetProperty("Match").GetValue(court);
                match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
                Set("flowStage", null); Call("Serve", true);
                int side = id % 2;
                Set("lastPlayer", 1 - side); Set("lastHitter", 1 - side); Set("serviceFlight", false);
                Set(side == 0 ? "nearOwner" : "farOwner", id);
                string priorStamp = side == 0 ? "aiContactAt" : "swingAt";
                Set("teamObserved", type.GetField(priorStamp, Hidden).GetValue(court));
                var think = (double[])type.GetField("teamThink", Hidden).GetValue(court);
                var refresh = (double[])type.GetField("teamRefresh", Hidden).GetValue(court);
                var targets = (Vector3[])type.GetField("teamTargets", Hidden).GetValue(court);
                for (int n = 1; n < 4; n++) { think[n] = Time.time - 1; refresh[n] = Time.time + 1; }
                var names = new[] { "Player", "Opponent", "Partner", "OpponentPartner" };
                var roots = names.Select(n => GameObject.Find(n).transform).ToArray();
                var positions = roots.Select(t => t.position).ToArray(); targets[id] = roots[id].position;
                var shuttle = GameObject.Find("Shuttle").transform;
                var visual = roots[id].Find("AthleteVisual").GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
                if (priorImpossible)
                {
                    shuttle.position = roots[id].TransformPoint(new Vector3(.3f, 3.65f, .25f)); Set("velocity", Vector3.down);
                    type.GetMethod("UpdateDoubles", Hidden).Invoke(court, null);
                    for (int n = 0; n < roots.Length; n++) roots[n].position = positions[n];
                }
                shuttle.position = roots[id].TransformPoint(new Vector3(.3f, height, .25f)); Set("velocity", Vector3.down);
                Assert.That(visual.GetType().GetMethod("CanReach").Invoke(visual, new object[] { shuttle.position, height >= 1.45f ? "Clear" : "Lift" }), Is.EqualTo(true));
                float instant = Time.time;
                type.GetMethod("UpdateDoubles", Hidden).Invoke(court, null);
                Assert.That((int)type.GetField("lastPlayer", Hidden).GetValue(court), Is.EqualTo(id), "Reachable contact must happen now");
                Assert.That(Time.time, Is.EqualTo(instant));
                string strokeField = id == 1 ? "opponentStroke" : id == 2 ? "partnerStroke" : "secondStroke";
                string shot = (string)type.GetField(strokeField, Hidden).GetValue(court);
                var velocity = (Vector3)type.GetProperty("ShuttleVelocity").GetValue(court);
                uint random = (uint)type.GetField("opponentRandom", Hidden).GetValue(court);
                if (!priorImpossible) { expectedVelocity = velocity; expectedShot = shot; expectedRandom = random; }
                else
                {
                    Assert.That(shot, Is.EqualTo(expectedShot), difficulty + " actor=" + id + " swapped=" + swapped + " height=" + height);
                    Assert.That(velocity, Is.EqualTo(expectedVelocity)); Assert.That(random, Is.EqualTo(expectedRandom)); cases++;
                }
            }
        }
        TestContext.WriteLine("Reachable decision pairs=" + cases);
    }
    [UnityTest] public IEnumerator DoublesReactionAndRefreshDoNotDependOnClockEpoch()
    {
        yield return Boot(); Call("ConfigureDiscipline", true); Call("ConfigureOpponent", "Easy", 71);
        var update = type.GetMethod("UpdateDoublesAt", Hidden);
        Assert.That(update, Is.Not.Null, "Missing clock seam for epoch reproduction");
        var parameter = update.GetParameters()[0].ParameterType;
        int expectedReaction = 0, expectedRefresh = 0;
        foreach (double epoch in new[] { .24, 4096.24 })
        {
            Call("StartMatch"); Call("Serve", true);
            var root = GameObject.Find("Opponent").transform;
            var shuttle = GameObject.Find("Shuttle").transform;
            shuttle.position = root.TransformPoint(new Vector3(.8f, 3.65f, .25f)); Set("velocity", Vector3.down);
            update.Invoke(court, new[] { Convert.ChangeType(epoch, parameter) });
            var start = root.position;
            var deadlines = (Array)type.GetField("teamRefresh", Hidden).GetValue(court);
            double initialRefresh = Convert.ToDouble(deadlines.GetValue(1));
            int reactionTick = 0, refreshTick = 0;
            for (int tick = 1; tick <= 20; tick++)
            {
                update.Invoke(court, new[] { Convert.ChangeType(epoch + tick * (double)Time.fixedDeltaTime, parameter) });
                if (reactionTick == 0 && Vector3.Distance(root.position, start) > .001f) reactionTick = tick;
                if (refreshTick == 0 && Convert.ToDouble(deadlines.GetValue(1)) != initialRefresh) refreshTick = tick;
            }
            TestContext.WriteLine("epoch=" + epoch + " reactionTick=" + reactionTick + " refreshTick=" + refreshTick);
            Assert.That(reactionTick, Is.GreaterThan(0)); Assert.That(refreshTick, Is.GreaterThan(0));
            if (expectedReaction == 0) { expectedReaction = reactionTick; expectedRefresh = refreshTick; }
            else
            {
                Assert.That(reactionTick, Is.EqualTo(expectedReaction), "Clock epoch changed reaction by a fixed tick");
                Assert.That(refreshTick, Is.EqualTo(expectedRefresh), "Clock epoch changed prediction refresh");
            }
            Assert.That(reactionTick * (double)Time.fixedDeltaTime, Is.GreaterThanOrEqualTo((double).28f));
            Assert.That(refreshTick * (double)Time.fixedDeltaTime, Is.GreaterThanOrEqualTo((double).16f));
        }
    }
    [Serializable] sealed class TeamTimerSnapshot { public TeamTimerValues doubles; }
    [Serializable] sealed class TeamTimerValues { public float[] think, refresh; }
    [UnityTest] public IEnumerator DoublesDeadlinesStayFrozenInReplayPauseAndSnapshot()
    {
        yield return Boot(); Call("ConfigureDiscipline", true); Call("ConfigureOpponent", "Easy", 71);
        Call("StartMatch"); Call("Serve", true);
        for (int i = 0; i < 3; i++) type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        var think = (double[])type.GetField("teamThink", Hidden).GetValue(court);
        var refresh = (double[])type.GetField("teamRefresh", Hidden).GetValue(court);
        for (int i = 0; i < 4; i++) { think[i] = Time.timeAsDouble + .28f; refresh[i] = Time.timeAsDouble + .16f; }
        Assert.That(Call("BeginReplay"), Is.EqualTo(true));
        var before = JsonUtility.FromJson<TeamTimerSnapshot>((string)Call("CaptureMatch")).doubles;
        yield return new WaitForSecondsRealtime(.12f);
        var during = JsonUtility.FromJson<TeamTimerSnapshot>((string)Call("CaptureMatch")).doubles;
        Assert.That(during.think, Is.EqualTo(before.think)); Assert.That(during.refresh, Is.EqualTo(before.refresh));
        Call("SkipReplay");
        string saved = (string)Call("CaptureMatch");
        var skipped = JsonUtility.FromJson<TeamTimerSnapshot>(saved).doubles;
        for (int i = 0; i < 4; i++)
        {
            Assert.That(skipped.think[i], Is.EqualTo(before.think[i]).Within(.000001f));
            Assert.That(skipped.refresh[i], Is.EqualTo(before.refresh[i]).Within(.000001f));
        }
        Call("StartMatch"); Call("RestoreMatch", saved);
        var restored = JsonUtility.FromJson<TeamTimerSnapshot>((string)Call("CaptureMatch")).doubles;
        Assert.That(restored.think, Is.EqualTo(skipped.think)); Assert.That(restored.refresh, Is.EqualTo(skipped.refresh));
        Time.timeScale = 0;
        yield return null;
        var paused = JsonUtility.FromJson<TeamTimerSnapshot>((string)Call("CaptureMatch")).doubles;
        yield return new WaitForSecondsRealtime(.08f);
        var later = JsonUtility.FromJson<TeamTimerSnapshot>((string)Call("CaptureMatch")).doubles;
        Assert.That(later.think, Is.EqualTo(paused.think)); Assert.That(later.refresh, Is.EqualTo(paused.refresh));
        Time.timeScale = 1;
    }
    [UnityTest] public IEnumerator DefaultLineJudgeNeverInventsBadCall()
    {
        yield return Boot(); Call("StartMatch");
        for (int i = 0; i < 1000; i++) Assert.That(type.GetMethod("AnnounceLanding", Hidden).Invoke(court, new object[] { new Vector3(2.58f, .04f, 3) }), Is.EqualTo(true));
    }
}
