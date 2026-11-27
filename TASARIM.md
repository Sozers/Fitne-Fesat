# Fitne&Fesat — oyun tasarımı

Durum: ilk sürüm. Kaynak: yapımcının roller ve mekanikler belgesi; boşluklar "Ayrıntı kararları" bölümünde kapatıldı. Değerler oyun testlerinden sonra değişebilir.

## 1. Özet

- Tür: gizli roller ve sosyal çıkarım. Gece roller gizlice eylem yapar, gündüz herkes tartışır ve oylar.
- Oynanış: yalnız gerçek oyuncular oynar; bot yoktur. Arkadaşlar aynı ortamda, yüz yüze oynar. Her oyuncunun kendi telefonu vardır, telefonlar yerel ağ (LAN) üzerinden bağlanır.
- Telefon ile yüz yüze olanın ayrımı: gece eylemleri, sonuçlar ve oylama telefondan yapılır; konuşma ve tartışma yüz yüze yapılır.
- Yönetim: oyun kendi kendini yönetir. Aşamaları, süreleri, gece çözümünü ve açıklamaları uygulama yürütür; anlatıcı ya da yönetici oyuncu gerekmez.
- Platform: Android ve iOS. Ekran: yalnız mobil, dikey.
- Oyuncu sayısı: 6–15.
- Para kazanma: oyun ücretsiz, reklamlı.
- İlke: derinlik karmaşık kurallardan değil, roller arasındaki etkileşimlerden ve oyuncuların birbirine söylediği yalanlardan gelir. Saldırı ve savunma seviyeleri, karmaşık bağışıklıklar ve ölüm sonrası saldırılar şimdilik yoktur.

## 2. Taraflar ve kazanma koşulları

Oyunda 3 taraf ve 15 rol vardır: Kasaba 8, Kumpas 3, Tarafsız 4.

| Taraf | Kazanma koşulu |
| --- | --- |
| Kasaba | Yaşayan Kumpas üyesi ve Yamyam kalmadığında kazanır. |
| Kumpas | Yaşayan Kasaba üyesi ve Yamyam kalmadığında kazanır. |
| Yamyam | Yaşayan Kasaba ve Kumpas üyesi kalmadığında kazanır. |
| Derviş, Kanlı, Soytarı | Kişisel zaferleri vardır (bkz. roller); ana oyunun sonucunu belirlemezler. |

Kumpas üyeleri birbirini tanır. Grup olarak her gece yalnız bir ortak saldırı yapabilir.

**Oyun sonu**
- Kasaba, Kumpas ve Yamyam birbirinin düşmanıdır. Bunlardan yalnız biri hayatta kaldığında oyun biter ve o kazanır. Yanında yaşayan Derviş, Kanlı ya da Soytarı bulunabilir.
- Üçünden hiçbiri hayatta kalmazsa (örneğin son Kasaba üyesi ile Yamyam aynı gece birbirini öldürürse) oyun berabere biter.
- Arka arkaya belirli sayıda tur kimse ölmezse oyun berabere biter (sayı: denge ayarlarında).
- Oyun bittiğinde Derviş hayattaysa o da kazanır. Hedefini idam ettirmiş Kanlı ve idam edilmiş Soytarı da kazananlar arasında gösterilir.

## 3. Roller

### Kasaba

**Ağa** — Kasaba, yönetim
- Gündüz kimliğini bütün kasabaya açıklayabilir. Açıkladıktan sonra oyu 2 oy değerindedir.
- Açıklanan kimlik tekrar gizlenemez. Kimliği açıklanmış Ağa'yı Hekim koruyamaz; herkesin bildiği bir hedef olur.
- Hak: 1 kez kimlik açıklama.

**Kadı** — Kasaba, kontrol
- Gündüz bir oyuncu seçer; o oyuncu gece zindana atılır.
- Hapsedilen oyuncu o gece yeteneğini kullanamaz ve dışarıdan gelen saldırılardan korunur.
- Hapsedilen oyuncu hapsedildiğini öğrenir.
- Kadı hapsettiği oyuncuyu gece infaz edebilir. İnfazı hiçbir koruma önleyemez.
- Hak: her gece hapsetme, oyun boyunca 2 infaz.
- Bir Kasaba üyesini infaz ederse infaz yeteneğini kaybeder.

