using Kingkode.Chronos.Clock.Persistences;
using System;

namespace Kingkode.Chronos.Clock.Services
{
    public interface IClock
    {
        DateTimeOffset UtcNow { get; }
        DateTimeOffset Now { get; }

        TrustedLevel TrustedLevel { get; }
        event Action<TrustedLevel> TrustedLevelChanged;

        bool SuspectedTampering { get; }
        event Action<Tamper> OnTamperDetected;

        void SyncWithServer(long serverUnixMs);
        void CheckClockJumping();
    }
}
