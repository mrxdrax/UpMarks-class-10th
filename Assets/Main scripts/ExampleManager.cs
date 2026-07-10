using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ExampleData
{
    public string exampleName;
    public GameObject exampleObject;
    public Camera exampleCamera;
}

public class ExampleManager : MonoBehaviour
{
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
    }

   public void NextExample()
{
    if (currentExample == 0)
    {
        ShowExample(1);
    }
    else
    {
        PlayerPrefs.SetInt("ReturnFromExperiment", 1);
        SceneManager.LoadScene("MainMenu");
    }
}
    public void PreviousExample()
    {
        if (currentExample > 0)
        {
            ShowExample(currentExample - 1);
        }
    }
    
}