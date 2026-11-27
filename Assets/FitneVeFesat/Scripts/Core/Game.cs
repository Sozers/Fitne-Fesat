using System;
using System.Collections.Generic;
using System.Linq;

namespace FitneFesat.Core
{
    public enum Phase
    {
        Night,
        Morning,
        Discussion,
        Voting,
        GameOver
    }

    // Oyunun kurallar motoru. Ağdan, zamandan ve Unity'den bağımsızdır:
    // süreleri ev sahibi (oda kuran telefon) tutar, aşama geçişlerini bu sınıfa bildirir.
    public class Game
    {
        enum AttackKind { Kumpas, Kabadayi, Yamyam, Execution, Suicide }

        struct Attack
        {
            public int Attacker;
            public int Target;
            public AttackKind Kind;
            public Attack(int attacker, int target, AttackKind kind) { Attacker = attacker; Target = target; Kind = kind; }
        }

        public readonly Balance Balance;
        public readonly List<Player> Players = new List<Player>();
        public Phase Phase { get; private set; }
        public int Round { get; private set; } = 1;

        public string ResultTitle { get; private set; } = "";
        public readonly List<int> Winners = new List<int>();

        // Gece seçimleri
        readonly Dictionary<int, int> nightTarget = new Dictionary<int, int>();
        readonly HashSet<int> confirmed = new HashSet<int>();
        int jailed = -1;
        int nextJail = -1;
        bool executeChoice;
        int jailClaim = -1;
        int munDead = -1;
        int munQuestion = -1;
        int deadAnswerPlayer = -1;
        int deadAnswerVerdict = -1;
        bool deadAnswered;

        // Gündüz
        readonly Dictionary<int, int> votes = new Dictionary<int, int>();

        // Bildirimler
        readonly List<string> publicLog = new List<string>();
        readonly Dictionary<int, List<string>> privateLog = new Dictionary<int, List<string>>();
        readonly List<string> morningPublic = new List<string>();
        readonly Dictionary<int, List<string>> morningPrivate = new Dictionary<int, List<string>>();
        readonly List<string> rolesInGame = new List<string>();

        int roundsWithoutDeath;
        bool deathThisRound;

        Game(Balance balance)
        {
            Balance = balance ?? new Balance();
        }

        // Rolleri rastgele dağıtarak yeni oyun.
        public static Game Create(IList<string> names, int seed, Balance balance = null)
        {
            var rng = new Random(seed);
            var roles = Distribution.Draw(names.Count, rng);
            Distribution.Shuffle(roles, rng);
            return CreateWithRoles(names, roles, seed, balance);
        }

        // Rolleri belirli olan oyun (testler için).
        public static Game CreateWithRoles(IList<string> names, IList<Role> roles, int seed, Balance balance = null)
        {
            if (names.Count != roles.Count) throw new ArgumentException("Ad ve rol sayısı eşit olmalı.");
            var game = new Game(balance);
            var rng = new Random(seed);
            for (int i = 0; i < names.Count; i++)
            {
                var p = new Player { Id = i, Name = names[i], Role = roles[i], StartRole = roles[i] };
                p.Shots = game.Balance.KabadayiShots;
                p.Executions = game.Balance.KadiExecutions;
                p.SelfHeals = game.Balance.HekimSelfHeals;
                game.Players.Add(p);
                game.privateLog[i] = new List<string>();
                game.morningPrivate[i] = new List<string>();
            }

            // Fedai kıdemi rastgele.
            var fedais = game.Players.Where(p => p.Role == Role.Fedai).ToList();
            Distribution.Shuffle(fedais, rng);
            for (int i = 0; i < fedais.Count; i++) fedais[i].Seniority = i;

            // Kanlı'nın hedefi: Ağa ve Kadı dışında rastgele bir Kasaba üyesi.
            foreach (var kanli in game.Players.Where(p => p.Role == Role.Kanli))
            {
                var candidates = game.Players.Where(p => p.Faction == Faction.Kasaba && p.Role != Role.Aga && p.Role != Role.Kadi).ToList();
                if (candidates.Count == 0) candidates = game.Players.Where(p => p.Faction == Faction.Kasaba).ToList();
                if (candidates.Count > 0) kanli.KanliTarget = candidates[rng.Next(candidates.Count)].Id;
            }

            foreach (var group in roles.GroupBy(r => r).OrderBy(g => RoleInfo.FactionOf(g.Key)).ThenBy(g => (int)g.Key))
            {
                int n = group.Count();
                game.rolesInGame.Add(n > 1 ? $"{RoleInfo.Name(group.Key)} ×{n}" : RoleInfo.Name(group.Key));
            }

            game.BeginNight();
            return game;
        }

        public Player P(int id)
        {
            return id >= 0 && id < Players.Count ? Players[id] : null;
        }

