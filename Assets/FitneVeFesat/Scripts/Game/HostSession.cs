using System;
using System.Collections.Generic;
using System.Linq;
using FitneFesat.Core;
using FitneFesat.Net;
using GameState = FitneFesat.Core.Game;

namespace FitneFesat.Game
{
    // Oda kuran telefonda çalışan yönetici: lobi, süreler, komutlar ve her oyuncuya görünüm gönderme.
    // Oda kuranın kendi istemcisi de buraya sıradan bir oyuncu gibi bağlanır.
    public class HostSession
    {
        class Seat
        {
            public int ConnId = -1;
            public string Name = "";
            public bool IsHost;
            public bool Connected = true;
            public int PlayerId = -1;
        }

        public readonly string Token = Guid.NewGuid().ToString("N");
        public readonly Balance Balance = new Balance();
        public int Port => server.Port;
        public string Address { get; private set; } = "";

        readonly HostServer server = new HostServer();
        readonly List<Seat> seats = new List<Seat>();
        GameState game;
        float phaseEnds;
        string roomName = "";
        float clock;

        public void Start(string room)
        {
            roomName = room;
            server.Start(room);
            Address = HostServer.LocalAddress();
        }

        public void Stop()
        {
            server.Stop();
        }

        public void Tick(float now)
        {
            clock = now;
            while (server.TryGetClosed(out int closedId)) OnClosed(closedId);
            while (server.TryReceive(out var incoming)) OnPacket(incoming.ConnId, incoming.Packet, now);
            if (game != null) TickGame(now);
        }

        // ------------------------------------------------------------ Bağlantılar

        void OnClosed(int connId)
        {
            var seat = seats.FirstOrDefault(s => s.ConnId == connId);
            if (seat == null) return;
            if (game == null)
            {
                seats.Remove(seat);
                BroadcastLobby();
            }
            else
            {
                seat.Connected = false;
                seat.ConnId = -1;
                BroadcastViews(clock);
            }
        }

        void OnPacket(int connId, Packet packet, float now)
        {
            try
            {
                if (packet.t == PacketTypes.Hello) OnHello(connId, UnityEngine.JsonUtility.FromJson<Hello>(packet.body), now);
                else if (packet.t == PacketTypes.Command) OnCommand(connId, UnityEngine.JsonUtility.FromJson<Command>(packet.body), now);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning("Fitne&Fesat: paket işlenemedi: " + e.Message);
            }
        }

        void OnHello(int connId, Hello hello, float now)
        {
            if (hello.version != NetConfig.ProtocolVersion)
            {
                server.Send(connId, PacketTypes.Error, new ErrorMessage { text = "Oyunun sürümü bu odayla uyumsuz. Uygulamayı güncelleyin." });
                return;
            }

            string name = string.IsNullOrWhiteSpace(hello.name) ? "Oyuncu" : hello.name.Trim();
            if (name.Length > 16) name = name.Substring(0, 16);

            if (game != null)
            {
                // Oyun sürerken yalnız bağlantısı kopan oyuncu aynı adla geri dönebilir.
                var seat = seats.FirstOrDefault(s => !s.Connected && s.Name == name);
                if (seat == null)
                {
                    server.Send(connId, PacketTypes.Error, new ErrorMessage { text = "Bu odada oyun sürüyor. Oyun bitince katılabilirsin." });
                    return;
                }
                seat.ConnId = connId;
                seat.Connected = true;
                server.Send(connId, PacketTypes.Welcome, new Welcome { connId = connId, isHost = seat.IsHost });
                BroadcastViews(now);
                return;
            }

            if (seats.Count >= Balance.MaxPlayers)
            {
                server.Send(connId, PacketTypes.Error, new ErrorMessage { text = "Oda dolu." });
                return;
            }

            string unique = name;
            for (int i = 2; seats.Any(s => s.Name == unique); i++) unique = $"{name} {i}";

            var newSeat = new Seat { ConnId = connId, Name = unique, IsHost = hello.token == Token };
            seats.Add(newSeat);
            server.Send(connId, PacketTypes.Welcome, new Welcome { connId = connId, isHost = newSeat.IsHost });
            BroadcastLobby();
        }

