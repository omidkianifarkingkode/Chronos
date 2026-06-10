using Kingkode.Chronos.Clock.Configurations;
using Kingkode.Chronos.Clock.Infrasturctures;
using Kingkode.Chronos.Clock.Persistences;
using Kingkode.Chronos.Clock.Services;
using UnityEngine;

namespace Kingkode.Chronos.Clock.Cheats
{
    public class CheatGameClockDebugOverlay : MonoBehaviour
    {
        // Configured via ChronosSettings (this component is added at runtime, so the
        // settings asset is the only way consumers can adjust it).
        private ClockCheatOverlayOptions _options;

        private Rect showRect;
        private Rect hideRect;
        private bool showClock;

        private GameClockCheat _cheat;
        private IClock _clock;
        private IDateTimeProvider _dateTimeProvider;
        private ISystemTickProvider _systemTickProvider;

        private FakeDateTimeProvider _serverDateTimeProvider;
        private DefaultDateTimeProvider _realDateTimeProvider;
        private DefaultSystemTickProvider _realSystemTickProvider;

        private GUIStyle _panelStyle;
        private GUIStyle _cardStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _sectionStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _accentButtonStyle;

        private Tamper _tamper;

        private void Awake()
        {
            _options = ChronosBootstrapper.Instance.Settings.ClockCheatOverlay;
            showRect = _options.ShowRect;
            hideRect = _options.HideRect;
            showClock = _options.StartExpanded;

            ChronosBootstrapper.Instance.OnServicesInitialized.AddListener((services) =>
            {
                _cheat = services.Resolve<GameClockCheat>();

                _clock = services.Resolve<IClock>();
                _dateTimeProvider = services.Resolve<IDateTimeProvider>();
                _systemTickProvider = services.Resolve<ISystemTickProvider>();

                _serverDateTimeProvider = services.Resolve<FakeDateTimeProvider>();
                _realDateTimeProvider = services.Resolve<DefaultDateTimeProvider>();
                _realSystemTickProvider = services.Resolve<DefaultSystemTickProvider>();

                _clock.OnTamperDetected += (tamper) => _tamper = tamper;
            });
        }

        private void OnGUI()
        {
            if (_clock == null) return;

            CreateStyles();

            if (showClock) Show();
            else Hide();
        }

