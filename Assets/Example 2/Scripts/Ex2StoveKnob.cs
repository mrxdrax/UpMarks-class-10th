using UnityEngine;

public class Ex2StoveKnob : MonoBehaviour
{
    [Header("============ COOKING MANAGER ============")]
    [SerializeField] private Ex2CookingManager cookingManager;

    [Header("============ EFFECTS ============")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private float rotationAmount = 45f; // degrees to rotate on click
    [SerializeField] private float rotationDuration = 0.3f;

    // State
    private bool isActivated = false;
    private AudioSource audioSource;
    private Quaternion startRotation;
    private bool isAnimating = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        startRotation = transform.rotation;
    }

    private void OnMouseDown()
    {
        if (isActivated)
        {
            Debug.Log("Stove already turned on");
            return;
        }

        ActivateStove();
    }

    /// <summary>
    /// Turn on stove and start cooking
    /// </summary>
    private void ActivateStove()
    {
        isActivated = true;

        // Play click sound
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        // Rotate knob for visual feedback
        if (!isAnimating)
        {
            StartCoroutine(RotateKnob());
        }

        // Start cooking
        if (cookingManager != null)
        {
            cookingManager.StartCooking();
            Debug.Log("Stove turned ON - starting to cook meat");
        }
    }

    /// <summary>
    /// Simple rotation animation for knob feedback
    /// </summary>
    private System.Collections.IEnumerator RotateKnob()
    {
        isAnimating = true;

        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, rotationAmount, 0f);
        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rotationDuration;

            // Ease out (start fast, end slow)
            float easeT = 1f - (1f - t) * (1f - t);

            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, easeT);

            yield return null;
        }

        transform.rotation = targetRotation;
        isAnimating = false;
    }

    /// <summary>
    /// Get activation state
    /// </summary>
    public bool IsActivated()
    {
        return isActivated;
    }
}
