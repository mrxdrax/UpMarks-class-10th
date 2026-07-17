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
    if(!dayCompleted || !inspectionCompleted)
        return;

    if(currentExample < examples.Count - 1)
    {
        ShowExample(currentExample + 1);
    }
    else
    {
        PlayerPrefs.SetInt("ReturnFromExperiment",1);
        SceneManager.LoadScene(mainMenuSceneName);
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
        PlayerPrefs.SetInt("ReturnFromExperiment",1);
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

public void InspectionCompleted()
{
    inspectionCompleted = true;
    CheckNextButton();

    Debug.Log("Inspection Complete");
}

private void CheckNextButton()
{
    if(nextButton != null)
        nextButton.interactable = dayCompleted && inspectionCompleted;
}
public void DayCompleted()
{
    dayCompleted = true;
    CheckNextButton();
}

}