using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
    [SerializeField] private float parallaxMultiplier = 0.5f;
    [SerializeField] private Transform cameraTransform;

    private Vector3 lastCameraPosition;

    private void Start()
    {
        lastCameraPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        float deltaMovement = cameraTransform.position.x - lastCameraPosition.x;
        transform.position += new Vector3(deltaMovement * parallaxMultiplier, 0f, 0f);
        lastCameraPosition = cameraTransform.position;
    }
}
