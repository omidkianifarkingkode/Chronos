# Repository Review (excluding `Runtime/Ticking`)

Date: 2026-04-28

## High-priority findings

1. **TLS certificate validation is fully bypassed in server time sync.**  
   `BypassCertificateHandler.ValidateCertificate` always returns `true`, which allows MITM and spoofed `Date` headers. This weakens the trust model of `ChronosClock.SyncWithServer`.  
   Relevant files: `DefaultServerTimeSyncer.cs`.

2. **Potential null reference during module initialization.**  
   `ChronosBootstrapper.InitializeModules()` assumes both child bootstrappers exist and dereferences `GetComponentInChildren(...).gameObject` without null checks. Scenes/prefabs that omit one module can crash at startup.  
   Relevant files: `ChronosBootstrapper.cs`.

3. **Persistent settings include an unused field.**  
   `ServerTimeSyncOptions.SyncInterval` is defined but never consumed, so users can configure an interval that has no effect.  
   Relevant files: `ServerTimeSyncOptions.cs`, `ClockBootstrapper.cs`, `DefaultServerTimeSyncer.cs`.

## Medium-priority findings

4. **Naming/typos reduce API clarity.**  
   Several public-facing names contain typos (`Synce`, `Bootstapper`, `Infrasturctures`) that will become long-term API debt if this package is consumed externally.

5. **Snapshot writes do not force-flush PlayerPrefs.**  
   `DefaultTimeSnapshotStorage.Save` sets JSON but does not call `PlayerPrefs.Save()`. While Unity eventually flushes, crash-sensitive data may not persist deterministically.

6. **`IClock.Now` implementation is awkward.**  
   `Now` returns `DateTimeOffset` but is derived via `UtcNow.LocalDateTime` (`DateTime`) implicit conversion. `UtcNow.ToLocalTime()` is clearer and avoids conversion ambiguity.

## Low-priority findings / polish

7. **`Duration.Millisecond` is semantically confusing.**  
   Value is `1000` (ms per second), but the name reads as `1ms`. Current calculations work, but readability suffers.

8. **Formatting edge cases for negative durations.**  
   `TimeFormat` helpers don’t guard/normalize negative inputs; output can become odd (`-1:-5`, etc.). If negatives are out of scope, document assumptions.

9. **Clock trust transition visibility.**  
   On weak snapshot load, `TrustedLevel` can remain `Unknown` until `UtcNow` is read at least once. Consider eagerly evaluating once during init if callers rely on immediate trust state.

## What looks good

- `ActionScheduler` uses object pooling and reverse-iteration removal, which is efficient and GC-conscious.
- Scheduler callback exception handling is configurable via `ActionSchedulerOptions.ExceptionHandler`.
- Clock tamper signals are explicit (`Tamper` enum + events), making integration straightforward.
