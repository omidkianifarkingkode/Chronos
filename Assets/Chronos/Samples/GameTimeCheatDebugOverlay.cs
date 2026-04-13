using Assets.Chronos.Samples;
using Kingkode.Chronos.Clock.Infrasturctures;
using UnityEngine;

public class GameTimeCheatDebugOverlay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameClockTest bootstrapper; // drag the bootstrapper here

    [SerializeField] Rect rect = new(10, 10, 380, 250);

    private ChronosClock _service;
    private GUIStyle _style;

    private void Start()
    {
        if (bootstrapper == null)
        {
            bootstrapper = FindObjectOfType<GameClockTest>();
        }

        if (bootstrapper != null)
        {
            _service = bootstrapper.clock;
        }
    }

    private void OnGUI()
    {
        if (_service == null)
        {
            return;
        }

        // Create style only once
        if (_style == null)
        {
            _style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                normal = { textColor = Color.yellow },
                padding = new RectOffset(10, 10, 8, 8),
                alignment = TextAnchor.UpperLeft
            };
        }

        // Background semi-transparent box
        GUILayout.BeginArea(rect);
        GUI.Box(new Rect(0, 0, rect.width, rect.height), "");
        GUILayout.BeginVertical();

        // GUILayout.Label("Game Time Debug", _style);

        GUILayout.Label($"Real Date Time: {bootstrapper.dateTimeProvider.UtcNow:T} ({bootstrapper.dateTimeProvider.Now:T})", _style);
        GUILayout.Label($"Fake Date Time: {bootstrapper.fakeDateTimeProvider.UtcNow:T} ({bootstrapper.fakeDateTimeProvider.Now:T})", _style);
        GUILayout.Label($"Game Date Time: {_service.UtcNow:T} ({_service.Now:T})", _style);

        // Unix seconds
        GUILayout.Label($"Real Ticks: {bootstrapper.systemTickProvider.GetTimestamp() / 10000000L}", _style);
        GUILayout.Label($"Fake Ticks: {bootstrapper.fakeSystemTickProvider.GetTimestamp() / 10000000L}", _style);

        // Suspicious flag
        string suspiciousText = _service.SuspectedTampering
            ? "<color=red>SUSPICIOUS MODE ACTIVE (fallback to wall-clock)</color>"
            : "<color=green>Monotonic mode trusted</color>";

        GUILayout.Label($"Reset Detected: {suspiciousText}", _style);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Cheat System Time: ", _style);
        if (GUILayout.Button("+1 H"))
        {
            bootstrapper.CheatDateTime(1);
        }
        if (GUILayout.Button("-1 H"))
        {
            bootstrapper.CheatDateTime(-1);
        }
        if (GUILayout.Button("Reset"))
        {
            bootstrapper.ResetDateTime();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Cheat System Tick: ", _style);
        if (GUILayout.Button("+1 H"))
        {
            bootstrapper.CheatSystemTick(10000L * 1000 * 60 * 60);
        }
        if (GUILayout.Button("-1 H"))
        {
            bootstrapper.CheatSystemTick(-10000L * 1000 * 60 * 60);
        }
        if (GUILayout.Button("Reset"))
        {
            bootstrapper.ResetSystemTick();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Save"))
        {
            bootstrapper.clock.CheckClockJumping();
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}