        private void Show()
        {
            showClock = true;

            GUI.Box(showRect, GUIContent.none, _panelStyle);

            var inner = new Rect(showRect.x + 16, showRect.y + 12, showRect.width - 32, showRect.height - 24);
            GUILayout.BeginArea(inner);

            DrawHeader();

            // ── Real device time ──────────────────────────────────────
            BeginCard("REAL TIME");
            GUILayout.Label($"{_realDateTimeProvider.UtcNow:T}  <color=#{ChronosGuiTheme.ToHex(ChronosGuiTheme.TextDim)}>({_realDateTimeProvider.Now:T} local)</color>", _labelStyle);
            EndCard();

            // ── Server time cheat ─────────────────────────────────────
            BeginCard("SERVER TIME");
            GUILayout.Label($"{_serverDateTimeProvider.UtcNow:T}  <color=#{ChronosGuiTheme.ToHex(ChronosGuiTheme.TextDim)}>({_serverDateTimeProvider.Now:T} local)</color>", _labelStyle);
            GUILayout.BeginHorizontal();
            if (CheatButton("+1 Min")) _cheat.CheatServerDateTime(1);
            if (CheatButton("-1 Min")) _cheat.CheatServerDateTime(-1);
            if (CheatButton("Reset")) _cheat.ResetServerDateTime();
            GUILayout.EndHorizontal();
            EndCard();

            // ── Local time cheat ──────────────────────────────────────
            BeginCard("LOCAL TIME");
            GUILayout.Label($"{_dateTimeProvider.UtcNow:T}  <color=#{ChronosGuiTheme.ToHex(ChronosGuiTheme.TextDim)}>({_dateTimeProvider.Now:T} local)</color>", _labelStyle);
            GUILayout.BeginHorizontal();
            if (CheatButton("+1 Hour")) _cheat.CheatLocalDateTime(1);
            if (CheatButton("-1 Hour")) _cheat.CheatLocalDateTime(-1);
            if (CheatButton("Reset")) _cheat.ResetLocalDateTime();
            GUILayout.EndHorizontal();
            EndCard();

            // ── System tick cheat ─────────────────────────────────────
            BeginCard("SYSTEM TICKS");
            // Unix seconds
            GUILayout.Label($"Real: {_realSystemTickProvider.GetTimestamp() / 10000000L}    <color=#{ChronosGuiTheme.ToHex(ChronosGuiTheme.Accent)}>Fake: {_systemTickProvider.GetTimestamp() / 10000000L}</color>", _labelStyle);
            GUILayout.BeginHorizontal();
            if (CheatButton("+1 Hour")) _cheat.CheatSystemTick(10000L * 1000 * 60 * 60);
            if (CheatButton("-1 Hour")) _cheat.CheatSystemTick(-10000L * 1000 * 60 * 60);
            if (CheatButton("Reset")) _cheat.ResetSystemTick();
            GUILayout.EndHorizontal();
            EndCard();

            // ── Integrity ─────────────────────────────────────────────
            BeginCard("INTEGRITY");
            string trustHex = ChronosGuiTheme.ToHex(ChronosGuiTheme.TrustColor(_clock.TrustedLevel));
            string tamperText = _tamper == null
                ? $"<color=#{ChronosGuiTheme.ToHex(ChronosGuiTheme.StrongColor)}>No Tamper</color>"
                : $"<color=#{ChronosGuiTheme.ToHex(ChronosGuiTheme.WeakColor)}>{_tamper.Issue}</color>";
            GUILayout.Label($"Trust: <color=#{trustHex}>{_clock.TrustedLevel}</color>    Tamper: {tamperText}", _labelStyle);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Sync", _accentButtonStyle, GUILayout.Height(_options.ButtonHeight))) _cheat.SynceWithServer();
            if (CheatButton("Save")) _clock.CheckClockJumping();
            if (CheatButton("Clear Cache")) _cheat.ClearCache();
            GUILayout.EndHorizontal();
            EndCard();

            GUILayout.EndArea();
        }

        private void Hide()
        {
            showClock = false;

            GUILayout.BeginArea(hideRect);

            if (GUILayout.Button("» Time Cheats", _accentButtonStyle, GUILayout.Height(_options.ExpandButtonHeight)))
            {
                Show();
            }

            GUILayout.EndArea();
        }

        // ─── Layout helpers ───────────────────────────────────────────

        private void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"<color=#{ChronosGuiTheme.ToHex(ChronosGuiTheme.Accent)}>CHRONOS</color>  ·  TIME CHEATS", _titleStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("«", _buttonStyle, GUILayout.Width(_options.ButtonHeight * 2), GUILayout.Height(_options.ButtonHeight)))
            {
                Hide();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        private void BeginCard(string title)
        {
            GUILayout.BeginVertical(_cardStyle);
            GUILayout.Label(title, _sectionStyle);
        }

        private void EndCard()
        {
            GUILayout.EndVertical();
        }

        private bool CheatButton(string text)
        {
            return GUILayout.Button(text, _buttonStyle, GUILayout.Height(_options.ButtonHeight));
        }

        private void CreateStyles()
        {
            if (_panelStyle != null)
                return;

            _panelStyle = ChronosGuiTheme.MakePanel(14);
            _cardStyle = ChronosGuiTheme.MakeCard(10);
            _titleStyle = ChronosGuiTheme.MakeLabel(_options.LabelFontSize, ChronosGuiTheme.TextPrimary, bold: true);
            _sectionStyle = ChronosGuiTheme.MakeLabel(Mathf.Max(12, (int)(_options.LabelFontSize * 0.55f)), ChronosGuiTheme.TextDim, bold: true);
            _labelStyle = ChronosGuiTheme.MakeLabel(_options.LabelFontSize, ChronosGuiTheme.TextPrimary);
            _buttonStyle = ChronosGuiTheme.MakeButton(_options.ButtonFontSize);
            _accentButtonStyle = ChronosGuiTheme.MakeAccentButton(_options.ButtonFontSize);
        }
    }
}
