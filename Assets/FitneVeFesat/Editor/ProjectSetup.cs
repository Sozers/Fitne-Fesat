using System.IO;
using FitneFesat.Core;
using FitneFesat.Game;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FitneFesat.EditorTools
{
    // Projeyi hazırlar (oyuncu ayarları, görsellerin içe aktarımı, sahne) ve deneme sürümü derler.
    // Komut satırından: Unity.exe -batchmode -projectPath . -executeMethod FitneFesat.EditorTools.ProjectSetup.SetupAndBuildWindows -quit
    public static class ProjectSetup
    {
        const string Root = "Assets/FitneVeFesat";
        const string ScenePath = Root + "/Sahneler/Ana.unity";
        const string ArtPath = Root + "/ArtLibrary.asset";
        const string WindowsExe = "Builds/Windows/FitneFesat.exe";

        [MenuItem("Fitne ve Fesat/Projeyi Hazırla")]
        public static void Setup()
        {
            ConfigurePlayer();
            ImportTextures();
            var art = CreateArtLibrary();
            CreateScene(art);
            AssetDatabase.SaveAssets();
            Debug.Log("Fitne&Fesat: proje hazır.");
        }

        [MenuItem("Fitne ve Fesat/Windows Denemesi Derle")]
        public static void BuildWindows()
        {
            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = WindowsExe,
                target = BuildTarget.StandaloneWindows64,
                targetGroup = BuildTargetGroup.Standalone,
                options = BuildOptions.None
            };
            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException("Windows derlemesi başarısız: " + report.summary.result);
            Debug.Log($"Fitne&Fesat: Windows derlemesi hazır: {WindowsExe} ({report.summary.totalSize / (1024 * 1024)} MB)");
        }

        public static void SetupAndBuildWindows()
        {
            Setup();
            BuildWindows();
        }

        const string AndroidApk = "Builds/Android/FitneFesat.apk";

        // Telefona doğrudan kurulacak APK (mağaza için ileride .aab gerekecek).
        // Komut satırından: Unity.exe -batchmode -buildTarget Android -projectPath . -executeMethod FitneFesat.EditorTools.ProjectSetup.SetupAndBuildAndroid -quit
        [MenuItem("Fitne ve Fesat/Android APK Derle")]
        public static void BuildAndroid()
        {
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            EditorUserBuildSettings.buildAppBundle = false;
            Directory.CreateDirectory(Path.GetDirectoryName(AndroidApk));
            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = AndroidApk,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.None
            };
            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException("Android derlemesi başarısız: " + report.summary.result);
            Debug.Log($"Fitne&Fesat: Android APK hazır: {AndroidApk} ({report.summary.totalSize / (1024 * 1024)} MB)");
        }

        public static void SetupAndBuildAndroid()
        {
            Setup();
            BuildAndroid();
        }

        static void ConfigurePlayer()
        {
            PlayerSettings.companyName = "Fitne ve Fesat";
            PlayerSettings.productName = "Fitne&Fesat";
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.fitnefesat.oyun");
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, "com.fitnefesat.oyun");

            // Telefon: yalnız dikey.
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.Android.forceInternetPermission = true;

            // Windows denemesi: telefon gibi dikey pencere, birden çok kopya yan yana çalışabilir.
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = 540;
            PlayerSettings.defaultScreenHeight = 960;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = true;
            PlayerSettings.visibleInBackground = true;
            PlayerSettings.forceSingleInstance = false;
            PlayerSettings.usePlayerLog = true;
        }

        static void ImportTextures()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { Root + "/Portreler", Root + "/UI" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith("FitneFesat_UI_Atlas.png")) continue; // Atlası kendi içe aktarma betiği böler.
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;
                bool portrait = path.Contains("/Portreler/");
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.maxTextureSize = portrait ? 1024 : 2048;
                importer.textureCompression = TextureImporterCompression.CompressedHQ;
                importer.SaveAndReimport();
            }
        }

        static ArtLibrary CreateArtLibrary()
        {
            var art = AssetDatabase.LoadAssetAtPath<ArtLibrary>(ArtPath);
            if (art == null)
            {
                art = ScriptableObject.CreateInstance<ArtLibrary>();
                AssetDatabase.CreateAsset(art, ArtPath);
            }

            art.portraits = new Sprite[RoleInfo.All.Length];
            foreach (var role in RoleInfo.All)
            {
                string path = $"{Root}/Portreler/{RoleInfo.PortraitFolder(role)}/{RoleInfo.Key(role)}.png";
                art.portraits[(int)role] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (art.portraits[(int)role] == null) Debug.LogWarning($"Fitne&Fesat: {RoleInfo.Name(role)} portresi yok ({path}).");
            }
            art.menuBackground = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/UI/main_menu_background.png");
            art.titleBanner = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/UI/Individual/ui_title_banner.png");
            art.panelVertical = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/UI/Individual/ui_panel_vertical.png");
            art.badgeShield = AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/UI/Individual/ui_badge_shield.png");
            EditorUtility.SetDirty(art);
            return art;
        }

        static void CreateScene(ArtLibrary art)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var camGo = new GameObject("Kamera", typeof(Camera));
            camGo.tag = "MainCamera";
            var cam = camGo.GetComponent<Camera>();
            cam.orthographic = true;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color32(0x10, 0x16, 0x2E, 0xFF);
            camGo.transform.position = new Vector3(0, 0, -10);

            var appGo = new GameObject("FitneFesat");
            var app = appGo.AddComponent<AppMain>();
            app.art = art;

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }
    }
}
