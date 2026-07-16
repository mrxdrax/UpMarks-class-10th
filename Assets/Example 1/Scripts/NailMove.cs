using UnityEngine;
using System.Collections;

public class NailMove : MonoBehaviour
{
    [Header("============ DRAG SETTINGS ============")]
    [SerializeField] private float dragSpeed = 5f;
    [SerializeField] private float dragSmoothness = 0.1f;
    [SerializeField] private float hoverScaleAmount = 1.15f;
    [SerializeField] private float returnSpeed = 4f;

    [Header("============ HOVER EFFECTS ============")]
    [SerializeField] private float glowIntensity = 1.5f;
    [SerializeField] private float outlineWidth = 3f;
    [SerializeField] private Material glowMaterial;
    [SerializeField] private Outline outlineComponent;

    [Header("============ DROP AREA ============")]
    [SerializeField] private Collider dropAreaCollider;
    [SerializeField] private Material dropAreaHighlight;
    [SerializeField] private Renderer dropAreaRenderer;

    [Header("============ SNAP ANIMATION ============")]
    [SerializeField] private Transform pointA; // Insertion point on board
    [SerializeField] private Transform pointB; // Deep insertion point
    [SerializeField] private float snapSpeed = 3f;
    [SerializeField] private AnimationCurve snapCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float liftHeight = 0.3f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("============ INSERTION ANIMATION ============")]
    [SerializeField] private float insertSpeed = 2f;
    [SerializeField] private float insertDepth = 0.5f;
    [SerializeField] private AnimationCurve insertCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float shakeAmount = 0.02f;
    [SerializeField] private float shakeDuration = 0.3f;

    [Header("============ EFFECTS ============")]
    [SerializeField] private ParticleSystem dustParticles;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private AudioClip woodSound;
    [SerializeField] private float soundVolume = 0.7f;

    [Header("============ RUST SETTINGS ============")]
    [SerializeField] private GameObject normalNail;
    [SerializeField] private GameObject rustNail;
    [SerializeField] private Material rustMaterial;
    [SerializeField] private float rustBlendSpeed = 0.5f;

    [Header("============ DAY CYCLE ============")]
    [SerializeField] private float dayDuration = 0.1f;
    [SerializeField] private float weatherDelay = 0f;

    [Header("============ REFERENCES ============")]
    [SerializeField] private DayManager dayManager;
    [SerializeField] private WeatherManager weatherManager;
    [SerializeField] private CameraZoom cameraZoom;
    [SerializeField] private RustInspection rustInspection;

    // Private state
    private bool isDragging = false;
    private bool hasSnapped = false;
    private bool isCycleRunning = false;
    
    private Vector3 dragStartPosition;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Vector3 lastValidPosition;
    
    private Material originalMaterial;
    private Material dropAreaOriginalMaterial;
    
    private Vector3 dragVelocity = Vector3.zero;
    private Renderer nailRenderer;
    private AudioSource audioSource;

    // Drop area highlight
    private bool isDropAreaHighlighted = false;
    private Material currentDropAreaMaterial;

    private void Start()
    {
        // Cache components
        nailRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        originalPosition = transform.position;
        originalScale = transform.localScale;
        lastValidPosition = originalPosition;
        
        if (nailRenderer != null)
        {
            originalMaterial = nailRenderer.material;
        }

        if (dropAreaRenderer != null)
        {
            dropAreaOriginalMaterial = dropAreaRenderer.material;
            currentDropAreaMaterial = dropAreaOriginalMaterial;
        }

        // Setup outline if not assigned
        if (outlineComponent == null)
        {
            outlineComponent = GetComponent<Outline>();
        }

        // Disable outline initially
        if (outlineComponent != null)
        {
            outlineComponent.enabled = false;
            outlineComponent.OutlineWidth = outlineWidth;
        }

        // Setup glow material
        if (glowMaterial == null && nailRenderer != null)
        {
            glowMaterial = nailRenderer.material;
        }
    }

    private void OnMouseEnter()
    {
        cameraZoom.Zoom();
        if (hasSnapped)
            return;

        // Smooth scale up
        StartCoroutine(ScaleToTarget(originalScale * hoverScaleAmount, 0.2f));

        // Enable outline
        if (outlineComponent != null)
        {
            outlineComponent.enabled = true;
        }

        // Glow effect
        if (nailRenderer != null && glowMaterial != null)
        {
            nailRenderer.material = glowMaterial;
        }

        // Change cursor





        //Cursor.SetCursor(null, Vector2.zero);
    }

    private void OnMouseExit()
    {
        if (hasSnapped || isDragging)
            return;

        // Smooth scale down
        StartCoroutine(ScaleToTarget(originalScale, 0.2f));

        // Disable outline
        if (outlineComponent != null)
        {
            outlineComponent.enabled = false;
        }

        // Reset material
        if (nailRenderer != null && originalMaterial != null)
        {
            nailRenderer.material = originalMaterial;
        }

        // Reset cursor






        //Cursor.SetCursor(null, Vector2.zero);


        
    }

    private void OnMouseDown()
    {
        if (hasSnapped || isCycleRunning)
            return;

        isDragging = true;
        dragStartPosition = Input.mousePosition;
        
        // Lift slightly
        StartCoroutine(LiftNail(liftHeight, 0.15f));
    }

    private void Update()
    {
        if (!isDragging)
            return;

        HandleDragging();
    }

    private void HandleDragging()
    {
        // Get mouse world position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane dragPlane = new Plane(Vector3.forward, originalPosition);
        
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 targetPosition = ray.origin + ray.direction * enter;
            
            // Lock Z axis - CRITICAL REQUIREMENT
            targetPosition.z = originalPosition.z;
            
            // Smooth damp for premium feel
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref dragVelocity,
                dragSmoothness
            );

            lastValidPosition = transform.position;
        }

        // Check if over drop area
        CheckDropAreaHighlight();

        // Release on mouse up
        if (Input.GetMouseButtonUp(0))
        {
            ReleaseDrag();
        }
    }

    private void CheckDropAreaHighlight()
    {
        if (dropAreaCollider == null)
            return;

        // Simple bounds check
        Vector3 closestPoint = dropAreaCollider.ClosestPoint(transform.position);
        float distanceToArea = Vector3.Distance(closestPoint, transform.position);
        bool isOverArea = distanceToArea < 0.1f;

        if (isOverArea && !isDropAreaHighlighted)
        {
            HighlightDropArea(true);
        }
        else if (!isOverArea && isDropAreaHighlighted)
        {
            HighlightDropArea(false);
        }
    }

    private void HighlightDropArea(bool highlight)
    {
        isDropAreaHighlighted = highlight;

        if (dropAreaRenderer == null)
            return;

        if (highlight && dropAreaHighlight != null)
        {
            dropAreaRenderer.material = dropAreaHighlight;
        }
        else if (!highlight && dropAreaOriginalMaterial != null)
        {
            dropAreaRenderer.material = dropAreaOriginalMaterial;
        }
    }

    private void ReleaseDrag()
    {
        isDragging = false;

        if (dropAreaCollider == null)
        {
            // No valid drop area - return to original
            ReturnToOriginal();
            return;
        }

        Vector3 closestPoint = dropAreaCollider.ClosestPoint(transform.position);
        float distanceToArea = Vector3.Distance(closestPoint, transform.position);

        if (distanceToArea < 0.15f)
        {
            // Valid placement
            HighlightDropArea(false);
            StartCoroutine(SnapAndInsert());
        }
        else
        {
            // Invalid placement - return
            HighlightDropArea(false);
            ReturnToOriginal();
        }
    }

    private void ReturnToOriginal()
    {
        StartCoroutine(SmoothReturn(originalPosition, originalScale, returnSpeed, 0.3f));
    }

    private IEnumerator SnapAndInsert()
    {
        hasSnapped = true;
        dragVelocity = Vector3.zero;

        if (outlineComponent != null)
            outlineComponent.enabled = false;

        // SNAP SEQUENCE: Lift → Move → Rotate → Align
        yield return StartCoroutine(LiftNail(liftHeight, 0.2f));
        yield return StartCoroutine(MoveToPoint(pointA, snapSpeed, 0.3f));
        yield return StartCoroutine(RotateToInsertion(0.3f));
        
        // INSERTION ANIMATION: Rotate + Move Down
        yield return StartCoroutine(InsertNail());

        // Impact effects
        PlayImpactEffects();
        
        // Start 30-day cycle after insertion
        if (dayManager != null)
        {
            dayManager.SetDayDuration(dayDuration);
            if (weatherDelay > 0)
                yield return new WaitForSeconds(weatherDelay);
            dayManager.StartCycle();
        }

        isCycleRunning = true;
    }

    private IEnumerator LiftNail(float height, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * height;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, EaseInOutCubic(t));
            yield return null;
        }

        transform.position = targetPos;
    }

    private IEnumerator MoveToPoint(Transform targetPoint, float speed, float duration)
    {
        if (targetPoint == null)
            yield break;

        Vector3 startPos = transform.position;
        Vector3 targetPos = targetPoint.position;

    Quaternion startRot = transform.rotation;
        Quaternion targetRot =
Quaternion.Euler(
-90f,
transform.eulerAngles.y,
transform.eulerAngles.z);

float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = snapCurve.Evaluate(elapsed / duration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.position = targetPos;
    }

    private IEnumerator RotateToInsertion(float duration)
{
    yield break;
}

    private IEnumerator InsertNail()
    {
        if (pointB == null)
            yield break;

        Vector3 startPos = transform.position;
        Vector3 targetPos = pointB.position;
        Quaternion startRot = transform.rotation;
        
        float elapsed = 0f;
        float duration = 1f / insertSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = insertCurve.Evaluate(elapsed / duration);

            // Move down with curve
            transform.position = Vector3.Lerp(startPos, targetPos, t);

            // Continuous rotation (invisible drill effect)
            transform.Rotate(Vector3.back, rotationSpeed * 360f * Time.deltaTime, Space.Self);

            yield return null;
        }

        transform.position = targetPos;
    }

    private void PlayImpactEffects()
    {
        // Impact sound
        if (impactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(impactSound, soundVolume);
        }

        // Wood hit sound
        if (woodSound != null && audioSource != null)
        {
            StartCoroutine(DelayedAudioPlay(woodSound, 0.1f));
        }

        // Dust particles
        if (dustParticles != null)
        {
            dustParticles.Play();
        }

        // Vibration
        StartCoroutine(ShakeNail());
    }

    private IEnumerator ShakeNail()
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            
            Vector3 randomShake = Random.insideUnitSphere * shakeAmount;
            transform.position = originalPos + randomShake;
            
            yield return null;
        }

        transform.position = originalPos;
    }

    private IEnumerator DelayedAudioPlay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, soundVolume);
        }
    }

    private IEnumerator SmoothReturn(Vector3 targetPos, Vector3 targetScale, float speed, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            transform.position = Vector3.Lerp(startPos, targetPos, EaseInOutCubic(t));
            transform.localScale = Vector3.Lerp(startScale, targetScale, EaseInOutCubic(t));
            
            yield return null;
        }

        transform.position = targetPos;
        transform.localScale = targetScale;

        // Reset materials
        if (nailRenderer != null && originalMaterial != null)
        {
            nailRenderer.material = originalMaterial;
        }
    }

    private IEnumerator ScaleToTarget(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = EaseInOutCubic(elapsed / duration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    // Easing function for premium feel
    private float EaseInOutCubic(float t)
    {
        return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }

    // PUBLIC METHODS for other systems

    public bool HasSnapped()
    {
        return hasSnapped;
    }

    public bool IsCycleRunning()
    {
        return isCycleRunning;
    }

    public void OnRustCycleComplete()
    {
        // Called from DayManager after day 30
        // Transition from normal to rust nail
        if (normalNail != null && rustNail != null)
        {
            normalNail.SetActive(false);
            rustNail.SetActive(true);
        }

        // Unlock inspection
        if (rustInspection != null)
        {
            rustInspection.UnlockInspection();
        }
    }
}
