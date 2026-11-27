# Fitne&Fesat — oyun tasarımı

Durum: ilk taslak. Kaynak: yapımcının roller ve mekanikler belgesi. Kesinleşmemiş noktalar en sondaki "Açık sorular" bölümündedir.

## 1. Özet

- Tür: gizli roller ve sosyal çıkarım. Gece roller gizlice eylem yapar, gündüz herkes tartışır ve oylar.
- Oynanış: yalnız gerçek oyuncular oynar; bot yoktur. Arkadaşlar aynı ortamda, yüz yüze oynar. Her oyuncunun kendi telefonu vardır, telefonlar yerel ağ (LAN) üzerinden bağlanır.
- Telefon ile yüz yüze olanın ayrımı: gece eylemleri, sonuçlar ve oylama telefondan yapılır; konuşma ve tartışma yüz yüze yapılır.
- Yönetim: oyun kendi kendini yönetir. Aşamaları, süreleri, gece çözümünü ve açıklamaları uygulama yürütür; anlatıcı ya da yönetici oyuncu gerekmez.
- Ekran: yalnız mobil, dikey.
- Oyuncu sayısı: 6–15.
- Para kazanma: oyun ücretsiz, reklamlı.
- İlke: derinlik karmaşık kurallardan değil, roller arasındaki etkileşimlerden ve oyuncuların birbirine söylediği yalanlardan gelir. Saldırı ve savunma seviyeleri, karmaşık bağışıklıklar ve ölüm sonrası saldırılar şimdilik yoktur.

## 2. Taraflar ve kazanma koşulları

Oyunda 3 taraf ve 15 rol vardır: Kasaba 8, Kumpas 3, Tarafsız 4.

| Taraf | Kazanma koşulu |
| --- | --- |
| Kasaba | Kumpas üyelerini ve Yamyam'ı ortadan kaldırmak. |
| Kumpas | Kasabanın kontrolünü ele geçirmek ve Yamyam'ı ortadan kaldırmak. |
| Tarafsız | Ortak takım yoktur; her rolün kendi kazanma koşulu vardır. |

Kumpas üyeleri birbirini tanır. Grup olarak her gece yalnız bir ortak saldırı yapabilir.

## 3. Roller

### Kasaba

**Ağa** — Kasaba, yönetim
- Gündüz kimliğini bütün kasabaya açıklayabilir. Açıkladıktan sonra oyu 2 oy değerindedir.
- Açıklanan kimlik tekrar gizlenemez.
- Hak: 1 kez kimlik açıklama.

**Kadı** — Kasaba, kontrol
- Gündüz bir oyuncu seçer; o oyuncu gece zindana atılır.
- Hapsedilen oyuncu o gece yeteneğini kullanamaz ve dışarıdan gelen saldırılardan korunur.
- Kadı hapsettiği oyuncuyu gece infaz edebilir.
- Hak: her gece hapsetme, oyun boyunca 2 infaz.
- Bir Kasaba üyesini infaz ederse infaz yeteneğini kaybeder.

**Subaşı** — Kasaba, araştırma
- Her gece bir oyuncuyu araştırır ve "Masum" ya da "Şüpheli" sonucunu öğrenir. Tam rolü göremez.
- Zorba masum görünür. Müfteri'nin o gece sahte delil oluşturduğu oyuncu şüpheli görünür.
- Hak: sınırsız.

**Kabadayı** — Kasaba, saldırı
- Gece seçtiği bir oyuncuyu öldürmeye çalışır.
- Bir Kasaba üyesini öldürürse suçluluktan sonraki gece intihar eder.
- Hak: 2 saldırı.

**Hekim** — Kasaba, koruma
- Her gece seçtiği bir oyuncuyu ölümcül saldırılardan korur.
- Koruduğu oyuncunun saldırıya uğrayıp uğramadığını öğrenir.
- Hak: sınırsız; kendini koruma oyun boyunca 1 kez.

**Gözcü** — Kasaba, araştırma
- Her gece bir oyuncuyu gözetler. O oyuncuyu ziyaret edenlerin adlarını öğrenir; ziyaret amacını ve rollerini öğrenemez.
- Hak: sınırsız.

