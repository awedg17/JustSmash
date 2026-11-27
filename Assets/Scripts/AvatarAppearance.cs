using System;
using UnityEngine;

namespace JustSmash
{
    [Serializable] public sealed class AvatarAppearance
    {
        public int jerseyStyleId, jerseyColorId, pantsId, shoeColorId, hairColorId, racketColorId, gripColorId, stringColorId;
        internal static readonly string[] Styles = { "Polos", "Panel bahu", "Garis samping" };
        internal static readonly string[] Colors = { "Gading", "Zamrud", "Marun", "Nila" };
        internal static readonly Color[] Palette = { new Color(.89f,.83f,.65f), new Color(.10f,.68f,.48f), new Color(.75f,.16f,.24f), new Color(.26f,.35f,.83f) };
        internal static readonly Color[] Dark = { new Color(.12f,.15f,.21f), new Color(.07f,.27f,.23f), new Color(.35f,.13f,.16f), new Color(.17f,.19f,.36f) };
        internal static readonly string[] DarkNames = { "Arang", "Hijau tua", "Merah tua", "Nila tua" };
        internal void Validate()
        {
            if ((uint)jerseyStyleId >= Styles.Length || (uint)jerseyColorId >= Palette.Length || (uint)pantsId >= Dark.Length
                || (uint)shoeColorId >= Palette.Length || (uint)hairColorId >= Dark.Length || (uint)racketColorId >= Palette.Length
                || (uint)gripColorId >= Dark.Length || (uint)stringColorId >= Palette.Length)
                throw new ArgumentException("Pilihan penampilan tidak valid");
        }
        internal AvatarAppearance Copy() => (AvatarAppearance)MemberwiseClone();
    }
}
