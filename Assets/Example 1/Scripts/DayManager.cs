using TMPro;
using UnityEngine;
using System.Collections;

public class DayManager : MonoBehaviour
{
    [Header("============ UI ============")]
    [SerializeField] private TMP_Text dayText;

    [Header("============ DAY CYCLE SETTINGS ============")]
    [SerializeField] private float dayDuration = 0.1f; // Default: 0.1 seconds per day
    [Range(0.05f, 5f)] public float dayDurationMin = 0.05f;
    [Range(0.1f, 10f)] public float dayDurationMax = 10f;

    [Header("============ PROGRESSION ============")]
    [SerializeField] private int currentDay = 1;
    [SerializeField] private int totalDays = 30;
    [SerializeField] private int rustStartDay = 20; // When to show rust stages

    [Header("============ REFERENCES ============")]
    [SerializeField] private WeatherManager weatherManager;
    [SerializeField] private ExampleManager exampleManager;
    [SerializeField] private GameObject normalNail;
    [SerializeField] private GameObject rustNail;
    [SerializeField] private NailMove nailMove;
    [SerializeField] private RustInspection rustInspection;

    // State
    private bool cycleRunning = false;
    private Coroutine cycleCoroutine;

    private void Start()
    {
        // Initialize UI
        if (dayText != null)
        {
            dayText.text = "Day: " + currentDay;
        }

        // Validate references
        if (normalNail != null)
            normalNail.SetActive(true);
        if (rustNail != null)
            rustNail.SetActive(false);
    }

    /// <summary>
    /// Called by NailMove when nail is inserted
    /// </summary>
    public void SetDayDuration(float duration)
    {
        dayDuration = Mathf.Clamp(duration, dayDurationMin, dayDurationMax);
    }

    /// <summary>
    /// Start the 30-day cycle
    /// Called automatically after nail insertion
    /// </summary>
    public void StartCycle()
    {
        if (cycleRunning)
            return;

        cycleRunning = true;
        currentDay = 1;

        // Start weather cycle
        if (weatherManager != null)
        {
            weatherManager.StartDayNight();
        }

        UpdateWeather();

        if (cycleCoroutine != null)
            StopCoroutine(cycleCoroutine);

        cycleCoroutine = StartCoroutine(DayCycle());
    }

    /// <summary>
    /// Main 30-day simulation loop
    /// </summary>
    private IEnumerator DayCycle()
    {
        while (currentDay < totalDays)
        {
            yield return new WaitForSeconds(dayDuration);

            currentDay++;
            if(currentDay >= totalDays)
{
    weatherManager.ForceSunnyFinalDay();

    StopAllCoroutines();

    //StartCoroutine(FinalInspection());
}

            // Update day display
            if (dayText != null)
            {
                dayText.text = "Day: " + currentDay;
            }

            // Update weather based on day
            UpdateWeather();

            // Check if time to show rust
            if (currentDay == rustStartDay)
            {
                TransitionToRust();
            }
        }

        // Cycle complete
        CycleComplete();
    }

    /// <summary>
    /// Update weather effects based on current day
    /// </summary>
    private void UpdateWeather()
    {
        if (weatherManager == null)
            return;

        // Weather pattern: Every 5 days change weather
        switch (currentDay)
        {
            case 1:
                weatherManager.SetSunny();
                break;
            case 6:
                weatherManager.SetLeafy();
                break;
            case 11:
                weatherManager.SetRainy();
                break;
            case 16:
                weatherManager.SetSunny();
                break;
            case 21:
                weatherManager.SetLeafy();
                break;
            case 26:
                weatherManager.SetRainy();
                break;
            case 30:
                weatherManager.SetSunny();
                weatherManager.StopDayNight();
                break;
        }
    }

    /// <summary>
    /// Transition from normal nail to rusty nail
    /// </summary>
    private void TransitionToRust()
    {
        if (normalNail != null)
            normalNail.SetActive(false);

        if (rustNail != null)
            rustNail.SetActive(true);

        Debug.Log("Nail started rusting - Day " + currentDay);
    }

    /// <summary>
    /// Called when 30-day cycle is complete
    /// </summary>
    private void CycleComplete()
    {
        exampleManager.DayCompleted();
        cycleRunning = false;

        // Ensure final state
        if (normalNail != null)
            normalNail.SetActive(false);

        if (rustNail != null)
            rustNail.SetActive(true);

        // Unlock inspection on rusty nail
        if (rustInspection != null)
        {
            rustInspection.UnlockInspection();
        }

        // Notify NailMove that cycle is complete
        if (nailMove != null)
        {
            nailMove.OnRustCycleComplete();
        }

        // Final weather state
        if (weatherManager != null)
        {
            weatherManager.StopDayNight();
            weatherManager.SetSunny();
        }

        Debug.Log("30-Day rust cycle complete!");
    }

    /// <summary>
    /// Get current day for external systems
    /// </summary>
    public int GetCurrentDay()
    {
        return currentDay;
    }

    /// <summary>
    /// Check if cycle is running
    /// </summary>
    public bool IsCycleRunning()
    {
        return cycleRunning;
    }

    /// <summary>
    /// Get total elapsed time in cycle
    /// </summary>
    public float GetCycleProgress()
    {
        return (float)currentDay / totalDays;
    }
}
