using Kingkode.Chronos.Clock.Infrasturctures;
using Kingkode.Chronos.Clock.Persistences;
using Kingkode.Chronos.Clock.Services;
using UnityEngine;

namespace Kingkode.Chronos.Clock.Cheats
{
    public class CheatGameClockDebugOverlay : MonoBehaviour
    {
        [SerializeField] Rect showRect = new(0, 25, 450, 360);
        [SerializeField] Rect hideRect = new(0, 25, 30, 22);
        [SerializeField] bool showClock = true;

        private GameClockCheat _cheat;
        private IClock _clock;
        private IDateTimeProvider _dateTimeProvider;
        private ISystemTickProvider _systemTickProvider;

        private FakeDateTimeProvider _serverDateTimeProvider;
        private DefaultDateTimeProvider _realDateTimeProvider;
        private DefaultSystemTickProvider _realSystemTickProvider;

        private GUIStyle _style;
        private Tamper _tamper;

        private void Awake()
        {
            var bootstrapper = FindFirstObjectByType<ChronosBootstrapper>();

            bootstrapper.OnServicesInitialized.AddListener((services) =>
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

            CreateStyle();

            if (showClock) Show();
            else Hide();
        }

        private void Show()
        {
            showClock = true;

            // Background semi-transparent box
            GUILayout.BeginArea(showRect);
            GUI.Box(new Rect(0, 0, showRect.width, showRect.height), "");
            if (GUILayout.Button("<<"))
            {
                Hide();
            }

            GUILayout.BeginVertical();

            GUILayout.Label($"{_realDateTimeProvider.UtcNow:T} ({_realDateTimeProvider.Now:T}) - Real Date Time", _style);

            string color =
                _clock.TrustedLevel == TrustedLevel.Weak ? "red" :
                _clock.TrustedLevel == TrustedLevel.Medium ? "yellow" :
                _clock.TrustedLevel == TrustedLevel.Strong ? "green" : "gray";
            //GUILayout.Label($"{_services.Clock.UtcNow:T} ({_services.Clock.Now:T}) - Game Date Time (Trusted:<color={color}>{_services.Clock.TrustedLevel}</color>)", _style);

            GUILayout.Label($"{_serverDateTimeProvider.UtcNow:T} ({_serverDateTimeProvider.Now:T}) - Server Date Time", _style);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+1 M"))
            {
                _cheat.CheatServerDateTime(1);
            }
            if (GUILayout.Button("-1 M"))
            {
                _cheat.CheatServerDateTime(-1);
            }
            if (GUILayout.Button("Reset"))
            {
                _cheat.ResetServerDateTime();
            }

            GUILayout.EndHorizontal();

            GUILayout.Label($"{_dateTimeProvider.UtcNow:T} ({_dateTimeProvider.Now:T}) - Local Date Time", _style);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+1 H"))
            {
                _cheat.CheatLocalDateTime(1);
            }
            if (GUILayout.Button("-1 H"))
            {
                _cheat.CheatLocalDateTime(-1);
            }
            if (GUILayout.Button("Reset"))
            {
                _cheat.ResetLocalDateTime();
            }

            GUILayout.EndHorizontal();

            // Unix seconds
            GUILayout.Label($"Real Ticks: {_realSystemTickProvider.GetTimestamp() / 10000000L}", _style);
            GUILayout.Label($"Fake Ticks: {_systemTickProvider.GetTimestamp() / 10000000L}", _style);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+1 H"))
            {
                _cheat.CheatSystemTick(10000L * 1000 * 60 * 60);
            }
            if (GUILayout.Button("-1 H"))
            {
                _cheat.CheatSystemTick(-10000L * 1000 * 60 * 60);
            }
            if (GUILayout.Button("Reset"))
            {
                _cheat.ResetSystemTick();
            }
            GUILayout.EndHorizontal();

            // Suspicious flag
            string suspiciousText = _tamper == null ?
                "<color=green>No Tamper</color>" :
                $"<color=red>{_tamper.Issue}</color>";

            GUILayout.Label($"Tamper Detected: {suspiciousText}", _style);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Sync"))
            {
                _cheat.SynceWithServer();
            }
            if (GUILayout.Button("Save"))
            {
                _clock.CheckClockJumping();
            }
            if (GUILayout.Button("Clear Cache"))
            {
                _cheat.ClearCache();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void Hide()
        {
            showClock = false;

            GUILayout.BeginArea(hideRect);

            if (GUILayout.Button(">>"))
            {
                Show();
            }

            GUILayout.EndArea();
        }

        private void CreateStyle()
        {
            if (_style != null)
                return;

            _style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                normal = { textColor = Color.yellow },
                padding = new RectOffset(10, 10, 8, 8),
                alignment = TextAnchor.UpperLeft
            };
        }
    }
}