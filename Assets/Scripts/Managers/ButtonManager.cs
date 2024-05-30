using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    string originalText;
    string selectedText;
    float selectedTimer = 0;
    bool selected = false;
    Button button;
    TextMeshProUGUI text;



    void Start()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        originalText = text.text;   
        selectedText = "> " + originalText + " <";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selected = true;
        selectedTimer = 0;
        text.text = selectedText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selected = false;
        text.text = originalText;
    }

    public void OnSelect(BaseEventData eventData)
    {
        selected = true;
        selectedTimer = 0;
        text.text = selectedText;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selected = false;
        text.text = originalText;
    }



    void Update()
    {
        if (selected)
        {
            BlinkSelectedText();
        }
    }

    void BlinkSelectedText()
    {
        selectedTimer += Time.deltaTime;
        if (selectedTimer >= 0.5f)
        {
            if (text.text == selectedText)
            {
                text.text = originalText;
            }
            else
            {
                text.text = selectedText;
            }
            selectedTimer = 0;
        }
    }
}
