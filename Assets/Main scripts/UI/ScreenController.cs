using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenController : MonoBehaviour
{
    [Header("Panel Flow - Ordered for Navigation")]
    [SerializeField] private GameObject[] screenPanels;

    [Header("Special Panels")]
    [SerializeField] private GameObject examplePanel;
    
    [Header("Transition")]
    [SerializeField] private GameObject transitionPanel;
    
    [Header("Scene to Load")]
    [SerializeField] private string gameplaySceneName = "Gameplay";

    [SerializeField] private UIManager uiManager;

    private int currentScreenIndex = 0;
    
    private bool isTransitioning = false;

    private void OnEnable()
    {
        Debug.Log("ScreenController Enabled");

        if (NavigationSystem.Instance != null)
        {
            NavigationSystem.Instance.OnNavigateNext += HandleNext;
            NavigationSystem.Instance.OnNavigateBack += HandleBack;
        }
    }

    private void OnDisable()
    {
        if (NavigationSystem.Instance != null)
        {
            NavigationSystem.Instance.OnNavigateNext -= HandleNext;
            NavigationSystem.Instance.OnNavigateBack -= HandleBack;
        }
    }

    private void Start()
    {
        if (screenPanels.Length == 0)
        {
            Debug.LogError("ScreenController: No panels assigned in screenPanels array!");
            return;
        }

        currentScreenIndex = 0;
        ShowScreen(currentScreenIndex);
    }

    private void HandleNext()
    {
        Debug.Log("ScreenController HandleNext");

        if (isTransitioning)
            return;

        // Special case: If on ExamplePanel, load Gameplay Scene
        if (currentScreenIndex < screenPanels.Length && 
            screenPanels[currentScreenIndex] == examplePanel)
        {
            LoadGameplayScene();
            return;
        }

        // Normal case: Move to next panel in array
        if (currentScreenIndex < screenPanels.Length - 1)
        {
            TransitionToScreen(currentScreenIndex + 1);
        }
    }

    private void HandleBack()
    {
        if (isTransitioning)
            return;

        // Move to previous panel in array
        if (currentScreenIndex > 0)
        {
            TransitionToScreen(currentScreenIndex - 1);
        }
    }

    private void TransitionToScreen(int newIndex)
    {
        if (newIndex < 0 || newIndex >= screenPanels.Length)
            return;

        StartCoroutine(TransitionRoutine(newIndex));
    }

    private System.Collections.IEnumerator TransitionRoutine(int newIndex)
    {
        isTransitioning = true;

        // CLOSE ANIMATION (1.4 seconds)
        if (transitionPanel != null)
        {
            transitionPanel.SetActive(true);
            yield return new WaitForSeconds(1.4f);
        }

        // CHANGE PANEL (happens behind closed transition)
        currentScreenIndex = newIndex;
        uiManager.ShowPanel(screenPanels[currentScreenIndex], screenPanels);

        // HOLD (1.2 seconds - new panel visible behind closed transition)
        if (transitionPanel != null)
        {
            yield return new WaitForSeconds(1.2f);
        }

        // OPEN ANIMATION (1.4 seconds)
        if (transitionPanel != null)
        {
            transitionPanel.SetActive(false);
            yield return new WaitForSeconds(1.4f);
        }

        isTransitioning = false;
    }

    private void ShowScreen(int index)
    {
        if (index < 0 || index >= screenPanels.Length)
            return;

        currentScreenIndex = index;
        uiManager.ShowPanel(screenPanels[index], screenPanels);
        
        Debug.Log($"ScreenController: Showing screen at index {index}");
    }

    private void LoadGameplayScene()
    {
        StartCoroutine(LoadGameplayRoutine());
    }

    private System.Collections.IEnumerator LoadGameplayRoutine()
    {
        isTransitioning = true;

        // CLOSE ANIMATION (1.4 seconds)
        if (transitionPanel != null)
        {
            transitionPanel.SetActive(true);
            yield return new WaitForSeconds(1.4f);
        }

        // Load scene
        SceneManager.LoadScene(gameplaySceneName);
    }

    /// <summary>
    /// Allows ExamplePanel buttons or other components to select a topic/example
    /// This doesn't trigger navigation - just selection.
    /// Next button will continue the flow.
    /// </summary>
    public void SelectTopic(int topicIndex)
    {
        // If you need to store selected topic for passing to Gameplay scene,
        // use PlayerPrefs or a separate data container
        PlayerPrefs.SetInt("SelectedTopicIndex", topicIndex);
        Debug.Log($"ScreenController: Topic {topicIndex} selected");
    }

    public int GetCurrentScreenIndex()
    {
        return currentScreenIndex;
    }

    public GameObject GetCurrentScreen()
    {
        if (currentScreenIndex >= 0 && currentScreenIndex < screenPanels.Length)
            return screenPanels[currentScreenIndex];
        return null;
    }
}
