using Kingkode.Chronos.Scheduling.Configurations;
using Kingkode.Chronos.Scheduling.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kingkode.Chronos.Scheduling
{
    public class ActionScheduler : IActionScheduler, IDisposable
    {
        private long _nextId = 1;
        private long _currentTickTimeMs;
        private bool _isDisposed;

        private readonly List<ScheduleNode> _activeTasks;
        private readonly List<ScheduleNode> _pendingTasks;
        private readonly Stack<ScheduleNode> _pool;

        private readonly ActionSchedulerOptions _options;
        private readonly ILogger _logger;

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

        public ScheduleHandle After(long delayMs, Action callback, string tag = null)
            => Schedule(_currentTickTimeMs + delayMs, 0, callback, tag);

        public ScheduleHandle After(TimeSpan delay, Action callback, string tag = null)
            => After((long)delay.TotalMilliseconds, callback, tag);
        public ScheduleHandle Every(long intervalMs, Action callback, string tag = null)
            => Schedule(_currentTickTimeMs + intervalMs, intervalMs, callback, tag);

        public ScheduleHandle Every(TimeSpan interval, Action callback, string tag = null)
            => Every((long)interval.TotalMilliseconds, callback, tag);

        public ScheduleHandle EverySecond(Action callback, string tag = null)
        {
            // Calculate milliseconds remaining until the next exact 1000ms boundary
            long msUntilNextSecond = 1000 - (_currentTickTimeMs % 1000);

            // Schedule it to fire at that boundary, and repeat every 1000ms
            return Schedule(_currentTickTimeMs + msUntilNextSecond, 1000, callback, tag);
        }


        public ScheduleHandle Schedule(long targetTimeMs, long intervalMs, Action callback, string tag = null)
        {
            if (_isDisposed) 
                return new ScheduleHandle(0);
            
            if (callback == null) 
                return new ScheduleHandle(0);

            var node = _pool.Count > 0 ? _pool.Pop() : new ScheduleNode();

            node.Id = _nextId++;
            node.Tag = tag;
            node.TargetTimeMs = targetTimeMs;
            node.IntervalMs = intervalMs;
            node.Callback = callback;
            node.IsCancelled = false;

            _pendingTasks.Add(node);

            return new ScheduleHandle(node.Id);
        }

        public void Cancel(ScheduleHandle handle)
        {
            if (_isDisposed || !handle.IsValid) return;

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

        public void CancelAll(string tag = null)
        {
            if (_isDisposed) return;

            // If tag is null or empty, we consider it a request to cancel EVERYTHING
            bool cancelEverything = string.IsNullOrEmpty(tag);

            for (int i = 0; i < _activeTasks.Count; i++)
            {
                if (cancelEverything || _activeTasks[i].Tag == tag)
                {
                    _activeTasks[i].IsCancelled = true;
                }
            }

            for (int i = 0; i < _pendingTasks.Count; i++)
            {
                if (cancelEverything || _pendingTasks[i].Tag == tag)
                {
                    _pendingTasks[i].IsCancelled = true;
                }
            }
        }


        /// <summary>
        /// Advances the scheduler. Should be called every frame (e.g., in Update).
        /// </summary>
        public void Tick(long ms)
        {
            if (_isDisposed) return;

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

        public void Dispose()
        {
            if (_isDisposed) return;

            // 1. Release references inside the nodes to prevent memory leaks 
            // (especially important for the Action delegates).
            foreach (var task in _activeTasks) task.Clear();
            foreach (var task in _pendingTasks) task.Clear();

            // Pooled tasks should already be cleared when they enter the pool, 
            // but doing it again is safe if you want to be perfectly thorough.
            foreach (var task in _pool) task.Clear();

            // 2. Clear the collections
            _activeTasks.Clear();
            _pendingTasks.Clear();
            _pool.Clear();

            _isDisposed = true;
        }

    }
}
