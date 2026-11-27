# Fitne&Fesat — görseller

Bütün görseller yapımcının özgün çizimleridir. PNG ve JPG dosyaları Git LFS ile tutulur (`.gitattributes`); depoyu çekenlerde `git lfs` kurulu olmalıdır.

Unity projesi deponun kökünde kurulacağı için görseller şimdiden `Assets/FitneVeFesat/` altına konmuştur.

## Rol portreleri

`Assets/FitneVeFesat/Portreler/<Taraf>/<Rol>.png` — 1254 × 1254, RGB, minyatür üslubunda, altta ad yazılacak boş bir kartuş ile.

| Taraf | Dosyalar |
| --- | --- |
| Kasaba | Aga, Kadi, Subasi, Kabadayi, Hekim, Gozcu, Cengi, Ahali |
| Kumpas | Zorba, Fedai, Mufteri |
| Tarafsiz | Dervis, Yamyam, Kanli, Maskara |

Dosya adları rolün koddaki kimliğidir (Türkçe karakter yok, numara yok). Maskara portresi kaynak klasördeki `15_Soytari.png` dosyasıdır; rol adı değişince yeniden adlandırıldı.

## Arayüz atlası

`Assets/FitneVeFesat/UI/`
- `FitneFesat_UI_Atlas.png` + `.json`: 2048 × 2048 RGBA atlas, 15 sprite (çerçeveler, düğmeler, pano, rozet, ilerleme çubuğu, 8 eşya simgesi).
- `Individual/`: aynı 15 sprite ayrı, şeffaf PNG olarak. Atlas ya da ayrı dosyalar kullanılır; ikisi birden gerekmez, kullanılmayan kopya build'den önce kaldırılabilir.
- `main_menu_background.png`: ana menü arka planı (1672 × 941).
- `Kaynak/Gizli_Meclis.png`: ana menü sahnesinin işlenmemiş hali (1672 × 941).
- `Kaynak/Tezhip_Ikon_Seti.png`: 6 simgelik tek sayfa (ateş, hilal ve yıldız, mektup, hançer, tezhip halkası, kapı); beyaz zeminli, henüz kesilip şeffaflaştırılmadı.

`Assets/FitneVeFesat/Editor/FitneFesatAtlasImporter.cs`: Unity'de **Tools → Fitne ve Fesat → Import UI Atlas** komutu atlası JSON'a göre 15 sprite'a böler. 2D Sprite paketi gerekir. Betik henüz Unity içinde denenmedi; 9-slice sınırları ve düğme durumları tanımlı değildir.

## Eksikler ve notlar

- **Müneccim portresi yok.** Rol tasarıma sonradan eklendi.
- **Ana menü arka planı ve çerçevesi yatay** (1672 × 941). Oyun dikey ekranda oynanacağı için dikey bir ana menü görseli gerekecek ya da bu görsel dikey ekrana kırpılarak kullanılacak.
- Portreler RGB'dir (şeffaflık yok); kart içinde çerçeveli kare olarak kullanılır. Mobil için Unity'de en büyük boyut 512 ya da 1024 ile içe aktarılmalı.
- Alternatif portreler (yapımcının `02_Alternatif_Portreler` klasörü) depoya alınmadı.
