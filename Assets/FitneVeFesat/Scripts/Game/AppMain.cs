using System;
using System.Collections.Generic;
using System.Linq;
using FitneFesat.Core;
using FitneFesat.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FitneFesat.Game
{
    // Uygulamanın tek sahnesindeki ana nesne: menü, lobi ve oyun ekranları, ağ bağlantısı.
    //
    // Deneme için komut satırı (Windows):
    //   -ad <isim>        oyuncu adı
    //   -oda-kur          açılışta oda kur
    //   -katil <adres>    açılışta bu adrese katıl (ör. 127.0.0.1)
    //   -pencere x,y      pencereyi ekranda bu konuma taşı
    //
    // Yalnız geliştirme sınaması için (oyunun parçası değildir):
    //   -otomatik         bu kopya seçimlerini rastgele yapar; oyun bitince kapanır
    //   -otomatik-basla N oda sahibi N oyuncu olunca oyunu kendisi başlatır
    //   -hizli            süreler kısalır (oda sahibinde)
    //   -ekran-goruntusu <klasör>  her ekran değişiminde görüntü kaydeder
    public class AppMain : MonoBehaviour
    {
        enum Page { Menu, Lobby, Game }

        public ArtLibrary art;

        Page page = Page.Menu;
        HostSession host;
        GameClient client;
        readonly RoomFinder finder = new RoomFinder();
        bool isHost;
        LobbyView lobby;
        PlayerView view;
        float viewAt;
        string viewKey = "";
        string message = "";
        bool dirty = true;
        string playerName = "";

        // Arayüz
        RectTransform root;
        RectTransform pageRoot;
        RectTransform overlayRoot;
        Image background;
        Text phaseText;
        Text timerText;
        RectTransform body;
        ScrollRect bodyScroll;
        RectTransform roomsBox;
        InputField nameInput;
        InputField addressInput;
        Text menuMessage;
        string roomsKey;
        float nextRoomsCheck;
        Page builtPage = (Page)(-1);
        int dayAnswerSelected = -1;

        // Geliştirme sınaması
        bool autoPlay;
        bool fast;
        int autoStartCount = 6;
        bool autoStarted;
        string autoActed = "";
        string loggedPhase = "";
        readonly System.Random autoRng = new System.Random(Environment.TickCount);
        string shotDir;
        int shotIndex;
        bool roleShotTaken;
        string renderedPhase = "";

        const string PrefName = "oyuncu_adi";
        const string PrefAddress = "son_adres";

        // ------------------------------------------------------------ Yaşam döngüsü

        void Awake()
        {
            Application.targetFrameRate = 30;
            Application.runInBackground = true;
            UnityEngine.Screen.sleepTimeout = SleepTimeout.NeverSleep;
            playerName = PlayerPrefs.GetString(PrefName, "");
            BuildCanvas();
        }

        void Start()
        {
            var args = Environment.GetCommandLineArgs();
            string argName = Arg(args, "-ad");
            if (!string.IsNullOrEmpty(argName)) playerName = argName;
            autoPlay = args.Contains("-otomatik");
            fast = args.Contains("-hizli");
            shotDir = Arg(args, "-ekran-goruntusu");
            if (int.TryParse(Arg(args, "-otomatik-basla"), out int n)) autoStartCount = n;

            string pos = Arg(args, "-pencere");
            if (!string.IsNullOrEmpty(pos))
            {
                var parts = pos.Split(',');
                if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                    UnityEngine.Screen.MoveMainWindowTo(UnityEngine.Screen.mainWindowDisplayInfo, new Vector2Int(x, y));
            }

            finder.Start();
            if (args.Contains("-oda-kur")) CreateRoom();
            else
            {
                string join = Arg(args, "-katil");
                if (!string.IsNullOrEmpty(join)) JoinRoom(join, NetConfig.TcpPort, true);
            }
        }

        static string Arg(string[] args, string key)
        {
            int i = Array.IndexOf(args, key);
            return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
        }

        void Update()
        {
            host?.Tick(Time.realtimeSinceStartup);
            ReadClient();

            if (page == Page.Menu && Time.realtimeSinceStartup >= nextRoomsCheck)
            {
                nextRoomsCheck = Time.realtimeSinceStartup + 0.5f;
                RefreshRooms();
            }

            if (dirty)
            {
                dirty = false;
                Render();
                Shot();
            }

            if (page == Page.Game && timerText != null && view != null)
            {
                timerText.text = view.over ? "" : Ui.Clock(view.secondsLeft - (Time.realtimeSinceStartup - viewAt));
            }
        }

        void OnApplicationQuit()
        {
            Shutdown();
        }

        void OnDestroy()
        {
            Shutdown();
        }

        void Shutdown()
        {
            client?.Close();
            client = null;
            host?.Stop();
            host = null;
            finder.Stop();
        }

        // ------------------------------------------------------------ Ağ

        void CreateRoom()
        {
            if (!SaveName()) return;
            try
            {
                host = new HostSession();
                if (fast)
                {
                    host.Balance.NightSeconds = 3f;
                    host.Balance.MorningSeconds = 1f;
                    host.Balance.DiscussionSeconds = 2f;
                    host.Balance.VotingSeconds = 3f;
                }
                host.Start($"{playerName} odası");
                ConnectClient("127.0.0.1", host.Port, host.Token);
            }
            catch (Exception e)
            {
                host?.Stop();
                host = null;
                ShowMenu("Oda kurulamadı: " + e.Message);
            }
        }

        void JoinRoom(string address, int port, bool fromArgs = false)
        {
            if (!fromArgs && !SaveName()) return;
            if (string.IsNullOrWhiteSpace(playerName)) playerName = "Oyuncu";
            try
            {
                ConnectClient(address.Trim(), port, "");
                PlayerPrefs.SetString(PrefAddress, address.Trim());
            }
            catch (Exception e)
            {
                ShowMenu(e.Message);
            }
        }

        void ConnectClient(string address, int port, string token)
        {
            client = new GameClient();
            client.Connect(address, port);
            client.Send(PacketTypes.Hello, new Hello { name = playerName, token = token });
            finder.Stop();
            message = "";
        }

        bool SaveName()
        {
            if (nameInput != null) playerName = nameInput.text.Trim();
            if (string.IsNullOrEmpty(playerName))
            {
                ShowMenu("Önce adını yaz.");
                return false;
            }
            PlayerPrefs.SetString(PrefName, playerName);
            return true;
        }

        void Leave(string why = "")
        {
            client?.Close();
            client = null;
            host?.Stop();
            host = null;
            lobby = null;
            view = null;
            isHost = false;
            finder.Start();
            ShowMenu(why);
        }

        void ReadClient()
        {
            if (client == null) return;
            while (client.TryReceive(out var p))
            {
                switch (p.t)
                {
                    case PacketTypes.Welcome:
                        isHost = JsonUtility.FromJson<Welcome>(p.body).isHost;
                        break;
                    case PacketTypes.Lobby:
                        lobby = JsonUtility.FromJson<LobbyView>(p.body);
                        view = null;
                        page = Page.Lobby;
                        dirty = true;
                        if (autoPlay && isHost && !autoStarted && lobby.players.Count >= autoStartCount)
                        {
                            autoStarted = true;
                            Send(Commands.Start);
                        }
                        break;
                    case PacketTypes.View:
                        var v = JsonUtility.FromJson<PlayerView>(p.body);
                        float left = v.secondsLeft;
                        v.secondsLeft = 0;
                        string key = JsonUtility.ToJson(v);
                        v.secondsLeft = left;
                        viewAt = Time.realtimeSinceStartup;
                        if (view == null || view.phase != v.phase || view.round != v.round) dayAnswerSelected = -1;
                        view = v;
                        LogPhase();
                        if (autoPlay)
                        {
                            if (!IsInvoking(nameof(AutoAct))) Invoke(nameof(AutoAct), 0.8f);
                        }
                        if (key != viewKey || page != Page.Game)
                        {
                            viewKey = key;
                            page = Page.Game;
                            dirty = true;
                        }
                        break;
                    case PacketTypes.Error:
                        string text = JsonUtility.FromJson<ErrorMessage>(p.body).text;
                        if (lobby == null && view == null) { Leave(text); return; }
                        message = text;
                        dirty = true;
                        break;
                }
            }
            if (client != null && !client.Connected) Leave("Oda ile bağlantı koptu.");
        }

        void LogPhase()
        {
            string key = view.phaseTitle + (view.over ? " " + view.resultTitle : "");
            if (key == loggedPhase) return;
            loggedPhase = key;
            Debug.Log($"[Fitne&Fesat] {playerName} ({view.myRoleName}, {(view.alive ? "sağ" : "ölü")}): {view.phaseTitle}");
            foreach (var line in view.morningPrivate) Debug.Log($"[Fitne&Fesat] {playerName} not: {line}");
            if (view.over)
            {
                Debug.Log($"[Fitne&Fesat] OYUN BİTTİ: {view.resultTitle} Kazananlar: {string.Join(", ", view.winners)}");
                foreach (var line in view.publicLog) Debug.Log($"[Fitne&Fesat] günlük: {line}");
            }
        }

        // Geliştirme sınaması: rastgele seçim yapan kopya. Oyunun parçası değildir.
        void AutoAct()
        {
            var v = view;
            T Pick<T>(List<T> list) => list[autoRng.Next(list.Count)];
            string key = $"{v.round}-{v.phase}";

            if (v.over)
            {
                if (autoActed != "over")
                {
                    autoActed = "over";
                    Invoke(nameof(AutoQuit), 3f);
                }
                return;
            }

            if (v.phase == (int)Phase.Night)
            {
                if (v.alive && !v.confirmed && autoActed != key)
                {
                    autoActed = key;
                    Debug.Log($"[Fitne&Fesat] {playerName} otomatik gece: {v.promptMode} seçenek {v.options.Count}");
                    switch (v.promptMode)
                    {
                        case PromptModes.Target: if (v.options.Count > 0) Send(Commands.NightTarget, Pick(v.options)); break;
                        case PromptModes.Claim: Send(Commands.Claim, autoRng.Next(v.choices2.Count)); break;
                        case PromptModes.Execute: Send(Commands.Execute, autoRng.Next(2)); break;
                        case PromptModes.Muneccim:
                            Send(Commands.Muneccim, Pick(v.options), autoRng.Next(v.choices2.Count));
                            break;
                    }
                    Send(Commands.Confirm);
                }
                else if (!v.alive && v.promptMode == PromptModes.Answer && !v.confirmed && v.options.Count > 0)
                {
                    Send(Commands.Answer, Pick(v.options), autoRng.Next(2));
                }
                return;
            }

            if (!v.alive || autoActed == key) return;
            if (v.phase == (int)Phase.Discussion)
            {
                autoActed = key;
                if (v.canReveal && autoRng.Next(3) == 0) Send(Commands.Reveal);
                if (v.canJail && v.jailOptions.Count > 0) Send(Commands.Jail, Pick(v.jailOptions));
            }
            else if (v.phase == (int)Phase.Voting)
            {
                autoActed = key;
                if (v.voteOptions.Count > 0 && autoRng.Next(10) < 7) Send(Commands.Vote, Pick(v.voteOptions));
            }
        }

        // Geliştirme: ekran değişince bir sonraki karede görüntü kaydet.
        void Shot()
        {
            if (string.IsNullOrEmpty(shotDir)) return;
            if (!IsInvoking(nameof(TakeShot))) Invoke(nameof(TakeShot), 0.4f);
        }

        void TakeShot()
        {
            System.IO.Directory.CreateDirectory(shotDir);
            string label = page == Page.Game && view != null ? view.phaseTitle : page.ToString();
            foreach (char c in System.IO.Path.GetInvalidFileNameChars()) label = label.Replace(c, '_');
            ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(shotDir, $"{playerName}_{shotIndex++:00}_{label.Replace(' ', '_')}.png"));
            if (page == Page.Game && view != null && !roleShotTaken)
            {
                roleShotTaken = true;
                ShowRoleCard();
                Invoke(nameof(TakeRoleShot), 0.5f);
            }
        }

        void TakeRoleShot()
        {
            ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(shotDir, $"{playerName}_{shotIndex++:00}_Rol_karti.png"));
            Invoke(nameof(CloseOverlay), 0.3f);
        }

        void CloseOverlay()
        {
            Ui.Clear(overlayRoot);
        }

        void AutoQuit()
        {
            Application.Quit();
        }

        void Send(string kind, int a = -1, int b = -1)
        {
            client?.Send(PacketTypes.Command, new Command { kind = kind, a = a, b = b });
        }

        // ------------------------------------------------------------ Arayüz iskeleti

        void BuildCanvas()
        {
            if (FindAnyObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem", typeof(EventSystem), typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));
                DontDestroyOnLoad(es);
            }

            var canvasGo = new GameObject("Tuval", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            root = (RectTransform)canvasGo.transform;
            background = Ui.Panel(root, Ui.Gece, "Zemin");
            Ui.Fill(background.rectTransform);

            pageRoot = Ui.Rect("Sayfa", root);
            Ui.Fill(pageRoot);
            overlayRoot = Ui.Rect("Ustkatman", root);
            Ui.Fill(overlayRoot);
        }

        void ShowMenu(string why)
        {
            page = Page.Menu;
            message = why ?? "";
            builtPage = (Page)(-1);
            dirty = true;
        }

        void Render()
        {
            if (page != builtPage)
            {
                Ui.Clear(pageRoot);
                Ui.Clear(overlayRoot);
                phaseText = null;
                timerText = null;
                body = null;
                builtPage = page;
                if (page == Page.Menu) BuildMenu();
                else if (page == Page.Lobby) BuildBodyPage();
                else BuildGamePage();
            }

            if (page == Page.Menu)
            {
                menuMessage.text = message;
                roomsKey = null;
                RefreshRooms();
            }
            else if (page == Page.Lobby) RenderLobby();
            else RenderGame();
        }

        // ------------------------------------------------------------ Menü

        void BuildMenu()
        {
            background.color = Ui.Gece;
            if (art != null && art.menuBackground != null)
            {
                var bg = Ui.Panel(pageRoot, Color.white, "MenuResmi");
                bg.sprite = art.menuBackground;
                var fitter = bg.gameObject.AddComponent<AspectRatioFitter>();
                fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
                fitter.aspectRatio = art.menuBackground.rect.width / art.menuBackground.rect.height;
                var shade = Ui.Panel(pageRoot, new Color(0.05f, 0.07f, 0.15f, 0.8f), "Karartma");
                Ui.Fill(shade.rectTransform);
            }

            Ui.Scroll(pageRoot, out var col, 24, 60);
            Ui.Fill((RectTransform)col.parent.parent);

            Ui.Space(col, 80);
            if (art != null && art.titleBanner != null)
            {
                var banner = Ui.Picture(col, art.titleBanner, 300);
                var title = Ui.Label(banner.transform, "Fitne & Fesat", 96, Ui.Murekkep, TextAnchor.MiddleCenter, FontStyle.Bold);
                Ui.Fill(title.rectTransform);
            }
            else
            {
                Ui.Label(col, "Fitne & Fesat", 110, Ui.Altin, TextAnchor.MiddleCenter, FontStyle.Bold);
            }
            Ui.Label(col, "Aynı odada, yüz yüze; herkes kendi telefonundan.", 40, Ui.Krem, TextAnchor.MiddleCenter, FontStyle.Italic);
            Ui.Space(col, 40);

            Ui.Label(col, "Adın", 44, Ui.Altin);
            nameInput = Ui.Input(col, "Adını yaz", playerName);
            nameInput.characterLimit = 16;

            Ui.Button(col, "Oda kur", CreateRoom, Ui.Altin, Ui.Murekkep, 54, 150);
            Ui.Space(col, 30);

            Ui.Label(col, "Yakındaki odalar", 44, Ui.Altin);
            roomsBox = Ui.Column(col, 16);
            Ui.Space(col, 30);

            Ui.Label(col, "Ya da oda sahibinin adresini yaz", 40, Ui.Krem);
            addressInput = Ui.Input(col, "ör. 192.168.1.20", PlayerPrefs.GetString(PrefAddress, ""));
            Ui.Button(col, "Adrese bağlan", () => JoinRoom(addressInput.text, NetConfig.TcpPort), Ui.Lacivert, Ui.Krem, 46, 130);

            menuMessage = Ui.Label(col, message, 42, new Color32(0xFF, 0xB4, 0xA8, 0xFF), TextAnchor.MiddleCenter);
        }

        void RefreshRooms()
        {
            if (roomsBox == null) return;
            var rooms = finder.Rooms().OrderBy(r => r.Name).ToList();
            string key = string.Join(";", rooms.Select(r => $"{r.Name}@{r.Address}:{r.Port}"));
            if (key == roomsKey) return;
            roomsKey = key;
            Ui.Clear(roomsBox);
            if (rooms.Count == 0)
            {
                Ui.Label(roomsBox, "Aranıyor… Aynı Wi-Fi ağında olun.", 40, Ui.Krem, TextAnchor.MiddleLeft, FontStyle.Italic);
                return;
            }
            foreach (var r in rooms)
            {
                var room = r;
                Ui.Button(roomsBox, $"{room.Name}  <size=32>({room.Address})</size>", () => JoinRoom(room.Address, room.Port), Ui.Lacivert, Ui.Krem, 46, 130);
            }
        }

        // ------------------------------------------------------------ Lobi

        void BuildBodyPage()
        {
            background.color = Ui.Gece;
            bodyScroll = Ui.Scroll(pageRoot, out body, 24, 48);
            Ui.Fill((RectTransform)bodyScroll.transform);
        }

        void RenderLobby()
        {
            if (lobby == null) return;
            Ui.Clear(body);
            Ui.Space(body, 40);
            Ui.Label(body, lobby.room, 70, Ui.Altin, TextAnchor.MiddleCenter, FontStyle.Bold);
            if (isHost)
                Ui.Label(body, $"Oda adresi: {lobby.hostAddress}\nArkadaşların aynı Wi-Fi ağında odayı listede görür ya da bu adresi yazar.", 38, Ui.Krem, TextAnchor.MiddleCenter);
            Ui.Space(body, 20);

            int count = lobby.players.Count;
            Ui.Label(body, $"Oyuncular ({count}/{lobby.maxPlayers})", 48, Ui.Krem, TextAnchor.MiddleLeft, FontStyle.Bold);
            foreach (var p in lobby.players)
            {
                string line = p.name + (p.isHost ? "  <color=#C9A13B>(oda sahibi)</color>" : "") + (p.name == playerName ? "  <i>(sen)</i>" : "");
                var row = Ui.Panel(body, Ui.Lacivert, "Oyuncu");
                row.gameObject.AddComponent<LayoutElement>().preferredHeight = 110;
                var t = Ui.Label(row.transform, line, 46, Ui.Krem);
                Ui.Fill(t.rectTransform, 36, 0, 36, 0);
            }
            Ui.Space(body, 30);

            if (isHost)
            {
                bool ok = count >= lobby.minPlayers && count <= lobby.maxPlayers;
                Ui.Button(body, ok ? "Oyunu başlat" : $"En az {lobby.minPlayers} oyuncu gerekli", () => Send(Commands.Start),
                    ok ? Ui.Altin : Ui.Gri, Ui.Murekkep, 54, 160, ok);
            }
            else
            {
                Ui.Label(body, "Oda sahibinin oyunu başlatması bekleniyor…", 42, Ui.Krem, TextAnchor.MiddleCenter, FontStyle.Italic);
            }

            string msg = !string.IsNullOrEmpty(message) ? message : lobby.message;
            if (!string.IsNullOrEmpty(msg)) Ui.Label(body, msg, 40, new Color32(0xFF, 0xB4, 0xA8, 0xFF), TextAnchor.MiddleCenter);

            Ui.Space(body, 40);
            Ui.Button(body, isHost ? "Odayı kapat" : "Odadan çık", () => Leave(), Ui.Kirmizi, Ui.Krem, 44, 120);
        }

        // ------------------------------------------------------------ Oyun

        void BuildGamePage()
        {
            var header = Ui.Panel(pageRoot, new Color(0, 0, 0, 0.35f), "Baslik");
            var hrt = header.rectTransform;
            hrt.anchorMin = new Vector2(0, 1);
            hrt.anchorMax = new Vector2(1, 1);
            hrt.pivot = new Vector2(0.5f, 1);
            hrt.sizeDelta = new Vector2(0, 300);
            hrt.anchoredPosition = Vector2.zero;

            phaseText = Ui.Label(header.transform, "", 58, Ui.Altin, TextAnchor.MiddleLeft, FontStyle.Bold);
            var prt = phaseText.rectTransform;
            prt.anchorMin = new Vector2(0, 0.5f);
            prt.anchorMax = new Vector2(0.72f, 1);
            prt.offsetMin = new Vector2(40, 0);
            prt.offsetMax = new Vector2(0, -10);

            timerText = Ui.Label(header.transform, "", 80, Ui.Krem, TextAnchor.MiddleRight, FontStyle.Bold);
            var trt = timerText.rectTransform;
            trt.anchorMin = new Vector2(0.72f, 0.5f);
            trt.anchorMax = new Vector2(1, 1);
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = new Vector2(-40, -10);

            var buttons = Ui.Row(header.transform, 24, 110);
            var brt = buttons;
            brt.anchorMin = new Vector2(0, 0);
            brt.anchorMax = new Vector2(1, 0.5f);
            brt.offsetMin = new Vector2(40, 20);
            brt.offsetMax = new Vector2(-40, -10);
            Ui.Button(buttons, "Rolüm", ShowRoleCard, Ui.Altin, Ui.Murekkep, 44, 110);
            Ui.Button(buttons, "Günlük", ShowLog, Ui.Lacivert, Ui.Krem, 44, 110);

            bodyScroll = Ui.Scroll(pageRoot, out body, 24, 48);
            Ui.Fill((RectTransform)bodyScroll.transform, 0, 300, 0, 0);
        }

        bool Night => view != null && view.phase == (int)Phase.Night;
        bool Day => view != null && (view.phase == (int)Phase.Morning || view.phase == (int)Phase.Discussion || view.phase == (int)Phase.Voting);
        Color TextColor => Day ? Ui.Murekkep : Ui.Krem;
        Color ButtonColor => Day ? Ui.Krem : Ui.Lacivert;
        Color ButtonTextColor => Day ? Ui.Murekkep : Ui.Krem;

        string NameOf(int id)
        {
            var e = view.players.FirstOrDefault(p => p.id == id);
            return e != null ? e.name : "?";
        }

        void RenderGame()
        {
            if (view == null) return;
            // Aşama değişince sayfa başa döner; aynı aşamada kaydırma yeri korunur.
            string phaseKey = $"{view.round}-{view.phase}";
            float scroll = bodyScroll != null && phaseKey == renderedPhase ? bodyScroll.verticalNormalizedPosition : 1f;
            renderedPhase = phaseKey;
            background.color = Night ? Ui.Gece : Day ? Ui.Parsomen : Ui.Lacivert;
            phaseText.text = view.phaseTitle;
            phaseText.color = Day ? Ui.Kirmizi : Ui.Altin;
            timerText.color = TextColor;

            Ui.Clear(body);
            if (!view.alive && !view.over)
                Ui.Label(body, "Öldün. Oyunu izliyorsun; tartışmada konuşma.", 38, Ui.Kirmizi, TextAnchor.MiddleCenter, FontStyle.Italic);

            switch ((Phase)view.phase)
            {
                case Phase.Night: RenderNight(); break;
                case Phase.Morning: RenderMorning(); break;
                case Phase.Discussion: RenderDiscussion(); break;
                case Phase.Voting: RenderVoting(); break;
                default: RenderGameOver(); break;
            }

            if (!string.IsNullOrEmpty(message))
                Ui.Label(body, message, 38, Ui.Kirmizi, TextAnchor.MiddleCenter);

            Canvas.ForceUpdateCanvases();
            if (bodyScroll != null) bodyScroll.verticalNormalizedPosition = scroll;
        }

        Button PlayerButton(Transform parent, int id, string extra, bool selected, Action onClick, bool interactable = true)
        {
            string label = NameOf(id) + (string.IsNullOrEmpty(extra) ? "" : $"  <size=34>{extra}</size>");
            if (selected) label = "▶ " + label;
            return Ui.Button(parent, label, onClick == null ? null : new UnityEngine.Events.UnityAction(onClick),
                selected ? Ui.Altin : ButtonColor, selected ? Ui.Murekkep : ButtonTextColor, 46, 120, interactable);
        }

        void RenderNight()
        {
            if (!view.alive)
            {
                if (view.promptMode == PromptModes.Answer)
                {
                    Ui.Label(body, view.promptText, 46, Ui.Krem);
                    bool verdict = view.choices2.Count > 0;
                    foreach (int id in view.options)
                    {
                        int target = id;
                        PlayerButton(body, id, "", view.selected == id, () =>
                        {
                            dayAnswerSelected = target;
                            Send(Commands.Answer, target, verdict ? Math.Max(0, view.selected2) : -1);
                        });
                    }
                    if (verdict)
                    {
                        var row = Ui.Row(body);
                        for (int i = 0; i < view.choices2.Count; i++)
                        {
                            int choice = i;
                            int chosen = view.selected >= 0 ? view.selected : dayAnswerSelected;
                            Ui.Button(row, view.choices2[i], () => { if (chosen >= 0) Send(Commands.Answer, chosen, choice); },
                                view.selected2 == i ? Ui.Altin : Ui.Lacivert, view.selected2 == i ? Ui.Murekkep : Ui.Krem, 44, 120);
                        }
                    }
                    if (view.confirmed) Ui.Label(body, "Yanıtın iletildi. Gece bitene kadar değiştirebilirsin.", 38, Ui.Altin, TextAnchor.MiddleCenter, FontStyle.Italic);
                }
                else
                {
                    Ui.Label(body, "Gece sürüyor. Yaşayanlar seçimlerini yapıyor…", 44, Ui.Krem, TextAnchor.MiddleCenter, FontStyle.Italic);
                }
                return;
            }

            Ui.Label(body, "Gece çöktü. Kimse konuşmasın; herkes telefonuna baksın.", 36, Ui.Altin, TextAnchor.MiddleCenter, FontStyle.Italic);
            Ui.Label(body, view.promptText, 46, Ui.Krem);

            switch (view.promptMode)
            {
                case PromptModes.Target:
                    foreach (int id in view.options)
                    {
                        int target = id;
                        var e = view.players.First(p => p.id == id);
                        string extra = e.teammate ? $"<color=#E07A6F>[{e.role}]</color>" : (e.you ? "(sen)" : "");
                        PlayerButton(body, id, extra, view.selected == id, () => Send(Commands.NightTarget, target));
                    }
                    break;

                case PromptModes.Claim:
                    var grid = Ui.Grid(body, 2, 110);
                    for (int i = 0; i < view.choices2.Count; i++)
                    {
                        int role = i;
                        bool sel = view.selected2 == i;
                        Ui.Button(grid, view.choices2[i], () => Send(Commands.Claim, role), sel ? Ui.Altin : Ui.Lacivert, sel ? Ui.Murekkep : Ui.Krem, 42, 110);
                    }
                    break;

                case PromptModes.Execute:
                    var row = Ui.Row(body);
                    Ui.Button(row, "İnfaz et", () => Send(Commands.Execute, 1), view.executeOn ? Ui.Kirmizi : Ui.Lacivert, Ui.Krem, 46, 130);
                    Ui.Button(row, "Bağışla", () => Send(Commands.Execute, 0), !view.executeOn ? Ui.Altin : Ui.Lacivert, !view.executeOn ? Ui.Murekkep : Ui.Krem, 46, 130);
                    break;

                case PromptModes.Muneccim:
                    Ui.Label(body, "Ölüler", 42, Ui.Altin, TextAnchor.MiddleLeft, FontStyle.Bold);
                    foreach (int id in view.options)
                    {
                        int dead = id;
                        var e = view.players.First(p => p.id == id);
                        PlayerButton(body, id, $"({e.role})", view.selected == id, () => Send(Commands.Muneccim, dead, -1));
                    }
                    Ui.Label(body, "Soru", 42, Ui.Altin, TextAnchor.MiddleLeft, FontStyle.Bold);
                    for (int i = 0; i < view.choices2.Count; i++)
                    {
                        int q = i;
                        bool sel = view.selected2 == i;
                        Ui.Button(body, view.choices2[i], () => Send(Commands.Muneccim, -1, q), sel ? Ui.Altin : Ui.Lacivert, sel ? Ui.Murekkep : Ui.Krem, 40, 110);
                    }
                    break;
            }

            Ui.Space(body, 20);
            if (view.confirmed)
                Ui.Button(body, "Onaylandı ✓  Ötekiler bekleniyor", null, Ui.Gri, Ui.Krem, 44, 140, false);
            else
                Ui.Button(body, "Geceyi onayla", () => Send(Commands.Confirm), Ui.Yesil, Ui.Krem, 50, 150);
        }

        void RenderMorning()
        {
            Ui.Label(body, "Sabah oldu.", 56, Ui.Murekkep, TextAnchor.MiddleCenter, FontStyle.Bold);
            foreach (var line in view.morningPublic) Ui.Label(body, "• " + line, 46, Ui.Murekkep);
            if (view.morningPrivate.Count > 0)
            {
                Ui.Space(body, 20);
                Ui.Label(body, "Yalnız sana", 44, Ui.Kirmizi, TextAnchor.MiddleLeft, FontStyle.Bold);
                foreach (var line in view.morningPrivate) Ui.Label(body, "• " + line, 44, Ui.Murekkep);
            }
            Ui.Space(body, 20);
            Ui.Label(body, "Notlarını oku; tartışma birazdan başlıyor.", 36, Ui.Gri, TextAnchor.MiddleCenter, FontStyle.Italic);
        }

        void RenderDayActions()
        {
            if (view.canReveal)
                Ui.Button(body, "Kimliğimi açıkla: Ben Ağa'yım", () => Send(Commands.Reveal), Ui.Kirmizi, Ui.Krem, 46, 140);

            if (view.canJail)
            {
                Ui.Label(body, "Bu gece kimi zindana atacaksın? (Gizli; yalnız sen görürsün)", 40, Ui.Kirmizi, TextAnchor.MiddleLeft, FontStyle.Bold);
                foreach (int id in view.jailOptions)
                {
                    int target = id;
                    bool sel = view.jailSelected == id;
                    PlayerButton(body, id, "", sel, () => Send(Commands.Jail, sel ? -1 : target));
                }
            }
        }

        void RenderPlayerList()
        {
            foreach (var e in view.players)
            {
                string status;
                if (!e.alive) status = $"<color=#9E2B25>öldü — {e.role}</color>";
                else if (e.agaRevealed) status = "<color=#9E2B25>Ağa (açıkladı)</color>";
                else if (e.teammate) status = $"<color=#9E2B25>[{e.role}]</color>";
                else status = "";
                string you = e.you ? " <i>(sen)</i>" : "";
                string off = e.connected ? "" : " <color=#777777>(bağlantı yok)</color>";
                Ui.Label(body, $"{(e.alive ? "●" : "○")} {e.name}{you}  {status}{off}", 44, Ui.Murekkep);
            }
        }

        void RenderDiscussion()
        {
            Ui.Label(body, "Yüz yüze tartışın!", 60, Ui.Kirmizi, TextAnchor.MiddleCenter, FontStyle.Bold);
            Ui.Label(body, "Kimden şüpheleniyorsunuz? Süre bitince oylama başlar.", 38, Ui.Murekkep, TextAnchor.MiddleCenter, FontStyle.Italic);
            if (view.alive) RenderDayActions();
            Ui.Space(body, 10);
            RenderPlayerList();
        }

        void RenderVoting()
        {
            Ui.Label(body, $"İdam için {view.votesNeeded} oy gerekli (toplam oy gücü {view.votePowerTotal}).", 42, Ui.Murekkep, TextAnchor.MiddleCenter, FontStyle.Bold);
            if (view.alive)
            {
                Ui.Label(body, "Birini seç. Seçimini süre bitene kadar değiştirebilirsin.", 36, Ui.Murekkep, TextAnchor.MiddleCenter, FontStyle.Italic);
                foreach (int id in view.voteOptions)
                {
                    int target = id;
                    var e = view.players.First(p => p.id == id);
                    PlayerButton(body, id, $"{e.votes} oy", view.myVote == id, () => Send(Commands.Vote, target));
                }
                Ui.Button(body, view.myVote >= 0 ? "Oyumu geri çek (çekimser)" : "Çekimserim", () => Send(Commands.Vote, -1), Ui.Gri, Ui.Krem, 42, 120);
                RenderDayActions();
            }
            else
            {
                foreach (var e in view.players.Where(p => p.alive))
                    Ui.Label(body, $"{e.name}: {e.votes} oy", 44, Ui.Murekkep);
            }
        }

        void RenderGameOver()
        {
            Ui.Space(body, 20);
            Ui.Label(body, view.resultTitle, 70, Ui.Altin, TextAnchor.MiddleCenter, FontStyle.Bold);
            if (view.winners.Count > 0)
                Ui.Label(body, "Kazananlar: " + string.Join(", ", view.winners), 44, Ui.Krem, TextAnchor.MiddleCenter);
            Ui.Space(body, 20);
            Ui.Label(body, "Roller", 46, Ui.Altin, TextAnchor.MiddleLeft, FontStyle.Bold);
            foreach (var line in view.finalRoles) Ui.Label(body, line, 42, Ui.Krem);
            Ui.Space(body, 30);
            if (isHost) Ui.Button(body, "Yeni oyun (lobiye dön)", () => Send(Commands.Restart), Ui.Altin, Ui.Murekkep, 50, 150);
            else Ui.Label(body, "Oda sahibi yeni oyun başlatabilir.", 38, Ui.Krem, TextAnchor.MiddleCenter, FontStyle.Italic);
            Ui.Button(body, "Ana menü", () => Leave(), Ui.Kirmizi, Ui.Krem, 44, 120);
            Ui.Space(body, 20);
            Ui.Label(body, "Olanlar", 46, Ui.Altin, TextAnchor.MiddleLeft, FontStyle.Bold);
            foreach (var line in view.publicLog) Ui.Label(body, "• " + line, 38, Ui.Krem);
        }

        // ------------------------------------------------------------ Üst katmanlar

        RectTransform OpenOverlay(out RectTransform col)
        {
            Ui.Clear(overlayRoot);
            var shade = Ui.Panel(overlayRoot, Ui.Golge, "Karartma");
            Ui.Fill(shade.rectTransform);
            var card = Ui.Panel(overlayRoot, Ui.Krem, "Kart");
            Ui.Fill(card.rectTransform, 40, 80, 40, 80);
            var scroll = Ui.Scroll(card.transform, out col, 22, 44);
            Ui.Fill((RectTransform)scroll.transform, 0, 0, 0, 170);
            var close = Ui.Button(card.transform, "Kapat", () => Ui.Clear(overlayRoot), Ui.Lacivert, Ui.Krem, 46, 130);
            var crt = (RectTransform)close.transform;
            crt.anchorMin = new Vector2(0, 0);
            crt.anchorMax = new Vector2(1, 0);
            crt.pivot = new Vector2(0.5f, 0);
            crt.sizeDelta = new Vector2(-88, 130);
            crt.anchoredPosition = new Vector2(0, 24);
            return card.rectTransform;
        }

        void ShowRoleCard()
        {
            if (view == null) return;
            OpenOverlay(out var col);
            Ui.Picture(col, art != null ? art.Portrait(view.myRole) : null, 760);
            Ui.Label(col, view.myRoleName, 80, Ui.Kirmizi, TextAnchor.MiddleCenter, FontStyle.Bold);
            Ui.Label(col, view.myFaction, 46, Ui.Murekkep, TextAnchor.MiddleCenter, FontStyle.Italic);
            Ui.Label(col, view.myDescription, 42, Ui.Murekkep);
            if (!string.IsNullOrEmpty(view.myExtra)) Ui.Label(col, view.myExtra, 42, Ui.Kirmizi);
            Ui.Space(col, 10);
            Ui.Label(col, "Bu oyundaki roller", 42, Ui.Murekkep, TextAnchor.MiddleLeft, FontStyle.Bold);
            Ui.Label(col, string.Join(", ", view.rolesInGame), 40, Ui.Murekkep);
        }

        void ShowLog()
        {
            if (view == null) return;
            OpenOverlay(out var col);
            Ui.Label(col, "Yalnız sana", 48, Ui.Kirmizi, TextAnchor.MiddleLeft, FontStyle.Bold);
            if (view.myLog.Count == 0) Ui.Label(col, "Henüz bir şey yok.", 40, Ui.Gri);
            foreach (var line in view.myLog) Ui.Label(col, "• " + line, 40, Ui.Murekkep);
            Ui.Space(col, 20);
            Ui.Label(col, "Herkesin bildikleri", 48, Ui.Kirmizi, TextAnchor.MiddleLeft, FontStyle.Bold);
            if (view.publicLog.Count == 0) Ui.Label(col, "Henüz bir şey yok.", 40, Ui.Gri);
            foreach (var line in view.publicLog) Ui.Label(col, "• " + line, 40, Ui.Murekkep);
        }
    }
}
