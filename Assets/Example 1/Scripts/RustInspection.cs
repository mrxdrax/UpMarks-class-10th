using UnityEngine;
using System.Collections;

public class RustInspection : MonoBehaviour
{
    [Header("============ INSPECTION SETUP ============")]
    [SerializeField] private Transform inspectionPoint;
    [SerializeField] private GameObject environment;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private ExampleManager exampleManager;

    [Header("============ CAMERA SETTINGS ============")]
    [SerializeField] private Vector3 cameraInspectionOffset = new Vector3(0, 0.5f, -0.8f);
    [SerializeField] private float cameraSpeed = 3f;
    [SerializeField] private float cameraReturnSpeed = 2.5f;
    [SerializeField] private Quaternion cameraLookRotation = Quaternion.identity;

    [Header("============ NAIL MOVEMENT ============")]
    [SerializeField] private float nailMoveSpeed = 2f;
    [SerializeField] private float nailScaleMultiplier = 1.5f;

    [Header("============ ROTATION CONTROLS ============")]
    [SerializeField] private float autoRotateSpeed = 20f;
    [SerializeField] private float dragRotateSpeed = 0.3f;
    [SerializeField] private Vector2 rotationClamp = new Vector2(-60f, 60f);

    [Header("============ BACKGROUND BLUR ============")]
    [SerializeField] private CanvasGroup backgroundCanvasGroup;
    [SerializeField] private float blurFadeSpeed = 3f;

    [Header("============ UI BUTTONS ============")]
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject exitButton;

    // State tracking
    private bool canInspect = false;
    private bool inspecting = false;
    private bool mouseControlActive = true;

    // Position/scale storage
    private Vector3 startPos;
    private Vector3 startScale;
    private Vector3 startCameraPos;
    private Quaternion startCameraRot;

    // Rotation state
    private float currentVerticalRotation = 0f;
    private float targetVerticalRotation = 0f;

    // Renderer and material caching
    private Renderer nailRenderer;
    private Material originalMaterial;

    private void Start()
    {
        // Cache references
        nailRenderer = GetComponent<Renderer>();
        if (nailRenderer != null)
        {
            originalMaterial = nailRenderer.material;
        }

        // Get main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Store starting positions
        startPos = transform.position;
        startScale = transform.localScale;

        if (mainCamera != null)
        {
            startCameraPos = mainCamera.transform.position;
            startCameraRot = mainCamera.transform.rotation;
        }

        // Setup canvas group for blur if not assigned
        if (backgroundCanvasGroup == null && environment != null)
        {
            backgroundCanvasGroup = environment.GetComponent<CanvasGroup>();
        }

        // Initialize UI buttons
        if (nextButton != null)
            nextButton.SetActive(false);
        if (exitButton != null)
            exitButton.SetActive(false);

        // Validation
        if (inspectionPoint == null)
        {
            Debug.LogWarning("RustInspection: inspectionPoint not assigned!");
        }
    }

    /// <summary>
    /// Called by DayManager when inspection becomes available (day 20+)
    /// </summary>
    public void UnlockInspection()
    {
        canInspect = true;
        Debug.Log("Nail inspection unlocked!");
    }

    /// <summary>
    /// Called by UI button to exit inspection
    /// </summary>
    public void ExitInspection()
    {
        if (inspecting)
        {
            StartCoroutine(EndInspection());
            exampleManager.InspectionCompleted();
        }
    }

