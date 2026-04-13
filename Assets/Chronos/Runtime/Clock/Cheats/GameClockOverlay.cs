using Kingkode.Chronos.Clock.Services;
using UnityEngine;

namespace Kingkode.Chronos.Clock.Cheats
{
    public class GameClockOverlay : MonoBehaviour
    {
        [Header("UI Settings")]
        // Draggable rect for the permanent display
        [SerializeField] Rect displayRect = new(0, 0, 130, 25); // Increased size for better borders/padding

        // Drag state
        private bool dragging = false;
        private Vector2 dragOffset;

        private GUIStyle[] trustedStyles;
        private IClock clock;

        private void Awake()
        {
            ChronosBootstrapper.Instance.OnServicesInitialized.AddListener((services) =>
            {
                clock = services.Resolve<IClock>();
            });
        }

        private void CreateStyles()
        {
            if (trustedStyles != null)
                return;

            Color getColor(int i) => i switch
            {
                3 => new Color(0.3f, 0.7f, 0.3f), // Strong
                2 => new Color(0.8f, 0.7f, 0.2f), // Medium
                1 => new Color(0.8f, 0.2f, 0.2f), // Weak
                _ => new Color(0.5f, 0.5f, 0.5f)  // Unknown
            };

            trustedStyles = new GUIStyle[4];

            for (int i = 0; i < trustedStyles.Length; i++)
            {
                // Create a new style based on GUI.skin.box to easily get a border/background
                trustedStyles[i] = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleCenter,
                    // Adjust padding to look good with the border
                    padding = new RectOffset(10, 10, 5, 5),

                    // Set the text color
                    normal = { textColor = getColor(i) },
                    fontStyle = FontStyle.Bold
                    // You can further customize the border appearance here if needed
                    // For example, by setting border widths, but GUI.skin.box provides a default look.
                };
            }
        }

        private void OnGUI()
        {
            if (clock == null) return;

            CreateStyles();

            // Draw the permanent, draggable label with a border
            DrawDraggableLabel(ref displayRect, clock.Now.ToString("yy-MM-dd HH:mm:ss"));
        }

        private void DrawDraggableLabel(ref Rect rect, string text)
        {
            int trust = (int)clock.TrustedLevel;
            GUIStyle style = trustedStyles[trust];

            Event e = Event.current;

            // --- Dragging Logic ---
            // Detect start drag
            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                dragging = true;
                dragOffset = e.mousePosition - new Vector2(rect.x, rect.y);
                e.Use();
            }

            // Drag move
            if (dragging && e.type == EventType.MouseDrag)
            {
                rect.x = Mathf.Max(0, e.mousePosition.x - dragOffset.x);
                rect.y = Mathf.Max(0, e.mousePosition.y - dragOffset.y);
                e.Use();
            }

            // Stop drag
            if (e.type == EventType.MouseUp)
                dragging = false;
            // ----------------------


            // Draw the label with the border style
            GUI.Label(rect, text, style);
        }
    }
}
