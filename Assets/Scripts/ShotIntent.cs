using System;
using UnityEngine;

namespace JustSmash
{
    public sealed class ShotIntent
    {
        public const float Window = .16f;
        public bool Pending { get; private set; }
        public int Kind { get; private set; }
        public Vector2 Aim { get; private set; }
        public float Power { get; private set; }
        public float CreatedAt { get; private set; }
        public float ExpiresAt { get; private set; }

        public void Queue(int kind, Vector2 aim, float power, float now)
        {
            if (kind < 0 || kind > 4) throw new ArgumentOutOfRangeException(nameof(kind));
            if (!float.IsFinite(aim.x) || !float.IsFinite(aim.y))
                throw new ArgumentOutOfRangeException(nameof(aim));
            if (!float.IsFinite(power) || power < 0 || power > 1)
                throw new ArgumentOutOfRangeException(nameof(power));
            if (!float.IsFinite(now) || now < 0 || now > float.MaxValue - Window)
                throw new ArgumentOutOfRangeException(nameof(now));
            Kind = kind;
            Aim = Vector2.ClampMagnitude(aim, 1);
            Power = power;
            CreatedAt = now;
            ExpiresAt = now + Window;
            Pending = true;
        }

        public bool TryTake(float now, bool reachable, out int kind)
        {
            if (!float.IsFinite(now) || now < 0) throw new ArgumentOutOfRangeException(nameof(now));
            kind = -1;
            if (!Pending) return false;
            if (now > ExpiresAt) { Clear(); return false; }
            if (now < CreatedAt || !reachable) return false;
            kind = Kind;
            Pending = false;
            return true;
        }

        public void Clear() { Pending = false; }
    }
}
