using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject topicPanel;
    [SerializeField] private GameObject examplePanel;
    [SerializeField] private GameObject reactionSummaryPanel;
    //[SerializeField] private GameObject experimentIntroPanel;

    [Header("Transition")]
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private Animator transitionAnimator;

    [Header("Scene")]
    [SerializeField] private string gameplaySceneName = "ChemicalExampleScene";

    private void Start()
    {
        // Transition Panel hamesha start me off rahe
        if (transitionPanel != null)
            transitionPanel.SetActive(false);

        // Gameplay se return hua?
        if (PlayerPrefs.GetInt("ReturnFromExperiment", 0) == 1)
        {
            PlayerPrefs.DeleteKey("ReturnFromExperiment");
            ShowReactionSummary();
        }
        else
        {
            ShowStartPanel();
        }
    }

    //-------------------------------------------------
    // Universal
    //-------------------------------------------------

    private void HideAllPanels()
    {
        startPanel.SetActive(false);
        topicPanel.SetActive(false);
        examplePanel.SetActive(false);
        reactionSummaryPanel.SetActive(false);
        //experimentIntroPanel.SetActive(false);
    }

    //-------------------------------------------------
    // Show Panels
    //-------------------------------------------------

    public void ShowStartPanel()
    {
        HideAllPanels();
        startPanel.SetActive(true);
    }

    public void ShowTopicPanel()
    {
        HideAllPanels();
        topicPanel.SetActive(true);
    }

    public void ShowExamplePanel()
    {
        HideAllPanels();
        examplePanel.SetActive(true);
    }

    public void ShowReactionSummary()
    {
        HideAllPanels();
        reactionSummaryPanel.SetActive(true);
    }

    public void ShowExperimentIntro()
    {
        HideAllPanels();
        //experimentIntroPanel.SetActive(true);
    }

    //-------------------------------------------------
    // Start Transition
    //-------------------------------------------------

    public void StartButton()
    {
        StartCoroutine(StartTransitionRoutine());
    }

    private IEnumerator StartTransitionRoutine()
    {
        // Transition Panel ON
        transitionPanel.SetActive(true);
        transitionPanel.transform.SetAsLastSibling();

        // Animation Start
        transitionAnimator.Play("UI_Transition", 0, 0f);

        // 1.4 sec baad panel change
        yield return new WaitForSeconds(1.4f);

        ShowTopicPanel();

        // Total 2.2 sec tak transition chale
        yield return new WaitForSeconds(0.8f);

        // Transition OFF
        transitionPanel.SetActive(false);
    }

    //-------------------------------------------------
    // Topic Screen
    //-------------------------------------------------

    public void Topic1Button()
    {
        ShowExamplePanel();
    }

    //-------------------------------------------------
    // Example Screen
    //-------------------------------------------------

    public void StartExamplesButton()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    //-------------------------------------------------
    // Reaction Summary
    //-------------------------------------------------

    public void NextButton()
    {
        ShowExperimentIntro();
    }

    //-------------------------------------------------
    // Back Buttons
    //-------------------------------------------------

    public void BackToStart()
    {
        ShowStartPanel();
    }

    public void BackToTopic()
    {
        ShowTopicPanel();
    }

    public void BackToExample()
    {
        ShowExamplePanel();
    }

    public void BackToReactionSummary()
    {
        ShowReactionSummary();
    }
}