        IEnumerable<Player> Alive => Players.Where(p => p.Alive);

        void Note(int playerId, string text)
        {
            morningPrivate[playerId].Add(text);
            privateLog[playerId].Add($"{Round}. gece: {text}");
        }

        void Public(string text)
        {
            publicLog.Add(text);
        }

        // ---------------------------------------------------------------- Aşamalar

        void BeginNight()
        {
            Phase = Phase.Night;
            nightTarget.Clear();
            confirmed.Clear();
            executeChoice = false;
            jailClaim = -1;
            munDead = -1;
            munQuestion = -1;
            deadAnswerPlayer = -1;
            deadAnswerVerdict = -1;
            deadAnswered = false;
            deathThisRound = false;

            var kadi = Alive.FirstOrDefault(p => p.Role == Role.Kadi);
            var jailTarget = P(nextJail);
            jailed = kadi != null && jailTarget != null && jailTarget.Alive ? jailTarget.Id : -1;
            nextJail = -1;
        }

        public void BeginDiscussion()
        {
            if (Phase == Phase.Morning) Phase = Phase.Discussion;
        }

        public void BeginVoting()
        {
            if (Phase == Phase.Discussion)
            {
                Phase = Phase.Voting;
                votes.Clear();
            }
        }

        // Gece erken bitebilir mi: bütün yaşayanlar onayladı ve Müneccim'in sorduğu ölü yanıtladı.
        public bool NightReady()
        {
            if (Phase != Phase.Night) return false;
            if (Alive.Any(p => !confirmed.Contains(p.Id))) return false;
            if (munDead >= 0 && !deadAnswered) return false;
            return true;
        }

        // ---------------------------------------------------------------- Gece eylemleri

        bool IsLastKumpasMufteri(Player p)
        {
            return p.Role == Role.Mufteri && p.Alive && !Alive.Any(o => o.IsKumpas && o.Role != Role.Mufteri);
        }

        Player KidemliFedai()
        {
            return Alive.Where(p => p.Role == Role.Fedai).OrderBy(p => p.Seniority).FirstOrDefault();
        }

        // Bu gece oyuncunun seçebileceği hedefler; mod "target" değilse boş.
        public List<int> TargetOptions(Player p, out string mode, out string text)
        {
            mode = PromptModes.Confirm;
            text = "";
            var list = new List<int>();
            if (!p.Alive || Phase != Phase.Night)
            {
                mode = PromptModes.None;
                return list;
            }

            if (p.Id == jailed)
            {
                mode = PromptModes.Claim;
                text = "Kadı seni zindana attı. Bu gece yeteneğini kullanamazsın ama saldırılardan korunursun. Kadı sana rolünü soruyor: bir rol seç (yalan söyleyebilirsin).";
                return list;
            }

            var others = Alive.Where(o => o.Id != p.Id).Select(o => o.Id).ToList();
            var nonKumpas = Alive.Where(o => !o.IsKumpas).Select(o => o.Id).ToList();

            switch (p.Role)
            {
                case Role.Kadi:
                    if (jailed >= 0)
                    {
                        mode = PromptModes.Execute;
                        bool can = p.Executions > 0 && !p.ExecutionLost;
                        string claim = jailClaim >= 0 ? RoleInfo.Name((Role)jailClaim) : "henüz yanıt vermedi";
                        text = $"Zindanda {P(jailed).Name} var. Sorguda söylediği rol: {claim}.\n" +
                               (can ? $"İnfaz hakkın: {p.Executions}." : "İnfaz hakkın kalmadı.");
                    }
                    else
                    {
                        text = Round == 1 ? "İlk gece kimseyi hapsedemezsin. Geceyi onayla." : "Bu gece zindan boş. Geceyi onayla.";
                    }
                    break;

                case Role.Kabadayi:
                    if (p.SuicidePending) text = "Bir Kasaba üyesinin kanı ellerinde. Vicdan azabı seni bu gece bitirecek.";
                    else if (Round == 1) text = "İlk gece saldıramazsın. Geceyi onayla.";
                    else if (p.Shots <= 0) text = "Saldırı hakkın kalmadı. Geceyi onayla.";
                    else { mode = PromptModes.Target; text = $"Kimi öldürmeye çalışacaksın? (Kalan hak: {p.Shots})"; list = others; }
                    break;

                case Role.Cengi:
                    mode = PromptModes.Target; text = "Kimin dikkatini dağıtacaksın?"; list = others;
                    break;
                case Role.Subasi:
                    mode = PromptModes.Target; text = "Kimi araştıracaksın?"; list = others;
                    break;
                case Role.Gozcu:
                    mode = PromptModes.Target; text = "Kimi gözetleyeceksin?"; list = others;
                    break;
                case Role.Yamyam:
                    mode = PromptModes.Target; text = "Bu gece kime saldıracaksın?"; list = others;
                    break;

                case Role.Hekim:
                    mode = PromptModes.Target;
                    text = p.SelfHeals > 0 ? "Kimi koruyacaksın? (Kendini bir kez koruyabilirsin.)" : "Kimi koruyacaksın?";
                    list = Alive.Where(o => !(o.Role == Role.Aga && o.Revealed) && (o.Id != p.Id || p.SelfHeals > 0)).Select(o => o.Id).ToList();
                    break;

                case Role.Zorba:
                case Role.Fedai:
                    mode = PromptModes.Target; text = KumpasText(p); list = nonKumpas;
                    break;

                case Role.Mufteri:
                    mode = PromptModes.Target;
                    if (IsLastKumpasMufteri(p)) { text = "Son kalan Kumpas üyesisin. Kime saldıracaksın?"; list = nonKumpas; }
                    else { text = "Kime sahte delil koyacaksın?\n" + KumpasSuggestions(); list = others; }
                    break;

                case Role.Muneccim:
                    var dead = Players.Where(o => !o.Alive).Select(o => o.Id).ToList();
                    if (dead.Count > 0) { mode = PromptModes.Muneccim; text = "Hangi ölüye hangi soruyu soracaksın?"; list = dead; }
                    else text = "Henüz konuşacak bir ölü yok. Geceyi onayla.";
                    break;

                default:
                    text = "Bu gece yapacağın bir şey yok. Kimse anlamasın diye geceyi onayla.";
                    break;
            }
            return list;
        }

