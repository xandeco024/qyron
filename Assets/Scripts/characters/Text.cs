using TMPro;
using UnityEngine;

public class Text : MonoBehaviour
{
    [SerializeField] private TextMeshPro textTMPRO;

    private void Start()
    {
        //textTMPRO = GetComponent<TextMeshPro>();
        Invoke("DestroyText", 0.5f);
    }

    public void SetDamage(string text, bool critical = false)
    {
        textTMPRO.text = text;
        if (critical)
        {
            //textTMPRO.color = new Color(164, 36, 69); unity colors goes from 0 to 1.
            textTMPRO.color = new Color(0.64f, 0.14f, 0.27f);
            textTMPRO.fontSize = 7;
            textTMPRO.text = text + "!";
        }
        textTMPRO.text = textTMPRO.text.Replace(',', '.');
    }

    public void SetHeal(string text)
    {
        textTMPRO.text = "+"+text;
        textTMPRO.color = new Color(0.2f, 0.8f, 0.2f);
        textTMPRO.text = textTMPRO.text.Replace(',', '.');
    }

    private void DestroyText()
    {
        Destroy(gameObject);
    }
}
