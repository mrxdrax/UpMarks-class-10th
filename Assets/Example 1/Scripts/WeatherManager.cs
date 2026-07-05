using UnityEngine;
using System.Collections;

public class WeatherManager : MonoBehaviour
{
    [Header("Weather FX")]
    public GameObject rainFX;
    public GameObject leafFX;

    [Header("Lighting")]
    public Light directionalLight;

    [Header("Daily Sun Rotation")]
    public Vector3 rotationPerDay = new Vector3(12f, 0f, 0f);

    [Header("Transition Time")]
    public float transitionTime = 1f;
[Header("Day Night Cycle")]
public bool enableDayNight = true;

public float sunRotationSpeed = 8f;

bool dayNightRunning = false;
    // ---------------- WEATHER ----------------

public void SetSunny()
{
    rainFX.SetActive(false);
    leafFX.SetActive(false);
}

public void SetRainy()
{
    rainFX.SetActive(true);
    leafFX.SetActive(false);
}

public void SetLeafy()
{
    rainFX.SetActive(false);
    leafFX.SetActive(true);
}
    
void Update()
{
    if (!dayNightRunning)
        return;

    directionalLight.transform.Rotate(
        sunRotationSpeed * Time.deltaTime,
        0f,
        0f,
        Space.Self
    );
}
public void StartDayNight()
{
    dayNightRunning = true;
}

public void StopDayNight()
{
    dayNightRunning = false;
}

    // ---------------- ROTATION ----------------

    IEnumerator RotateLight(Vector3 targetRotation)
    {
        Quaternion startRotation = directionalLight.transform.rotation;

        Quaternion endRotation = Quaternion.Euler(targetRotation);

        float timer = 0f;

        while (timer < transitionTime)
        {
            timer += Time.deltaTime;

            directionalLight.transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    endRotation,
                    timer / transitionTime
                );

            yield return null;
        }

        directionalLight.transform.rotation = endRotation;
    }
}