        string KumpasText(Player p)
        {
            string head = p.Role == Role.Zorba
                ? "Bu gece Kumpas kimi öldürsün? Son karar senin."
                : (KidemliFedai() == p ? "Kumpas'ın saldırısını sen yapacaksın. Hedefi Zorba seçer; Zorba yoksa ya da seçmezse senin seçimin geçerli." : "Yedek Fedai'sin. Seçimin öneri olarak görünür.");
            return head + "\n" + KumpasSuggestions();
        }

        string KumpasSuggestions()
        {
            var parts = new List<string>();
            foreach (var k in Alive.Where(o => o.IsKumpas))
            {
                if (nightTarget.TryGetValue(k.Id, out int t) && P(t) != null)
                {
                    string verb = k.Role == Role.Mufteri && !IsLastKumpasMufteri(k) ? "iftira" : "hedef";
                    parts.Add($"{k.Name} ({RoleInfo.Name(k.Role)}) {verb}: {P(t).Name}");
                }
            }
            return parts.Count == 0 ? "Kumpas henüz seçim yapmadı." : "Kumpas: " + string.Join("; ", parts);
        }

        public bool SetNightTarget(int playerId, int target)
        {
            var p = P(playerId);
            if (p == null || Phase != Phase.Night) return false;
            var options = TargetOptions(p, out string mode, out _);
            if (mode != PromptModes.Target || !options.Contains(target)) return false;
            nightTarget[playerId] = target;
            return true;
        }

        public bool SetMuneccimQuestion(int playerId, int deadId, int question)
        {
            var p = P(playerId);
            if (p == null || Phase != Phase.Night) return false;
            var options = TargetOptions(p, out string mode, out _);
            if (mode != PromptModes.Muneccim) return false;
            if (deadId >= 0 && !options.Contains(deadId)) return false;
            int newDead = deadId >= 0 ? deadId : munDead;
            int newQuestion = question >= 0 && question < Questions.All.Length ? question : (munQuestion >= 0 ? munQuestion : 0);
            if (newDead != munDead || newQuestion != munQuestion)
            {
                // Soru ya da ölü değişince eski yanıt geçersizdir.
                deadAnswered = false;
                deadAnswerPlayer = -1;
                deadAnswerVerdict = -1;
            }
            munDead = newDead;
            munQuestion = newQuestion;
            return true;
        }

        public bool SetClaim(int playerId, int roleIndex)
        {
            if (Phase != Phase.Night || playerId != jailed) return false;
            if (roleIndex < 0 || roleIndex >= RoleInfo.All.Length) return false;
            jailClaim = roleIndex;
            return true;
        }

        public bool SetExecute(int playerId, bool execute)
        {
            var p = P(playerId);
            if (p == null || !p.Alive || p.Role != Role.Kadi || Phase != Phase.Night || jailed < 0) return false;
            if (execute && (p.Executions <= 0 || p.ExecutionLost)) return false;
            executeChoice = execute;
            return true;
        }

        public bool Confirm(int playerId)
        {
            var p = P(playerId);
            if (p == null || !p.Alive || Phase != Phase.Night) return false;
            confirmed.Add(playerId);
            return true;
        }

