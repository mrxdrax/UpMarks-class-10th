using TMPro;
using UnityEngine;
using System.Collections;

public class DayManager : MonoBehaviour
{
        public TMP_Text dayText;

    public int currentDay = 1;

    public WeatherManager weatherManager;

    public GameObject normalNail;
    public GameObject rustNail;
    //public NailInspection nailInspection;
    public RustInspection rustInspection;

  public void StartCycle()
{
    weatherManager.StartDayNight();
    UpdateWeather(); // Day 1 pe turant Sunny

    StartCoroutine(DayCycle());
}

    IEnumerator DayCycle()
    {
        while(currentDay < 30)
        {
            yield return new WaitForSeconds(1f);

            currentDay++;

dayText.text = "Day : " + currentDay;


UpdateWeather();
        }

        normalNail.SetActive(false);
        rustNail.SetActive(true);

       // nailInspection.canInspect = true;
    }

void UpdateWeather()
{
    if(currentDay == 1)
    {
        weatherManager.SetSunny();
    }
    else if(currentDay == 6)
    {
        weatherManager.SetLeafy();
    }
    else if(currentDay == 11)
    {
        weatherManager.SetRainy();
    }
    else if(currentDay == 16)
    {
        weatherManager.SetSunny();
    }
    else if(currentDay == 21)
    {
        weatherManager.SetLeafy();
    }
    else if(currentDay == 26)
    {
        weatherManager.SetRainy();
    }
    else if(currentDay == 30)
    {
        weatherManager.StopDayNight();
        weatherManager.SetSunny();
    }
    if(currentDay == 20)
    {
        normalNail.SetActive(false);

        rustNail.SetActive(true);

        rustInspection.UnlockInspection();
    }
}

}