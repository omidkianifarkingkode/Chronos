using System;
using UnityEngine;

namespace Kingkode.Chronos.Clock.Configurations
{
    /// <summary>
    /// Layout and presentation of the permanent clock overlay (GameClockOverlay),
    /// shown while GameClockOptions.OverlayEnable is on.
    /// </summary>
    [Serializable]
    public class ClockOverlayOptions
    {
        [field: SerializeField]
        public NormalizedRect DisplayRect { get; set; } = new()
        {
            X = 0.02f,
            Y = 0.02f,
            Width = 0.25f,
            Height = 0.08f
        };

        [Tooltip("Allow repositioning the clock label by dragging it.")]
        [field: SerializeField] public bool Draggable { get; set; } = true;

        public int FontSize => Mathf.Clamp(Mathf.RoundToInt(Screen.width * FontScale), 12, 48);
        [field: SerializeField] public float FontScale { get; set; } = 0.015f;

        [Tooltip("DateTime format string used to render the clock.")]
        [field: SerializeField] public string TimeFormat { get; set; } = "yy-MM-dd HH:mm:ss";
    }

    [Serializable]
    public struct NormalizedRect
    {
        [Range(0f, 1f)]
        public float X;

        [Range(0f, 1f)]
        public float Y;

        [Range(0f, 1f)]
        public float Width;

        [Range(0f, 1f)]
        public float Height;

        public Rect ToScreenRect()
        {
            return new Rect(X * Screen.width, Y * Screen.height, Width * Screen.width, Height * Screen.height);
        }

        public void FromScreenRect(Rect rect)
        {
            X = rect.x / Screen.width;
            Y = rect.y / Screen.height;
            Width = rect.width / Screen.width;
            Height = rect.height / Screen.height;
        }
    }
}