        public bool Answer(int deadId, int answerPlayer, int verdict)
        {
            var d = P(deadId);
            if (d == null || d.Alive || Phase != Phase.Night || deadId != munDead) return false;
            if (P(answerPlayer) == null) return false;
            deadAnswerPlayer = answerPlayer;
            deadAnswerVerdict = Questions.WantsVerdict(munQuestion) ? (verdict == 1 ? 1 : 0) : -1;
            deadAnswered = true;
            return true;
        }

        // ---------------------------------------------------------------- Gece çözümü

        public void ResolveNight()
        {
            if (Phase != Phase.Night) return;
            morningPublic.Clear();
            foreach (var list in morningPrivate.Values) list.Clear();

            var blocked = new HashSet<int>();
            var visits = new Dictionary<int, List<int>>();
            var attacks = new List<Attack>();
            Func<Player, bool> free = p => p != null && p.Alive && p.Id != jailed && !blocked.Contains(p.Id) && !p.SuicidePending;
            Func<Player, int> targetOf = p => p != null && nightTarget.TryGetValue(p.Id, out int t) && P(t) != null && P(t).Alive ? t : -1;
            Action<int, int> visit = (who, whom) =>
            {
                if (!visits.TryGetValue(whom, out var l)) visits[whom] = l = new List<int>();
                if (!l.Contains(who)) l.Add(who);
            };

            if (jailed >= 0) Note(jailed, "Geceyi Kadı'nın zindanında geçirdin.");

            // 2. Çengi (engellenemez; zindandaysa çalışmaz)
            var cengi = Alive.FirstOrDefault(p => p.Role == Role.Cengi);
            if (cengi != null && cengi.Id != jailed)
            {
                int t = targetOf(cengi);
                if (t >= 0)
                {
                    visit(cengi.Id, t);
                    var tp = P(t);
                    if (tp.Role == Role.Yamyam)
                    {
                        if (tp.Id != jailed)
                        {
                            attacks.Add(new Attack(tp.Id, cengi.Id, AttackKind.Yamyam));
                            visit(tp.Id, cengi.Id);
                        }
                    }
                    else if (tp.Role != Role.Kadi)
                    {
                        blocked.Add(t);
                        Note(t, "Biri seni oyaladı; bu gece yeteneğini kullanamadın.");
                    }
                }
            }

            // 3. Müfteri
            int framed = -1;
            var mufteri = Alive.FirstOrDefault(p => p.Role == Role.Mufteri);
            if (mufteri != null && !IsLastKumpasMufteri(mufteri) && free(mufteri))
            {
                int t = targetOf(mufteri);
                if (t >= 0) { framed = t; visit(mufteri.Id, t); }
            }

            // 4. Hekim
            int healed = -1;
            var hekim = Alive.FirstOrDefault(p => p.Role == Role.Hekim);
            if (hekim != null && free(hekim))
            {
                int t = targetOf(hekim);
                var tp = P(t);
                if (tp != null && !(tp.Role == Role.Aga && tp.Revealed) && (t != hekim.Id || hekim.SelfHeals > 0))
                {
                    healed = t;
                    if (t == hekim.Id) hekim.SelfHeals--;
                    else visit(hekim.Id, t);
                }
            }

            // 5. Subaşı
            var subasi = Alive.FirstOrDefault(p => p.Role == Role.Subasi);
            if (subasi != null && free(subasi))
            {
                int t = targetOf(subasi);
                if (t >= 0)
                {
                    visit(subasi.Id, t);
                    bool suspicious = RoleInfo.LooksSuspicious(P(t).Role) || framed == t;
                    Note(subasi.Id, $"{P(t).Name} araştırmanın sonucu: {(suspicious ? "Şüpheli" : "Masum")}.");
                }
            }

            // 6–7. Kumpas saldırısı
            var zorba = Alive.FirstOrDefault(p => p.Role == Role.Zorba);
            var kidemli = KidemliFedai();
            var lastMufteri = mufteri != null && IsLastKumpasMufteri(mufteri) ? mufteri : null;
            int kumpasTarget = -1;
            if (zorba != null && free(zorba) && targetOf(zorba) >= 0) kumpasTarget = targetOf(zorba);
            else if (kidemli != null && targetOf(kidemli) >= 0) kumpasTarget = targetOf(kidemli);
            else if (zorba == null && kidemli == null && lastMufteri != null) kumpasTarget = targetOf(lastMufteri);

            Player kumpasAttacker = null;
            if (kidemli != null) kumpasAttacker = free(kidemli) ? kidemli : null;
            else if (zorba != null) kumpasAttacker = free(zorba) ? zorba : null;
            else if (lastMufteri != null) kumpasAttacker = free(lastMufteri) ? lastMufteri : null;

            if (kumpasAttacker != null && kumpasTarget >= 0 && !P(kumpasTarget).IsKumpas)
            {
                attacks.Add(new Attack(kumpasAttacker.Id, kumpasTarget, AttackKind.Kumpas));
                visit(kumpasAttacker.Id, kumpasTarget);
            }

            // Kabadayı
            var kabadayi = Alive.FirstOrDefault(p => p.Role == Role.Kabadayi);
            if (kabadayi != null)
            {
                if (kabadayi.SuicidePending)
                {
                    attacks.Add(new Attack(kabadayi.Id, kabadayi.Id, AttackKind.Suicide));
                    kabadayi.SuicidePending = false;
                }
                else if (Round > 1 && kabadayi.Shots > 0 && free(kabadayi))
                {
                    int t = targetOf(kabadayi);
                    if (t >= 0)
                    {
                        kabadayi.Shots--;
                        attacks.Add(new Attack(kabadayi.Id, t, AttackKind.Kabadayi));
                        visit(kabadayi.Id, t);
                    }
                }
            }

            // Yamyam (engellenemez; zindandaysa saldıramaz)
            var yamyam = Alive.FirstOrDefault(p => p.Role == Role.Yamyam);
            if (yamyam != null && yamyam.Id != jailed)
            {
                int t = targetOf(yamyam);
                if (t >= 0)
                {
                    attacks.Add(new Attack(yamyam.Id, t, AttackKind.Yamyam));
                    visit(yamyam.Id, t);
                }
            }

            // Kadı'nın infazı
            var kadi = Alive.FirstOrDefault(p => p.Role == Role.Kadi);
            if (kadi != null && jailed >= 0 && executeChoice && kadi.Executions > 0 && !kadi.ExecutionLost)
            {
                kadi.Executions--;
                attacks.Add(new Attack(kadi.Id, jailed, AttackKind.Execution));
            }

            // Saldırıların sonucu (hepsi aynı anda)
            var deaths = new Dictionary<int, List<AttackKind>>();
            var savedByHekim = false;
            foreach (var a in attacks)
            {
                bool dies;
                if (a.Kind == AttackKind.Execution || a.Kind == AttackKind.Suicide) dies = true;
                else if (a.Target == jailed) dies = false;
                else if (a.Target == healed) { dies = false; savedByHekim = true; }
                else dies = true;

                if (dies)
                {
                    if (!deaths.TryGetValue(a.Target, out var kinds)) deaths[a.Target] = kinds = new List<AttackKind>();
                    if (!kinds.Contains(a.Kind)) kinds.Add(a.Kind);
                }
            }

            if (hekim != null && healed >= 0)
            {
                bool healedDied = deaths.ContainsKey(healed);
                if (savedByHekim && !healedDied)
                {
                    Note(hekim.Id, healed == hekim.Id ? "Gece saldırıya uğradın; kendini korudun." : $"{P(healed).Name} gece saldırıya uğradı; onu kurtardın.");
                    if (healed != hekim.Id) Note(healed, "Gece saldırıya uğradın ama bir hekim seni kurtardı.");
                }
                else if (!healedDied)
                {
                    Note(hekim.Id, $"Koruduğun {P(healed).Name} bu gece saldırıya uğramadı.");
                }
            }

            foreach (var a in attacks)
            {
                if (a.Kind == AttackKind.Kabadayi && deaths.ContainsKey(a.Target))
                {
                    Note(kabadayi.Id, $"{P(a.Target).Name} kurşununla öldü.");
                    if (P(a.Target).Faction == Faction.Kasaba && !deaths.ContainsKey(kabadayi.Id))
                    {
                        kabadayi.SuicidePending = true;
                        Note(kabadayi.Id, "Öldürdüğün kişi Kasaba'dandı. Vicdan azabı seni ertesi gece bitirecek.");
                    }
                }
                if (a.Kind == AttackKind.Execution)
                {
                    Note(kadi.Id, $"{P(a.Target).Name} kişisini infaz ettin.");
                    if (P(a.Target).Faction == Faction.Kasaba)
                    {
                        kadi.ExecutionLost = true;
                        Note(kadi.Id, "Masum bir Kasaba üyesini infaz ettin; infaz hakkını kaybettin.");
                    }
                }
            }

            // 8. Gözcü
            var gozcu = Alive.FirstOrDefault(p => p.Role == Role.Gozcu);
            if (gozcu != null && free(gozcu))
            {
                int t = targetOf(gozcu);
                if (t >= 0)
                {
                    var seen = visits.TryGetValue(t, out var l) ? l.Where(v => v != gozcu.Id).Select(v => P(v).Name).ToList() : new List<string>();
                    Note(gozcu.Id, seen.Count == 0
                        ? $"{P(t).Name} kişisini gözetledin; kimse ziyaret etmedi."
                        : $"{P(t).Name} kişisini gözetledin. Ziyaret edenler: {string.Join(", ", seen)}.");
                }
            }

            // Müneccim
            var muneccim = Alive.FirstOrDefault(p => p.Role == Role.Muneccim);
            if (muneccim != null && free(muneccim) && munDead >= 0 && munQuestion >= 0)
            {
                string q = Questions.All[munQuestion];
                if (deadAnswered && P(deadAnswerPlayer) != null)
                {
                    string verdict = deadAnswerVerdict == 1 ? " — Şüpheli" : deadAnswerVerdict == 0 ? " — Masum" : "";
                    Note(muneccim.Id, $"{P(munDead).Name} ölüsüne sordun: \"{q}\" Yanıtı: {P(deadAnswerPlayer).Name}{verdict}.");
                }
                else
                {
                    Note(muneccim.Id, $"{P(munDead).Name} ölüsüne sordun: \"{q}\" Yanıt gelmedi.");
                }
            }

            // 9. Ölümler
            foreach (var kv in deaths)
            {
                var dp = P(kv.Key);
                dp.Alive = false;
                dp.DeathText = $"{Round}. gece: {CauseText(kv.Value)}";
                string line = $"{dp.Name} öldü ({CauseText(kv.Value)}). Rolü: {RoleInfo.Name(dp.Role)}.";
                morningPublic.Add(line);
                Public($"{Round}. gece: {line}");
                if (dp.Role == Role.Maskara) Note(dp.Id, "Gece öldün; Maskara olarak kaybettin.");
            }
            if (deaths.Count == 0)
            {
                morningPublic.Add("Bu gece kimse ölmedi.");
                Public($"{Round}. gece: kimse ölmedi.");
            }
            else deathThisRound = true;

            ConvertKanli(deaths.Keys);

            jailed = -1;
            if (CheckGameOver(false)) return;
            Phase = Phase.Morning;
        }

