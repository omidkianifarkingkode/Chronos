namespace Kingkode.Chronos.Scheduling
{
    using System;

    /// <summary>
    /// Service for scheduling real-time System.Action callbacks.
    /// </summary>
    public interface IActionScheduler
    {
        /// <summary>
        /// Executes the callback once after the specified delay in milliseconds.
        /// </summary>
        ScheduleHandle After(long delayMs, Action callback, string tag = null);

        /// <summary>
        /// Executes the callback once after the specified TimeSpan.
        /// </summary>
        ScheduleHandle After(TimeSpan delay, Action callback, string tag = null);

        /// <summary>
        /// Executes the callback repeatedly at the specified interval in milliseconds.
        /// </summary>
        ScheduleHandle Every(long intervalMs, Action callback, string tag = null);

        /// <summary>
        /// Executes the callback repeatedly at the specified TimeSpan interval.
        /// </summary>
        ScheduleHandle Every(TimeSpan interval, Action callback, string tag = null);

        /// <summary>
        /// Executes the callback every second.
        /// </summary>
        ScheduleHandle EverySecond(Action callback, string tag = null);

        /// <summary>
        /// Cancels a scheduled task safely using its handle.
        /// </summary>
        void Cancel(ScheduleHandle handle);

        /// <summary>
        /// Cancels all scheduled tasks with the specified tag.
        /// If no tag is provided, all tasks will be cancelled.
        /// </summary>
        /// <param name="tag"></param>
        void CancelAll(string tag = null);
    }
}
