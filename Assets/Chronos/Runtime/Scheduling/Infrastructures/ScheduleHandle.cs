namespace Kingkode.Chronos.Scheduling
{
    using System;

    public readonly struct ScheduleHandle : IEquatable<ScheduleHandle>
    {
        public readonly long Id;
        public bool IsValid => Id > 0;

        internal ScheduleHandle(long id)
        {
            Id = id;
        }

        public bool Equals(ScheduleHandle other) => Id == other.Id;
        public override bool Equals(object obj) => obj is ScheduleHandle other && Equals(other);
        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(ScheduleHandle left, ScheduleHandle right) => left.Equals(right);
        public static bool operator !=(ScheduleHandle left, ScheduleHandle right) => !left.Equals(right);
    }
}
