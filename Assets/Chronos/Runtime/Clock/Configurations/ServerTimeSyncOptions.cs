using System;
using UnityEngine;

namespace Kingkode.Chronos.Clock.Configurations
{
    [Serializable]
    public class ServerTimeSyncOptions
    {
        [field: SerializeField] public string ServerUrl { get; set; } = "https://google.com";
        [field: SerializeField] public bool SyncOnAppStart { get; set; } = true;
        [field: SerializeField] public bool SyncOnAppFocus { get; set; } = false;
        [field: SerializeField] public int SyncInterval { get; set; } = 0;

        [Tooltip("Seconds before the time request is aborted. 0 = no timeout.")]
        [field: SerializeField] public int TimeoutSeconds { get; set; } = 10;

        [Tooltip("Skip SSL certificate validation on the time request, so syncing still works when the device clock is wrong. Disable to enforce normal validation.")]
        [field: SerializeField] public bool BypassCertificateValidation { get; set; } = true;
    }
}