**Subaşı** — Kasaba, araştırma
- Her gece bir oyuncuyu araştırır ve "Masum" ya da "Şüpheli" sonucunu öğrenir. Tam rolü göremez.
- Zorba masum görünür. Müfteri'nin o gece sahte delil oluşturduğu oyuncu şüpheli görünür.
- Hak: sınırsız.

**Kabadayı** — Kasaba, saldırı
- Gece seçtiği bir oyuncuyu öldürmeye çalışır.
- İlk gece saldıramaz.
- Bir Kasaba üyesini öldürürse suçluluktan sonraki gece intihar eder.
- Hak: 2 saldırı.

**Hekim** — Kasaba, koruma
- Her gece seçtiği bir oyuncuyu ölümcül saldırılardan korur.
- Koruduğu oyuncunun saldırıya uğrayıp uğramadığını öğrenir. Korunan oyuncu da saldırıdan kurtarıldığını öğrenir.
- Kimliğini açıklamış Ağa'yı koruyamaz.
- Kadı'nın infazını ve Kabadayı'nın intiharını önleyemez.
- Hak: sınırsız; kendini koruma oyun boyunca 1 kez.

**Gözcü** — Kasaba, araştırma
- Her gece bir oyuncuyu gözetler. O oyuncuyu ziyaret edenlerin adlarını öğrenir; ziyaret amacını ve rollerini öğrenemez.
- Hak: sınırsız.

**Çengi** — Kasaba, engelleme
- Her gece bir oyuncunun dikkatini dağıtır; o oyuncu o gece yeteneğini kullanamaz.
- Kumpas saldırısını, araştırmaları ve korumayı engelleyebilir.
- Yamyam'ı engelleyemez: onu ziyaret ederse Yamyam tarafından öldürülür.
- Aynı oyuncuyu üst üste iki gece engelleyemez.
- Engellediği oyuncu engellendiğini öğrenir.
- Hak: sınırsız.

**Ahali** — Kasaba, sivil
- Özel yeteneği yoktur. Tartışma, çıkarım ve oylamayla Kasaba'ya yardım eder.
- Birden fazla oyuncuya verilebilen tek roldür.

### Kumpas

**Zorba** — Kumpas, lider
- Her gece Kumpas'ın öldüreceği oyuncuyu belirler. Kumpas kendi üyesini hedef seçemez.
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
- Engellenemez: Çengi onu ziyaret ederse Yamyam Çengi'yi öldürür ve kendi hedefine de saldırır.
- Hak: sınırsız.
- Zafer: yaşayan Kasaba ve Kumpas üyesi kalmaması.

**Kanlı** — Tarafsız, intikam
- Oyun başında kendisine rastgele bir Kasaba üyesi (Ağa ve Kadı dışında) hedef olarak atanır. Hedefi suçlayıp gündüz oylamasında idam ettirmeye çalışır.
- Hedefi gündüz idamı dışında bir yolla ölürse (örneğin gece öldürülürse) Soytarı'ya dönüşür.
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
| Kanlı'nın hedefi idam dışında ölür | Kanlı Soytarı'ya dönüşür. |
| Çengi → Yamyam | Çengi ölür; Yamyam engellenmez ve kendi hedefine de saldırır. |
| Hekim → Çengi, Çengi → Yamyam | Çengi hayatta kalır; Yamyam yine engellenmez. |
| Hekim → kimliği açıklanmış Ağa | Koruma işlemez. |
| Hekim → Kadı'nın infaz ettiği oyuncu | Koruma işlemez; infaz gerçekleşir. |
| Hekim, Kabadayı'nın hedefini korur | Hedef hayatta kalır; Kabadayı hakkını yine harcamış olur. |
| Kumpas → Kumpas üyesi | Yapılamaz; Kumpas kendi üyesini hedef seçemez. |
| Saldırıdan kurtulan oyuncu | Saldırıya uğradığını ve kurtarıldığını öğrenir. |
| Çengi'nin ya da Kadı'nın hedefi | Engellendiğini ya da hapsedildiğini öğrenir. |

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

