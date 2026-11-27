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

Oyunda 3 taraf ve 16 rol vardır: Kasaba 9, Kumpas 3, Tarafsız 4.

| Taraf | Kazanma koşulu |
| --- | --- |
| Kasaba | Yaşayan Kumpas üyesi ve Yamyam kalmadığında kazanır. |
| Kumpas | Yaşayan Kasaba üyesi ve Yamyam kalmadığında kazanır. |
| Yamyam | Yaşayan Kasaba ve Kumpas üyesi kalmadığında kazanır. |
| Derviş, Kanlı, Maskara | Kişisel zaferleri vardır (bkz. roller); ana oyunun sonucunu belirlemezler. |

Kumpas üyeleri birbirini tanır. Grup olarak her gece yalnız bir ortak saldırı yapabilir.

**Oyun sonu**
- Kasaba, Kumpas ve Yamyam birbirinin düşmanıdır. Bunlardan yalnız biri hayatta kaldığında oyun biter ve o kazanır. Yanında yaşayan Derviş, Kanlı ya da Maskara bulunabilir.
- Üçünden hiçbiri hayatta kalmazsa (örneğin son Kasaba üyesi ile Yamyam aynı gece birbirini öldürürse) oyun berabere biter.
- Arka arkaya belirli sayıda tur kimse ölmezse oyun berabere biter (sayı: denge ayarlarında).
- Oyun bittiğinde Derviş hayattaysa o da kazanır. Hedefini idam ettirmiş Kanlı ve idam edilmiş Maskara da kazananlar arasında gösterilir.

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
- Sorgu otomatiktir: hapsedilen oyuncu gece telefonda oyunun bütün rolleri arasından (oyunda olmayanlar dahil) birini seçerek "rolüm bu" der; doğruyu da söyleyebilir, yalan da. Kadı bu yanıtı görür. Yanıt vermezse Kadı "yanıt vermedi" görür.
- Kadı, sorgunun yanıtına göre hapsettiği oyuncuyu gece infaz edebilir. İnfazı hiçbir koruma önleyemez.
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
- Kumpas saldırısını, araştırmaları, korumayı, sahte delili ve gözetlemeyi engelleyebilir. Kadı'yı engelleyemez (hapis gündüz seçilir).
- Yamyam'ı engelleyemez: onu ziyaret ederse Yamyam tarafından öldürülür. Hapisteki Yamyam onu öldüremez.
- Kendisi engellenemez. Kendini seçemez. Aynı oyuncuyu üst üste gecelerde de engelleyebilir.
- Hedefini ziyaret ettiği için Gözcü onu hedefin yanında görür.
- Engellediği oyuncu "biri seni oyaladı, yeteneğini kullanamadın" bildirimini alır.
- Hak: sınırsız.

**Müneccim** — Kasaba, ruhani
- Gece ölülerle konuşur. Ölü yoksa (örneğin ilk gece) yalnız geceyi onaylar.
- Konuşma yazışma değil, seçimle yapılır; böylece Müneccim'in telefon hareketi öteki gece eylemlerinden ayırt edilmez. Müneccim her gece ölülerden birini ve hazır sorulardan birini seçer; ölü, yanıtını yine seçerek verir (bir oyuncu adı, "Masum/Şüpheli" ya da bir rol).
- Hazır sorular (başlangıç önerisi): "Seni kim öldürdü sence?", "En çok kimden şüpheleniyordun?", "Kime güveniyordun?", "Öldüğün gece ne öğrendin?".
- Ölüler Müneccim'in kim olduğunu görmez; soruyu "Müneccim" adıyla alır. Ölü yalan söyleyebilir (örneğin ölü bir Kumpas üyesi).
- Soru ve yanıt gece içinde verilir; Müneccim yanıtı sabah, kişisel sonuçlarla birlikte görür. Yanıt vermeyen ölü için "yanıt gelmedi" görür.
- Kimseyi ziyaret etmez; Gözcü onu görmez. Çengi onu engellerse ya da Kadı hapsederse o gecenin yanıtı ona iletilmez.
- Hak: sınırsız (gecede bir soru).

**Ahali** — Kasaba, sivil
- Özel yeteneği yoktur. Tartışma, çıkarım ve oylamayla Kasaba'ya yardım eder.
- Gece telefonda geceyi onaylar; böylece öteki oyuncular onun yeteneği olmadığını anlayamaz.
- Kasaba'da birden fazla oyuncuya verilebilen tek roldür.

### Kumpas

