using System;

namespace JustSmash
{
    public readonly struct OpponentProfile
    {
        public readonly float Reaction, Refresh, AimError;
        public const float MovementSpeed = 4.5f;

        OpponentProfile(float reaction, float refresh, float aimError)
        { Reaction = reaction; Refresh = refresh; AimError = aimError; }

        public static OpponentProfile For(string difficulty)
        {
            switch (difficulty)
            {
                case "Easy": return new OpponentProfile(.28f, .16f, .35f);
                case "Normal": return new OpponentProfile(.18f, .10f, .18f);
                case "Hard": return new OpponentProfile(.11f, .06f, .08f);
                default: throw new ArgumentException("Unknown difficulty", nameof(difficulty));
            }
        }
    }
}
