#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;

public static class FitneFesatAtlasImporter
{
    [Serializable] private class AtlasData { public int width, height, padding; public Entry[] sprites; }
    [Serializable] private class Entry { public string name; public int x, y, width, height, unityY; }
    private const string BasePath = "Assets/FitneVeFesat/UI/FitneFesat_UI_Atlas";

    [MenuItem("Tools/Fitne ve Fesat/Import UI Atlas")]
    public static void ImportAtlas()
    {
        var importer = AssetImporter.GetAtPath(BasePath + ".png") as TextureImporter;
        if (importer == null || !File.Exists(BasePath + ".json"))
        {
            Debug.LogError("FitneFesat atlas PNG or JSON not found.");
            return;
        }
        var data = JsonUtility.FromJson<AtlasData>(File.ReadAllText(BasePath + ".json"));
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = Mathf.Max(data.width, data.height);
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.spritePixelsPerUnit = 100;
        importer.SaveAndReimport();

        var factories = new SpriteDataProviderFactories();
        factories.Init();
        var provider = factories.GetSpriteEditorDataProviderFromObject(importer);
        provider.InitSpriteEditorDataProvider();
        var old = provider.GetSpriteRects().ToDictionary(s => s.name, s => s.spriteID);
        var rects = data.sprites.Select(s => new SpriteRect {
            name = s.name,
            rect = new Rect(s.x, s.unityY, s.width, s.height),
            alignment = SpriteAlignment.Center,
            pivot = new Vector2(0.5f, 0.5f),
            spriteID = old.TryGetValue(s.name, out var id) ? id : GUID.Generate()
        }).ToArray();
        provider.SetSpriteRects(rects);
        var ids = provider.GetDataProvider<ISpriteNameFileIdDataProvider>();
        if (ids != null)
            ids.SetNameFileIdPairs(rects.Select(s => new SpriteNameFileIdPair(s.name, s.spriteID)));
        provider.Apply();
        importer.SaveAndReimport();
        Debug.Log($"Fitne ve Fesat: {rects.Length} UI sprites imported.");
    }
}
#endif
