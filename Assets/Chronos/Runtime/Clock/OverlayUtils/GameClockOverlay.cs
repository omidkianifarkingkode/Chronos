using Kingkode.Chronos.Clock.Configurations;
using Kingkode.Chronos.Clock.Services;
using UnityEngine;

namespace Kingkode.Chronos.Clock.Overlay
{
    public class GameClockOverlay : MonoBehaviour
    {
        // Configured via ChronosSettings (this component is added at runtime, so the
        // settings asset is the only way consumers can adjust it).
        private ClockOverlayOptions _options;

        // Draggable rect for the permanent display
        private Rect displayRect;

        // Drag state
        private bool dragging = false;
        private Vector2 dragOffset;

        private GUIStyle _panelStyle;
        private GUIStyle _timeStyle;

        private int _lastScreenWidth;
        private int _lastScreenHeight;

        private IClock clock;

        private void Awake()
        {
            var chronos = FindAnyObjectByType<ChronosBootstrapper>();
            chronos.OnServicesInitialized.AddListener((services) =>
            {
                clock = services.Resolve<IClock>();
                _options = services.Resolve<ChronosSettings>().ClockOverlay;

                displayRect = _options.DisplayRect.ToScreenRect();
            });
        }

        private void CreateStyles()
        {
            if (_panelStyle != null)
                return;

            _panelStyle = ChronosGuiTheme.MakePanel(12);
            _timeStyle = ChronosGuiTheme.MakeLabel(_options.FontSize, ChronosGuiTheme.TextPrimary, bold: true);
        }

        private const string BlockerId = "clock-overlay";

        private void OnDisable()
        {
            ChronosOverlayRaycastBlocker.RemoveRegion(BlockerId);
        }

        private void OnGUI()
        {
            if (clock == null) return;

            if (_lastScreenWidth != Screen.width || _lastScreenHeight != Screen.height)
            {
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;

                displayRect = _options.DisplayRect.ToScreenRect();
            }

            CreateStyles();

            HandleDragging(ref displayRect);
            DrawClockPanel(displayRect, clock.Now.ToString(_options.TimeFormat));

            // Mirror the panel into the EventSystem so clicks on it don't fall
            // through to gameplay input.
            ChronosOverlayRaycastBlocker.SetRegion(BlockerId, displayRect);
        }

        private void DrawClockPanel(Rect rect, string text)
        {
            Color trustColor = ChronosGuiTheme.TrustColor(clock.TrustedLevel);

            // Panel background
            GUI.Box(rect, GUIContent.none, _panelStyle);

            // Trust-level dot, vertically centered on the left
            float dotSize = Mathf.Clamp(rect.height * 0.22f, 10f, 18f);
            var dotRect = new Rect(rect.x + 14, rect.y + (rect.height - dotSize) * 0.5f, dotSize, dotSize);
            GUI.DrawTexture(dotRect, ChronosGuiTheme.Rounded(trustColor, (int)(dotSize * 0.5f)));

            // Time text
            float textX = dotRect.xMax + 10;
            GUI.Label(new Rect(textX, rect.y, rect.xMax - textX - 8, rect.height - 4), text, _timeStyle);

            // Thin trust strip along the bottom edge
            GUI.DrawTexture(new Rect(rect.x + 10, rect.yMax - 5, rect.width - 20, 3), ChronosGuiTheme.Solid(trustColor));
        }

        private void HandleDragging(ref Rect rect)
        {
            Event e = Event.current;

            if (_options.Draggable &&
                e.type == EventType.MouseDown &&
                rect.Contains(e.mousePosition))
            {
                dragging = true;
                dragOffset = e.mousePosition - new Vector2(rect.x, rect.y);
                e.Use();
            }

            if (dragging && e.type == EventType.MouseDrag)
            {
                rect.x = Mathf.Clamp(
                    e.mousePosition.x - dragOffset.x,
                    0,
                    Screen.width - rect.width);

                rect.y = Mathf.Clamp(
                    e.mousePosition.y - dragOffset.y,
                    0,
                    Screen.height - rect.height);

                _options.DisplayRect.FromScreenRect(displayRect);

                e.Use();
            }

            if (e.type == EventType.MouseUp)
                dragging = false;
        }
    }
}
