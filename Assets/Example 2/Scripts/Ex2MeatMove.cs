using UnityEngine;

public class Ex2MeatMove : MonoBehaviour
{
    [Header("Points")]
    public Transform panPoint;

    [Header("Managers")]
    public Ex2CookingManager cookingManager;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotateSpeed = 4f;

    private bool moving = false;
    private bool reachedPan = false;

    void OnMouseDown()
    {
        
        if (reachedPan)
            return;

        moving = true;
    }

    void Update()
    {
        if (!moving)
            return;

        transform.position = Vector3.Lerp(
            transform.position,
            panPoint.position,
            Time.deltaTime * moveSpeed
        );
        if (Vector3.Distance(transform.position, panPoint.position) < 0.01f)
        {
            transform.position = panPoint.position;

            moving = false;
            reachedPan = true;

            Debug.Log("Raw Meat Reached Pan");

            cookingManager.StartCooking();
        }
        cookingManager.MeatPlaced();
    }
}