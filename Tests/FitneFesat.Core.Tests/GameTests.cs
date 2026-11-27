using System;
using System.Collections.Generic;
using System.Linq;
using FitneFesat.Core;
using NUnit.Framework;

namespace FitneFesat.Core.Tests
{
    public class GameTests
    {
        // Rolleri verilen sırayla oyunculara dağıtır; oyuncu adları rol adlarıdır (Aga, Kadi...).
        static Game Make(params Role[] roles)
        {
            var names = new List<string>();
            var seen = new Dictionary<Role, int>();
            foreach (var r in roles)
            {
                seen[r] = seen.TryGetValue(r, out int n) ? n + 1 : 1;
                names.Add(seen[r] == 1 ? r.ToString() : r.ToString() + seen[r]);
            }
            return Game.CreateWithRoles(names, roles, 1);
        }

        static Player Of(Game g, Role r) => g.Players.First(p => p.StartRole == r);
        static int Id(Game g, Role r) => Of(g, r).Id;

        static void ToNextNight(Game g, Action<Game> votes = null)
        {
            g.BeginDiscussion();
            g.BeginVoting();
            votes?.Invoke(g);
            g.ResolveVoting();
        }

        static List<string> Notes(Game g, Role r) => g.BuildView(Id(g, r)).morningPrivate;

        static readonly Role[] Basic =
        {
            Role.Subasi, Role.Hekim, Role.Cengi, Role.Gozcu, Role.Ahali, Role.Ahali,
            Role.Zorba, Role.Fedai, Role.Yamyam
        };

        // ------------------------------------------------------------ Dağılım

        [Test]
        public void Dagilim_sayilari_ve_kurallari_tutar()
        {
            for (int seed = 0; seed < 400; seed++)
            {
                for (int n = 6; n <= 15; n++)
                {
                    var roles = Distribution.Draw(n, new Random(seed * 31 + n));
                    Distribution.TryGetCounts(n, out int ka, out int ku, out int ta);
                    Assert.AreEqual(n, roles.Count);
                    Assert.AreEqual(ka, roles.Count(r => RoleInfo.FactionOf(r) == Faction.Kasaba));
                    Assert.AreEqual(ku, roles.Count(r => RoleInfo.FactionOf(r) == Faction.Kumpas));
                    Assert.AreEqual(ta, roles.Count(r => RoleInfo.FactionOf(r) == Faction.Tarafsiz));
                    Assert.AreEqual(1, roles.Count(r => r == Role.Subasi), "Subaşı her oyunda tek");
                    Assert.IsTrue(roles.Any(r => r == Role.Zorba || r == Role.Fedai), "Kumpas'ta saldırgan olmalı");
                    foreach (var g in roles.GroupBy(r => r))
                        if (g.Key != Role.Ahali && g.Key != Role.Fedai)
                            Assert.AreEqual(1, g.Count(), g.Key + " tekrarlanmamalı");
                }
            }
        }

        [Test]
        public void Rastgele_oyun_kurulur_ve_kanli_hedefi_aga_kadi_olmaz()
        {
            for (int seed = 0; seed < 200; seed++)
            {
                var names = Enumerable.Range(0, 15).Select(i => "O" + i).ToList();
                var g = Game.Create(names, seed);
                Assert.AreEqual(Phase.Night, g.Phase);
                foreach (var k in g.Players.Where(p => p.Role == Role.Kanli))
                {
                    var t = g.P(k.KanliTarget);
                    Assert.AreEqual(Faction.Kasaba, t.Faction);
                    Assert.AreNotEqual(Role.Aga, t.Role);
                    Assert.AreNotEqual(Role.Kadi, t.Role);
                }
            }
        }

        // ------------------------------------------------------------ Gece

