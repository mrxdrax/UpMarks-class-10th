using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class ExampleData
{
    public string exampleName;
    public GameObject exampleObject;
    public Camera exampleCamera;
}
public class ExampleManager : MonoBehaviour
{
    [Header("UI")]
[SerializeField] private GameObject backButton;
[SerializeField] private Button nextButton;

private bool dayCompleted = false;
private bool inspectionCompleted = false;
    public List<ExampleData> examples = new List<ExampleData>();

    [Header("Scene")]
    public string mainMenuSceneName = "MainMenu";

    private int currentExample = 0;
    public static bool OpenReactionPanel = false;
public static bool OpenExamplePanel = false;

    void Start()
    {
        ShowExample(0);
    }

    public void ShowExample(int index)
    {
        if (index < 0 || index >= examples.Count)
            return;

        foreach (ExampleData ex in examples)
        {
            ex.exampleObject.SetActive(false);

            if (ex.exampleCamera != null)
                ex.exampleCamera.enabled = false;
        }

        examples[index].exampleObject.SetActive(true);

        if (examples[index].exampleCamera != null)
            examples[index].exampleCamera.enabled = true;

        currentExample = index;

        Debug.Log("Current Example : " + examples[index].exampleName);
        dayCompleted = false;
inspectionCompleted = false;

if(nextButton != null)
    nextButton.interactable = false;
    }
public void NextExample()
{
    Debug.Log("Current Example = " + currentExample);
    Debug.Log("Total Examples = " + examples.Count);

    if (!dayCompleted || !inspectionCompleted)
        return;

    if (currentExample < examples.Count - 1)
    {
        Debug.Log("Opening Next Example");
        ShowExample(currentExample + 1);
    }
    else
    {
        Debug.Log("Opening Reaction Panel");

        OpenReactionPanel = true;
        SceneManager.LoadScene("MainMenu");
    }
}
public void PreviousExample()
{
    if(currentExample>0)
    {
        ShowExample(currentExample-1);
    }
    else
{
    OpenExamplePanel = true;
    SceneManager.LoadScene("MainMenu");
}
}

public void InspectionCompleted()
{
    inspectionCompleted = true;

    Debug.Log("Inspection Complete");
    Debug.Log("Day = " + dayCompleted);
    Debug.Log("Inspection = " + inspectionCompleted);

    CheckNextButton();

    Debug.Log("Next Button = " + nextButton.interactable);
}

private void CheckNextButton()
{
    Debug.Log("CheckNextButton Called");

    if(nextButton != null)
    {
        nextButton.interactable = dayCompleted && inspectionCompleted;

        Debug.Log("Button State = " + nextButton.interactable);
    }
}
public void DayCompleted()
{
    dayCompleted = true;

    Debug.Log("Day Completed");
    Debug.Log("Day = " + dayCompleted);
    Debug.Log("Inspection = " + inspectionCompleted);

    CheckNextButton();

    Debug.Log("Next Button = " + nextButton.interactable);
}
public void BackToReactionPanel()
{
    PlayerPrefs.SetString("OpenPanel", "ReactionPanel");
    SceneManager.LoadScene("Scene1");   // Apni Scene1 ka exact naam likh
}
public void BackToExamplePanel()
{
    PlayerPrefs.SetString("OpenPanel", "ExamplePanel");
    SceneManager.LoadScene("Scene1");   // Apni Scene1 ka exact naam likh
}

}