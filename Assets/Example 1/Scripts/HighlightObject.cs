using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    public Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    void OnMouseEnter()
    {
        outline.enabled = true;
    }

    void OnMouseExit()
    {
        outline.enabled = false;
    }
}