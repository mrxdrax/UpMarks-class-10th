using UnityEngine;
using System.Collections;

public class Ex2CookingManager : MonoBehaviour
{
    [Header("Meat Objects")]
public GameObject rawMeat;
public GameObject cookedMeat;

[Header("Inspection")]
public Ex2Inspection inspection;
    public GameObject gasFlame;
    public GameObject smokeFX;
    public Ex2MeatFlip meatFlip;

    public float cookingTime = 15f;

    public bool meatPlaced = false;

    bool gasStarted = false;

    public bool cookingFinished = false;

    public void MeatPlaced()
    {
        meatPlaced = true;

        Debug.Log("Meat Placed");
    }

    public void StartCooking()
    {
        if(!meatPlaced)
        {
            Debug.Log("Place Meat First");
            return;
        }

        if(gasStarted)
            return;

        StartCoroutine(CookingRoutine());
    }

IEnumerator CookingRoutine()
{
    gasStarted = true;

    // Fire ON
    gasFlame.SetActive(true);

    Debug.Log("Gas ON");

    // 2 sec baad smoke
    yield return new WaitForSeconds(2f);

    smokeFX.SetActive(true);

    // Pehli side cook
    yield return new WaitForSeconds((cookingTime / 2) - 2f);

    Debug.Log("Flip Available");

    meatFlip.EnableFlip();
}
    public void StartSecondCooking()
{
    StartCoroutine(SecondCookingRoutine());
}

   

IEnumerator SecondCookingRoutine()
{
    Debug.Log("Second Side");

    yield return new WaitForSeconds(cookingTime / 2);

    gasFlame.SetActive(false);
    smokeFX.SetActive(false);

    cookingFinished = true;

    rawMeat.SetActive(false);
    cookedMeat.SetActive(true);

    inspection.UnlockInspection();

    Debug.Log("Cooking Finished");
}
}