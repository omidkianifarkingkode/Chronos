using System;

namespace Kingkode.Chronos.Clock.Persistences
{
    [Serializable]
    public sealed class Tamper : IComparable<Tamper>
    {
        public string Issue { get; }
        public string Code { get; }

        public Tamper(string code, string issue)
        {
            Code = code;
            Issue = issue;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Tamper)
                return false;
            return ((Tamper)obj).Code == Code;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public static bool operator ==(Tamper left, Tamper right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Tamper left, Tamper right)
        {
            return !(left == right);
        }

        public int CompareTo(Tamper other)
        {
            if (other is null) return 1;

            return string.Compare(this.Code, other.Code, StringComparison.Ordinal);
        }

        public static readonly Tamper DeviceClockResumeDivergence = new("DateTime.Manipulate", "Device clock differs from trusted time on resume.");
        public static readonly Tamper TrustedTimeRegression = new("CpuTick.Backward", "Trusted time went backwards (snapshot anomaly).");
        public static readonly Tamper MonotonicDeltaError = new("Device.Reboot", "Monotonic delta < 0.");
        public static readonly Tamper LargeServerSyncSkew = new("DateTime.Skew", "Large device/server time skew at sync.");
    }
}