        [Test]
        public void Kumpas_saldirisi_oldurur_ve_herkese_rol_aciklanir()
        {
            var g = Make(Basic);
            Assert.IsTrue(g.SetNightTarget(Id(g, Role.Zorba), Id(g, Role.Subasi)));
            g.ResolveNight();
            Assert.IsFalse(Of(g, Role.Subasi).Alive);
            Assert.That(g.BuildView(Id(g, Role.Ahali)).morningPublic[0], Does.Contain("Subaşı"));
        }

        [Test]
        public void Fedai_yokken_zorba_saldiriyi_kendisi_yapar()
        {
            var g = Make(Role.Subasi, Role.Aga, Role.Cengi, Role.Muneccim, Role.Zorba, Role.Mufteri);
            Assert.IsTrue(g.SetNightTarget(Id(g, Role.Zorba), Id(g, Role.Aga)));
            g.SetNightTarget(Id(g, Role.Mufteri), Id(g, Role.Subasi));
            g.ResolveNight();
            Assert.IsFalse(Of(g, Role.Aga).Alive);
        }

        [Test]
        public void Kumpas_kendi_uyesini_hedef_secemez()
        {
            var g = Make(Basic);
            Assert.IsFalse(g.SetNightTarget(Id(g, Role.Zorba), Id(g, Role.Fedai)));
        }

        [Test]
        public void Hekim_korudugu_kisiyi_kurtarir_ve_ikisi_de_ogrenir()
        {
            var g = Make(Basic);
            g.SetNightTarget(Id(g, Role.Zorba), Id(g, Role.Subasi));
            g.SetNightTarget(Id(g, Role.Hekim), Id(g, Role.Subasi));
            g.ResolveNight();
            Assert.IsTrue(Of(g, Role.Subasi).Alive);
            Assert.That(Notes(g, Role.Hekim)[0], Does.Contain("kurtardın"));
            Assert.That(Notes(g, Role.Subasi)[0], Does.Contain("hekim seni kurtardı"));
        }

        [Test]
        public void Cengi_fedaiyi_engellerse_kumpas_saldirisi_olmaz()
        {
            var g = Make(Basic);
            g.SetNightTarget(Id(g, Role.Zorba), Id(g, Role.Subasi));
            g.SetNightTarget(Id(g, Role.Cengi), Id(g, Role.Fedai));
            g.ResolveNight();
            Assert.IsTrue(Of(g, Role.Subasi).Alive);
            Assert.That(Notes(g, Role.Fedai)[0], Does.Contain("oyaladı"));
        }

        [Test]
        public void Cengi_zorbayi_engellerse_fedai_kendi_hedefine_saldirir()
        {
            var g = Make(Basic);
            g.SetNightTarget(Id(g, Role.Zorba), Id(g, Role.Subasi));
            g.SetNightTarget(Id(g, Role.Fedai), Id(g, Role.Gozcu));
            g.SetNightTarget(Id(g, Role.Cengi), Id(g, Role.Zorba));
            g.ResolveNight();
            Assert.IsTrue(Of(g, Role.Subasi).Alive);
            Assert.IsFalse(Of(g, Role.Gozcu).Alive);
        }

        [Test]
        public void Yamyami_ziyaret_eden_cengi_olur_yamyam_kendi_hedefine_de_saldirir()
        {
            var g = Make(Basic);
            g.SetNightTarget(Id(g, Role.Cengi), Id(g, Role.Yamyam));
            g.SetNightTarget(Id(g, Role.Yamyam), Id(g, Role.Gozcu));
            g.ResolveNight();
            Assert.IsFalse(Of(g, Role.Cengi).Alive);
            Assert.IsFalse(Of(g, Role.Gozcu).Alive);
        }

        [Test]
        public void Hekim_yamyama_giden_cengiyi_kurtarabilir()
        {
            var g = Make(Basic);
            g.SetNightTarget(Id(g, Role.Cengi), Id(g, Role.Yamyam));
            g.SetNightTarget(Id(g, Role.Hekim), Id(g, Role.Cengi));
            g.ResolveNight();
            Assert.IsTrue(Of(g, Role.Cengi).Alive);
        }