        static string CauseText(List<AttackKind> kinds)
        {
            var parts = kinds.Select(k =>
            {
                switch (k)
                {
                    case AttackKind.Kumpas: return "Kumpas tarafından öldürüldü";
                    case AttackKind.Kabadayi: return "Kabadayı'nın kurşunuyla öldü";
                    case AttackKind.Yamyam: return "Yamyam tarafından parçalandı";
                    case AttackKind.Execution: return "Kadı tarafından infaz edildi";
                    default: return "vicdan azabıyla canına kıydı";
                }
            });
            return string.Join("; ", parts);
        }

        void ConvertKanli(IEnumerable<int> diedNotByVote)
        {
            var died = new HashSet<int>(diedNotByVote);
            foreach (var kanli in Alive.Where(p => p.Role == Role.Kanli && !p.PersonalWin))
            {
                if (died.Contains(kanli.KanliTarget))
                {
                    kanli.Role = Role.Maskara;
                    Note(kanli.Id, "Hedefin idam edilmeden öldü. Artık Maskara'sın: amacın gündüz idam edilmek.");
                }
            }
        }

        // ---------------------------------------------------------------- Gündüz

        public bool RevealAga(int playerId)
        {
            var p = P(playerId);
            if (p == null || !p.Alive || p.Role != Role.Aga || p.Revealed) return false;
            if (Phase != Phase.Discussion && Phase != Phase.Voting) return false;
            p.Revealed = true;
            Public($"{Round}. gün: {p.Name} kendini Ağa olarak açıkladı. Oyu artık {Balance.AgaVotePower} sayılır.");
            return true;
        }

