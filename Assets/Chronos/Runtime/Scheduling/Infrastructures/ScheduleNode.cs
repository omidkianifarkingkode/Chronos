namespace Kingkode.Chronos.Scheduling.Internal
{
    using System;

    /// <summary>
    /// Internal representation of a scheduled task. 
    /// Pooled to achieve zero allocations.
    /// </summary>
    internal class ScheduleNode
    {
        public long Id;
        public string Tag;
        public long TargetTimeMs;
        public long IntervalMs;
        public Action Callback;
        public bool IsCancelled;

        public void Clear()
        {
            Id = 0;
            Tag = null;
            TargetTimeMs = 0;
            IntervalMs = 0;
            Callback = null; // Important: Release the closure reference!
            IsCancelled = false;
        }
    }
}
