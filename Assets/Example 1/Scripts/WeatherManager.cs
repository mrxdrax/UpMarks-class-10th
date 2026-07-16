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

    private bool dayNightRunning = false;

    // ============ WEATHER ============

    public void SetSunny()
    {
        if (rainFX != null) rainFX.SetActive(false);
        if (leafFX != null) leafFX.SetActive(false);
    }

    public void SetRainy()
    {
        if (rainFX != null) rainFX.SetActive(true);
        if (leafFX != null) leafFX.SetActive(false);
    }

    public void SetLeafy()
    {
        if (rainFX != null) rainFX.SetActive(false);
        if (leafFX != null) leafFX.SetActive(true);
    }

    // ============ DAY NIGHT CYCLE ============

    private void Update()
    {
        if (!dayNightRunning || directionalLight == null)
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

    // ============ ROTATION ============

    public IEnumerator RotateLight(Vector3 targetRotation)
    {
        if (directionalLight == null)
            yield break;

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
