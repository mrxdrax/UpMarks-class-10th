using UnityEngine;

public class UIFloat : MonoBehaviour
{
    [Header("Float")]
    public bool floatX = false;
    public bool floatY = true;

    public float amplitudeX = 10f;
    public float amplitudeY = 10f;

    public float speed = 1f;

    [Header("Rotation")]
    public bool rotate = false;
    public float rotationAmount = 2f;

    [Header("Random")]
    public bool randomOffset = true;

    Vector3 startPos;
    Quaternion startRot;
    float offset;

    void Awake()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;

        offset = randomOffset
            ? Random.Range(0f, 100f)
            : 0f;
    }

    void Update()
    {
        float t = (Time.unscaledTime + offset) * speed;

        float x = floatX ? Mathf.Sin(t) * amplitudeX : 0f;
        float y = floatY ? Mathf.Cos(t) * amplitudeY : 0f;

        transform.localPosition = startPos + new Vector3(x, y, 0);

        if (rotate)
        {
            transform.localRotation =
                startRot *
                Quaternion.Euler(
                    0,
                    0,
                    Mathf.Sin(t) * rotationAmount
                );
        }
    }
}