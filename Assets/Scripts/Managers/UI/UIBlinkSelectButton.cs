using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;

public class UIBlinkSelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private string mainText;
    [SerializeField] private string initialChar;
    [SerializeField] private string closureChar;
    private string selectedText;
    [SerializeField] private float blinkSpeed;
    private bool selected;
    private float selectedTimer = 0;

    [SerializeField] private Color selectedColor;
    private Color mainColor;

    [SerializeField] List<TextMeshProUGUI> mainTextList = new List<TextMeshProUGUI>();
    [SerializeField] List<TextMeshProUGUI> textDecorationList = new List<TextMeshProUGUI>();
    

    void Awake()
    {
        mainText = mainTextList[0].text;
        mainColor = mainTextList[0].color;
    }

    void Start()
    {
        selectedText = initialChar + " " + mainText + " " + closureChar;
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            BlinkColor();
        }
    }

    void BlinkColor()
    {
        selectedTimer += Time.deltaTime;
        if (selectedTimer > blinkSpeed)
        {
            if (mainTextList[0].text == selectedText)
            {
                foreach (TextMeshProUGUI text in mainTextList)
                {
                    text.text = mainText;;
                }
                
                foreach (TextMeshProUGUI text in textDecorationList)
                {
                    text.text = mainText;
                }
            }

            else
            {
                foreach (TextMeshProUGUI text in mainTextList)
                {
                    text.text = selectedText;
                }

                foreach (TextMeshProUGUI text in textDecorationList)
                {
                    text.text = selectedText;
                }
            }

            selectedTimer = 0;
        }
    }

    void HandleSelect()
    {
        selected = true;
        selectedTimer = 0;

        foreach (TextMeshProUGUI text in mainTextList)
        {
            text.text = selectedText;
            text.color = selectedColor;
        }

        foreach (TextMeshProUGUI text in textDecorationList)
        {
            text.text = selectedText;
            //text.GetComponent<TextMeshProUGUI>().color = selectedColor;
        }
    }

    void HandleDeselect()
    {
        selected = false;
        selectedTimer = 0;

        foreach (TextMeshProUGUI text in mainTextList)
        {
            text.text = mainText;
            text.color = mainColor;
        }

        foreach (TextMeshProUGUI text in textDecorationList)
        {
            text.text = mainText;
            //text.GetComponent<TextMeshProUGUI>().color = mainColor;
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
}
