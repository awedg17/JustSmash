using UnityEngine;
namespace JustSmash
{
    public sealed partial class CourtPrototype
    {
        string flowStage;
        float pointWait;
        void ContinuePoint(float dt)
        {
            if (Match == null || MatchState != "Point") { pointWait = 0; return; }
            if (replaying || Time.timeScale == 0 || testerOpen) return;
            pointWait += dt;
            if (pointWait >= 3) { pointWait = 0; NextPoint(); }
        }
        int flowGame = 1;
        bool flowEnds;
        public string FlowStage => Match == null ? "Training" : Match.Winner >= 0 ? "MatchEnd" : flowStage ?? MatchState;
        public string MatchSummary => Match == null ? "Training" : "Official15 | " + Difficulty + " | Game " + Match.GameNumber + " | Games " + Match.Games[0] + " - " + Match.Games[1] + " | " + (Match.Winner >= 0 ? (Match.Winner == 0 ? "You win" : "Opponent wins") : "Server: " + (Match.Server == 0 ? "You" : "Opponent"));
        public void BeginExhibition() { StartMatch(); flowStage = "Warmup"; }
        public void AdvanceFlow()
        {
            if (Time.timeScale == 0 || replaying) return;
            flowStage = FlowStage == "Warmup" ? "Ready" : FlowStage == "GameEnd" ? "Interval" : FlowStage == "Interval" ? "ChangeEnds" : FlowStage == "ChangeEnds" ? "Ready" : null;
        }
        void ResetFlow() { flowGame = 1; flowEnds = false; flowStage = "Ready"; }
        void PrepareFlow()
        {
            if (Match.GameNumber > flowGame) { flowGame = Match.GameNumber; flowStage = "GameEnd"; }
            else if (flowEnds != Match.EndsSwapped) flowStage = "ChangeEnds";
            flowEnds = Match.EndsSwapped;
        }
    }
}
