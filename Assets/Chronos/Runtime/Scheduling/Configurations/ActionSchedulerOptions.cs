namespace Kingkode.Chronos.Scheduling.Configurations
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Configuration options for the Action Scheduler.
    /// </summary>
    [Serializable]
    public class ActionSchedulerOptions
    {
        /// <summary>
        /// How many nodes to pre-allocate on startup to avoid initial garbage generation.
        /// Default is 64.
        /// </summary>
        [field: SerializeField] public int InitialPoolCapacity { get; set; } = 64;

        /// <summary>
        /// Defines how the scheduler handles exceptions thrown by scheduled Actions.
        /// If null, exceptions might bubble up and interrupt the Tick loop.
        /// </summary>
        [field: SerializeField] public UnityEvent<Exception> ExceptionHandler { get; set; }
    }
}
