using UnityEngine;
using System.Collections;

public class Ex2Inspection : MonoBehaviour
{
    public Transform inspectionPoint;

    public GameObject environment;

    public float moveSpeed = 3f;

    bool unlocked = false;
    bool inspecting = false;
    [Header("UI")]
public GameObject nextButton;

    public void UnlockInspection()
    {
        unlocked = true;
    }

    void OnMouseDown()
    {
        if(!unlocked)
            return;

        if(inspecting)
            return;

        StartCoroutine(StartInspection());
        nextButton.SetActive(true);
    }

    IEnumerator StartInspection()
    {
        inspecting = true;

        environment.SetActive(false);

        while(Vector3.Distance(transform.position,
                               inspectionPoint.position) > 0.01f)
        {
            transform.position =
                Vector3.Lerp(
                    transform.position,
                    inspectionPoint.position,
                    Time.deltaTime * moveSpeed
                );

            yield return null;
        }

        transform.position =
            inspectionPoint.position;
    }
}