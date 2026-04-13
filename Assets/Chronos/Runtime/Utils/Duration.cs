namespace Kingkode.Chronos
{
    public static class Duration
    {
        public const long Millisecond = 1000;

        public static long Milliseconds(this int v) => v;
        public static long Seconds(this int v) => (long)v * Millisecond;
        public static long Minutes(this int v) => (long)v * 60 * Millisecond;
        public static long Hours(this int v) => (long)v * 60 * 60 * Millisecond;
        public static long Days(this int v) => (long)v * 24 * 60 * 60 * Millisecond;
    }
}