**Çengi** — Kasaba, engelleme
- Her gece bir oyuncunun dikkatini dağıtır; o oyuncu o gece yeteneğini kullanamaz.
- Kumpas saldırısını, araştırmaları ve korumayı engelleyebilir.
- Aynı oyuncuyu üst üste iki gece engelleyemez.
- Hak: sınırsız.

**Ahali** — Kasaba, sivil
- Özel yeteneği yoktur. Tartışma, çıkarım ve oylamayla Kasaba'ya yardım eder.
- Birden fazla oyuncuya verilebilen tek roldür.

### Kumpas

**Zorba** — Kumpas, lider
- Her gece Kumpas'ın öldüreceği oyuncuyu belirler.
- Subaşı'nın araştırmasında masum görünür.
- Fedai hayatta değilse saldırıyı kendisi yapar.
- Hak: sınırsız.

**Fedai** — Kumpas, saldırı
- Zorba'nın belirlediği hedefe saldırır.
- Zorba ölünce hedef seçme yetkisini devralır.
- Gözcü onu saldırdığı yerde görebilir.
- Hak: sınırsız.

**Müfteri** — Kumpas, yanıltma
- Her gece bir oyuncu hakkında sahte delil oluşturur. Subaşı o gece bu oyuncuyu araştırırsa şüpheli sonucunu alır.
- Sahte delil yalnız oluşturulduğu gece geçerlidir.
- Son kalan Kumpas üyesi olursa iftira yeteneğini kaybeder, saldırı yeteneği kazanır.
- Hak: sınırsız.

### Tarafsız

**Derviş** — Tarafsız, hayatta kalma
- Hiçbir tarafa üye değildir; kimin kazandığıyla ilgilenmez.
- Aktif yeteneği yoktur. Taraflarla iş birliği yapabilir, taraf değiştirebilir; rolü değişmez.
- Zafer: oyun sona erdiğinde hayatta olmak.

**Yamyam** — Tarafsız, saldırı
- Her gece istediği bir oyuncuya saldırabilir. Kasaba'nın ve Kumpas'ın ortak düşmanıdır.
- Subaşı'nın araştırmasında şüpheli görünür.
- Hak: sınırsız.
- Zafer: kendisine tehdit oluşturan bütün oyuncuları ortadan kaldırmak.

**Kanlı** — Tarafsız, intikam
- Oyun başında kendisine rastgele bir Kasaba üyesi hedef olarak atanır. Hedefi suçlayıp gündüz oylamasında idam ettirmeye çalışır.
- Hedefi gece öldürülürse Soytarı'ya dönüşür.
- Zafer: hedefin gündüz oylamasıyla idam edilmesi.

**Soytarı** — Tarafsız, aldatmaca
- Kendini şüpheli gösterip oyuncuları kendi aleyhine oy vermeye ikna etmeye çalışır.
- Gece öldürülürse kaybeder.
- Zafer: gündüz oylamasıyla idam edilmek. Soytarı kazandığında öteki oyuncuların oyunu sürer.

## 4. Subaşı araştırma sonuçları

Araştırma yalnız iki sonuç verir; gerçek rol hiçbir zaman doğrudan öğrenilmez. Bu, blöfü güçlendirir.

| Rol | Sonuç |
| --- | --- |
| Ağa, Kadı, Subaşı, Kabadayı, Hekim, Gözcü, Çengi, Ahali | Masum |
| Zorba | Masum |
| Fedai, Müfteri | Şüpheli |
| Derviş, Kanlı, Soytarı | Masum |
| Yamyam | Şüpheli |
| Müfteri'nin o gece sahte delil oluşturduğu oyuncu | Şüpheli |

## 5. Oyun döngüsü

| Aşama | Süre | Ne olur |
| --- | --- | --- |
| Gece | 20 sn | Gizli rol yetenekleri kullanılır. |
| Sabah | 10 sn | Ölümler ve kişisel sonuçlar açıklanır. |
| Tartışma | 60 sn | Oyuncular yüz yüze tartışır. |
| Oylama | 15 sn | Oyuncular idam edilecek kişiyi seçer. |
| **Toplam tur** | **105 sn** | |

