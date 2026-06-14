using Kingkode.Chronos.Clock.Persistences;
using System.Collections.Generic;
using UnityEngine;

namespace Kingkode.Chronos.Clock.Overlay
{
    /// <summary>
    /// Shared look for the runtime debug overlays: dark rounded panels, accent buttons and
    /// trust-level colors. Textures are generated lazily (IMGUI styles can only be built
    /// inside OnGUI) and cached for the lifetime of the app.
    /// </summary>
    internal static class ChronosGuiTheme
    {
        // ─── Palette ──────────────────────────────────────────────────
        public static readonly Color Panel = new Color(0.07f, 0.08f, 0.10f, 0.94f);
        public static readonly Color Card = new Color(0.12f, 0.14f, 0.18f, 1f);
        public static readonly Color Accent = new Color(0.25f, 0.75f, 0.95f, 1f);
        public static readonly Color ButtonFace = new Color(0.18f, 0.21f, 0.26f, 1f);
        public static readonly Color ButtonHover = new Color(0.25f, 0.30f, 0.38f, 1f);
        public static readonly Color ButtonActive = new Color(0.20f, 0.55f, 0.75f, 1f);
        public static readonly Color TextPrimary = new Color(0.92f, 0.94f, 0.96f, 1f);
        public static readonly Color TextDim = new Color(0.60f, 0.66f, 0.73f, 1f);

        public static readonly Color StrongColor = new Color(0.30f, 0.85f, 0.45f, 1f);
        public static readonly Color MediumColor = new Color(0.95f, 0.80f, 0.25f, 1f);
        public static readonly Color WeakColor = new Color(0.95f, 0.30f, 0.30f, 1f);
        public static readonly Color UnknownColor = new Color(0.55f, 0.58f, 0.62f, 1f);

        public static Color TrustColor(TrustedLevel level)
        {
            switch (level)
            {
                case TrustedLevel.Strong: return StrongColor;
                case TrustedLevel.Medium: return MediumColor;
                case TrustedLevel.Weak: return WeakColor;
                default: return UnknownColor;
            }
        }

        public static string ToHex(Color color) => ColorUtility.ToHtmlStringRGB(color);

        // ─── Textures ─────────────────────────────────────────────────
        private static readonly Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();

        public static Texture2D Solid(Color color) => Rounded(color, 0);

        /// <summary>Rounded-rect texture meant to be nine-sliced via GUIStyle.border.</summary>
        public static Texture2D Rounded(Color color, int radius)
        {
            string key = (Color32)color + "-" + radius;
            if (_textures.TryGetValue(key, out var cached) && cached != null)
                return cached;

            int size = Mathf.Max(radius * 2 + 2, 2);
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            var clear = new Color(0f, 0f, 0f, 0f);
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    tex.SetPixel(x, y, InsideRoundedRect(x, y, size, radius) ? color : clear);

            tex.Apply();
            _textures[key] = tex;
            return tex;
        }

        private static bool InsideRoundedRect(int x, int y, int size, int radius)
        {
            if (radius <= 0)
                return true;

            // Only the four corner squares can be outside the shape.
            int cx = x < radius ? radius : (x >= size - radius ? size - radius - 1 : -1);
            int cy = y < radius ? radius : (y >= size - radius ? size - radius - 1 : -1);
            if (cx == -1 || cy == -1)
                return true;

            float dx = x - cx;
            float dy = y - cy;
            return dx * dx + dy * dy <= radius * radius;
        }

        // ─── Style factories (call from OnGUI) ────────────────────────
        public static GUIStyle MakePanel(int radius = 12)
        {
            return new GUIStyle(GUIStyle.none)
            {
                normal = { background = Rounded(Panel, radius) },
                border = new RectOffset(radius, radius, radius, radius)
            };
        }

        public static GUIStyle MakeCard(int radius = 8)
        {
            return new GUIStyle(GUIStyle.none)
            {
                normal = { background = Rounded(Card, radius) },
                border = new RectOffset(radius, radius, radius, radius),
                padding = new RectOffset(12, 12, 8, 10),
                margin = new RectOffset(0, 0, 2, 6)
            };
        }

        public static GUIStyle MakeLabel(int fontSize, Color color, bool bold = false)
        {
            return new GUIStyle(GUIStyle.none)
            {
                fontSize = fontSize,
                fontStyle = bold ? FontStyle.Bold : FontStyle.Normal,
                richText = true,
                wordWrap = false,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = color },
                padding = new RectOffset(2, 2, 2, 2)
            };
        }

        public static GUIStyle MakeButton(int fontSize, int radius = 8)
        {
            return new GUIStyle(GUIStyle.none)
            {
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                border = new RectOffset(radius, radius, radius, radius),
                margin = new RectOffset(3, 3, 3, 3),
                padding = new RectOffset(10, 10, 4, 4),
                normal = { background = Rounded(ButtonFace, radius), textColor = TextPrimary },
                hover = { background = Rounded(ButtonHover, radius), textColor = TextPrimary },
                active = { background = Rounded(ButtonActive, radius), textColor = Color.white }
            };
        }

        public static GUIStyle MakeAccentButton(int fontSize, int radius = 8)
        {
            var style = MakeButton(fontSize, radius);
            style.normal.background = Rounded(new Color(Accent.r * 0.75f, Accent.g * 0.75f, Accent.b * 0.75f, 1f), radius);
            style.hover.background = Rounded(Accent, radius);
            style.active.background = Rounded(ButtonActive, radius);
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            return style;
        }
    }
}
