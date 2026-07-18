using UnityEngine;
using System.Collections;

public class Ex2MeatFlip : MonoBehaviour
{
    [Header("============ COOKING MANAGER ============")]
    [SerializeField] private Ex2CookingManager cookingManager;

    [Header("============ FLIP SETTINGS ============")]
    [SerializeField] private float flipTime = 0.6f;
    [SerializeField] private float flipTossHeight = 0.08f;
    [SerializeField] private float panHeightOffset = 0.025f;

    [Header("============ ANIMATION CURVE ============")]
    [SerializeField] private AnimationCurve flipCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("============ EFFECTS ============")]
    [SerializeField] private AudioClip flipSound;

    // State
    private bool canFlip = false;
    private bool flipped = false;
    private bool flipping = false;
    private AudioSource audioSource;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    /// <summary>
    /// Called by CookingManager when first side is cooked
    /// Allows player to manually click to flip, or auto-flip after delay
    /// </summary>
    public void EnableFlip()
{
    canFlip = true;

    // Save CURRENT pan position
    startPosition = transform.position;
    startRotation = transform.rotation;

    Debug.Log("Meat can be flipped");
}

    private void OnMouseDown()
    {
        if (!canFlip || flipped || flipping)
            return;

        Debug.Log("Player clicked meat - starting flip");
        StartCoroutine(FlipRoutine());
    }

    /// <summary>
    /// Flip animation: rotate 180°, toss upward, smooth landing
    /// </summary>
    private IEnumerator FlipRoutine()
    {
        flipping = true;

        // Play flip sound
        if (flipSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(flipSound);
        }

       Vector3 midPos = transform.position;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(180f, 0f, 0f);

        float elapsed = 0f;

        while (elapsed < flipTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flipTime;

            // Apply easing curve
            float easeT = flipCurve.Evaluate(t);

            // Rotation: smoothly rotate 180°
            transform.rotation = Quaternion.Slerp(startRot, endRot, easeT);

            // Toss: arc motion up and back down
            Vector3 tossPos = midPos;
            tossPos.y += Mathf.Sin(t * Mathf.PI) * flipTossHeight;
            transform.position = tossPos;

            yield return null;
        }

        // Ensure final state
        Vector3 finalPos = midPos;
finalPos.y += panHeightOffset;
transform.position = finalPos;
        transform.rotation = endRot;

        flipped = true;
        flipping = false;
        canFlip = false;

        Debug.Log("Flip complete - continuing second side");

        // Continue cooking second side
        if (cookingManager != null)
        {
            cookingManager.StartSecondCooking();
        }
    }

    /// <summary>
    /// Get flip state
    /// </summary>
    public bool IsFlipped()
    {
        return flipped;
    }

    public bool CanFlip()
    {
        return canFlip;
    }
}
