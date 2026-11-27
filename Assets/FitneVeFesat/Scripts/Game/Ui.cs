using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FitneFesat.Game
{
    // Arayüzü koddan kurmak için küçük yardımcılar (uGUI). Ölçüler 1080 × 1920 dikey ekrana göredir.
    public static class Ui
    {
        public static readonly Color Gece = new Color32(0x10, 0x16, 0x2E, 0xFF);
        public static readonly Color Lacivert = new Color32(0x1B, 0x2A, 0x5C, 0xFF);
        public static readonly Color Krem = new Color32(0xF4, 0xE8, 0xC8, 0xFF);
        public static readonly Color Parsomen = new Color32(0xE9, 0xD9, 0xAE, 0xFF);
        public static readonly Color Altin = new Color32(0xC9, 0xA1, 0x3B, 0xFF);
        public static readonly Color Kirmizi = new Color32(0x9E, 0x2B, 0x25, 0xFF);
        public static readonly Color Yesil = new Color32(0x3F, 0x6B, 0x3A, 0xFF);
        public static readonly Color Gri = new Color32(0x77, 0x77, 0x77, 0xFF);
        public static readonly Color Murekkep = new Color32(0x2A, 0x1E, 0x12, 0xFF);
        public static readonly Color Beyaz = Color.white;
        public static readonly Color Golge = new Color(0f, 0f, 0f, 0.6f);

        static Font font;
        public static Font Font
        {
            get
            {
                if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                return font;
            }
        }

        public static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = 5; // UI
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            return rt;
        }

        public static void Fill(RectTransform rt, float left = 0, float top = 0, float right = 0, float bottom = 0)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
        }

        public static Image Panel(Transform parent, Color color, string name = "Panel")
        {
            var rt = Rect(name, parent);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = color;
            return img;
        }

        public static Text Label(Transform parent, string text, int size, Color color,
            TextAnchor align = TextAnchor.MiddleLeft, FontStyle style = FontStyle.Normal)
        {
            var rt = Rect("Yazi", parent);
            var t = rt.gameObject.AddComponent<Text>();
            t.font = Font;
            t.text = text;
            t.fontSize = size;
            t.color = color;
            t.alignment = align;
            t.fontStyle = style;
            t.supportRichText = true;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.raycastTarget = false;
            return t;
        }

        public static Button Button(Transform parent, string label, UnityAction onClick, Color background, Color textColor,
            int size = 46, float height = 130, bool interactable = true)
        {
            var img = Panel(parent, background, "Dugme");
            var button = img.gameObject.AddComponent<Button>();
            var colors = button.colors;
            colors.disabledColor = new Color(0.75f, 0.75f, 0.75f, 0.6f);
            button.colors = colors;
            button.interactable = interactable;
            if (onClick != null) button.onClick.AddListener(onClick);
            var le = img.gameObject.AddComponent<LayoutElement>();
            le.minHeight = height;
            le.preferredHeight = height;

            var text = Label(img.transform, label, size, textColor, TextAnchor.MiddleCenter);
            Fill(text.rectTransform, 24, 8, 24, 8);
            return button;
        }

        public static Text ButtonText(Button b)
        {
            return b.GetComponentInChildren<Text>();
        }

        public static RectTransform Column(Transform parent, float spacing = 20, int padding = 0, string name = "Sutun")
        {
            var rt = Rect(name, parent);
            var layout = rt.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = new RectOffset(padding, padding, padding, padding);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return rt;
        }

        public static RectTransform Row(Transform parent, float spacing = 20, float height = 130)
        {
            var rt = Rect("Satir", parent);
            var layout = rt.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            var le = rt.gameObject.AddComponent<LayoutElement>();
            le.minHeight = height;
            le.preferredHeight = height;
            return rt;
        }

        public static RectTransform Grid(Transform parent, int columns, float cellHeight, float spacing = 16)
        {
            var rt = Rect("Izgara", parent);
            var grid = rt.gameObject.AddComponent<GridLayoutGroup>();
            float width = (1080 - 2 * 48 - (columns - 1) * spacing) / columns;
            grid.cellSize = new Vector2(width, cellHeight);
            grid.spacing = new Vector2(spacing, spacing);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            var fitter = rt.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return rt;
        }

        // Kaydırılabilir alan; içerik sütun olarak döner.
        public static ScrollRect Scroll(Transform parent, out RectTransform content, float spacing = 20, int padding = 48)
        {
            var root = Rect("Kaydirma", parent);
            var scroll = root.gameObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 60f;

            var viewport = Rect("Pencere", root);
            Fill(viewport);
            viewport.gameObject.AddComponent<RectMask2D>();
            var hit = viewport.gameObject.AddComponent<Image>();
            hit.color = new Color(0, 0, 0, 0);

            content = Column(viewport, spacing, padding, "Icerik");
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(0.5f, 1);
            content.offsetMin = Vector2.zero;
            content.offsetMax = Vector2.zero;
            var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.viewport = viewport;
            scroll.content = content;
            return scroll;
        }

        public static InputField Input(Transform parent, string placeholder, string value, int size = 48, float height = 130)
        {
            var bg = Panel(parent, Krem, "Giris");
            var le = bg.gameObject.AddComponent<LayoutElement>();
            le.minHeight = height;
            le.preferredHeight = height;

            var text = Label(bg.transform, "", size, Murekkep);
            text.supportRichText = false;
            text.raycastTarget = false;
            Fill(text.rectTransform, 30, 10, 30, 10);

            var ph = Label(bg.transform, placeholder, size, new Color(0.35f, 0.3f, 0.25f, 0.6f), TextAnchor.MiddleLeft, FontStyle.Italic);
            Fill(ph.rectTransform, 30, 10, 30, 10);

            var input = bg.gameObject.AddComponent<InputField>();
            input.textComponent = text;
            input.placeholder = ph;
            input.text = value ?? "";
            input.lineType = InputField.LineType.SingleLine;
            return input;
        }

        public static Image Picture(Transform parent, Sprite sprite, float height)
        {
            var img = Panel(parent, Color.white, "Resim");
            img.sprite = sprite;
            img.preserveAspect = true;
            if (sprite == null) img.color = new Color(1, 1, 1, 0.1f);
            var le = img.gameObject.AddComponent<LayoutElement>();
            le.minHeight = height;
            le.preferredHeight = height;
            return img;
        }

        public static void Space(Transform parent, float height)
        {
            var rt = Rect("Bosluk", parent);
            var le = rt.gameObject.AddComponent<LayoutElement>();
            le.minHeight = height;
            le.preferredHeight = height;
        }

        public static void Clear(Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
            {
                var child = t.GetChild(i).gameObject;
                child.SetActive(false);
                UnityEngine.Object.Destroy(child);
            }
        }

        public static string Clock(float seconds)
        {
            int s = Mathf.Max(0, Mathf.CeilToInt(seconds));
            return $"{s / 60}:{s % 60:00}";
        }
    }
}
