using Kingkode.Chronos.Scheduling.Configurations;
using Kingkode.Chronos.Scheduling.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kingkode.Chronos.Scheduling
{

    public class ActionScheduler : IActionScheduler
    {

        private List<ScheduleNode> _activeTasks;
        private List<ScheduleNode> _pendingTasks;
        private Stack<ScheduleNode> _pool;

        private long _nextId = 1;
        private long _currentTickTimeMs;

        private ActionSchedulerOptions _options;
        private ILogger _logger;

        public ActionScheduler(ActionSchedulerOptions actionSchedulerOptions, ILogger logger)
        {
            _options = actionSchedulerOptions;
            _logger = logger;

            _activeTasks = new List<ScheduleNode>();
            _pendingTasks = new List<ScheduleNode>();
            _pool = new Stack<ScheduleNode>(_options.InitialPoolCapacity);

            // Pre-allocate the pool to prevent initial startup spikes
            for (int i = 0; i < _options.InitialPoolCapacity; i++)
            {
                _pool.Push(new ScheduleNode());
            }
        }

        public ScheduleHandle After(long delayMs, Action callback)
            => Schedule(_currentTickTimeMs + delayMs, 0, callback);

        public ScheduleHandle After(TimeSpan delay, Action callback)
            => After((long)delay.TotalMilliseconds, callback);

        public ScheduleHandle Every(long intervalMs, Action callback)
            => Schedule(_currentTickTimeMs + intervalMs, intervalMs, callback);

        public ScheduleHandle Every(TimeSpan interval, Action callback)
            => Every((long)interval.TotalMilliseconds, callback);

        public ScheduleHandle Schedule(long targetTimeMs, long intervalMs, Action callback)
        {
            if (callback == null)
                return new ScheduleHandle(0);

            var node = _pool.Count > 0 ? _pool.Pop() : new ScheduleNode();

            node.Id = _nextId++;
            node.TargetTimeMs = targetTimeMs;
            node.IntervalMs = intervalMs;
            node.Callback = callback;
            node.IsCancelled = false;

            _pendingTasks.Add(node);

            return new ScheduleHandle(node.Id);
        }

        public void Cancel(ScheduleHandle handle)
        {
            if (!handle.IsValid) return;

            // Mark cancelled in active list
            for (int i = 0; i < _activeTasks.Count; i++)
            {
                if (_activeTasks[i].Id == handle.Id)
                {
                    _activeTasks[i].IsCancelled = true;
                    return;
                }
            }

            // Mark cancelled in pending list
            for (int i = 0; i < _pendingTasks.Count; i++)
            {
                if (_pendingTasks[i].Id == handle.Id)
                {
                    _pendingTasks[i].IsCancelled = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Advances the scheduler. Should be called every frame (e.g., in Update).
        /// </summary>
        public void Tick(long ms)
        {
            // Cache current time so After/Every called INSIDE a callback use the exact same timestamp
            _currentTickTimeMs = ms;

            if (_pendingTasks.Count > 0)
            {
                _activeTasks.AddRange(_pendingTasks);
                _pendingTasks.Clear();
            }

            // Iterate backwards for O(1) removal without breaking the loop
            for (int i = _activeTasks.Count - 1; i >= 0; i--)
            {
                ScheduleNode task = _activeTasks[i];

                if (task.IsCancelled)
                {
                    RemoveAndPool(i);
                    continue;
                }

                if (_currentTickTimeMs >= task.TargetTimeMs)
                {
                    ExecuteCallbackSafely(task);

                    // Callback might have called Cancel() on itself
                    if (task.IsCancelled)
                    {
                        RemoveAndPool(i);
                        continue;
                    }

                    if (task.IntervalMs > 0)
                    {
                        task.TargetTimeMs = _currentTickTimeMs + task.IntervalMs;
                    }
                    else
                    {
                        RemoveAndPool(i);
                    }
                }
            }
        }

        private void ExecuteCallbackSafely(ScheduleNode task)
        {
            if (_options.ExceptionHandler == null)
            {
                // No handler configured, execute directly (might throw and break the loop)
                task.Callback.Invoke();
                return;
            }

            try
            {
                task.Callback.Invoke();
            }
            catch (Exception ex)
            {
                _options.ExceptionHandler.Invoke(ex);
            }
        }

        private void RemoveAndPool(int index)
        {
            ScheduleNode task = _activeTasks[index];

            // Fast O(1) removal: swap with last, then remove last
            int lastIndex = _activeTasks.Count - 1;
            _activeTasks[index] = _activeTasks[lastIndex];
            _activeTasks.RemoveAt(lastIndex);

            task.Clear();
            _pool.Push(task);
        }
    }
}
