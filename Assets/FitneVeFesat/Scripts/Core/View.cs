using System;
using System.Collections.Generic;

namespace FitneFesat.Core
{
    // Her oyuncunun telefonuna gönderilen görünüm. Yalnız o oyuncunun bilmesi gerekenleri içerir.
    // Alanlar açık (public field) tutulur; Unity'nin JsonUtility'si ile ağ üzerinden gider.

    [Serializable]
    public class PlayerEntry
    {
        public int id;
        public string name = "";
        public bool alive = true;
        public string role = "";        // Ölüyse ya da Ağa kimliğini açıkladıysa görünür; Kumpas kendi üyelerini görür.
        public bool agaRevealed;
        public bool teammate;
        public bool you;
        public int votes;               // Oylamada aldığı oy gücü.
        public bool connected = true;
    }

    public static class PromptModes
    {
        public const string None = "";
        public const string Confirm = "confirm";   // Yeteneği yok ya da bu gece kullanamaz: yalnız onay.
        public const string Target = "target";     // Bir oyuncu seç.
        public const string Claim = "claim";       // Zindandasın: bir rol söyle.
        public const string Execute = "execute";   // Kadı: infaz et / etme.
        public const string Muneccim = "muneccim"; // Bir ölü ve bir soru seç.
        public const string Answer = "answer";     // Ölü: Müneccim'in sorusunu yanıtla.
    }

    [Serializable]
    public class PlayerView
    {
        public int you = -1;
        public int phase;
        public int round;
        public float secondsLeft;
        public string phaseTitle = "";

        public List<PlayerEntry> players = new List<PlayerEntry>();

        // Rol kartı
        public int myRole = -1;
        public string myRoleName = "";
        public string myFaction = "";
        public string myDescription = "";
        public string myExtra = "";
        public bool alive = true;

        // Gece
        public string promptMode = "";
        public string promptText = "";
        public List<int> options = new List<int>();
        public int selected = -1;
        public List<string> choices2 = new List<string>();
        public int selected2 = -1;
        public bool confirmed;
        public bool executeOn;

        // Gündüz
        public bool canReveal;
        public bool canJail;
        public List<int> jailOptions = new List<int>();
        public int jailSelected = -1;
        public List<int> voteOptions = new List<int>();
        public int myVote = -1;
        public int votePowerTotal;
        public int votesNeeded;

        // Bildirimler
        public List<string> morningPublic = new List<string>();
        public List<string> morningPrivate = new List<string>();
        public List<string> publicLog = new List<string>();
        public List<string> myLog = new List<string>();
        public List<string> rolesInGame = new List<string>();

        // Oyun sonu
        public bool over;
        public string resultTitle = "";
        public List<string> winners = new List<string>();
        public List<string> finalRoles = new List<string>();
    }
}
