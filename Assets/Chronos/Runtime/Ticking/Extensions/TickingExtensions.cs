using Kingkode.Chronos.Ticking.Services;

namespace Kingkode.Chronos.Ticking.Extensions
{
    public static class TickingExtensions
    {
        public static long FromSeconds(this ITickProvider tickSystem, float seconds)
        {
            return (long)(seconds * tickSystem.TicksPerSecond);
        }

        public static long FromMilliseconds(this ITickProvider tickSystem, float ms)
        {
            return (long)(ms * tickSystem.TicksPerSecond / 1000f);
        }

        public static float ToSeconds(this ITickProvider tickSystem, long ticks)
        {
            return ticks / (float)tickSystem.TicksPerSecond;
        }
    }
}
