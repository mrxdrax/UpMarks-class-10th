using UnityEngine;
using System.Collections;

public class RustInspection : MonoBehaviour
{
    public Transform inspectionPoint;
    public GameObject environment;

    public float autoRotateSpeed = 20f;
    public float dragRotateSpeed = 0.3f;
    public float moveSpeed = 2f;

    bool canInspect = false;
    bool inspecting = false;

    Vector3 startPos;
    Vector3 startScale;


    void Start()
    {
        startPos = transform.position;
        startScale = transform.localScale;
    }

    public void UnlockInspection()
    {
        canInspect = true;
    }

    void OnMouseDown()
{
    print("Clicked on Rust Nail");

    if(!canInspect)
    {
        print("Inspection Not Unlocked");
        return;
    }

    if(!inspecting)
    {
        print("Inspection Started");
        StartCoroutine(StartInspection());
    }
}

    IEnumerator StartInspection()
    {
        print("ienumrtor ke andar h");
        inspecting = true;

        environment.SetActive(false);

        Vector3 targetScale = startScale * 3f;

        while(Vector3.Distance(transform.position,
                               inspectionPoint.position) > 0.01f)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                inspectionPoint.position,
                Time.deltaTime * moveSpeed
            );

            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.deltaTime * moveSpeed
            );

            yield return null;
        }

        transform.position = inspectionPoint.position;
    }

    void Update()
    {
        if(!inspecting)
            return;

        // Auto Rotation
        transform.Rotate(
            Vector3.up,
            autoRotateSpeed * Time.deltaTime,
            Space.World
        );

        // Mouse Drag
        if(Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            transform.Rotate(
                Vector3.up,
                -mouseX * dragRotateSpeed * 100f,
                Space.World
            );

            transform.Rotate(
                Vector3.right,
                mouseY * dragRotateSpeed * 100f,
                Space.World
            );
        }
    }
    
}