**Zorba** — Kumpas, lider
- Her gece Kumpas'ın öldüreceği oyuncuyu belirler. Kumpas kendi üyesini hedef seçemez.
- Subaşı'nın araştırmasında masum görünür.
- Fedai hayatta değilse saldırıyı kendisi yapar.
- Hak: sınırsız.

**Fedai** — Kumpas, saldırı
- Zorba'nın belirlediği hedefe saldırır.
- Zorba ölünce hedef seçme yetkisini devralır.
- Bir oyunda birden fazla Fedai olabilir. Kumpas'ın gecelik tek saldırısını kıdemli Fedai yapar (oyun başında rastgele belirlenir); öteki Fedai yedektir, kıdemli ölünce onun yerine geçer.
- Gözcü onu saldırdığı yerde görebilir.
- Hak: sınırsız.

**Müfteri** — Kumpas, yanıltma
- Her gece bir oyuncu hakkında sahte delil oluşturur. Subaşı o gece bu oyuncuyu araştırırsa şüpheli sonucunu alır.
- Sahte delil yalnız oluşturulduğu gece geçerlidir.
- Son kalan Kumpas üyesi olursa iftira yeteneğini kaybeder, saldırı yeteneği kazanır.
- Subaşı her oyunda bulunduğu için yeteneği hep işe yarar.
- Hak: sınırsız.

### Tarafsız

**Derviş** — Tarafsız, hayatta kalma
- Hiçbir tarafa üye değildir; kimin kazandığıyla ilgilenmez.
- Aktif yeteneği yoktur; gece telefonda geceyi onaylar. Taraflarla iş birliği yapabilir, taraf değiştirebilir; rolü değişmez.
- Zafer: oyun sona erdiğinde hayatta olmak.

**Yamyam** — Tarafsız, saldırı
- Her gece istediği bir oyuncuya saldırabilir. Kasaba'nın ve Kumpas'ın ortak düşmanıdır.
- Subaşı'nın araştırmasında şüpheli görünür.
- Engellenemez: Çengi onu ziyaret ederse Yamyam Çengi'yi öldürür ve kendi hedefine de saldırır.
- Hak: sınırsız.
- Zafer: yaşayan Kasaba ve Kumpas üyesi kalmaması.

**Kanlı** — Tarafsız, intikam
- Oyun başında kendisine rastgele bir Kasaba üyesi (Ağa ve Kadı dışında) hedef olarak atanır. Hedefi suçlayıp gündüz oylamasında idam ettirmeye çalışır.
- Hedefi gündüz idamı dışında bir yolla ölürse (örneğin gece öldürülürse) Maskara'ya dönüşür.
- Zafer: hedefin gündüz oylamasıyla idam edilmesi.

**Maskara** — Tarafsız, aldatmaca
- Kendini şüpheli gösterip oyuncuları kendi aleyhine oy vermeye ikna etmeye çalışır.
- Gece öldürülürse kaybeder.
- Zafer: gündüz oylamasıyla idam edilmek. Maskara kazandığında öteki oyuncuların oyunu sürer.

## 4. Subaşı araştırma sonuçları

Araştırma yalnız iki sonuç verir; gerçek rol hiçbir zaman doğrudan öğrenilmez. Bu, blöfü güçlendirir.

| Rol | Sonuç |
| --- | --- |
| Ağa, Kadı, Subaşı, Kabadayı, Hekim, Gözcü, Çengi, Müneccim, Ahali | Masum |
| Zorba | Masum |
| Fedai, Müfteri | Şüpheli |
| Derviş, Kanlı, Maskara | Masum |
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
| 8 | Gözcü, Müneccim | Ziyaretlerin tespiti; ölülerin yanıtının iletilmesi |
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
| Maskara gündüz idam edilir | Maskara kişisel zafer kazanır. |
| Kanlı'nın hedefi idam dışında ölür | Kanlı Maskara'ya dönüşür. |
| Çengi → Yamyam | Çengi ölür; Yamyam engellenmez ve kendi hedefine de saldırır. |
| Hekim → Çengi, Çengi → Yamyam | Çengi hayatta kalır; Yamyam yine engellenmez. |
| Hekim → kimliği açıklanmış Ağa | Koruma işlemez. |
| Hekim → Kadı'nın infaz ettiği oyuncu | Koruma işlemez; infaz gerçekleşir. |
| Hekim, Kabadayı'nın hedefini korur | Hedef hayatta kalır; Kabadayı hakkını yine harcamış olur. |
| Kumpas → Kumpas üyesi | Yapılamaz; Kumpas kendi üyesini hedef seçemez. |
| Saldırıdan kurtulan oyuncu | Saldırıya uğradığını ve kurtarıldığını öğrenir. |
| Çengi'nin ya da Kadı'nın hedefi | Engellendiğini ya da hapsedildiğini öğrenir. |
| Çengi → Gözcü | Gözcü o gece kimseyi göremez. |
| Müneccim → ölü oyuncu | Ölü, seçimle yanıt verir; Müneccim'in kim olduğunu bilmez. |

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