        [Test]
        public void Subasi_zorbayi_masum_fedaiyi_supheli_gorur()
        {
            var g = Make(Basic);
            g.SetNightTarget(Id(g, Role.Subasi), Id(g, Role.Zorba));
            g.ResolveNight();
            Assert.That(Notes(g, Role.Subasi)[0], Does.Contain("Masum"));

            ToNextNight(g);
            g.SetNightTarget(Id(g, Role.Subasi), Id(g, Role.Fedai));
            g.ResolveNight();
            Assert.That(Notes(g, Role.Subasi)[0], Does.Contain("Şüpheli"));
        }

        [Test]
        public void Mufterinin_iftirasi_ahaliyi_supheli_gosterir()
        {
            var g = Make(Role.Subasi, Role.Ahali, Role.Ahali, Role.Ahali, Role.Zorba, Role.Mufteri);
            g.SetNightTarget(Id(g, Role.Mufteri), Id(g, Role.Ahali));
            g.SetNightTarget(Id(g, Role.Subasi), Id(g, Role.Ahali));
            g.ResolveNight();
            Assert.That(Notes(g, Role.Subasi)[0], Does.Contain("Şüpheli"));
        }

        [Test]
        public void Gozcu_ziyaretcileri_gorur()
        {
            var g = Make(Basic);
            g.SetNightTarget(Id(g, Role.Zorba), Id(g, Role.Ahali));
            g.SetNightTarget(Id(g, Role.Hekim), Id(g, Role.Ahali));
            g.SetNightTarget(Id(g, Role.Gozcu), Id(g, Role.Ahali));
            g.ResolveNight();
            var note = Notes(g, Role.Gozcu)[0];
            Assert.That(note, Does.Contain("Fedai"));
            Assert.That(note, Does.Contain("Hekim"));
            Assert.That(note, Does.Not.Contain("Zorba"), "Fedai varken Zorba ziyaret etmez");
        }

        [Test]
        public void Kabadayi_ilk_gece_saldiramaz_kasaba_oldurunce_ertesi_gece_intihar_eder()
        {
            var g = Make(Role.Subasi, Role.Kabadayi, Role.Ahali, Role.Ahali, Role.Ahali, Role.Ahali, Role.Zorba, Role.Fedai);
            Assert.IsFalse(g.SetNightTarget(Id(g, Role.Kabadayi), Id(g, Role.Subasi)));
            g.ResolveNight();
            ToNextNight(g);

            Assert.IsTrue(g.SetNightTarget(Id(g, Role.Kabadayi), Id(g, Role.Subasi)));
            g.ResolveNight();
            Assert.IsFalse(Of(g, Role.Subasi).Alive);
            Assert.IsTrue(Of(g, Role.Kabadayi).Alive);
            ToNextNight(g);

            g.ResolveNight();
            Assert.IsFalse(Of(g, Role.Kabadayi).Alive);
        }

        [Test]
        public void Kadi_zindandakini_infaz_eder_hekim_onleyemez_kasaba_infazinda_hakki_gider()
        {
            var g = Make(Role.Subasi, Role.Kadi, Role.Hekim, Role.Ahali, Role.Ahali, Role.Ahali, Role.Zorba, Role.Fedai);
            g.ResolveNight();
            g.BeginDiscussion();
            Assert.IsTrue(g.SetJail(Id(g, Role.Kadi), Id(g, Role.Subasi)));
            g.BeginVoting();
            g.ResolveVoting();

            // Zindandaki saldırıdan korunur, Kadı'nın infazından korunmaz.
            Assert.AreEqual("claim", g.BuildView(Id(g, Role.Subasi)).promptMode);
            Assert.IsTrue(g.SetClaim(Id(g, Role.Subasi), (int)Role.Subasi));
            Assert.IsTrue(g.SetExecute(Id(g, Role.Kadi), true));
            g.SetNightTarget(Id(g, Role.Hekim), Id(g, Role.Subasi));
            g.ResolveNight();
            Assert.IsFalse(Of(g, Role.Subasi).Alive);
            Assert.IsTrue(Of(g, Role.Kadi).ExecutionLost);
        }