    private void OnMouseDown()
    {
        // Only start inspection if unlocked and not already inspecting
        if (!canInspect || inspecting)
            return;

        StartCoroutine(StartInspection());
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
    /// Smooth entry into inspection mode
    /// </summary>
    private IEnumerator StartInspection()
    {
        exampleManager.InspectionCompleted();

Debug.Log("Example Completed");
        inspecting = true;
        mouseControlActive = true;
        currentVerticalRotation = 0f;
        targetVerticalRotation = 0f;

        // Show UI buttons
        if (nextButton != null)
            nextButton.SetActive(true);
        if (exitButton != null)
            exitButton.SetActive(true);

        // Hide environment with fade
        StartCoroutine(FadeEnvironment(false, blurFadeSpeed));

        // Move nail to inspection point and scale up
        if (inspectionPoint != null)
        {
            yield return StartCoroutine(MoveNailToInspection());
        }

        // Move camera to inspection view
        if (mainCamera != null)
        {
            //yield return StartCoroutine(MoveCameraToInspection());
        }

        Debug.Log("Inspection mode active");
    }

    /// <summary>
    /// Move nail smoothly to inspection point and scale it up
    /// </summary>
    private IEnumerator MoveNailToInspection()
    {
        Vector3 targetScale = startScale * nailScaleMultiplier;
        float elapsed = 0f;
        float duration = 1f / nailMoveSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = EaseInOutCubic(elapsed / duration);

            transform.position = Vector3.Lerp(startPos, inspectionPoint.position, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        transform.position = inspectionPoint.position;
        transform.localScale = targetScale;
    }

    /// <summary>
    /// Move camera to inspection view
    /// </summary>
    private IEnumerator MoveCameraToInspection()
    {
        Vector3 targetPos = inspectionPoint.position + cameraInspectionOffset;
        Quaternion targetRot = Quaternion.LookRotation((inspectionPoint.position - targetPos).normalized);

        float elapsed = 0f;
        float duration = 1f / cameraSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = EaseInOutCubic(elapsed / duration);

            mainCamera.transform.position = Vector3.Lerp(startCameraPos, targetPos, t);
            mainCamera.transform.rotation = Quaternion.Lerp(startCameraRot, targetRot, t);

            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;
    }

    /// <summary>
    /// Handle nail rotation during inspection
    /// </summary>
    private void HandleRotation()
    {
        if (!mouseControlActive)
            return;

        // Auto rotation
        transform.Rotate(Vector3.up, autoRotateSpeed * Time.deltaTime, Space.World);

        // Mouse drag rotation (vertical only - clamped)
        if (Input.GetMouseButton(0))
        {
            float mouseY = Input.GetAxis("Mouse Y");
            targetVerticalRotation -= mouseY * dragRotateSpeed * 100f;
            targetVerticalRotation = Mathf.Clamp(targetVerticalRotation, rotationClamp.x, rotationClamp.y);
        }

        // Smooth apply vertical rotation
        currentVerticalRotation = Mathf.Lerp(currentVerticalRotation, targetVerticalRotation, Time.deltaTime * 5f);
        transform.RotateAround(transform.position, Vector3.right, currentVerticalRotation - targetVerticalRotation);
    }

    /// <summary>
    /// Smooth exit from inspection mode
    /// </summary>
    private IEnumerator EndInspection()
    {
        inspecting = false;
        mouseControlActive = false;

        // Hide UI buttons
        if (nextButton != null)
            nextButton.SetActive(false);
        if (exitButton != null)
            exitButton.SetActive(false);

        // Fade in environment
        StartCoroutine(FadeEnvironment(true, blurFadeSpeed));

        // Return nail to original position and scale
        float elapsed = 0f;
        float duration = 1f / nailMoveSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = EaseInOutCubic(elapsed / duration);

            transform.position = Vector3.Lerp(inspectionPoint.position, startPos, t);
            transform.localScale = Vector3.Lerp(transform.localScale, startScale, t);

            yield return null;
        }

        transform.position = startPos;
        transform.localScale = startScale;

        Debug.Log("Exited inspection mode");
    }

    /// <summary>
    /// Fade environment in/out with alpha
    /// </summary>
    private IEnumerator FadeEnvironment(bool fadeIn, float speed)
    {
        if (backgroundCanvasGroup == null)
            yield break;

        float targetAlpha = fadeIn ? 1f : 0f;
        float startAlpha = backgroundCanvasGroup.alpha;
        float elapsed = 0f;
        float duration = 1f / speed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            backgroundCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        backgroundCanvasGroup.alpha = targetAlpha;
    }

    /// <summary>
    /// Cubic easing for premium feel
    /// </summary>
    private float EaseInOutCubic(float t)
    {
        return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }

    /// <summary>
    /// Query inspection state
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
