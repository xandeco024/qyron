using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class damageText : MonoBehaviour
{
    private float timeLastText;
    private Animator damageTextAnimator;
    private TextMeshPro damageTextMesh;

    private void Start()
    {
        damageTextAnimator = GetComponent<Animator>();
        damageTextMesh = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        if(Time.time - timeLastText >= .3)
        {
            damageTextAnimator.SetBool("fade", true);
        }

        else
        {
            damageTextAnimator.SetBool("fade", false);
        }
    }

    public void SetText(string text)
    {
        timeLastText = Time.time;
        damageTextMesh.text = text;
    }
}
