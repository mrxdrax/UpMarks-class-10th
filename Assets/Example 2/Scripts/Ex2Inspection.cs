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

    [Header("============ MANAGER REFERENCES ============")]
    [SerializeField] private ExampleManager exampleManager;

    // State
    private bool canInspect = false;
    private bool inspecting = false;
    private Vector3 startPos;
    private Vector3 startScale;

    private void Start()
    {
        startPos = transform.position;
        startScale = transform.localScale;
    }

    /// <summary>
    /// Called by CookingManager when cooking is complete
    /// </summary>
    public void UnlockInspection()
    {
        canInspect = true;
        Debug.Log("Meat inspection unlocked");
    }

    private void OnMouseDown()
    {
        if (!canInspect || inspecting)
            return;

        Debug.Log("Starting meat inspection");
        StartCoroutine(StartInspection());
    }

    /// <summary>
    /// Enter inspection mode
    /// </summary>
    private IEnumerator StartInspection()
    {
        inspecting = true;

        // Hide environment
        if (environment != null)
        {
            environment.SetActive(false);
        }

        // Move to inspection point and scale up
        Vector3 targetScale = startScale * scaleMultiplier;
        float elapsed = 0f;
        float duration = 1f / moveSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easeT = EaseInOutCubic(t);

            transform.position = Vector3.Lerp(startPos, inspectionPoint.position, easeT);
            transform.localScale = Vector3.Lerp(startScale, targetScale, easeT);

            yield return null;
        }

        transform.position = inspectionPoint.position;
        transform.localScale = targetScale;

        // Notify ExampleManager immediately (don't wait for exit)
        if (exampleManager != null)
        {
            exampleManager.InspectionCompleted();
            Debug.Log("Notified ExampleManager: InspectionCompleted");
        }

        Debug.Log("Inspection mode active");
    }

    private void Update()
    {
        if (!inspecting)
            return;

        HandleRotation();

        // ESC to exit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitInspection();
        }
    }

    /// <summary>
    /// Handle auto-rotation and mouse drag rotation
    /// </summary>
    private void HandleRotation()
    {
        // Auto rotation around Y axis
        transform.Rotate(
            Vector3.up,
            autoRotateSpeed * Time.deltaTime,
            Space.World
        );

        // Mouse drag rotation
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Horizontal drag
            transform.Rotate(
                Vector3.up,
                -mouseX * dragRotateSpeed * 100f,
                Space.World
            );

            // Vertical drag
            transform.Rotate(
                Vector3.right,
                mouseY * dragRotateSpeed * 100f,
                Space.World
            );
        }
    }

    /// <summary>
    /// Exit inspection mode
    /// </summary>
    public void ExitInspection()
    {
        if (!inspecting)
            return;

        StartCoroutine(EndInspection());
    }

    /// <summary>
    /// Return to normal view
    /// </summary>
    private IEnumerator EndInspection()
    {
        inspecting = false;

        // Return to original position and scale
        float elapsed = 0f;
        float duration = 1f / moveSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easeT = EaseInOutCubic(t);

            transform.position = Vector3.Lerp(inspectionPoint.position, startPos, easeT);
            transform.localScale = Vector3.Lerp(transform.localScale, startScale, easeT);

            yield return null;
        }

        transform.position = startPos;
        transform.localScale = startScale;

        // Show environment
        if (environment != null)
        {
            environment.SetActive(true);
        }

        Debug.Log("Exited inspection mode");
    }

    /// <summary>
    /// Cubic easing for premium feel
    /// </summary>
    private float EaseInOutCubic(float t)
    {
        return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }

    /// <summary>
    /// Get inspection state
    /// </summary>
    public bool IsInspecting()
    {
        return inspecting;
    }

    public bool CanInspect()
    {
        return canInspect;
    }
}
