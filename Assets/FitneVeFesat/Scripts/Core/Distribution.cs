using System;
using System.Collections.Generic;
using System.Linq;

namespace FitneFesat.Core
{
    // Oyuncu sayısına göre taraf sayıları ve rastgele rol seçimi: TASARIM.md 8. bölüm.
    public static class Distribution
    {
        public static bool TryGetCounts(int players, out int kasaba, out int kumpas, out int tarafsiz)
        {
            switch (players)
            {
                case 6: kasaba = 4; kumpas = 2; tarafsiz = 0; return true;
                case 7: kasaba = 5; kumpas = 2; tarafsiz = 0; return true;
                case 8: kasaba = 5; kumpas = 2; tarafsiz = 1; return true;
                case 9: kasaba = 6; kumpas = 2; tarafsiz = 1; return true;
                case 10: kasaba = 6; kumpas = 3; tarafsiz = 1; return true;
                case 11: kasaba = 7; kumpas = 3; tarafsiz = 1; return true;
                case 12: kasaba = 8; kumpas = 3; tarafsiz = 1; return true;
                case 13: kasaba = 8; kumpas = 3; tarafsiz = 2; return true;
                case 14: kasaba = 9; kumpas = 3; tarafsiz = 2; return true;
                case 15: kasaba = 10; kumpas = 3; tarafsiz = 2; return true;
                default: kasaba = kumpas = tarafsiz = 0; return false;
            }
        }

        static readonly Role[] KasabaSpecials =
        {
            Role.Aga, Role.Kadi, Role.Kabadayi, Role.Hekim, Role.Gozcu, Role.Cengi, Role.Muneccim
        };

        static readonly Role[] Neutrals = { Role.Dervis, Role.Yamyam, Role.Kanli, Role.Maskara };

        // Rolleri seçer (sırasız). Oyunculara dağıtmak için ayrıca karıştırılır.
        public static List<Role> Draw(int players, Random rng)
        {
            if (!TryGetCounts(players, out int kasaba, out int kumpas, out int tarafsiz))
                throw new ArgumentOutOfRangeException(nameof(players), "Oyuncu sayısı 6 ile 15 arasında olmalı.");

            var roles = new List<Role>();

            // Kasaba: Subaşı her oyunda; kalan yerler 7 özel rol ile Ahali arasından.
            roles.Add(Role.Subasi);
            var specials = new List<Role>(KasabaSpecials);
            for (int i = 1; i < kasaba; i++)
            {
                int pick = rng.Next(specials.Count + 1);
                if (pick == specials.Count)
                {
                    roles.Add(Role.Ahali);
                }
                else
                {
                    roles.Add(specials[pick]);
                    specials.RemoveAt(pick);
                }
            }

            // Kumpas: Zorba ve Müfteri birer kez, Fedai tekrarlanabilir; en az bir saldırgan.
            bool zorbaUsed = false, mufteriUsed = false, hasAttacker = false;
            for (int i = 0; i < kumpas; i++)
            {
                bool lastSlot = i == kumpas - 1;
                var options = new List<Role>();
                if (!zorbaUsed) options.Add(Role.Zorba);
                options.Add(Role.Fedai);
                if (!mufteriUsed && !(lastSlot && !hasAttacker)) options.Add(Role.Mufteri);

                Role r = options[rng.Next(options.Count)];
                if (r == Role.Zorba) zorbaUsed = true;
                if (r == Role.Mufteri) mufteriUsed = true;
                if (r != Role.Mufteri) hasAttacker = true;
                roles.Add(r);
            }

            // Tarafsız: 4 rolden birbirinden farklı.
            var neutrals = new List<Role>(Neutrals);
            Shuffle(neutrals, rng);
            roles.AddRange(neutrals.Take(tarafsiz));

            return roles;
        }

        public static void Shuffle<T>(IList<T> list, Random rng)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                T tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }
    }
}
