using System;
using UnityEngine;

public class NavigationSystem : MonoBehaviour
{
    public static NavigationSystem Instance;

    public Action OnNavigateNext;
    public Action OnNavigateBack;

private void Awake()
{
    Debug.Log("NavigationSystem Awake");

    Instance = this;

    // Abhi isko comment kar de
    //DontDestroyOnLoad(gameObject);
}

    public void GoNext()
    {
        Debug.Log("NavigationSystem: GoNext called.");
        OnNavigateNext?.Invoke();
    }

    public void GoBack()
    {
        OnNavigateBack?.Invoke();
    }
}
