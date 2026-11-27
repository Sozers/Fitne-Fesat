using System;

namespace FitneFesat.Core
{
    public enum Faction
    {
        Kasaba,
        Kumpas,
        Tarafsiz
    }

    // Sıra önemlidir: ağ üzerinden sayı olarak gider.
    public enum Role
    {
        Aga,
        Kadi,
        Subasi,
        Kabadayi,
        Hekim,
        Gozcu,
        Cengi,
        Muneccim,
        Ahali,
        Zorba,
        Fedai,
        Mufteri,
        Dervis,
        Yamyam,
        Kanli,
        Maskara
    }

    public static class RoleInfo
    {
        public static readonly Role[] All = (Role[])Enum.GetValues(typeof(Role));

        public static Faction FactionOf(Role role)
        {
            switch (role)
            {
                case Role.Zorba:
                case Role.Fedai:
                case Role.Mufteri:
                    return Faction.Kumpas;
                case Role.Dervis:
                case Role.Yamyam:
                case Role.Kanli:
                case Role.Maskara:
                    return Faction.Tarafsiz;
                default:
                    return Faction.Kasaba;
            }
        }

        public static string Name(Role role)
        {
            switch (role)
            {
                case Role.Aga: return "Ağa";
                case Role.Kadi: return "Kadı";
                case Role.Subasi: return "Subaşı";
                case Role.Kabadayi: return "Kabadayı";
                case Role.Hekim: return "Hekim";
                case Role.Gozcu: return "Gözcü";
                case Role.Cengi: return "Çengi";
                case Role.Muneccim: return "Müneccim";
                case Role.Ahali: return "Ahali";
                case Role.Zorba: return "Zorba";
                case Role.Fedai: return "Fedai";
                case Role.Mufteri: return "Müfteri";
                case Role.Dervis: return "Derviş";
                case Role.Yamyam: return "Yamyam";
                case Role.Kanli: return "Kanlı";
                case Role.Maskara: return "Maskara";
                default: return role.ToString();
            }
        }

        public static string FactionName(Faction faction)
        {
            switch (faction)
            {
                case Faction.Kasaba: return "Kasaba";
                case Faction.Kumpas: return "Kumpas";
                default: return "Tarafsız";
            }
        }

        // Görsel dosyalarının adı ve klasörü (Assets/FitneVeFesat/Portreler/<Klasör>/<Anahtar>.png).
        public static string Key(Role role)
        {
            return role.ToString();
        }

        public static string PortraitFolder(Role role)
        {
            switch (FactionOf(role))
            {
                case Faction.Kumpas: return "Kumpas";
                case Faction.Tarafsiz: return "Tarafsiz";
                default: return "Kasaba";
            }
        }

        // Subaşı'nın araştırmasında şüpheli görünen roller (sahte delil ayrıca işlenir).
        public static bool LooksSuspicious(Role role)
        {
            return role == Role.Fedai || role == Role.Mufteri || role == Role.Yamyam;
        }

        public static string Description(Role role)
        {
            switch (role)
            {
                case Role.Aga:
                    return "Gündüz kimliğini bir kez açıklayabilirsin. Açıkladıktan sonra oyun iki sayılır, ama Hekim artık seni koruyamaz.";
                case Role.Kadi:
                    return "Gündüz bir oyuncu seçersin, o gece zindana girer: yeteneğini kullanamaz ve saldırılardan korunur. Gece sorgudaki yanıtına bakıp onu infaz edebilirsin (2 hak). Bir Kasaba üyesini infaz edersen infaz hakkını kaybedersin.";
                case Role.Subasi:
                    return "Her gece bir oyuncuyu araştırırsın: Masum ya da Şüpheli. Zorba masum görünür; Müfteri'nin iftira attığı kişi şüpheli görünür.";
                case Role.Kabadayi:
                    return "Gece bir oyuncuyu öldürmeye çalışırsın (2 hak, ilk gece yok). Bir Kasaba üyesini öldürürsen ertesi gece vicdan azabından canına kıyarsın.";
                case Role.Hekim:
                    return "Her gece bir oyuncuyu saldırılardan korursun. Kendini oyun boyunca bir kez koruyabilirsin. Kimliğini açıklamış Ağa'yı koruyamazsın.";
                case Role.Gozcu:
                    return "Her gece bir oyuncuyu gözetlersin ve onu ziyaret edenlerin adlarını öğrenirsin.";
                case Role.Cengi:
                    return "Her gece bir oyuncunun dikkatini dağıtırsın; o gece yeteneğini kullanamaz. Kadı'yı engelleyemezsin. Yamyam'ı ziyaret edersen seni öldürür.";
                case Role.Muneccim:
                    return "Gece ölülerden birine hazır bir soru sorarsın; yanıtını sabah öğrenirsin. Ölüler senin kim olduğunu bilmez, yalan da söyleyebilir.";
                case Role.Ahali:
                    return "Özel yeteneğin yok. Gece geceyi onayla; gündüz tartış, çıkarım yap ve oyunla Kasaba'yı kurtar.";
                case Role.Zorba:
                    return "Kumpas'ın lideri. Her gece öldürülecek kişiyi sen seçersin. Subaşı'ya masum görünürsün. Fedai yoksa saldırıyı kendin yaparsın.";
                case Role.Fedai:
                    return "Zorba'nın seçtiği hedefe saldırırsın. Zorba yoksa ya da seçmezse hedefi sen seçersin. Subaşı'ya şüpheli görünürsün.";
                case Role.Mufteri:
                    return "Her gece bir oyuncuya sahte delil koyarsın; Subaşı o gece onu araştırırsa şüpheli görür. Son kalan Kumpas üyesi olursan saldırı yeteneği kazanırsın.";
                case Role.Dervis:
                    return "Hiçbir tarafa ait değilsin. Oyun bittiğinde hayattaysan kazanırsın.";
                case Role.Yamyam:
                    return "Her gece istediğin bir oyuncuya saldırırsın. Kimse seni engelleyemez; seni oyalamaya gelen Çengi'yi de öldürürsün. Kasaba ve Kumpas tükenince kazanırsın.";
                case Role.Kanli:
                    return "Sana bir Kasaba üyesi hedef verildi. Onu gündüz oylamasıyla idam ettirirsen kazanırsın. Hedefin başka bir yolla ölürse Maskara olursun.";
                case Role.Maskara:
                    return "Kendini şüpheli göster ve gündüz idam edilmeyi başar; idam edilirsen kazanırsın. Gece ölürsen kaybedersin.";
                default:
                    return "";
            }
        }

        public static bool IsKumpas(Role role)
        {
            return FactionOf(role) == Faction.Kumpas;
        }
    }
}
