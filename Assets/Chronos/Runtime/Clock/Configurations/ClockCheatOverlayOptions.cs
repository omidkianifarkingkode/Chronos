using System;
using UnityEngine;

namespace Kingkode.Chronos.Clock.Configurations
{
    /// <summary>
    /// Layout and presentation of the clock cheat debug panel (CheatGameClockDebugOverlay),
    /// shown while GameClockOptions.CheatEnable is on.
    /// </summary>
    [Serializable]
    public class ClockCheatOverlayOptions
    {
        [Tooltip("Design resolution the rects and font sizes below are authored against. " +
                 "At runtime the whole panel is scaled uniformly to fit the actual screen, " +
                 "so it keeps these proportions at any resolution. Set X or Y to 0 to disable scaling.")]
        [field: SerializeField] public Vector2 ReferenceResolution { get; set; } = new Vector2(1920f, 1080f);

        [Tooltip("Screen rect of the expanded cheat panel (in ReferenceResolution space).")]
        [field: SerializeField] public Rect ShowRect { get; set; } = new Rect(50, 160, 1350, 620);

        [Tooltip("Screen rect of the collapsed panel (only the expand button), in ReferenceResolution space.")]
        [field: SerializeField] public Rect HideRect { get; set; } = new Rect(0, 165, 350, 500);

        [Tooltip("Whether the panel starts expanded or collapsed.")]
        [field: SerializeField] public bool StartExpanded { get; set; } = true;

        [field: SerializeField] public int LabelFontSize { get; set; } = 32;
        [field: SerializeField] public int ButtonFontSize { get; set; } = 26;
        [field: SerializeField] public int ButtonHeight { get; set; } = 50;

        [field: SerializeField] public float DragHandleWidth { get; set; } = 60f;


        [Tooltip("Height of the expand (>>) button shown while the panel is collapsed.")]
        [field: SerializeField] public int ExpandButtonHeight { get; set; } = 75;
    }
}
