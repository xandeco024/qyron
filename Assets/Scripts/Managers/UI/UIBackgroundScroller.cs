using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBackgroundScroller : MonoBehaviour
{
    [SerializeField] Vector2 scrollSpeed;
    RawImage rawImage;
    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        rawImage.uvRect = new Rect(rawImage.uvRect.position + (scrollSpeed * Time.deltaTime), rawImage.uvRect.size);
    }
}
