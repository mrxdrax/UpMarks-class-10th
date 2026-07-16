using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    [SerializeField] private Outline outline;
    [SerializeField] private float outlineWidth = 3f;

    private void Start()
    {
        // Get outline component if not assigned
        if (outline == null)
        {
            outline = GetComponent<Outline>();
        }

        // Initialize outline as disabled
        if (outline != null)
        {
            outline.enabled = false;
            outline.OutlineWidth = outlineWidth;
        }
    }

    /// <summary>
    /// Enable outline on mouse enter
    /// NOTE: NailMove handles its own hover effects
    /// This script is kept for potential use on other interactive objects
    /// </summary>
    private void OnMouseEnter()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    /// <summary>
    /// Disable outline on mouse exit
    /// </summary>
    private void OnMouseExit()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    /// <summary>
    /// Public method to manually control outline
    /// </summary>
    public void SetOutlineActive(bool active)
    {
        if (outline != null)
        {
            outline.enabled = active;
        }
    }

    public void SetOutlineWidth(float width)
    {
        if (outline != null)
        {
            outline.OutlineWidth = width;
        }
    }
}
