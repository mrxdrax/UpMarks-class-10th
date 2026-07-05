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

        Quaternion startRot = transform.rotation;

        Quaternion endRot =
            startRot * Quaternion.Euler(180f, 0f, 0f);

        float timer = 0;

        while (timer < flipTime)
        {
            timer += Time.deltaTime;

            transform.rotation =
                Quaternion.Slerp(
                    startRot,
                    endRot,
                    timer / flipTime
                );

            yield return null;
        }

        transform.rotation = endRot;

        flipped = true;
        flipping = false;
        canFlip = false;

        Debug.Log("Meat Flipped");

        cookingManager.StartSecondCooking();
    }
}