        public bool SetJail(int playerId, int target)
        {
            var p = P(playerId);
            if (p == null || !p.Alive || p.Role != Role.Kadi) return false;
            if (Phase != Phase.Discussion && Phase != Phase.Voting) return false;
            if (target == -1) { nextJail = -1; return true; }
            var t = P(target);
            if (t == null || !t.Alive || t.Id == p.Id) return false;
            nextJail = target;
            return true;
        }

        public int VotePower(Player p)
        {
            return p.Role == Role.Aga && p.Revealed ? Balance.AgaVotePower : 1;
        }

        public int TotalVotePower()
        {
            return Alive.Sum(VotePower);
        }

        public int Tally(int target)
        {
            return votes.Where(kv => kv.Value == target && P(kv.Key).Alive).Sum(kv => VotePower(P(kv.Key)));
        }

        public bool Vote(int voter, int target)
        {
            var p = P(voter);
            if (p == null || !p.Alive || Phase != Phase.Voting) return false;
            if (target == -1) { votes.Remove(voter); return true; }
            var t = P(target);
            if (t == null || !t.Alive || t.Id == voter) return false;
            votes[voter] = target;
            return true;
        }

        public void ResolveVoting()
        {
            if (Phase != Phase.Voting) return;
            int total = TotalVotePower();
            int executed = -1;
            foreach (var p in Alive)
            {
                if (Tally(p.Id) * 2 > total) { executed = p.Id; break; }
            }

            if (executed >= 0)
            {
                var ep = P(executed);
                ep.Alive = false;
                ep.DeathText = $"{Round}. gün: oylamayla idam edildi";
                deathThisRound = true;
                Public($"{Round}. gün: {ep.Name} oylamayla idam edildi. Rolü: {RoleInfo.Name(ep.Role)}.");
                if (ep.Role == Role.Maskara)
                {
                    ep.PersonalWin = true;
                    Public($"{ep.Name} Maskara'ydı ve idam edilerek kazandı!");
                }
                foreach (var kanli in Alive.Where(k => k.Role == Role.Kanli && k.KanliTarget == executed))
                {
                    kanli.PersonalWin = true;
                    privateLog[kanli.Id].Add($"{Round}. gün: hedefin idam edildi, kazandın!");
                    Public($"Bir Kanlı, hedefi {ep.Name} idam edildiği için kazandı!");
                }
            }
            else
            {
                Public($"{Round}. gün: oylamada kimse idam edilmedi.");
            }

            if (CheckGameOver(true)) return;
            Round++;
            BeginNight();
        }

