# Fitne&Fesat — ortak çalışma kuralları

Bu dosya depoda çalışan bütün yapay zekâ yardımcıları (Claude, ChatGPT/Codex) için ortak kuralları tutar. `CLAUDE.md` bu dosyayı içe aktarır; kurallar yalnız burada yazılır.

## Oyun

- Ad: **Fitne&Fesat** (klasör ve depo adı `FitneFesat`; `&` komut satırında sorun çıkardığı için).
- Tür: gizli roller ve sosyal çıkarım (gece eylemleri, gündüz tartışma ve oylama).
- Tema: Osmanlı.
- Platform: mobil (telefon), Unity.
- Roller, adlar, görseller ve metinler özgündür. Başka oyunların rol adları, karakterleri, sanatı veya metinleri kopyalanmaz; yalnız türün genel mekanikleri kullanılır.

## Dil

- Yapımcıyla konuşma, belgeler ve commit iletileri Türkçe yazılır.
- Kod içindeki adlar (sınıf, değişken) İngilizce olabilir.

## Git ve GitHub

- Depo: https://github.com/Sozers/FitneFesat, ana dal `main`.
- Her yardımcı kendi dalında çalışır: Claude için `claude/<konu>`, ChatGPT için `chatgpt/<konu>` (Codex'in kendi dal adları da olur).
- İşe başlamadan önce `main` çekilir (`git pull`), dal güncel `main` üzerinden açılır.
- Değişiklik `main`'e çekme isteğiyle (pull request) birleşir. Öteki yardımcının açık dalına ve çekme isteğine onun izni olmadan yazılmaz.
- Paylaşılan dallarda geçmiş yeniden yazılmaz: `main`'e zorla gönderme (force push) yapılmaz.
- Aynı dosyada iki yardımcı aynı anda çalışmasın diye, büyük bir işe başlarken `PLAN.md` içinde madde "üzerinde çalışılıyor: <yardımcı>" diye işaretlenir.
- Gizli bilgiler (anahtarlar, parolalar) depoya konmaz.

## Belgeler

- `TASARIM.md`: oyun tasarımı (döngü, taraflar, roller, kazanma koşulları, arayüz akışı).
- `PLAN.md`: yol haritası; bir madde bitince işaretlenir.
- Bir kural ya da karar değişince ilgili belge aynı değişiklikte güncellenir.
