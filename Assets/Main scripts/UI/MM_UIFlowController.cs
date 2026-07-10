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

    [Header("Scene")]
    [SerializeField] private string gameplaySceneName = "ChemicalExampleScene";

    private void Start()
    {
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
        //[SerializeField] private GameObject experimentIntroPanel;
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
      //  experimentIntroPanel.SetActive(true);
    }

    //-------------------------------------------------
    // Button Functions
    //-------------------------------------------------

    // Start Screen
    public void StartButton()
    {
        ShowTopicPanel();
    }

    // Topic Screen
    public void Topic1Button()
    {
        ShowExamplePanel();
    }

    // Center Start Button (Example Panel)
    public void StartExamplesButton()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    // Reaction Summary
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