        void OnCommand(int connId, Command cmd, float now)
        {
            var seat = seats.FirstOrDefault(s => s.ConnId == connId);
            if (seat == null) return;

            if (cmd.kind == Commands.Start)
            {
                if (seat.IsHost && game == null) StartGame(now);
                return;
            }
            if (cmd.kind == Commands.Restart)
            {
                if (seat.IsHost && game != null && game.Phase == Phase.GameOver) BackToLobby();
                return;
            }
            if (game == null || seat.PlayerId < 0) return;

            int id = seat.PlayerId;
            switch (cmd.kind)
            {
                case Commands.NightTarget: game.SetNightTarget(id, cmd.a); break;
                case Commands.Muneccim: game.SetMuneccimQuestion(id, cmd.a, cmd.b); break;
                case Commands.Claim: game.SetClaim(id, cmd.a); break;
                case Commands.Execute: game.SetExecute(id, cmd.a == 1); break;
                case Commands.Confirm: game.Confirm(id); break;
                case Commands.Answer: game.Answer(id, cmd.a, cmd.b); break;
                case Commands.Reveal: game.RevealAga(id); break;
                case Commands.Jail: game.SetJail(id, cmd.a); break;
                case Commands.Vote: game.Vote(id, cmd.a); break;
            }
            BroadcastViews(now);
        }

        // ------------------------------------------------------------ Oyun akışı

        void StartGame(float now)
        {
            var players = seats.Where(s => s.Connected).ToList();
            if (players.Count < Balance.MinPlayers || players.Count > Balance.MaxPlayers)
            {
                BroadcastLobby($"Oyun {Balance.MinPlayers}–{Balance.MaxPlayers} oyuncuyla başlar. Şu an {players.Count} kişi var.");
                return;
            }
            seats.RemoveAll(s => !s.Connected);
            for (int i = 0; i < seats.Count; i++) seats[i].PlayerId = i;
            game = GameState.Create(seats.Select(s => s.Name).ToList(), Environment.TickCount, Balance);
            phaseEnds = now + Balance.NightSeconds;
            BroadcastViews(now);
        }

        void BackToLobby()
        {
            game = null;
            seats.RemoveAll(s => !s.Connected);
            foreach (var s in seats) s.PlayerId = -1;
            BroadcastLobby();
        }

        void TickGame(float now)
        {
            bool changed = false;
            switch (game.Phase)
            {
                case Phase.Night:
                    if (now >= phaseEnds || game.NightReady())
                    {
                        game.ResolveNight();
                        phaseEnds = now + Balance.MorningSeconds;
                        changed = true;
                    }
                    break;
                case Phase.Morning:
                    if (now >= phaseEnds)
                    {
                        game.BeginDiscussion();
                        phaseEnds = now + Balance.DiscussionSeconds;
                        changed = true;
                    }
                    break;
                case Phase.Discussion:
                    if (now >= phaseEnds)
                    {
                        game.BeginVoting();
                        phaseEnds = now + Balance.VotingSeconds;
                        changed = true;
                    }
                    break;
                case Phase.Voting:
                    if (now >= phaseEnds)
                    {
                        game.ResolveVoting();
                        phaseEnds = now + Balance.NightSeconds;
                        changed = true;
                    }
                    break;
            }
            if (changed) BroadcastViews(now);
        }

        // ------------------------------------------------------------ Gönderim

        void BroadcastLobby(string message = "")
        {
            var view = new LobbyView
            {
                room = roomName,
                hostAddress = Address,
                minPlayers = Balance.MinPlayers,
                maxPlayers = Balance.MaxPlayers,
                message = message
            };
            foreach (var s in seats) view.players.Add(new LobbyPlayer { name = s.Name, connected = s.Connected, isHost = s.IsHost });
            foreach (var s in seats.Where(s => s.Connected)) server.Send(s.ConnId, PacketTypes.Lobby, view);
        }

        void BroadcastViews(float now)
        {
            if (game == null) return;
            float left = game.Phase == Phase.GameOver ? 0f : Math.Max(0f, phaseEnds - now);
            foreach (var s in seats.Where(s => s.Connected && s.PlayerId >= 0))
            {
                var view = game.BuildView(s.PlayerId);
                view.secondsLeft = left;
                foreach (var e in view.players)
                {
                    var owner = seats.FirstOrDefault(x => x.PlayerId == e.id);
                    e.connected = owner != null && owner.Connected;
                }
                server.Send(s.ConnId, PacketTypes.View, view);
            }
        }
    }
}
