using UnityEngine;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Hides all panels in the provided array and shows the specified panel.
    /// This ensures only one panel is active at a time.
    /// </summary>
    public void ShowPanel(GameObject panel, GameObject[] allPanels)
    {
        // Hide all panels first
        if (allPanels != null)
        {
            foreach (GameObject p in allPanels)
            {
                if (p != null)
                {
                    p.SetActive(false);
                }
            }
        }

        // Show only the specified panel
        if (panel != null)
        {
            panel.SetActive(true);
            Debug.Log($"UIManager: Showing panel {panel.name}");
        }
    }

    /// <summary>
    /// Hides a single panel.
    /// </summary>
    public void HidePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
}
