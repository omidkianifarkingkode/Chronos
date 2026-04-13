using System;

namespace Kingkode.Chronos.Ticking.Services
{
    /// <summary>
    /// Provides a centralized fixed-rate ticking system.
    /// 
    /// The tick system emits two types of ticks:
    /// 
    /// App Tick:
    ///     Runs regardless of pause state. Useful for UI, timers, background logic.
    /// 
    /// Gameplay Tick:
    ///     Affected by pause and time scale. Used for gameplay simulation.
    /// 
    /// Ticks run at a fixed frequency defined by TicksPerSecond.
    /// </summary>
    public interface ITickProvider
    {
        /// <summary>
        /// True if gameplay ticks are paused.
        /// </summary>
        bool Paused { get; }

        /// <summary>
        /// Multiplier applied to gameplay delta time.
        /// </summary>
        float TimeScale { get; }

        /// <summary>
        /// Number of ticks executed per second.
        /// </summary>
        int TicksPerSecond { get; }

        /// <summary>
        /// Global tick counter since system start.
        /// </summary>
        long CurrentTick { get; }

        /// <summary>
        /// Called every application tick (unscaled).
        /// </summary>
        event Action<float> OnAppTick;

        /// <summary>
        /// Called every gameplay tick (scaled and pause-aware).
        /// </summary>
        event Action<long, float> OnGameplayTick;

        /// <summary>
        /// Pause or resume gameplay ticks.
        /// </summary>
        void SetPaused(bool paused);

        /// <summary>
        /// Set gameplay time scale.
        /// </summary>
        void SetTimeScale(float scale);

        /// <summary>
        /// Change the tick frequency at runtime.
        /// </summary>
        void SetTicksPerSecond(int tps);
    }
}
