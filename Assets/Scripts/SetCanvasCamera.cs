using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvasCamera : MonoBehaviour
{
    public Canvas canvas;

    void Start()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null && canvas != null)
        {
            Debug.Log("Main Camera found: " + mainCamera.name);
            canvas.worldCamera = mainCamera;
        }
        else
        {
            Debug.LogError("Main Camera or Canvas is missing!");
        }
    }
}