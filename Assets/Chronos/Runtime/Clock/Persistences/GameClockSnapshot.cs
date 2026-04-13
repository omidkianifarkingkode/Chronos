using System;

namespace Kingkode.Chronos.Clock.Persistences
{
    [Serializable]
    public struct GameClockSnapshot
    {
        public long trustedUnixMsAtSync;
        public long monotonicTicksAtSync;
        public long stopwatchFrequency;
        public long deviceUnixMsAtSync;     // only for tamper diagnostics
        public int tamperCount;
        public TrustedLevel lastTrustedLevel;
        public Tamper tamper;
        public long lastKnownTrustedUnixMs; // persisted “last computed” for sanity checks
    }
}
