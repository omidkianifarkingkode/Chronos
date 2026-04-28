using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Kingkode.Chronos.Formatting;

public class DateTimeUiElement : MonoBehaviour
{
    public enum TimerMode { dateTime, duration }

    [SerializeField] TimerMode mode;
    [SerializeField] int maxDurationSeconds;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI text;

    public DateTimeOffset TimeOffset { get; private set; } = DateTimeOffset.Now;
    public TimeSpan Duration => TimeOffset - DateTimeOffset.Now;

    private void Update() 
    {
        TimeOffset = DateTimeOffset.Now.AddSeconds(slider.value * maxDurationSeconds);
        
        if(mode == TimerMode.dateTime)
            text.SetText(TimeOffset.ToString("g"));
        else 
            text.SetText(Duration.ToString("t"));
    }
}
