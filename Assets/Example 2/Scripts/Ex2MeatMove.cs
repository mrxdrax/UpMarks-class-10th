using UnityEngine;
using System.Collections;

public class Ex2MeatMove : MonoBehaviour
{
    [Header("============ THROW SETTINGS ============")]
    [SerializeField] private Transform panPoint;
    [SerializeField] private float throwDuration = 0.8f;
    [SerializeField] private float arcHeight = 0.5f;
    [SerializeField] private float rotationAmount = 720f; // degrees during flight

    [Header("============ LANDING EFFECTS ============")]
    [SerializeField] private float bounceHeight = 0.1f;
    [SerializeField] private float bounceDuration = 0.3f;
    [SerializeField] private float squashAmount = 0.15f; // how much to squash on landing
    [SerializeField] private float squashDuration = 0.2f;

    [Header("============ ANIMATION CURVES ============")]
    [SerializeField] private AnimationCurve throwCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve bounceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("============ EFFECTS ============")]
    [SerializeField] private ParticleSystem landingParticles;
    [SerializeField] private AudioClip throwSound;
    [SerializeField] private AudioClip landSound;

    [Header("============ MANAGER REFERENCES ============")]
    [SerializeField] private Ex2CookingManager cookingManager;

    // State
    private bool hasThrown = false;
    private bool isInPan = false;
    private AudioSource audioSource;
    private Vector3 startPosition;
    private Vector3 startScale;

    private void Start()
    {
        startPosition = transform.position;
        startScale = transform.localScale;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnMouseDown()
    {
        if (hasThrown || isInPan)
            return;

        StartCoroutine(ThrowMeatToPan());
    }

    /// <summary>
    /// Complete throw sequence: arc motion, rotation, landing, bounce, squash
    /// </summary>
    private IEnumerator ThrowMeatToPan()
    {
        hasThrown = true;

        // Play throw sound
        if (throwSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(throwSound);
        }

        // Arc motion with rotation
        yield return StartCoroutine(ArcThrow());

        // Landing effects
        yield return StartCoroutine(LandingSequence());

        isInPan = true;

        // Notify cooking manager that meat is placed
        if (cookingManager != null)
        {
            cookingManager.MeatPlaced();
            
            // Wait 7 seconds before allowing flip
            yield return new WaitForSeconds(7f);
            
            // Prepare for flip
            cookingManager.MeatReady();
        }
    }

    /// <summary>
    /// Parabolic arc motion with rotation
    /// </summary>
    private IEnumerator ArcThrow()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = panPoint.position;
        Quaternion startRot = transform.rotation;
        
        float elapsed = 0f;

        while (elapsed < throwDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / throwDuration;
            
            // Apply easing curve
            float easeT = throwCurve.Evaluate(t);
            
            // Linear horizontal/vertical movement
            Vector3 linearPos = Vector3.Lerp(startPos, endPos, easeT);
            
            // Arc height (parabolic - peaks at midpoint)
            float arcT = Mathf.Sin(t * Mathf.PI) * arcHeight;
            linearPos.y += arcT;
            
            transform.position = linearPos;
            
            // Rotation during flight
            float rotationZ = rotationAmount * t;
            transform.rotation = startRot * Quaternion.Euler(0f, 0f, rotationZ);
            
            yield return null;
        }

        // Ensure final position
        transform.position = endPos;
        transform.rotation = startRot;
    }

    /// <summary>
    /// Landing sequence: bounce + squash/stretch
    /// </summary>
    private IEnumerator LandingSequence()
    {
        Vector3 landPos = transform.position;
        Vector3 normalScale = startScale;

        // Play landing sound
        if (landSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(landSound);
        }

        // Play landing particles
        if (landingParticles != null)
        {
            landingParticles.Play();
        }

        // Bounce (up and down)
        yield return StartCoroutine(BounceEffect(landPos, normalScale));

        // Squash and stretch
        yield return StartCoroutine(SquashEffect(normalScale));

        // Return to normal scale
        transform.localScale = normalScale;
    }

    /// <summary>
    /// Bouncing animation after landing
    /// </summary>
    private IEnumerator BounceEffect(Vector3 landPos, Vector3 normalScale)
    {
        float elapsed = 0f;

        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bounceDuration;
            
            // Ease out bounce (quick up, slow down)
            float bounceT = Mathf.Sin(t * Mathf.PI);
            
            Vector3 bouncePos = landPos;
            bouncePos.y += bounceT * bounceHeight;
            
            transform.position = bouncePos;
            
            yield return null;
        }

        transform.position = landPos;
    }

    /// <summary>
    /// Squash/stretch effect on landing
    /// </summary>
    private IEnumerator SquashEffect(Vector3 normalScale)
    {
        float elapsed = 0f;

        while (elapsed < squashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / squashDuration;
            
            // Ease back to normal
            float easeT = 1f - t;
            float squashT = Mathf.Lerp(squashAmount, 0f, bounceCurve.Evaluate(t));
            
            // Squash Y (compress), stretch X and Z (wider)
            Vector3 squashedScale = normalScale;
            squashedScale.y *= (1f - squashT);
            squashedScale.x *= (1f + squashT * 0.5f);
            squashedScale.z *= (1f + squashT * 0.5f);
            
            transform.localScale = squashedScale;
            
            yield return null;
        }

        transform.localScale = normalScale;
    }

    /// <summary>
    /// Get current state
    /// </summary>
    public bool IsInPan()
    {
        return isInPan;
    }

    public bool HasThrown()
    {
        return hasThrown;
    }
}
