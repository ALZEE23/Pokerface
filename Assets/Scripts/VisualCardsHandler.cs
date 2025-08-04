using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualCardsHandler : MonoBehaviour
{
    public static VisualCardsHandler instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Multiple VisualCardsHandler instances found!");
            Destroy(this);
            return;
        }

        instance = this;
        Debug.Log("VisualCardsHandler initialized");
    }

    // Hanya untuk debugging
    private void OnTransformChildrenChanged()
    {
        Debug.Log("VisualCardsHandler children changed. Total children: " + transform.childCount);
    }
}
