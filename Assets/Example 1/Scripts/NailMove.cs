using UnityEngine;

public class NailMove : MonoBehaviour
{

    public CameraZoom cameraZoom;

    [Header("Points")]
    public Transform pointA;
    public Transform pointB;
    public Transform pointC;
    public Transform pointD;

    [Header("Settings")]
    public GameObject dayManager;
    public float moveSpeed = 3f;
    public float rotateSpeed = 3f;

    private bool moving = false;
    private bool reachedPointA = false;

    private int hitCount = 0;
    public WeatherManager weatherManager;

    void OnMouseDown()
{
    if (!reachedPointA)
    {
        cameraZoom.Zoom();
        moving = true;
    }
}

    void Update()
    {
        // TEMPORARY TEST
        // Space = Hammer Hit
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HitNail();
        }

        if (moving)
        {
            // Move towards Point A
            transform.position = Vector3.Lerp(
                transform.position,
                pointA.position,
                Time.deltaTime * moveSpeed
            );

            // Rotate while moving
            Quaternion targetRotation =
                Quaternion.Euler(-90f, 0f, 0f);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * rotateSpeed
            );

            // Stop when reached Point A
            if (Vector3.Distance(transform.position, pointA.position) < 0.01f)
            {
                transform.position = pointA.position;
                transform.rotation = targetRotation;

                moving = false;
                reachedPointA = true;

                Debug.Log("Nail Ready For Hammer");
            }
        }
    }

    public bool IsReady()
    {
        return reachedPointA;
    }

    public void HitNail()
    {
        if (!reachedPointA)
            return;

        hitCount++;

        if (hitCount == 1)
        {
            transform.position = pointB.position;
            Debug.Log("Hit 1 → Point B");
        }
        else if (hitCount == 2)
        {
            transform.position = pointC.position;
            Debug.Log("Hit 2 → Point C");
        }
        else if (hitCount == 3)
        {
            transform.position = pointD.position;

            Debug.Log("Hit 3 → Point D");
            Debug.Log("Nail Fully Inserted");
           weatherManager.StartDayNight();
            dayManager.GetComponent<DayManager>().StartCycle();
        }
    }
}