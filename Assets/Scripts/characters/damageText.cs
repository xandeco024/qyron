using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class damageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro textTMPRO;

    private void Start()
    {
        //textTMPRO = GetComponent<TextMeshPro>();
        Invoke("DestroyText", 0.5f);
    }

    public void SetText(string text, bool critical = false)
    {
        if (critical)
        {
            textTMPRO.color = Color.red;
            textTMPRO.fontSize = 7;
            textTMPRO.text = text + "!";
        }
        else
        {
            textTMPRO.text = text;
        }
    }

    private void DestroyText()
    {
        Destroy(gameObject);
    }
}
