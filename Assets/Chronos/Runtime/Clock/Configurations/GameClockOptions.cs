using System;
using UnityEngine;

namespace Kingkode.Chronos.Clock.Configurations
{
    [Serializable]
    public class GameClockOptions
    {
        [field: SerializeField] public bool OverlayEnable { get; set; } = true;
        [field: SerializeField] public long SkewThreshold { get; set; } = 10 * 60 * Duration.Millisecond; // 10 min
        [field: SerializeField] public long DeltaThreshold { get; set; } = 5 * 60 * Duration.Millisecond; // 5 min
        [field: SerializeField] public bool CheatEnable { get; set; } = false;
    }
}
