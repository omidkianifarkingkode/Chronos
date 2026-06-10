using UnityEditor;
using UnityEngine;

namespace Kingkode.Chronos.Editor
{
    /// <summary>
    /// Styled inspector for the ChronosSettings asset: banner, accent-colored collapsible
    /// sections, module status pills and configuration sanity warnings.
    /// </summary>
    [CustomEditor(typeof(ChronosSettings))]
    public sealed class ChronosSettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty _logEnabled;
        private SerializedProperty _logLevel;
        private SerializedProperty _schedulerEnable;
        private SerializedProperty _tickingEnable;
        private SerializedProperty _clock;
        private SerializedProperty _serverTimeSync;
        private SerializedProperty _clockOverlay;
        private SerializedProperty _clockCheatOverlay;
        private SerializedProperty _scheduler;
        private SerializedProperty _ticking;

        private static readonly Color BannerBg = new Color(0.10f, 0.12f, 0.16f);
        private static readonly Color AccentClock = new Color(0.25f, 0.75f, 0.95f);
        private static readonly Color AccentModules = new Color(0.60f, 0.48f, 0.95f);
        private static readonly Color AccentLogging = new Color(0.55f, 0.62f, 0.70f);
        private static readonly Color AccentOverlay = new Color(0.92f, 0.45f, 0.70f);
        private static readonly Color AccentScheduling = new Color(0.95f, 0.62f, 0.25f);
        private static readonly Color AccentTicking = new Color(0.35f, 0.85f, 0.55f);
        private static readonly Color PillOn = new Color(0.22f, 0.62f, 0.35f);
        private static readonly Color PillOff = new Color(0.45f, 0.48f, 0.52f);

        private GUIStyle _bannerTitleStyle;
        private GUIStyle _bannerSubStyle;
        private GUIStyle _bannerGlyphStyle;
        private GUIStyle _sectionTitleStyle;
        private GUIStyle _pillStyle;

        private void OnEnable()
        {
            _logEnabled = Find("LogEnabled");
            _logLevel = Find("LogLevel");
            _schedulerEnable = Find("SchedulerEnable");
            _tickingEnable = Find("TickingEnable");
            _clock = Find("Clock");
            _serverTimeSync = Find("ServerTimeSync");
            _clockOverlay = Find("ClockOverlay");
            _clockCheatOverlay = Find("ClockCheatOverlay");
            _scheduler = Find("Scheduler");
            _ticking = Find("Ticking");
        }

        private SerializedProperty Find(string propertyName)
            => serializedObject.FindProperty($"<{propertyName}>k__BackingField");

        private static SerializedProperty FindRelative(SerializedProperty parent, string propertyName)
            => parent.FindPropertyRelative($"<{propertyName}>k__BackingField");

        public override void OnInspectorGUI()
        {
            BuildStyles();
            serializedObject.Update();

            DrawBanner();
            GUILayout.Space(8);

            DrawSection("Logging", AccentLogging, () =>
            {
                EditorGUILayout.PropertyField(_logEnabled);
                using (new EditorGUI.DisabledScope(!_logEnabled.boolValue))
                    EditorGUILayout.PropertyField(_logLevel);
            });

            DrawSection("Modules", AccentModules, () =>
            {
                DrawModuleRow(_schedulerEnable, "Scheduler", "Delayed and repeated actions via Chronos.Scheduler.");
                DrawModuleRow(_tickingEnable, "Ticking", "Fixed-rate gameplay ticks via Chronos.TickProvider.");
            });

            DrawSection("Clock", AccentClock, () =>
            {
                EditorGUILayout.PropertyField(_clock);
                EditorGUILayout.PropertyField(_serverTimeSync);
            });

            DrawSection("Overlays", AccentOverlay, () =>
            {
                EditorGUILayout.PropertyField(_clockOverlay);
                EditorGUILayout.PropertyField(_clockCheatOverlay);
            });

            DrawSection("Scheduling", AccentScheduling, () =>
            {
                using (new EditorGUI.DisabledScope(!_schedulerEnable.boolValue))
                    EditorGUILayout.PropertyField(_scheduler);

                if (!_schedulerEnable.boolValue)
                    EditorGUILayout.LabelField("Scheduler module is disabled.", EditorStyles.centeredGreyMiniLabel);
            });

            DrawSection("Ticking", AccentTicking, () =>
            {
                using (new EditorGUI.DisabledScope(!_tickingEnable.boolValue))
                    EditorGUILayout.PropertyField(_ticking);

                if (!_tickingEnable.boolValue)
                    EditorGUILayout.LabelField("Ticking module is disabled.", EditorStyles.centeredGreyMiniLabel);
            });

            GUILayout.Space(4);
            DrawWarnings();
            DrawFooter();

            serializedObject.ApplyModifiedProperties();
        }

