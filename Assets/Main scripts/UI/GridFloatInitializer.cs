using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GridFloatInitializer : MonoBehaviour
{
    [Header("Wait")]
    public int framesToWait = 2;

    GridLayoutGroup grid;

    IEnumerator Start()
    {
        grid = GetComponent<GridLayoutGroup>();

        for(int i=0;i<framesToWait;i++)
            yield return null;

        foreach(RectTransform child in transform)
        {
            UIFloat f = child.gameObject.GetComponent<UIFloat>();

            if(f==null)
                child.gameObject.AddComponent<UIFloat>();
        }

        if(grid!=null)
            grid.enabled=false;
    }
}