Kumpas en çok 3 kişidir. Yalnız Ahali ve Fedai birden fazla çıkabilir; öteki roller bir oyunda en çok bir kez bulunur.

**Rol seçimi: rastgele.** Taraf sayıları yukarıdaki tablodan gelir; her taraftaki roller rastgele seçilir.
- Kasaba: Subaşı her oyunda bulunur. Kalan her yer için 7 özel rol (Ağa, Kadı, Kabadayı, Hekim, Gözcü, Çengi, Müneccim) ile Ahali arasından rastgele seçilir. Özel roller birer kez çıkabilir, Ahali birden fazla çıkabilir.
- Kumpas: her yer için Zorba, Fedai ve Müfteri arasından rastgele seçilir. Zorba ve Müfteri birer kez çıkabilir, Fedai birden fazla çıkabilir. Kumpas'ta en az bir saldırgan (Zorba ya da Fedai) hep bulunur; Müfteri her oyunda olmayabilir.
- Tarafsız: 4 rolden rastgele, birbirinden farklı seçilir.
- Oyun başında oyunda hangi rollerin bulunduğu herkese gösterilir.

## 9. Genel kurallar

- Roller oyun başında rastgele dağıtılır.
- Gece her yaşayan oyuncu telefonda ya yeteneğini kullanır ya da geceyi onaylar. Yeteneği olmayan rollere (Ahali, Derviş, Kanlı, Maskara) de bir gece ekranı ve onay düğmesi gösterilir; böylece kimin rolü olduğu telefon hareketlerinden anlaşılmaz. Herkes onaylarsa gece süresi dolmadan biter.
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
| Çengi'nin aynı hedefi üst üste engellemesi | Serbest |
| Zorba'nın gece saldırılarına bağışıklığı | Yok |
| Yamyam'ın gece saldırılarına bağışıklığı | Yok |
| Derviş'in kişisel koruması | Yok |
| Maskara'nın ölüm sonrası intikamı | Yok |
| Kanlı'nın hedefi idam dışında ölürse | Maskara'ya dönüşür |
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
4. Ölen oyuncu telefonda oyunun akışını (aşamalar, ölümler, açıklanan roller) izler; yaşayanların gizli rollerini ve gece sonuçlarını göremez. Yüz yüze tartışmada susar. Gece Müneccim'in sorusunu telefondan yanıtlayabilir.

**Yüz yüze oyuna özgü**
5. Kumpas gece telefonda birbirinin hedef önerisini görür; son kararı Zorba verir. Zorba yoksa ya da seçim yapmazsa kıdemli Fedai'nin seçimi geçerlidir.
6. Kadı ile hapsedilen arasında yazışma yoktur. Sorgu otomatiktir: hapsedilen, oyunun bütün rolleri arasından birini seçip söyler (yalan olabilir); Kadı yanıtı görüp infaz kararını verir.

**Rol etkileşimleri**
7. Çengi Zorba'yı engellerse ve Fedai yaşıyorsa kıdemli Fedai kendi seçtiği hedefe saldırır; seçim yapmamışsa o gece Kumpas saldırısı olmaz. Çengi kıdemli Fedai'yi engellerse ya da Kadı onu hapsederse saldırıyı yedek Fedai yapmaz; o gece Kumpas saldırısı olmaz.
8. Kadı engellenemez; hapis gündüz seçildiği için Çengi'den önce işler. Yamyam da engellenemez ve kendisini ziyaret eden Çengi'yi öldürür; ancak hapisteki Yamyam kimseyi öldüremez.
9. Hapis yalnız hapsedilenin yeteneğini ve ona yapılan saldırıları durdurur. Hapsedileni araştırma, koruma, gözetleme ve sahte delil normal işler.
10. Kabadayı'nın intiharını Hekim önleyemez.
11. Saldırıyı kim yaparsa Gözcü onu hedefin yanında görür: Fedai varsa kıdemli Fedai, yoksa Zorba (ya da son kalan Müfteri).
12. Kanlı'nın hedefi Ağa ya da Kadı olamaz. Hedef gündüz idamı dışında herhangi bir yolla ölürse Kanlı Maskara'ya dönüşür.
13. Kanlı hedefini idam ettirince kazanır ve oyunda kalmaya devam eder; sonradan ölse de zaferi geçerlidir.

