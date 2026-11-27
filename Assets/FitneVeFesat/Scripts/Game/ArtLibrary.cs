using FitneFesat.Core;
using UnityEngine;

namespace FitneFesat.Game
{
    // Oyunun kullandığı görsellerin listesi. Editördeki "Fitne ve Fesat > Projeyi Hazırla" komutu doldurur.
    [CreateAssetMenu(menuName = "Fitne ve Fesat/Görsel Kütüphanesi", fileName = "ArtLibrary")]
    public class ArtLibrary : ScriptableObject
    {
        [Tooltip("Role sırasıyla (Aga, Kadi, ... Maskara) rol portreleri.")]
        public Sprite[] portraits = new Sprite[16];
        public Sprite menuBackground;
        public Sprite titleBanner;
        public Sprite panelVertical;
        public Sprite badgeShield;

        public Sprite Portrait(int role)
        {
            return portraits != null && role >= 0 && role < portraits.Length ? portraits[role] : null;
        }

        public Sprite Portrait(Role role)
        {
            return Portrait((int)role);
        }
    }
}
