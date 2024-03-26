using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class fade : MonoBehaviour
{
    [SerializeField] float fadeDistance, movementSpeed, movementAmplitude;

    [SerializeField] bool isText;
    private GameObject qyron;
    private SpriteRenderer SR;
    private TextMeshPro textMeshPro;
    private Vector3 startPos;

    void Start()
    {
        qyron = GameObject.FindWithTag("Player");
        if(!isText) SR = GetComponent<SpriteRenderer>();
        if(isText) textMeshPro = GetComponent<TextMeshPro>();
        startPos = transform.position;
    }

    void Update()
    {
        float distance = Vector3.Distance(qyron.transform.position, transform.position);
        float alpha = Mathf.InverseLerp(fadeDistance, 0, distance);
        alpha = Mathf.Clamp(alpha, 0, 1);

        if (isText)
        {
            Color color = textMeshPro.color;
            color.a = alpha;
            textMeshPro.color = color;
        }
        else if (!isText)
        {
            Color color = SR.color;
            color.a = alpha;
            SR.color = color;
        }


        Vector3 pos = startPos;
        pos.y += Mathf.Sin(Time.time * movementSpeed) * movementAmplitude;
        transform.position = pos;
    }
}
