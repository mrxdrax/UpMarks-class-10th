using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Transform zoomPoint;
    public float speed = 2f;

    private bool move = false;

    private void Update()
    {
        if (!move)
            return;

        transform.position = Vector3.Lerp(
            transform.position,
            zoomPoint.position,
            Time.deltaTime * speed
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            zoomPoint.rotation,
            Time.deltaTime * speed
        );

        if (Vector3.Distance(transform.position, zoomPoint.position) < 0.01f)
        {
            transform.position = zoomPoint.position;
            transform.rotation = zoomPoint.rotation;

            move = false;
        }
    }

    public void Zoom()
    {
        move = true;
    }
}