**Rol seçimi: rastgele.** Taraf sayıları yukarıdaki tablodan gelir; her taraftaki roller rastgele seçilir.
- Kasaba: her yer için 7 özel rol (Ağa, Kadı, Subaşı, Kabadayı, Hekim, Gözcü, Çengi) ile Ahali arasından rastgele seçilir. Özel roller birer kez çıkabilir, Ahali birden fazla çıkabilir. Hiçbir rolün her oyunda bulunması zorunlu değildir.
- Kumpas: 2 kişilik oyunda Zorba, Fedai ve Müfteri'den rastgele 2'si, 3 kişilik oyunda üçü birden. Her durumda Zorba ya da Fedai bulunduğu için Kumpas'ın saldırısı hep vardır. Zorba yoksa hedefi Fedai seçer.
- Tarafsız: 4 rolden rastgele, birbirinden farklı seçilir.
- Oyun başında oyunda hangi rollerin bulunduğu herkese gösterilir.

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
| Kanlı'nın hedefi idam dışında ölürse | Soytarı'ya dönüşür |
| İdam için gereken oy | Yaşayan oyuncuların oy gücünün salt çoğunluğu |
| Kimse ölmeden geçen tur sayısı (beraberlik) | 3 |
| Kabadayı'nın ilk gece saldırması | Yasak |
| Yamyam'ı ziyaret eden Çengi | Ölür |
| Hekim'in kimliği açıklanmış Ağa'yı koruması | Yasak |

## 11. Ayrıntı kararları

Yapımcının isteğiyle türün yerleşik kurallarına göre kararlaştırıldı; oyun testlerinden sonra değişebilir.

**Akış ve zamanlama**
1. Oyun gece başlar. İlk gece Kadı hapsedemez (önünde gündüz yoktur); öteki roller yeteneklerini kullanabilir.
2. Kadı hapis hedefini ve Ağa kimlik açıklamasını gündüz, tartışma ya da oylama sırasında telefondan yapar. Kadı'nın seçimi gündüz bitene kadar değişebilir.
3. Oyuncu kendine oy veremez. Oy, oylama süresi bitene kadar değiştirilebilir ya da geri çekilebilir.
4. Ölen oyuncu telefonda oyunun akışını (aşamalar, ölümler, açıklanan roller) izler; yaşayanların gizli rollerini ve gece sonuçlarını göremez. Yüz yüze tartışmada susar.

**Yüz yüze oyuna özgü**
5. Kumpas gece telefonda birbirinin hedef önerisini görür; son kararı Zorba verir. Zorba yoksa ya da seçim yapmazsa Fedai'nin seçimi geçerlidir.
6. Kadı ile hapsettiği oyuncu gece telefonda kısa yazışabilir. İnfaz kararını Kadı verir.

**Rol etkileşimleri**
7. Çengi Zorba'yı engellerse ve Fedai yaşıyorsa Fedai kendi seçtiği hedefe saldırır; Fedai seçim yapmamışsa o gece Kumpas saldırısı olmaz.
8. Kadı engellenemez; hapis gündüz seçildiği için Çengi'den önce işler. Yamyam da engellenemez ve kendisini ziyaret eden Çengi'yi öldürür; ancak hapisteki Yamyam kimseyi öldüremez.
9. Hapis yalnız hapsedilenin yeteneğini ve ona yapılan saldırıları durdurur. Hapsedileni araştırma, koruma, gözetleme ve sahte delil normal işler.
10. Kabadayı'nın intiharını Hekim önleyemez.
11. Saldırıyı kim yaparsa Gözcü onu hedefin yanında görür: Fedai varsa Fedai, yoksa Zorba (ya da son kalan Müfteri).
12. Kanlı'nın hedefi Ağa ya da Kadı olamaz. Hedef gündüz idamı dışında herhangi bir yolla ölürse Kanlı Soytarı'ya dönüşür.
13. Kanlı hedefini idam ettirince kazanır ve oyunda kalmaya devam eder; sonradan ölse de zaferi geçerlidir.

