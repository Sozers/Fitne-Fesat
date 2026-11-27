using System;
using System.Collections.Generic;

namespace FitneFesat.Net
{
    public static class NetConfig
    {
        public const int TcpPort = 47778;
        public const int DiscoveryPort = 47777;
        public const string DiscoveryAsk = "FITNEFESAT?";
        public const string DiscoveryReply = "FITNEFESAT!";
        public const int ProtocolVersion = 1;
    }

    // Kablodaki her ileti: tür + JSON gövde.
    [Serializable]
    public class Packet
    {
        public string t = "";
        public string body = "";
    }

    public static class PacketTypes
    {
        public const string Hello = "hello";     // istemci → ev sahibi
        public const string Command = "cmd";     // istemci → ev sahibi
        public const string Welcome = "welcome"; // ev sahibi → istemci
        public const string Lobby = "lobby";     // ev sahibi → istemci
        public const string View = "view";       // ev sahibi → istemci
        public const string Error = "error";     // ev sahibi → istemci
    }

    [Serializable]
    public class Hello
    {
        public string name = "";
        public string token = "";   // Ev sahibinin kendi istemcisi bu belirteçle tanınır.
        public int version = NetConfig.ProtocolVersion;
    }

    [Serializable]
    public class Welcome
    {
        public int connId;
        public bool isHost;
    }

    [Serializable]
    public class Command
    {
        public string kind = "";
        public int a = -1;
        public int b = -1;
    }

    public static class Commands
    {
        public const string Start = "lobby.start";
        public const string Restart = "restart";
        public const string NightTarget = "night.target";   // a: hedef
        public const string Muneccim = "night.muneccim";    // a: ölü, b: soru
        public const string Claim = "night.claim";          // a: rol
        public const string Execute = "night.execute";      // a: 1/0
        public const string Confirm = "night.confirm";
        public const string Answer = "dead.answer";         // a: oyuncu, b: Masum 0 / Şüpheli 1
        public const string Reveal = "day.reveal";
        public const string Jail = "day.jail";              // a: hedef, -1 boşalt
        public const string Vote = "vote";                  // a: hedef, -1 geri çek
    }

    [Serializable]
    public class LobbyPlayer
    {
        public string name = "";
        public bool connected = true;
        public bool isHost;
    }

    [Serializable]
    public class LobbyView
    {
        public string room = "";
        public string hostAddress = "";
        public List<LobbyPlayer> players = new List<LobbyPlayer>();
        public int minPlayers = 6;
        public int maxPlayers = 15;
        public string message = "";
    }

    [Serializable]
    public class ErrorMessage
    {
        public string text = "";
    }
}
