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
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private List<TextMeshProUGUI> textDecorationList;
    [SerializeField] private bool colorSwap;
    [SerializeField] private Color selectedColor;
    private Color mainTextOriginalColor;
    [SerializeField] private float blinkSpeed;
    [SerializeField] private string initialChar;
    [SerializeField] private string closureChar;

    void Start()
    {
        button = GetComponent<Button>();

        originalText = mainText.text;   
        mainTextOriginalColor = mainText.color;

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
        HandleSelect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HandleDeselect();
    }

    public void OnSelect(BaseEventData eventData)
    {
        HandleSelect();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        HandleDeselect();
    }



    void HandleSelect()
    {
        selected = true;
        selectedTimer = 0;

        mainText.text = selectedText;

        foreach (TextMeshProUGUI t in textDecorationList)
        {
            t.text = selectedText;
        }

        if (colorSwap) mainText.color = selectedColor;
    }

    void HandleDeselect()
    {
        selected = false;
        mainText.text = originalText;

        foreach (TextMeshProUGUI t in textDecorationList)
        {
            t.text = originalText;
        }

        if (colorSwap) mainText.color = mainTextOriginalColor;
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
            if (mainText.text == selectedText)
            {
                mainText.text = originalText;

                foreach (TextMeshProUGUI t in textDecorationList)
                {
                    t.text = originalText;
                }
            }
            else
            {
                mainText.text = selectedText;

                foreach (TextMeshProUGUI t in textDecorationList)
                {
                    t.text = selectedText;
                }
            }
            
            selectedTimer = 0;
        }
    }
}
