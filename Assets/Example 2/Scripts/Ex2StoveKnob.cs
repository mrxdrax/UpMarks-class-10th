using UnityEngine;

public class Ex2StoveKnob : MonoBehaviour
{
    public Ex2CookingManager cookingManager;

    void OnMouseDown()
    {
        Debug.Log("Stove on");
        cookingManager.StartCooking();
    }
}