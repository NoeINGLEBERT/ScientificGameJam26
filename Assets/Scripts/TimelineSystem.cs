using UnityEngine;
using TMPro;
using System;

public class TimelineSystem : MonoBehaviour
{
    [Header("Time Settings")]
    public float monthDuration = 30f;

    [Range(0, 11)]
    public int startMonth = 0;

    [Header("UI")]
    public RectTransform timelineBar;
    public float timelineWidth = 1000f;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI temperatureText;

    [Header("Timeline Controls")]
    public float januaryOffset = 0f;
    public int direction = 1;

    [Header("Temperature (Min / Max / Avg per month)")]
    public Vector3[] monthlyTemperatures = new Vector3[12];
    // x = min, y = max, z = avg

    // =========================
    // EVENTS
    // =========================
    public Action<int> OnMonthEnded;

    private float timer;
    private int currentMonth;
    private int currentDay;
    private float currentTemperature;

    private bool isPaused = false;

    private readonly int[] daysInMonths = new int[]
    {
        31,28,31,30,31,30,31,31,30,31,30,31
    };

    private readonly string[] monthNames = new string[]
    {
        "January","February","March","April","May","June",
        "July","August","September","October","November","December"
    };

    public TreeSystem tree;

    void Start()
    {
        currentMonth = startMonth;
        timer = 0f;

        GenerateTemperature(); // initial temp
        UpdateDateUI();
    }

    void Update()
    {
        if (isPaused) return;

        float dt = Time.deltaTime;
        timer += dt;

        float monthProgress = timer / monthDuration;

        // END OF MONTH
        if (monthProgress >= 1f)
        {
            PauseForEvent();
            return;
        }

        UpdateDay(monthProgress);
        UpdateTimelineVisual(monthProgress);
    }

    // =========================
    // PAUSE / EVENT SYSTEM
    // =========================
    void PauseForEvent()
    {
        isPaused = true;
        tree.isPaused = true;

        // Notify external systems
        OnMonthEnded?.Invoke(currentMonth);
    }

    public void ResumeAfterEvent()
    {
        isPaused = false;
        tree.isPaused = false;

        timer -= monthDuration;
        NextMonth();
    }

    // =========================
    // DAY + TEMPERATURE
    // =========================
    void UpdateDay(float monthProgress)
    {
        int daysThisMonth = daysInMonths[currentMonth];

        int newDay = Mathf.FloorToInt(monthProgress * daysThisMonth) + 1;
        newDay = Mathf.Clamp(newDay, 1, daysThisMonth);

        if (newDay != currentDay)
        {
            currentDay = newDay;

            GenerateTemperature(); // NEW DAY = NEW TEMP
            UpdateDateUI();
        }
    }

    void GenerateTemperature()
    {
        if (monthlyTemperatures.Length < 12) return;

        Vector3 data = monthlyTemperatures[currentMonth];

        float min = data.x;
        float max = data.y;
        float avg = data.z;

        // Bias toward average (more natural distribution)
        float t = UnityEngine.Random.value;
        float biased = Mathf.Lerp(min, max, Mathf.Lerp(t, 0.5f, 0.5f));

        currentTemperature = biased;

        UpdateTemperatureUI();
    }

    // =========================
    // MONTH LOOP
    // =========================
    void NextMonth()
    {
        currentMonth = (currentMonth + 1) % 12;
        currentDay = 1;

        GenerateTemperature();
        UpdateDateUI();
    }

    // =========================
    // UI
    // =========================
    void UpdateDateUI()
    {
        if (dateText != null)
        {
            dateText.text = $"{currentDay} {monthNames[currentMonth]}";
        }
    }

    void UpdateTemperatureUI()
    {
        if (temperatureText != null)
        {
            temperatureText.text = $"{(int)currentTemperature}°C";
        }
    }

    void UpdateTimelineVisual(float progress)
    {
        if (timelineBar == null) return;

        float movement = (progress + currentMonth) * timelineWidth * direction;
        float x = januaryOffset + movement;

        x = x % (timelineWidth * 12);

        timelineBar.anchoredPosition = new Vector2(x, timelineBar.anchoredPosition.y);
    }

    // =========================
    // GETTERS
    // =========================
    public int GetCurrentMonth() => currentMonth;
    public int GetCurrentDay() => currentDay;
    public float GetTemperature() => currentTemperature;
}