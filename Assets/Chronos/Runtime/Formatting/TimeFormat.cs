using System.Text;

namespace Kingkode.Chronos.Formatting
{
    public static class TimeFormat
    {
        public static string HHmmss(this long totalSeconds)
        {
            var sb = StringBuilderPool.Get();
            var absSeconds = AppendSignAndGetAbsoluteSeconds(sb, totalSeconds);

            var h = absSeconds / 3600;
            var m = (absSeconds % 3600) / 60;
            var s = absSeconds % 60;

            Append2(sb, h); sb.Append(':');
            Append2(sb, m); sb.Append(':');
            Append2(sb, s);

            return StringBuilderPool.ToStringAndRelease(sb);
        }

        public static string MMss(this long totalSeconds)
        {
            var sb = StringBuilderPool.Get();
            var absSeconds = AppendSignAndGetAbsoluteSeconds(sb, totalSeconds);

            var m = absSeconds / 60;
            var s = absSeconds % 60;

            Append2(sb, m); sb.Append(':');
            Append2(sb, s);

            return StringBuilderPool.ToStringAndRelease(sb);
        }

        public static string Compact(this long totalSeconds)
        {
            var sb = StringBuilderPool.Get();
            var absSeconds = AppendSignAndGetAbsoluteSeconds(sb, totalSeconds);
            var hasUnit = false;

            var days = absSeconds / 86400; absSeconds %= 86400;
            var hours = absSeconds / 3600; absSeconds %= 3600;
            var mins = absSeconds / 60;
            var secs = absSeconds % 60;

            if (days > 0) { sb.Append(days).Append("d "); hasUnit = true; }
            if (hours > 0) { sb.Append(hours).Append("h "); hasUnit = true; }
            if (mins > 0) { sb.Append(mins).Append("m "); hasUnit = true; }
            if (secs > 0 || !hasUnit) { sb.Append(secs).Append("s"); }

            return StringBuilderPool.ToStringAndRelease(sb);
        }

        public static string LargestUnit(this long totalSeconds)
        {
            var isNegative = totalSeconds < 0;
            var absSeconds = ToAbsoluteSeconds(totalSeconds);

            string value;
            if (absSeconds >= 86400) value = (absSeconds / 86400) + "d";
            else if (absSeconds >= 3600) value = (absSeconds / 3600) + "h";
            else if (absSeconds >= 60) value = (absSeconds / 60) + "m";
            else value = absSeconds + "s";

            return isNegative ? "-" + value : value;
        }

        private static ulong AppendSignAndGetAbsoluteSeconds(StringBuilder sb, long totalSeconds)
        {
            if (totalSeconds < 0)
                sb.Append('-');

            return ToAbsoluteSeconds(totalSeconds);
        }

        private static ulong ToAbsoluteSeconds(long totalSeconds)
        {
            // Handles long.MinValue safely without overflow.
            return totalSeconds < 0 ? (ulong)(-(totalSeconds + 1)) + 1UL : (ulong)totalSeconds;
        }

        private static void Append2(StringBuilder sb, ulong v)
        {
            if (v < 10) sb.Append('0');
            sb.Append(v);
        }
    }
}
