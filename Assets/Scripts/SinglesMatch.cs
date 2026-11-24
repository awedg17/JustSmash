using System;

namespace JustSmash
{
    [Serializable]
    public sealed class SinglesMatch
    {
        public int[] Points = new int[2], Games = new int[2];
        public int[] GamePoints = Array.Empty<int>();
        public int[] Challenges = { 2, 2 };
        public bool Review(int side, bool successful)
        {
            if (side < 0 || side > 1) throw new ArgumentOutOfRangeException(nameof(side));
            if (Challenges[side] <= 0) return false;
            if (!successful) Challenges[side]--;
            return true;
        }
        // ponytail: shared side scoring retains legacy type/save name; player IDs 0/2 near team, 1/3 far team.
        public bool Doubles;
        public int[] RightPlayers = { 0, 1 };
        public int ServerPlayer => !Doubles ? Server : RightPlayers[Server] ^ (Points[Server] % 2 == 0 ? 0 : 2);
        public int ReceiverPlayer => !Doubles ? 1 - Server : RightPlayers[1 - Server] ^ (Points[Server] % 2 == 0 ? 0 : 2);
        public int Server, Winner = -1, GameNumber = 1;
        public bool EndsSwapped;
        [UnityEngine.SerializeField] bool decidingEndsChanged;
        public void ValidateGamePoints(bool legacy)
        {
            int count = Games[0] + Games[1];
            bool ended = Winner >= 0;
            if (GameNumber != count + (ended ? 0 : 1) || (ended && (Games[Winner] != 2 || Games[1 - Winner] > 1))
                || (!ended && (Games[0] > 1 || Games[1] > 1))) throw new ArgumentException("Invalid game history state");
            if (legacy && (GamePoints == null || GamePoints.Length == 0))
            {
                GamePoints = new int[count * 2];
                for (int i = 0; i < GamePoints.Length; i++) GamePoints[i] = -1;
                if (ended) { GamePoints[GamePoints.Length - 2] = Points[0]; GamePoints[GamePoints.Length - 1] = Points[1]; }
            }
            if (GamePoints == null || GamePoints.Length != count * 2) throw new ArgumentException("Invalid game history count");
            var wins = new int[2]; bool known = false;
            for (int i = 0; i < count; i++)
            {
                int a = GamePoints[i * 2], b = GamePoints[i * 2 + 1];
                if (a == -1 && b == -1 && !known && !(ended && i == count - 1)) continue;
                known = true;
                int high = Math.Max(a, b), low = Math.Min(a, b), side = a > b ? 0 : 1;
                if (low < 0 || high > 21 || high < 15 || (high == 15 ? low > 13 : high < 21 ? low != high - 2 : low < 19 || low > 20))
                    throw new ArgumentException("Invalid completed game points");
                if (++wins[side] > Games[side] || (wins[side] == 2 && i != count - 1)) throw new ArgumentException("Invalid game history winner");
                if (ended && i == count - 1 && (side != Winner || a != Points[0] || b != Points[1])) throw new ArgumentException("Invalid final game points");
            }
        }
        public void AwardPoint(int side)
        {
            if (side < 0 || side > 1) throw new ArgumentOutOfRangeException(nameof(side));
            if (Winner >= 0) throw new InvalidOperationException("Match ended");
            if (Doubles && Server == side) RightPlayers[side] ^= 2;
            Points[side]++;
            Server = side;
            if (GameNumber == 3 && !decidingEndsChanged && Points[side] >= 8)
            { EndsSwapped = !EndsSwapped; decidingEndsChanged = true; }
            if (Points[side] < 15 || (Points[side] < 21 && Points[side] - Points[1 - side] < 2)) return;
            int previous = GamePoints == null ? 0 : GamePoints.Length;
            Array.Resize(ref GamePoints, GameNumber * 2);
            for (int i = previous; i < GamePoints.Length; i++) GamePoints[i] = -1;
            GamePoints[(GameNumber - 1) * 2] = Points[0]; GamePoints[(GameNumber - 1) * 2 + 1] = Points[1];
            Games[side]++;
            if (Games[side] == 2) { Winner = side; return; }
            int firstServer = ServerPlayer;
            RightPlayers[0] = 0; RightPlayers[1] = 1; RightPlayers[side] = firstServer;
            Points[0] = Points[1] = 0;
            Challenges[0] = Challenges[1] = 2;
            GameNumber++;
            EndsSwapped = !EndsSwapped;
        }
    }
}
