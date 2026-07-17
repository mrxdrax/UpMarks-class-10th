using UnityEngine;
using System.Collections;

public class Ex2CookingManager : MonoBehaviour
{
    [Header("============ MEAT OBJECTS ============")]
    [SerializeField] private GameObject rawMeat;
    [SerializeField] private GameObject cookedMeat;

    [Header("============ COOKING EFFECTS ============")]
    [SerializeField] private GameObject gasFlame;
    [SerializeField] private GameObject smokeFX;
    [SerializeField] private AudioClip gasStartSound;
    [SerializeField] private AudioClip gasSizzleSound;
    [SerializeField] private float sizzleVolume = 0.5f;

    [Header("============ TIMING ============")]
    [SerializeField] private float cookingTime = 15f;
    [SerializeField] private float smokeDelay = 2f; // When smoke starts

    [Header("============ MANAGER REFERENCES ============")]
    [SerializeField] private Ex2MeatFlip meatFlip;
    [SerializeField] private ExampleManager exampleManager;

    // State
    private bool meatPlaced = false;
    private bool gasStarted = false;
    private bool cookingFinished = false;
    private bool firstSideCooked = false;
    
    private AudioSource audioSource;
    private Coroutine cookingCoroutine;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Called when meat reaches pan
    /// </summary>
    public void MeatPlaced()
    {
        meatPlaced = true;
        Debug.Log("Meat placed in pan - waiting for stove interaction");
    }

    /// <summary>
    /// Called when player clicks stove knob
    /// </summary>
    public void StartCooking()
    {
        if (!meatPlaced)
        {
            Debug.Log("Place meat in pan first");
            return;
        }

        if (gasStarted)
        {
            Debug.Log("Already cooking");
            return;
        }

        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
        }

        cookingCoroutine = StartCoroutine(CookingRoutine());
    }

    /// <summary>
    /// First side cooking (half the total time)
    /// </summary>
    private IEnumerator CookingRoutine()
    {
        gasStarted = true;

        // Activate gas flame
        if (gasFlame != null)
        {
            gasFlame.SetActive(true);
        }

        // Play gas start sound
        if (gasStartSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gasStartSound);
        }

        Debug.Log("Gas turned ON - cooking first side");

        // Wait before smoke appears
        yield return new WaitForSeconds(smokeDelay);

        // Activate smoke
        if (smokeFX != null)
        {
            smokeFX.SetActive(true);
        }

        // Play sizzle sound loop
        if (gasSizzleSound != null && audioSource != null)
        {
            audioSource.clip = gasSizzleSound;
            audioSource.volume = sizzleVolume;
            audioSource.loop = true;
            audioSource.Play();
        }

        // First side cooking time
        float firstSideTime = cookingTime / 2f;
        yield return new WaitForSeconds(firstSideTime);

        Debug.Log("First side cooked - ready to flip");

        firstSideCooked = true;

        // Enable flip on meat
        if (meatFlip != null)
        {
            meatFlip.EnableFlip();
        }
    }

    /// <summary>
    /// Called by MeatFlip after flip animation completes
    /// Continues cooking second side
    /// </summary>
    public void StartSecondCooking()
    {
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
        }

        cookingCoroutine = StartCoroutine(SecondCookingRoutine());
    }

    /// <summary>
    /// Second side cooking (remaining half)
    /// </summary>
    private IEnumerator SecondCookingRoutine()
    {
        Debug.Log("Cooking second side");

        float secondSideTime = cookingTime / 2f;
        yield return new WaitForSeconds(secondSideTime);

        Debug.Log("Cooking finished - meat fully cooked");

        // Turn off gas and effects
        CookingComplete();
    }

    /// <summary>
    /// Called when cooking finishes
    /// </summary>
    private void CookingComplete()
    {
        // Stop audio
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // Turn off effects
        if (gasFlame != null)
        {
            gasFlame.SetActive(false);
        }

        if (smokeFX != null)
        {
            smokeFX.SetActive(false);
        }

        // Swap meshes: hide raw, show cooked
        if (rawMeat != null)
        {
            rawMeat.SetActive(false);
        }

        if (cookedMeat != null)
        {
            cookedMeat.SetActive(true);
        }

        cookingFinished = true;

        // Notify ExampleManager that cooking is complete
        if (exampleManager != null)
        {
            exampleManager.DayCompleted();
            Debug.Log("Notified ExampleManager: DayCompleted");
        }

        Debug.Log("Cooking finished - inspection available");
    }

    /// <summary>
    /// Called when meat is ready for flip (7 sec after landing)
    /// </summary>
    public void MeatReady()
    {
        Debug.Log("Meat ready for flip after 7 second wait");
    }

    /// <summary>
    /// Get current cooking state
    /// </summary>
    public bool IsCookingFinished()
    {
        return cookingFinished;
    }

    public bool IsFirstSideCooked()
    {
        return firstSideCooked;
    }

    public bool IsGasOn()
    {
        return gasStarted;
    }
}
