using Kingkode.Chronos.Clock.Configurations;
using Kingkode.Chronos.Clock.Persistences;
using Kingkode.Chronos.Clock.Services;
using System;
using UnityEngine;

namespace Kingkode.Chronos.Clock.Infrasturctures
{
    public sealed class ChronosClock : IClock
    {
        public DateTimeOffset UtcNow
        {
            get
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(TrustedUnixMsNow);
            }
        }
        public DateTimeOffset Now => UtcNow.LocalDateTime;

        public TrustedLevel TrustedLevel
        {
            get => _trustedLevel;
            private set
            {
                if (_trustedLevel == value)
                    return;

                _trustedLevel = value;

                _snapshot.lastTrustedLevel = _trustedLevel;

                _logger.Log(LogType.Log, "[Chronos] [Clock] Trusted level change to : " + _trustedLevel);

                TrustedLevelChanged.Invoke(_trustedLevel);
            }
        }
        private TrustedLevel _trustedLevel = TrustedLevel.Unknown;
        public event Action<TrustedLevel> TrustedLevelChanged = delegate { };

        public long TrustedUnixMsNow => ComputeTrustedUnixMs(false);
        public long UnixTimeSeconds => UtcNow.ToUnixTimeSeconds();

        public bool SuspectedTampering { get; private set; }
        public int TamperCount => _snapshot.tamperCount;

        public event Action<Tamper> OnTamperDetected = delegate { };

        private readonly ITimeSnapshotStorage _storage;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ISystemTickProvider _tickProvider;
        private readonly GameClockOptions _config;
        private readonly ILogger _logger;

        private GameClockSnapshot _snapshot;

        public ChronosClock(ITimeSnapshotStorage storage, IDateTimeProvider dateTimeProvider, ISystemTickProvider tickProvider, ILogger logger, GameClockOptions config)
        {
            _storage = storage;
            _dateTimeProvider = dateTimeProvider;
            _tickProvider = tickProvider;
            _logger = logger;
            _config = config;

            CheckExistingSnapshot();
        }

        /// <summary>Call when you successfully fetch server UTC.</summary>
        public void SyncWithServer(long serverUnixMs)
        {
            if (serverUnixMs < 0)
                return;

            var serverTime = DateTimeOffset.FromUnixTimeMilliseconds(serverUnixMs);
            var deviceMs = _dateTimeProvider.UtcNow.ToUnixTimeMilliseconds();

            _logger.Log(LogType.Log, $"[Chronos] [Clock] Synced with server time:" +
                $"Server:{serverTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}, " +
                $"Local:{_dateTimeProvider.Now:yyyy-MM-dd HH:mm:ss}");

            ResetBaseline(serverUnixMs, TrustedLevel.Strong);
            _snapshot.deviceUnixMsAtSync = deviceMs;

            // Optional: detect large device skew for analytics (non-blocking)
            CheckForClockAnomalies(deviceMs, serverUnixMs);

            _storage.Save(_snapshot);
        }

        /// <summary>Call on app resume to detect device clock jumps (non-blocking).</summary>
        public void CheckClockJumping()
        {
            // compute trusted “now” using monotonic
            var trustedNow = ComputeTrustedUnixMs();

            // compare to device time to detect big jumps (diagnostic only)
            var deviceMs = _dateTimeProvider.UtcNow.ToUnixTimeMilliseconds();
            var delta = Math.Abs(deviceMs - trustedNow);

            // threshold: choose your own policy
            if (delta > _config.DeltaThreshold)
            {
                TrustedLevel = TrustedLevel.Weak;
                FlagTamper(Tamper.DeviceClockResumeDivergence);
            }

            // additional sanity: trusted time should never go backwards across sessions
            if (trustedNow < _snapshot.lastKnownTrustedUnixMs)
            {
                TrustedLevel = TrustedLevel.Weak;
                FlagTamper(Tamper.TrustedTimeRegression);
            }

            _snapshot.lastKnownTrustedUnixMs = Math.Max(_snapshot.lastKnownTrustedUnixMs, trustedNow);
            _storage.Save(_snapshot);
        }

        private void CheckExistingSnapshot()
        {
            var hasSnapshot = false;
            if (_storage.TryLoad(out _snapshot))
                hasSnapshot = _snapshot.stopwatchFrequency > 0;

            if (!hasSnapshot)
            {
                _logger.Log(LogType.Log, "[Chronos] [Clock] No valid snapshot found - bootstrapping with device time");

                // bootstrap with device time, but once synced, device time is only diagnostic
                var deviceMs = _dateTimeProvider.UtcNow.ToUnixTimeMilliseconds();
                ResetBaseline(deviceMs, TrustedLevel.Weak);

                _storage.Save(_snapshot);

                return;
            }

            // check again if strong and medium level
            if (_snapshot.lastTrustedLevel != TrustedLevel.Weak)
            {
                TrustedLevel = TrustedLevel.Medium; // set a medium by default

                ComputeTrustedUnixMs();
            }
        }

        private long ComputeTrustedUnixMs(bool flagTamper = true)
        {
            var monoNow = _tickProvider.GetTimestamp();
            var monoDeltaTicks = monoNow - _snapshot.monotonicTicksAtSync;

            // convert delta ticks to ms
            var ms = (monoDeltaTicks * 1000L) / _snapshot.stopwatchFrequency;
            var trusted = _snapshot.trustedUnixMsAtSync + ms;

            // monotonic safety: if ms < 0 something is very wrong
            if (ms < 0)
            {
                TrustedLevel = TrustedLevel.Weak;
                if (flagTamper) FlagTamper(Tamper.MonotonicDeltaError);
            }
            else if (trusted < _snapshot.lastKnownTrustedUnixMs)
            {
                TrustedLevel = TrustedLevel.Weak;
                if (flagTamper) FlagTamper(Tamper.TrustedTimeRegression);
            }
            else
            {
                TrustedLevel = _snapshot.lastTrustedLevel;
            }

            return trusted;
        }

        private void ResetBaseline(long trustedUnixMs, TrustedLevel trustedLevel)
        {
            _snapshot.trustedUnixMsAtSync = trustedUnixMs;
            _snapshot.monotonicTicksAtSync = _tickProvider.GetTimestamp();
            _snapshot.stopwatchFrequency = _tickProvider.Frequency;
            _snapshot.lastKnownTrustedUnixMs = trustedUnixMs;
            _snapshot.tamper = default;
            _snapshot.lastTrustedLevel = trustedLevel;

            SuspectedTampering = false;
        }

        private void CheckForClockAnomalies(long deviceUnixMs, long trustedUnixMs)
        {
            var skew = Math.Abs(deviceUnixMs - trustedUnixMs);
            if (skew > _config.SkewThreshold)
                FlagTamper(Tamper.LargeServerSyncSkew);
        }

        private void FlagTamper(Tamper tamper)
        {
            SuspectedTampering = true;
            _snapshot.tamperCount++;
            _snapshot.tamper = tamper;

            _logger.Log(LogType.Warning, $"[Chronos] [Clock] Tamper detected: {tamper}");

            OnTamperDetected.Invoke(tamper);
        }
    }
}
