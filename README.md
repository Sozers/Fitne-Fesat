# Fitne&Fesat

Osmanlı temalı, gizli rollere dayanan bir sosyal çıkarım oyunu (mobil, Unity).

Oyuncular gizli rollerle bir kasabada yaşar: geceleri roller gizlice eylem yapar, gündüzleri herkes tartışır ve şüpheliyi oylar. Fesatçılar kasabayı içten çökertmeye, sadıklar onları bulup ortaya çıkarmaya çalışır.

## Durum

İlk oynanabilir prototip: kurallar çekirdeği, yerel ağ (LAN) bağlantısı ve dikey ekran arayüzü. Tasarım `TASARIM.md`, görseller `GORSELLER.md`, yol haritası `PLAN.md` içinde.

## Proje yapısı

- Unity projesi deponun kökündedir (Unity 6000.6.9f1, 2D URP). Git LFS gerekir.
- `Assets/FitneVeFesat/Scripts/Core`: kurallar çekirdeği; saf C#, Unity'ye ve ağa bağlı değil.
- `Assets/FitneVeFesat/Scripts/Net`: yerel ağ: oda kuran telefon sunucudur (TCP), odalar UDP yayınıyla bulunur.
- `Assets/FitneVeFesat/Scripts/Game`: Unity tarafı: oda yönetimi, süreler, ekranlar (arayüz koddan kurulur).
- `Assets/FitneVeFesat/Editor/ProjectSetup.cs`: projeyi hazırlar ve Windows deneme sürümünü derler.
- `Tests/FitneFesat.Core.Tests`: kurallar çekirdeğinin testleri.

## Kurallar testleri

```
dotnet test Tests/FitneFesat.Core.Tests
```

## Bilgisayarda telefon gibi deneme (Windows)

1. Unity'de projeyi açın, **Fitne ve Fesat → Projeyi Hazırla**, sonra **Fitne ve Fesat → Windows Denemesi Derle**. Ya da komut satırından:
   ```
   Unity.exe -batchmode -projectPath . -executeMethod FitneFesat.EditorTools.ProjectSetup.SetupAndBuildWindows -quit -logFile Logs/kurulum.log
   ```
2. `Araclar/deneme.bat` dosyasını çalıştırın. Ekranda 6 dikey pencere açılır; her biri bir telefondur. İlki odayı kurar, ötekiler kendiliğinden katılır. Oyuncu sayısı için: `Araclar\deneme.bat 8`.
3. Ali'nin penceresinde (oda sahibi) **Oyunu başlat**'a basın.

İlk açılışta Windows Güvenlik Duvarı ağ izni sorabilir; yerel ağda oynamak için izin verin.

Pencereler elle de açılabilir: `FitneFesat.exe -ad Ali -oda-kur`, `FitneFesat.exe -ad Ayse -katil 127.0.0.1`. Ayrı bilgisayarlar aynı ağdaysa oda listede görünür ya da oda sahibinin adresi yazılır.

## Birlikte çalışma

Depoda hem Claude hem ChatGPT çalışır. Ortak kurallar `AGENTS.md` içindedir.