        // ---------------------------------------------------------------- Oyun sonu

        bool CheckGameOver(bool endOfRound)
        {
            int kasaba = Alive.Count(p => p.Faction == Faction.Kasaba);
            int kumpas = Alive.Count(p => p.Faction == Faction.Kumpas);
            int yamyam = Alive.Count(p => p.Role == Role.Yamyam);
            int groups = (kasaba > 0 ? 1 : 0) + (kumpas > 0 ? 1 : 0) + (yamyam > 0 ? 1 : 0);

            if (endOfRound)
            {
                roundsWithoutDeath = deathThisRound ? 0 : roundsWithoutDeath + 1;
            }

            string title = null;
            Func<Player, bool> mainWinner = null;
            if (groups == 1)
            {
                if (kasaba > 0) { title = "Kasaba kazandı!"; mainWinner = p => p.Faction == Faction.Kasaba; }
                else if (kumpas > 0) { title = "Kumpas kazandı!"; mainWinner = p => p.Faction == Faction.Kumpas; }
                else { title = "Yamyam kazandı!"; mainWinner = p => p.Role == Role.Yamyam; }
            }
            else if (groups == 0)
            {
                title = "Berabere: kimse ayakta kalmadı.";
            }
            else if (endOfRound && roundsWithoutDeath >= Balance.NoDeathRoundsForDraw)
            {
                title = $"Berabere: {Balance.NoDeathRoundsForDraw} tur boyunca kimse ölmedi.";
            }

            if (title == null) return false;

            ResultTitle = title;
            Winners.Clear();
            foreach (var p in Players)
            {
                bool win = (mainWinner != null && mainWinner(p))
                           || (p.Role == Role.Dervis && p.Alive)
                           || p.PersonalWin;
                if (win) Winners.Add(p.Id);
            }
            Public(title);
            Phase = Phase.GameOver;
            return true;
        }

        // ---------------------------------------------------------------- Görünüm

        public static string PhaseTitle(Phase phase, int round)
        {
            switch (phase)
            {
                case Phase.Night: return $"{round}. Gece";
                case Phase.Morning: return $"{round}. Sabah";
                case Phase.Discussion: return $"{round}. Gün — Tartışma";
                case Phase.Voting: return $"{round}. Gün — Oylama";
                default: return "Oyun bitti";
            }
        }

        public PlayerView BuildView(int playerId)
        {
            var me = P(playerId);
            var v = new PlayerView
            {
                you = playerId,
                phase = (int)Phase,
                round = Round,
                phaseTitle = PhaseTitle(Phase, Round),
                over = Phase == Phase.GameOver,
                resultTitle = ResultTitle
            };
            v.rolesInGame.AddRange(rolesInGame);
            v.publicLog.AddRange(publicLog);
            v.morningPublic.AddRange(morningPublic);
            if (me == null) return v;

            bool showAll = Phase == Phase.GameOver;
            foreach (var p in Players)
            {
                var e = new PlayerEntry { id = p.Id, name = p.Name, alive = p.Alive, you = p.Id == playerId, agaRevealed = p.Revealed };
                bool knowsRole = showAll || !p.Alive || p.Id == playerId || (p.Role == Role.Aga && p.Revealed) || (me.IsKumpas && p.IsKumpas);
                if (knowsRole) e.role = RoleInfo.Name(p.Role);
                e.teammate = me.IsKumpas && p.IsKumpas && p.Id != playerId;
                if (Phase == Phase.Voting && p.Alive) e.votes = Tally(p.Id);
                v.players.Add(e);
            }

            v.alive = me.Alive;
            v.myRole = (int)me.Role;
            v.myRoleName = RoleInfo.Name(me.Role);
            v.myFaction = RoleInfo.FactionName(me.Faction);
            v.myDescription = RoleInfo.Description(me.Role);
            v.myExtra = ExtraInfo(me);
            v.myLog.AddRange(privateLog[playerId]);
            v.morningPrivate.AddRange(morningPrivate[playerId]);

            if (Phase == Phase.Night) FillNight(me, v);

            if ((Phase == Phase.Discussion || Phase == Phase.Voting) && me.Alive)
            {
                v.canReveal = me.Role == Role.Aga && !me.Revealed;
                if (me.Role == Role.Kadi)
                {
                    v.canJail = true;
                    v.jailOptions = Alive.Where(o => o.Id != me.Id).Select(o => o.Id).ToList();
                    v.jailSelected = nextJail;
                }
            }

            if (Phase == Phase.Voting)
            {
                v.votePowerTotal = TotalVotePower();
                v.votesNeeded = v.votePowerTotal / 2 + 1;
                if (me.Alive)
                {
                    v.voteOptions = Alive.Where(o => o.Id != me.Id).Select(o => o.Id).ToList();
                    v.myVote = votes.TryGetValue(me.Id, out int t) ? t : -1;
                }
            }

            if (showAll)
            {
                foreach (var id in Winners) v.winners.Add(P(id).Name);
                foreach (var p in Players)
                {
                    string role = RoleInfo.Name(p.Role);
                    if (p.StartRole != p.Role) role = $"{RoleInfo.Name(p.StartRole)} → {role}";
                    v.finalRoles.Add($"{p.Name}: {role}{(p.Alive ? "" : " (öldü)")}{(Winners.Contains(p.Id) ? " ★" : "")}");
                }
            }
            return v;
        }

