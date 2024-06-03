using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    string originalText;
    string selectedText;
    float selectedTimer = 0;
    bool selected = false;
    Button button;
    [SerializeField] List<TextMeshProUGUI> textList;
    [SerializeField] bool colorSwap;
    [SerializeField] Color selectedColor;
    Color mainTextOriginalColor;
    [SerializeField] float blinkSpeed;
    [SerializeField] string initialChar;
    [SerializeField] string closureChar;

    void Start()
    {
        button = GetComponent<Button>();
        
        if (textList.Count == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<TextMeshProUGUI>() != null)
                {
                    textList.Add(transform.GetChild(i).GetComponent<TextMeshProUGUI>());
                }
            }
        }

        originalText = textList[0].text;   
        mainTextOriginalColor = textList[textList.Count-1].color;

        if (closureChar != "")
        {
            selectedText = initialChar + " " + originalText + " " + closureChar;
        }
        else
        {
            selectedText = initialChar + " " + originalText + " " + initialChar;
        }   
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selected = true;
        selectedTimer = 0;

        foreach (TextMeshProUGUI t in textList)
        {
            t.text = selectedText;
        }

        if (colorSwap) textList[textList.Count-1].color = selectedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selected = false;
        foreach (TextMeshProUGUI t in textList)
        {
            t.text = originalText;
        }

        if (colorSwap) textList[textList.Count-1].color = mainTextOriginalColor;
    }

    public void OnSelect(BaseEventData eventData)
    {
        selected = true;
        selectedTimer = 0;
        foreach (TextMeshProUGUI t in textList)
        {
            t.text = selectedText;
        }

        if (colorSwap) textList[textList.Count-1].color = selectedColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selected = false;
        foreach (TextMeshProUGUI t in textList)
        {
            t.text = originalText;
        }

        if (colorSwap) textList[textList.Count-1].color = mainTextOriginalColor;
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
        if (selectedTimer >= blinkSpeed)
        {
            if (textList[0].text == selectedText)
            {
                foreach (TextMeshProUGUI t in textList)
                {
                    t.text = originalText;
                }
            }
            else
            {
                foreach (TextMeshProUGUI t in textList)
                {
                    t.text = selectedText;
                }
            }
            selectedTimer = 0;
        }
    }
}
