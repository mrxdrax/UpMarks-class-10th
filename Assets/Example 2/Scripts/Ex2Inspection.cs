using UnityEngine;
using System.Collections;

public class Ex2Inspection : MonoBehaviour
{
     [Header("============ INSPECTION SETUP ============")]
    [SerializeField] private Transform inspectionPoint;
    [SerializeField] private GameObject environment;

    [Header("============ CAMERA SETTINGS ============")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float autoRotateSpeed = 20f;
    [SerializeField] private float dragRotateSpeed = 0.3f;

    [Header("============ SCALING ============")]
    [SerializeField] private float scaleMultiplier = 3f;

    [Header("============ INSPECTION ROTATION ============")]
    [SerializeField] private Vector3 inspectionRotation = new Vector3(-180f,0f,0f);

    [Header("============ MANAGER REFERENCES ============")]
    [SerializeField] private ExampleManager exampleManager;

    bool canInspect, inspecting;
    Vector3 startPos,startScale;
    Quaternion startRotation;

    void Start(){
        startPos=transform.position;
        startScale=transform.localScale;
        startRotation=transform.rotation;
    }

    public void UnlockInspection(){ canInspect=true; }

    void OnMouseDown()
{
    Debug.Log("Mouse Down");

    Debug.Log("canInspect = " + canInspect);
    Debug.Log("inspecting = " + inspecting);

    if (!canInspect)
    {
        Debug.Log("Inspection Locked");
        return;
    }

    if (inspecting)
    {
        Debug.Log("Already Inspecting");
        return;
    }

    Debug.Log("Starting Coroutine");

    startPos = transform.position;
    startScale = transform.localScale;
    startRotation = transform.rotation;

    StartCoroutine(StartInspection());

    Debug.Log("Coroutine Started");
}

    IEnumerator StartInspection()
{
    Debug.Log("1. Coroutine Started");

    inspecting = true;

    Debug.Log("2. Hiding Environment");

    if (environment != null)
        environment.SetActive(false);

    Debug.Log("3. Environment Hidden");

    if (inspectionPoint == null)
    {
        Debug.LogError("Inspection Point is NULL");
        yield break;
    }

    Debug.Log("4. Inspection Point OK");

    Vector3 targetScale = startScale * scaleMultiplier;
    Quaternion targetRot = Quaternion.Euler(inspectionRotation);

    float e = 0f;
    float d = 1f / moveSpeed;

    while (e < d)
    {
        e += Time.deltaTime;

        float t = Mathf.Clamp01(e / d);
        t = t < 0.5f
            ? 4f * t * t * t
            : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;

        transform.position = Vector3.Lerp(startPos, inspectionPoint.position, t);
        transform.localScale = Vector3.Lerp(startScale, targetScale, t);
        transform.rotation = Quaternion.Slerp(startRotation, targetRot, t);

        yield return null;
    }

    Debug.Log("5. Animation Finished");

    transform.SetPositionAndRotation(inspectionPoint.position, targetRot);
    transform.localScale = targetScale;

    Debug.Log("6. Final Position Set");

    if (exampleManager != null)
    {
        exampleManager.InspectionCompleted();
        Debug.Log("7. Example Manager Called");
    }
    else
    {
        Debug.LogWarning("ExampleManager NULL");
    }

    Debug.Log("8. Inspection Complete");
}

    void Update(){
        if(!inspecting) return;

        transform.Rotate(Vector3.up,autoRotateSpeed*Time.deltaTime,Space.World);

        if(Input.GetMouseButton(0)){
            float mx=Input.GetAxis("Mouse X");
            float my=Input.GetAxis("Mouse Y");
            transform.Rotate(Vector3.up,-mx*dragRotateSpeed*100f,Space.World);
            transform.Rotate(Vector3.right,my*dragRotateSpeed*100f,Space.Self);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
            StartCoroutine(EndInspection());
    }

    IEnumerator EndInspection(){
        inspecting=false;
        Vector3 currentScale=transform.localScale;
        Quaternion currentRot=transform.rotation;

        float e=0,d=1f/moveSpeed;
        while(e<d){
            e+=Time.deltaTime;
            float t=Mathf.Clamp01(e/d);
            t=t<0.5f?4f*t*t*t:1f-Mathf.Pow(-2f*t+2f,3f)/2f;

            transform.position=Vector3.Lerp(inspectionPoint.position,startPos,t);
            transform.localScale=Vector3.Lerp(currentScale,startScale,t);
            transform.rotation=Quaternion.Slerp(currentRot,startRotation,t);
            yield return null;
        }

        transform.position=startPos;
        transform.localScale=startScale;
        transform.rotation=startRotation;

        if(environment) environment.SetActive(true);
    }

    public bool IsInspecting()=>inspecting;
    public bool CanInspect()=>canInspect;
}
