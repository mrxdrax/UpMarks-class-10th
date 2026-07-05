using System.Collections.Generic;
using UnityEngine;

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

    int currentExample = 0;

    void Start()
    {
        ShowExample(0);
    }

    public void ShowExample(int index)
    {
        if (index < 0 || index >= examples.Count)
            return;

        // Sab OFF
        foreach (ExampleData ex in examples)
        {
            ex.exampleObject.SetActive(false);
            ex.exampleCamera.enabled = false;
        }

        // Sirf selected ON
        examples[index].exampleObject.SetActive(true);
        examples[index].exampleCamera.enabled = true;

        currentExample = index;

        Debug.Log("Current Example : " + examples[index].exampleName);
    }

    public void NextExample()
    {
        if (currentExample < examples.Count - 1)
        {
            ShowExample(currentExample + 1);
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