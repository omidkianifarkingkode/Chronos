using UnityEngine;
using System;
using System.Collections.Generic;
using Kingkode.Chronos.Scheduling;
using Kingkode.Chronos; // Added your namespace

public class SchedulerUITester : MonoBehaviour
{
    [Tooltip("Drag your ActionScheduler component here")]
    public IActionScheduler Scheduler => Chronos.Scheduler;

    private ScheduleHandle _repeatingHandle;

    // For logging to the GUI
    private List<string> _logMessages = new List<string>();
    private Vector2 _scrollPosition;

    private void Log(string message)
    {
        string timeStr = Time.time.ToString("F1");
        _logMessages.Add($"[{timeStr}] {message}");

        // Keep only the last 20 messages to prevent memory buildup
        if (_logMessages.Count > 20) _logMessages.RemoveAt(0);

        // Auto-scroll to bottom
        _scrollPosition.y = Mathf.Infinity;
    }

    void OnGUI()
    {
        if (Scheduler == null)
        {
            GUI.Label(new Rect(10, 10, 400, 30), "Please assign the ActionScheduler in the Inspector!");
            return;
        }

        int buttonWidth = 200;
        int buttonHeight = 40;
        int spacing = 10;
        int yPos = 10;

        GUI.Label(new Rect(10, yPos, buttonWidth, 20), "<b>ActionScheduler Tests</b>");
        yPos += 30;

        // [ 1 ] Test Single Delay
        if (GUI.Button(new Rect(10, yPos, buttonWidth, buttonHeight), "1. Test 'After' (2s)"))
        {
            Log("Scheduled 'After' task for 2000ms.");
            Scheduler.After(2000, () => Log(" -> 'After' task executed!"));
        }
        yPos += buttonHeight + spacing;

        // [ 2 ] Test Repeating Task
        if (GUI.Button(new Rect(10, yPos, buttonWidth, buttonHeight), "2. Start 'Every' (0.5s)"))
        {
            Log("Starting 500ms repeating task.");
            _repeatingHandle = Scheduler.Every(500, () => Log(" -> Beep! (500ms)"));
        }
        yPos += buttonHeight + spacing;

        // [ 3 ] Test Handle Cancellation
        if (GUI.Button(new Rect(10, yPos, buttonWidth, buttonHeight), "3. Cancel 'Every' Task"))
        {
            if (_repeatingHandle.IsValid)
            {
                Scheduler.Cancel(_repeatingHandle);
                Log("Repeating task cancelled via Scheduler.");

                // FIXED: Use default instead of internal constructor
                _repeatingHandle = default;
            }
            else
            {
                Log("No valid handle to cancel.");
            }
        }

        yPos += buttonHeight + spacing;

        // [ 4 ] Test Synchronized EverySecond
        if (GUI.Button(new Rect(10, yPos, buttonWidth, buttonHeight), "4. Start 'EverySecond'"))
        {
            Log("Scheduling an EverySecond task...");
            Scheduler.EverySecond(() => Log(" -> Synchronized Second Tick!"));
        }
        yPos += buttonHeight + spacing;

        // [ 5 ] Test Tagging
        if (GUI.Button(new Rect(10, yPos, buttonWidth, buttonHeight), "5. Spawn Tagged Tasks"))
        {
            Log("Spawning 3 'Enemies' tasks and 1 untagged task.");

            // UPDATED: Pass the tag as a parameter based on your interface
            Scheduler.Every(1000, () => Log(" -> Goblin alive!"), "Enemies");
            Scheduler.Every(1200, () => Log(" -> Orc alive!"), "Enemies");
            Scheduler.Every(1500, () => Log(" -> Troll alive!"), "Enemies");

            Scheduler.Every(2000, () => Log(" -> Untagged UI Element ticking..."));
        }
        yPos += buttonHeight + spacing;

        // [ 6 ] Test Tag Cancellation
        if (GUI.Button(new Rect(10, yPos, buttonWidth, buttonHeight), "6. Cancel Tag 'Enemies'"))
        {
            Log("Cancelling all tasks with tag 'Enemies'...");
            Scheduler.CancelAll("Enemies");
        }
        yPos += buttonHeight + spacing;

        // [ 7 ] Test Exception Handling
        if (GUI.Button(new Rect(10, yPos, buttonWidth, buttonHeight), "7. Test Exception Crash"))
        {
            Log("Scheduling a crashing task in 1s...");
            Scheduler.After(1000, () => throw new InvalidOperationException("Simulated crash!"));
            Scheduler.After(1500, () => Log(" -> Scheduler survived the exception!"));
        }

        // --- Draw Log Label Area ---
        int logX = buttonWidth + 30;
        GUI.Label(new Rect(logX, 10, 200, 20), "<b>Event Log:</b>");

        // Create a scroll view for the log text
        Rect viewRect = new Rect(0, 0, Screen.width - logX - 40, _logMessages.Count * 25);
        _scrollPosition = GUI.BeginScrollView(
            new Rect(logX, 40, Screen.width - logX - 20, Screen.height - 60),
            _scrollPosition,
            viewRect
        );

        string fullLog = string.Join("\n", _logMessages);
        GUI.Label(viewRect, fullLog);

        GUI.EndScrollView();
    }
}
