using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void ShowPanel(GameObject panel)
    {
        HideAllPanels();

        if (panel != null)
            panel.SetActive(true);
    }

    public void HidePanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(false);
    }

private void HideAllPanels()
{
    GameObject[] panels = GameObject.FindGameObjectsWithTag("Panel");

    foreach (GameObject panel in panels)
    {
        panel.SetActive(false);
    }
}
}