## 6. Gece işlem sırası

Bütün oyuncular kararlarını aynı 20 saniyede verir; oyun motoru kararları aşağıdaki sırayla çözer.

| Öncelik | Rol | İşlem |
| --- | --- | --- |
| 1 | Kadı | Hapsetme |
| 2 | Çengi | Yetenek engelleme |
| 3 | Müfteri | Sahte delil |
| 4 | Hekim | Koruma |
| 5 | Subaşı | Araştırma |
| 6 | Zorba | Saldırı emrinin işlenmesi |
| 7 | Fedai, Kabadayı, Yamyam, Kadı | Saldırı ve infaz |
| 8 | Gözcü | Ziyaretlerin tespiti |
| 9 | Sistem | Ölümlerin ve kişisel sonuçların açıklanması |

Saldırılar aynı anda gerçekleşmiş sayılır. Örneğin Kabadayı ile Yamyam birbirine saldırırsa ikisi de ölebilir.

## 7. Önemli rol etkileşimleri

| Etkileşim | Sonuç |
| --- | --- |
| Hekim → Fedai'nin hedefi | Hedef hayatta kalır. |
| Hekim → Yamyam'ın hedefi | Hedef hayatta kalır. |
| Müfteri → Ahali, Subaşı → Ahali | Ahali şüpheli görünür. |
| Subaşı → Zorba | Zorba masum görünür. |
| Çengi → Hekim | Hekim koruma yapamaz. |
| Çengi → Fedai | Kumpas saldırısı engellenir. |
| Kadı → Yamyam | Yamyam o gece saldıramaz. |
| Kadı → Hekim | Hekim o gece koruyamaz. |
| Kadı bir Kasaba üyesini infaz eder | Kadı infaz hakkını kaybeder. |
| Kabadayı bir Kasaba üyesini öldürür | Kabadayı sonraki gece intihar eder. |
| Gözcü → Fedai'nin hedefi | Gözcü Fedai'nin ziyaretini öğrenir. |
| Soytarı gündüz idam edilir | Soytarı kişisel zafer kazanır. |
| Kanlı'nın hedefi gece ölür | Kanlı Soytarı'ya dönüşür. |

## 8. Oyuncu dağılımları

| Oyuncu | Kasaba | Kumpas | Tarafsız |
| --- | --- | --- | --- |
| 6 | 4 | 2 | 0 |
| 7 | 5 | 2 | 0 |
| 8 | 5 | 2 | 1 |
| 9 | 6 | 2 | 1 |
| 10 | 6 | 3 | 1 |
| 11 | 7 | 3 | 1 |
| 12 | 8 | 3 | 1 |
| 13 | 8 | 3 | 2 |
| 14 | 9 | 3 | 2 |
| 15 | 10 | 3 | 2 |

Kumpas en çok 3 kişidir. Böylece bütün özel roller tek kalır; yalnız Ahali tekrarlanır.

## 9. Genel kurallar

- Roller oyun başında rastgele dağıtılır.
- Her oyuncu kendi rolünü bilir. Kumpas üyeleri birbirini tanır.
- Oyuncular oyunda hangi rollerin bulunduğunu bilir, kime dağıtıldığını bilmez.
- Ölen oyuncunun rolü herkese açıklanır.
- Ölen oyuncu yetenek kullanamaz, tartışmaya ve oylamaya katılamaz.
- Oyuncular çekimser kalabilir.
- İdam için yaşayan oyuncuların toplam oy gücünün salt çoğunluğu gerekir. Eşitlikte kimse idam edilmez.
- Tarafsızların kişisel zaferleri ana oyunu bitirmez.

## 10. Denge ayarları (başlangıç önerisi)

Oyun testlerinden sonra değişebilir. Kodda bu değerler tek bir ayar yerinde tutulmalı.

