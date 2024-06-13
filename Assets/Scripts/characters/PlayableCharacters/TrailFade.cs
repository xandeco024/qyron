using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailFade : MonoBehaviour
{
    private SpriteRenderer dashTrailSR;
    private float duration = 0.5f;
    private float timer;

    void Start()
    {
        dashTrailSR = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / duration);
        Color color = dashTrailSR.color;
        color.a = alpha;
        dashTrailSR.color = color;
    }
}
