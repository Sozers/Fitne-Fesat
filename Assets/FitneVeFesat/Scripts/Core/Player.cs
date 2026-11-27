namespace FitneFesat.Core
{
    public class Player
    {
        public int Id;
        public string Name = "";
        public Role Role;
        public Role StartRole;
        public bool Alive = true;

        public bool Revealed;           // Ağa kimliğini açıkladı.
        public int Shots;               // Kabadayı
        public int Executions;          // Kadı
        public bool ExecutionLost;      // Kadı bir Kasaba üyesini infaz etti.
        public int SelfHeals;           // Hekim
        public bool SuicidePending;     // Kabadayı bir Kasaba üyesini öldürdü.
        public int KanliTarget = -1;    // Kanlı
        public int Seniority;           // Fedai: küçük olan kıdemli.
        public bool PersonalWin;        // Kanlı ya da Maskara kişisel zaferi.

        public string DeathText = "";

        public Faction Faction => RoleInfo.FactionOf(Role);
        public bool IsKumpas => RoleInfo.IsKumpas(Role);
    }
}