        // ─── Banner ───────────────────────────────────────────────────

        private void DrawBanner()
        {
            Rect rect = GUILayoutUtility.GetRect(0, 58, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, BannerBg);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 4, rect.height), AccentClock);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 2, rect.width, 2), AccentClock * 0.6f);

            var glyphRect = new Rect(rect.x + 14, rect.y, 40, rect.height);
            GUI.Label(glyphRect, "◷", _bannerGlyphStyle);

            var titleRect = new Rect(rect.x + 56, rect.y + 9, rect.width - 60, 24);
            GUI.Label(titleRect, "CHRONOS", _bannerTitleStyle);

            var subRect = new Rect(rect.x + 56, rect.y + 32, rect.width - 60, 18);
            GUI.Label(subRect, "Time, scheduling & ticking — project settings", _bannerSubStyle);
        }

        // ─── Sections ─────────────────────────────────────────────────

        private void DrawSection(string title, Color accent, System.Action drawContent)
        {
            string stateKey = "Chronos.Settings." + title;
            bool open = SessionState.GetBool(stateKey, true);

            Rect header = GUILayoutUtility.GetRect(0, 26, GUILayout.ExpandWidth(true));
            Color headerBg = EditorGUIUtility.isProSkin
                ? new Color(0.17f, 0.18f, 0.20f)
                : new Color(0.80f, 0.81f, 0.83f);

            if (header.Contains(Event.current.mousePosition))
                headerBg = Color.Lerp(headerBg, accent, 0.08f);

            EditorGUI.DrawRect(header, headerBg);
            EditorGUI.DrawRect(new Rect(header.x, header.y, 3, header.height), accent);

            var arrowRect = new Rect(header.x + 8, header.y + 5, 16, 16);
            EditorGUI.Foldout(arrowRect, open, GUIContent.none, false);

            var titleRect = new Rect(header.x + 24, header.y, header.width - 28, header.height);
            GUI.Label(titleRect, title, _sectionTitleStyle);

            if (GUI.Button(header, GUIContent.none, GUIStyle.none))
            {
                open = !open;
                SessionState.SetBool(stateKey, open);
            }

            if (open)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(2);
                drawContent();
                GUILayout.Space(2);
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(4);
        }

        private void DrawModuleRow(SerializedProperty enableProp, string label, string description)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(enableProp, new GUIContent(label, description));

            bool on = enableProp.boolValue;
            Rect pillRect = GUILayoutUtility.GetRect(64, 16, GUILayout.Width(64));
            pillRect.y += 1;
            EditorGUI.DrawRect(pillRect, on ? PillOn : PillOff);
            GUI.Label(pillRect, on ? "ENABLED" : "OFF", _pillStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField(description, EditorStyles.miniLabel);
            GUILayout.Space(2);
        }

        // ─── Validation & footer ──────────────────────────────────────

        private void DrawWarnings()
        {
            var cheatEnable = FindRelative(_clock, "CheatEnable");
            if (cheatEnable != null && cheatEnable.boolValue)
                EditorGUILayout.HelpBox("Clock cheat overlay is enabled — remember to turn it off for release builds.", MessageType.Warning);

            var serverUrl = FindRelative(_serverTimeSync, "ServerUrl");
            var syncOnStart = FindRelative(_serverTimeSync, "SyncOnAppStart");
            if (serverUrl != null && string.IsNullOrWhiteSpace(serverUrl.stringValue)
                && syncOnStart != null && syncOnStart.boolValue)
                EditorGUILayout.HelpBox("Server sync is enabled but the server URL is empty.", MessageType.Warning);

            var ticksPerSecond = FindRelative(_ticking, "TicksPerSecond");
            if (_tickingEnable.boolValue && ticksPerSecond != null && ticksPerSecond.intValue <= 0)
                EditorGUILayout.HelpBox("Ticking is enabled but Ticks Per Second must be greater than 0.", MessageType.Error);
        }

        private void DrawFooter()
        {
            EditorGUILayout.HelpBox(
                "Resolution order: explicit asset on ChronosBootstrapper → Resources/" + ChronosSettings.ResourcesFileName + " → package defaults.",
                MessageType.Info);
        }

        // ─── Styles ───────────────────────────────────────────────────

        private void BuildStyles()
        {
            if (_bannerTitleStyle != null)
                return;

            _bannerTitleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 17,
                normal = { textColor = Color.white }
            };

            _bannerSubStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 10,
                normal = { textColor = new Color(0.65f, 0.72f, 0.80f) }
            };

            _bannerGlyphStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 30,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = AccentClock }
            };

            _sectionTitleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleLeft
            };

            _pillStyle = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 9,
                normal = { textColor = Color.white }
            };
        }
    }
}
