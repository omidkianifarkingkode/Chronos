using Kingkode.Chronos.Ticking.Configurations;
using Kingkode.Chronos.Ticking.Services;
using System;
using UnityEngine;

namespace Kingkode.Chronos.Ticking.Infrasturctures
{

    /// <summary>
    /// Unity MonoBehaviour implementation of ITickSystem.
    /// 
    /// Runs a fixed-rate tick loop using an accumulator.
    /// This ensures ticks occur at a deterministic frequency
    /// regardless of frame rate.
    /// 
    /// Features:
    /// - Fixed ticks per second
    /// - Pause support
    /// - Time scaling
    /// - Spiral-of-death protection (MaxTicksPerFrame)
    /// - Global tick counter
    /// </summary>
    public sealed class TickProvider : ITickProvider
    {
        public bool Paused { get; private set; }
        public float TimeScale { get; private set; } = 1f;

        public int TicksPerSecond => _configurations.TicksPerSecond;

        /// <summary>
        /// Global deterministic tick index.
        /// </summary>
        public long CurrentTick { get; private set; }

        public event Action<float> OnAppTick;
        public event Action<long, float> OnGameplayTick;

        private float tickInterval;
        private float accumulator;

        private TickingOptions _configurations;
        private ILogger _logger;

        public TickProvider(TickingOptions tickSystemConfigurations, ILogger logger)
        {
            _configurations = tickSystemConfigurations;
            _logger = logger;

            RecalculateInterval();
        }

        /// <summary>
        /// Recompute the tick interval based on TPS.
        /// </summary>
        private void RecalculateInterval()
        {
            tickInterval = 1f / Mathf.Max(1, _configurations.TicksPerSecond);
        }

        public void SetTicksPerSecond(int tps)
        {
            _configurations.TicksPerSecond = Mathf.Max(1, tps);
            RecalculateInterval();
        }

        public void SetPaused(bool paused)
        {
            Paused = paused;
        }

        public void SetTimeScale(float scale)
        {
            TimeScale = Mathf.Max(0f, scale);
        }

        /// <summary>
        /// Unity update loop that accumulates real time
        /// and emits fixed-rate ticks.
        /// </summary>
        internal void Tick()
        {
            accumulator += Time.unscaledDeltaTime;

            int ticksThisFrame = 0;

            while (accumulator >= tickInterval && ticksThisFrame < _configurations.MaxTicksPerFrame)
            {
                accumulator -= tickInterval;
                ticksThisFrame++;

                CurrentTick++;

                float appDt = tickInterval;
                OnAppTick?.Invoke(appDt);

                float gameplayDt = Paused ? 0f : appDt * TimeScale;
                OnGameplayTick?.Invoke(CurrentTick, gameplayDt);
            }

            // If too many ticks were required, drop remaining accumulated time
            // to prevent runaway catch-up.
            if (ticksThisFrame == _configurations.MaxTicksPerFrame)
            {
                accumulator = 0f;
            }
        }
    }
}