        string ExtraInfo(Player me)
        {
            var lines = new List<string>();
            if (me.IsKumpas)
            {
                var mates = Players.Where(p => p.IsKumpas && p.Id != me.Id).Select(p => $"{p.Name} ({RoleInfo.Name(p.Role)}{(p.Alive ? "" : ", öldü")})").ToList();
                lines.Add(mates.Count > 0 ? "Kumpas arkadaşların: " + string.Join(", ", mates) : "Kumpas'ta yalnızsın.");
                if (me.Role == Role.Fedai)
                {
                    var k = KidemliFedai();
                    if (Players.Count(p => p.Role == Role.Fedai) > 1)
                        lines.Add(k == me ? "Kıdemli Fedai sensin: saldırıyı sen yaparsın." : "Yedek Fedai'sin.");
                }
            }
            if (me.Role == Role.Kanli && P(me.KanliTarget) != null) lines.Add($"Hedefin: {P(me.KanliTarget).Name}");
            if (me.Role == Role.Kadi) lines.Add(me.ExecutionLost ? "İnfaz hakkını kaybettin." : $"Kalan infaz hakkı: {me.Executions}");
            if (me.Role == Role.Kabadayi) lines.Add($"Kalan saldırı hakkı: {me.Shots}");
            if (me.Role == Role.Hekim) lines.Add($"Kendini koruma hakkı: {me.SelfHeals}");
            if (me.StartRole == Role.Kanli && me.Role == Role.Maskara) lines.Add("Kanlı'yken hedefin öldü; artık Maskara'sın.");
            if (me.PersonalWin) lines.Add("Kişisel zaferini kazandın!");
            return string.Join("\n", lines);
        }

        void FillNight(Player me, PlayerView v)
        {
            if (!me.Alive)
            {
                if (me.Id == munDead && munQuestion >= 0)
                {
                    v.promptMode = PromptModes.Answer;
                    v.promptText = $"Müneccim sana soruyor: \"{Questions.All[munQuestion]}\" Bir oyuncu seç.";
                    v.options = Players.Where(p => p.Id != me.Id).Select(p => p.Id).ToList();
                    v.selected = deadAnswered ? deadAnswerPlayer : -1;
                    if (Questions.WantsVerdict(munQuestion))
                    {
                        v.choices2 = new List<string> { "Masum", "Şüpheli" };
                        v.selected2 = deadAnswerVerdict;
                    }
                    v.confirmed = deadAnswered;
                }
                return;
            }

            var options = TargetOptions(me, out string mode, out string text);
            v.promptMode = mode;
            v.promptText = text;
            v.options = options;
            v.confirmed = confirmed.Contains(me.Id);

            switch (mode)
            {
                case PromptModes.Target:
                    v.selected = nightTarget.TryGetValue(me.Id, out int t) ? t : -1;
                    break;
                case PromptModes.Claim:
                    v.choices2 = RoleInfo.All.Select(RoleInfo.Name).ToList();
                    v.selected2 = jailClaim;
                    break;
                case PromptModes.Execute:
                    v.executeOn = executeChoice;
                    v.selected = jailed;
                    break;
                case PromptModes.Muneccim:
                    v.selected = munDead;
                    v.choices2 = Questions.All.ToList();
                    v.selected2 = munQuestion;
                    break;
            }
        }
    }
}
