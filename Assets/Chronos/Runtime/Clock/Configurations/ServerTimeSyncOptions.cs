using System;
using UnityEngine;

namespace Kingkode.Chronos.Clock.Configurations
{
    [Serializable]
    public class ServerTimeSyncOptions 
    {
        [field: SerializeField] public string ServerUrl { get; set; } = "https://google.com";
        [field: SerializeField] public bool SyncOnAppStart { get; set; } = true;
        [field: SerializeField] public bool SyncOnAppFocus { get; set; } = true;
        [field: SerializeField] public int SyncInterval { get; set; } = 0;
    }
}
