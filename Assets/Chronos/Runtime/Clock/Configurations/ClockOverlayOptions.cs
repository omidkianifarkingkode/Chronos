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
        [Tooltip("Screen rect of the clock label.")]
        [field: SerializeField] public Rect DisplayRect { get; set; } = new Rect(75, 50, 345, 100);

        [Tooltip("Allow repositioning the clock label by dragging it.")]
        [field: SerializeField] public bool Draggable { get; set; } = true;

        [field: SerializeField] public int FontSize { get; set; } = 24;

        [Tooltip("DateTime format string used to render the clock.")]
        [field: SerializeField] public string TimeFormat { get; set; } = "yy-MM-dd HH:mm:ss";
    }
}