        [Test]
        public void Zindandaki_saldiridan_korunur_ve_yetenegini_kullanamaz()
        {
            var g = Make(Basic.Concat(new[] { Role.Kadi }).ToArray());
            g.ResolveNight();
            g.BeginDiscussion();
            g.SetJail(Id(g, Role.Kadi), Id(g, Role.Subasi));
            g.BeginVoting();
            g.ResolveVoting();

            Assert.IsFalse(g.SetNightTarget(Id(g, Role.Subasi), Id(g, Role.Zorba)), "zindandaki seçim yapamaz");
            g.SetNightTarget(Id(g, Role.Zorba), Id(g, Role.Subasi));
            g.ResolveNight();
            Assert.IsTrue(Of(g, Role.Subasi).Alive);
        }

        [Test]
        public void Aciklanmis_agayi_hekim_koruyamaz_ve_oyu_iki_sayilir()
        {
            var g = Make(Role.Aga, Role.Subasi, Role.Hekim, Role.Ahali, Role.Ahali, Role.Zorba, Role.Fedai);
            g.ResolveNight();
            g.BeginDiscussion();
            Assert.IsTrue(g.RevealAga(Id(g, Role.Aga)));
            g.BeginVoting();
            Assert.AreEqual(8, g.TotalVotePower());
            g.ResolveVoting();
            Assert.IsFalse(g.SetNightTarget(Id(g, Role.Hekim), Id(g, Role.Aga)));
        }

        [Test]
        public void Kanlinin_hedefi_gece_olurse_maskara_olur()
        {
            var g = Make(Role.Subasi, Role.Ahali, Role.Ahali, Role.Hekim, Role.Ahali, Role.Zorba, Role.Fedai, Role.Kanli);
            var kanli = Of(g, Role.Kanli);
            g.SetNightTarget(Id(g, Role.Zorba), kanli.KanliTarget);
            g.ResolveNight();
            Assert.AreEqual(Role.Maskara, kanli.Role);
        }

        [Test]
        public void Muneccim_oluden_yanit_alir()
        {
            var g = Make(Role.Subasi, Role.Muneccim, Role.Ahali, Role.Ahali, Role.Ahali, Role.Ahali, Role.Zorba, Role.Fedai);
            g.SetNightTarget(Id(g, Role.Zorba), Id(g, Role.Subasi));
            g.ResolveNight();
            ToNextNight(g);

            Assert.IsTrue(g.SetMuneccimQuestion(Id(g, Role.Muneccim), Id(g, Role.Subasi), 1));
            Assert.AreEqual("answer", g.BuildView(Id(g, Role.Subasi)).promptMode);
            Assert.IsTrue(g.Answer(Id(g, Role.Subasi), Id(g, Role.Zorba), -1));
            g.ResolveNight();
            Assert.That(Notes(g, Role.Muneccim)[0], Does.Contain("Zorba"));
        }

        [Test]
        public void Gece_herkes_onaylayinca_erken_biter()
        {
            var g = Make(Basic);
            Assert.IsFalse(g.NightReady());
            foreach (var p in g.Players) g.Confirm(p.Id);
            Assert.IsTrue(g.NightReady());
        }

        // ------------------------------------------------------------ Oylama ve oyun sonu

