using UnityEngine;
using System.Collections;

public class Ex2MeatFlip : MonoBehaviour
{
    [Header("Cooking Manager")]
    public Ex2CookingManager cookingManager;

    [Header("Flip Settings")]
    public float flipTime = 0.6f;

    bool canFlip = false;
    bool flipped = false;
    bool flipping = false;

    public void EnableFlip()
    {
        canFlip = true;

        Debug.Log("Flip Available");
    }
void OnMouseDown()
{
    Debug.Log("Mouse Clicked");

    Debug.Log("canFlip = " + canFlip);

    Debug.Log("flipped = " + flipped);

    Debug.Log("flipping = " + flipping);

    if (!canFlip)
        return;

    if (flipped)
        return;

    if (flipping)
        return;

    Debug.Log("Starting Flip");

    StartCoroutine(FlipRoutine());
}

    IEnumerator FlipRoutine()
{
    flipping = true;

    Vector3 startPos = transform.position;

    Quaternion startRot = transform.rotation;

    Quaternion endRot =
        startRot * Quaternion.Euler(180f, 0f, 0f);

    float timer = 0;

    while (timer < flipTime)
    {
        timer += Time.deltaTime;

        float t = timer / flipTime;

        // Rotation
        transform.rotation =
            Quaternion.Slerp(startRot, endRot, t);

        // Toss
        Vector3 pos = startPos;

        pos.y += Mathf.Sin(t * Mathf.PI) * 0.08f;

        transform.position = pos;

        yield return null;
    }

    transform.position = startPos;

    transform.rotation = endRot;

    flipped = true;

    flipping = false;

    canFlip = false;

    cookingManager.StartSecondCooking();
}
}