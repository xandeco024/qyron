using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

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
        selectedText = $"{initialChar} {mainText} {closureChar}";
    }

    void OnEnable()
    {
        ResetState();
    }

    void Update()
    {
        if (selected)
        {
            BlinkText();
        }
    }

    private void ResetState()
    {
        selected = false;
        selectedTimer = 0;
        UpdateText(mainText, mainColor);
    }

    private void BlinkText()
    {
        selectedTimer += Time.deltaTime;
        if (selectedTimer > blinkSpeed)
        {
            if (mainTextList[0].text == selectedText)
            {
                UpdateText(mainText, selectedColor);
            }
            else
            {
                UpdateText(selectedText, selectedColor);
            }
            selectedTimer = 0;
        }
    }

    private void UpdateText(string text, Color color)
    {
        foreach (TextMeshProUGUI textElement in mainTextList)
        {
            textElement.text = text;
            textElement.color = color;
        }

        foreach (TextMeshProUGUI textElement in textDecorationList)
        {
            textElement.text = text;
            //textElement.color = color; // Uncomment if decorations should also change color
        }
    }

    private void HandleSelect()
    {
        selected = true;
        selectedTimer = 0;
        UpdateText(selectedText, selectedColor);
    }

    private void HandleDeselect()
    {
        selected = false;
        selectedTimer = 0;
        UpdateText(mainText, mainColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //HandleSelect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //HandleDeselect();
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