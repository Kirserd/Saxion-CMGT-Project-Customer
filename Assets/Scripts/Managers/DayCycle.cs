using UnityEngine;

public class DayCycle : MonoBehaviour
{
    public enum TimeInterval
    {
        Morning = 6,
        Afternoon = 11,
        Evening = 16,
        Night = 21,
        All = 255
    }
    public delegate void OnTimeIntervalChangedHandler(TimeInterval interval);
    public static OnTimeIntervalChangedHandler OnTimeIntervalChanged;

    public delegate void OnTimeChangedHandler(float hour, float minute);
    public static OnTimeChangedHandler OnTimeChanged;

    [Header("Time-related parameters")]
    [SerializeField]
    [Range(1F, 10F)]
    private float _timeMultiplier = 1f;
    [SerializeField]
    [Range(0F, 500F)]
    private float _hourLength = 100f;
    [SerializeField]
    private float _timer = 0f;

    [Header("Translated time")]
    [SerializeField]
    private float _hour = 8f;
    [SerializeField]
    private float _minute = 0f;
    [SerializeField]
    private TimeInterval _interval = TimeInterval.Morning;
    private TimeInterval _prevInterval = TimeInterval.Evening;

    private void Start()
    {
        Refresh();
        OnTimeIntervalChanged.Invoke(TimeInterval.All);
    }
    private void Refresh() => _timer = _hour * _hourLength;

    private void Update()
    {
        CountTime();
        ValidateInterval();
    }

    private void CountTime()
    {
        _timer += Time.deltaTime * _timeMultiplier;
        _hour = Mathf.Floor(_timer / _hourLength);
        _minute = Mathf.Round((_timer - _hourLength * _hour) / _hourLength * 60);
        OnTimeChanged?.Invoke(_hour, _minute);
    }

    private void ValidateInterval()
    {
        if (_interval != _prevInterval)
        {
            UpdateTasks();
            return;
        }
        else
        {
            if (_hour > (int)_interval + 5)
                _interval = (TimeInterval)((int)_interval + 5);
        }
    }

    private void UpdateTasks()
    {
        OnTimeIntervalChanged.Invoke(_interval);
        _prevInterval = _interval;
    }

    public static string IntervalToDigit(TimeInterval interval)
    {
        int start = (int)interval + 2;
        int end = start + 4;

        return (start < 13 ? start : start - 12) + 
               (start < 13 ? "AM" : "PM") + 
               " - " + 
               (end < 13 ? end : end - 12) + 
               (end < 13 ? "AM" : "PM");
    }
}