| Mekanik | Değer |
| --- | --- |
| Ağa'nın oy gücü (açıklamadan sonra) | 2 |
| Kadı'nın infaz hakkı | 2 |
| Kabadayı'nın saldırı hakkı | 2 |
| Hekim'in kendini koruma hakkı | 1 |
| Çengi'nin aynı hedefi üst üste engellemesi | Yasak |
| Zorba'nın gece saldırılarına bağışıklığı | Yok |
| Yamyam'ın gece saldırılarına bağışıklığı | Yok |
| Derviş'in kişisel koruması | Yok |
| Soytarı'nın ölüm sonrası intikamı | Yok |
| Kanlı'nın hedefi gece ölürse | Soytarı'ya dönüşür |
| İdam için gereken oy | Yaşayan oyuncuların oy gücünün salt çoğunluğu |

## 11. Açık sorular

Kurallar çekirdeği yazılmadan önce yapımcının karar vermesi gereken noktalar.

**Kazanma ve oyun sonu**
1. Kumpas'ın "kasabanın kontrolünü ele geçirmesi" tam olarak ne zaman olur? Öneri: Yamyam ölüyse ve yaşayan Kumpas sayısı yaşayan öteki oyunculara eşit ya da fazlaysa.
2. Yamyam ne zaman kazanır? Öneri: yaşayan Kasaba ve Kumpas üyesi kalmadığında. Derviş gibi tarafsızlar yaşıyor olabilir.
3. Kanlı hedefini idam ettirince oyunda kalır mı, yoksa kazanıp çıkar mı? Soytarı idamla kazanınca zaten ölüdür.
4. Hiç kimsenin kazanamadığı durumda (örneğin yalnız tarafsızlar kaldığında) oyun nasıl biter?

**Akış ve zamanlama**
5. Oyun gece mi, gündüz mü başlar? Kadı gündüz hapis seçtiğine göre ilk gece hapis olur mu?
6. Kadı hapis hedefini ve Ağa kimlik açıklamasını gündüzün hangi aşamasında yapar: tartışmada mı, oylamada mı?
7. Oyuncu oylamada kendine oy verebilir mi? Oylama süresi bitmeden oy değiştirilebilir mi?
8. Ölen oyuncular telefondan oyunu izleyebilir mi, rolleri görebilir mi?

**Yüz yüze oyuna özgü**
9. Kumpas aynı odada gizlice nasıl anlaşır? Öneri: gece Kumpas üyeleri telefonda Zorba'nın seçtiği hedefi görür; Fedai ve Müfteri telefonda hedef önerebilir.
10. Kadı hapsettiği oyuncuyla konuşabilir mi (telefonda kısa yazışma), yoksa yalnız infaz kararı mı verir?

**Rol etkileşimlerinin boşlukları**
11. Çengi Zorba'yı engellerse ve Fedai yaşıyorsa Kumpas saldırısı olur mu? (Öneri: Fedai hedef olmadan saldırmaz.)
12. Çengi Kadı'yı ya da Yamyam'ı engelleyebilir mi? Kadı öncelik 1'de olduğu için Çengi'den önce çözülür.
13. Hapisteki oyuncuyu ziyaret etmeye çalışanlar Gözcü'ye görünür mü? Hapisteki oyuncuya Hekim koruması ya da Subaşı araştırması işler mi?
14. Kabadayı'nın intiharını Hekim önleyebilir mi? (Öneri: hayır.)
15. Zorba yalnız kaldığında (Fedai öldü) Gözcü onu hedefin yanında görür mü? (Öneri: evet, saldırıyı kendisi yaptığı için.)
16. Kanlı'nın hedefi Ağa ya da Kadı olabilir mi? Hedef gündüz başka bir yolla (örneğin Kabadayı'nın kendini öldürmesi) ölürse ne olur?

**Dağılım**
17. Her oyuncu sayısında hangi Kasaba ve Tarafsız rollerinin bulunacağı rastgele mi seçilir, yoksa sabit bir listeye mi bağlıdır? Örneğin 6 kişilik oyundaki 4 Kasaba rolü hangileri? Subaşı ve Hekim her oyunda bulunmalı mı?
18. 6–7 kişilik oyunda Kumpas'ın 2 rolü Zorba ile Fedai mi, Zorba ile Müfteri mi?
