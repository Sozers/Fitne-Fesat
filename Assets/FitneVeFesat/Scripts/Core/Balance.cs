using System;

namespace FitneFesat.Core
{
    // Denge ayarları: TASARIM.md 10. bölüm. Bütün değerler tek yerde tutulur.
    [Serializable]
    public class Balance
    {
        public int MinPlayers = 6;
        public int MaxPlayers = 15;

        public int AgaVotePower = 2;
        public int KadiExecutions = 2;
        public int KabadayiShots = 2;
        public int HekimSelfHeals = 1;
        public int NoDeathRoundsForDraw = 3;

        public float NightSeconds = 20f;
        public float MorningSeconds = 10f;
        public float DiscussionSeconds = 60f;
        public float VotingSeconds = 15f;
    }

    public static class Questions
    {
        // Müneccim'in hazır soruları. Yanıt her zaman bir oyuncu; son soruda ayrıca Masum/Şüpheli.
        public static readonly string[] All =
        {
            "Seni kim öldürdü sence?",
            "En çok kimden şüpheleniyordun?",
            "Kime güveniyordun?",
            "Öldüğün gece ne öğrendin?"
        };

        public static bool WantsVerdict(int questionIndex)
        {
            return questionIndex == 3;
        }
    }
}
