using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraLookCanvas : MonoBehaviour
{
    private CameraManager cameraManager;
    private Canvas canvas;

    void Start()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        canvas = GetComponent<Canvas>();

        if (cameraManager == null)
        {
            Debug.LogError("CameraManager not found");
        }

        if (canvas == null)
        {
            canvas = GetComponentInChildren<Canvas>();

            if (canvas == null)
            {
                Debug.LogError("Canvas not found");
            }
        }
    }

    void Update()
    {
        if (cameraManager != null && canvas != null)
        {
            canvas.transform.LookAt(cameraManager.transform);
        }
    }
}