        [Test]
        public void Oylama_salt_cogunlukla_idam_eder_esitlikte_etmez()
        {
            var g = Make(Basic);
            g.ResolveNight();
            ToNextNight(g, x =>
            {
                // 9 oyuncu, 4 oy: çoğunluk değil.
                foreach (var r in new[] { Role.Subasi, Role.Hekim, Role.Cengi, Role.Gozcu })
                    x.Vote(Id(x, r), Id(x, Role.Zorba));
            });
            Assert.IsTrue(Of(g, Role.Zorba).Alive);

            g.ResolveNight();
            ToNextNight(g, x =>
            {
                foreach (var r in new[] { Role.Subasi, Role.Hekim, Role.Cengi, Role.Gozcu, Role.Ahali })
                    x.Vote(Id(x, r), Id(x, Role.Zorba));
            });
            Assert.IsFalse(Of(g, Role.Zorba).Alive);
        }

        [Test]
        public void Kendine_oy_verilemez()
        {
            var g = Make(Basic);
            g.ResolveNight();
            g.BeginDiscussion();
            g.BeginVoting();
            Assert.IsFalse(g.Vote(Id(g, Role.Subasi), Id(g, Role.Subasi)));
        }

        [Test]
        public void Maskara_idam_edilince_kazanir_oyun_surer()
        {
            var g = Make(Role.Subasi, Role.Ahali, Role.Ahali, Role.Ahali, Role.Ahali, Role.Zorba, Role.Fedai, Role.Maskara);
            g.ResolveNight();
            ToNextNight(g, x =>
            {
                foreach (var p in x.Players.Where(p => p.Role != Role.Maskara))
                    x.Vote(p.Id, Id(x, Role.Maskara));
            });
            Assert.IsTrue(Of(g, Role.Maskara).PersonalWin);
            Assert.AreEqual(Phase.Night, g.Phase);
        }

        [Test]
        public void Kumpas_tukenince_kasaba_kazanir_dervis_de_kazanir()
        {
            var g = Make(Role.Subasi, Role.Ahali, Role.Ahali, Role.Ahali, Role.Dervis, Role.Zorba);
            g.ResolveNight();
            ToNextNight(g, x =>
            {
                foreach (var p in x.Players.Where(p => p.Role != Role.Zorba))
                    x.Vote(p.Id, Id(x, Role.Zorba));
            });
            Assert.AreEqual(Phase.GameOver, g.Phase);
            Assert.That(g.ResultTitle, Does.Contain("Kasaba"));
            Assert.Contains(Id(g, Role.Dervis), g.Winners);
            Assert.IsFalse(g.Winners.Contains(Id(g, Role.Zorba)));
        }

        [Test]
        public void Kasaba_tukenince_kumpas_kazanir()
        {
            var g = Make(Role.Subasi, Role.Ahali, Role.Zorba, Role.Fedai, Role.Dervis, Role.Dervis);
            g.SetNightTarget(Id(g, Role.Zorba), Id(g, Role.Subasi));
            g.ResolveNight();
            ToNextNight(g, x =>
            {
                foreach (var p in x.Players.Where(p => p.Alive && p.Role != Role.Ahali))
                    x.Vote(p.Id, Id(x, Role.Ahali));
            });
            Assert.AreEqual(Phase.GameOver, g.Phase);
            Assert.That(g.ResultTitle, Does.Contain("Kumpas"));
        }

        [Test]
        public void Uc_tur_kimse_olmezse_berabere()
        {
            var g = Make(Basic);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreNotEqual(Phase.GameOver, g.Phase);
                g.ResolveNight();
                ToNextNight(g);
            }
            Assert.AreEqual(Phase.GameOver, g.Phase);
            Assert.That(g.ResultTitle, Does.Contain("Berabere"));
        }

        [Test]
        public void Gorunum_baskasinin_rolunu_gostermez_kumpas_birbirini_gorur()
        {
            var g = Make(Basic);
            var ahaliView = g.BuildView(Id(g, Role.Ahali));
            Assert.AreEqual("", ahaliView.players.First(e => e.id == Id(g, Role.Zorba)).role);
            var zorbaView = g.BuildView(Id(g, Role.Zorba));
            Assert.AreEqual("Fedai", zorbaView.players.First(e => e.id == Id(g, Role.Fedai)).role);
        }
    }
}
