using System;
using UnityEngine;

namespace Kingkode.Chronos.Ticking.Configurations
{
    [Serializable]
    public class TickingOptions 
    {
        [field: SerializeField] public int TicksPerSecond { get; set; } = 60;

        /// <summary>
        /// Maximum ticks allowed in a single frame to avoid
        /// spiral-of-death when frames stall.
        /// </summary>
        [field: SerializeField] public int MaxTicksPerFrame { get; set; } = 5;
    }
}
