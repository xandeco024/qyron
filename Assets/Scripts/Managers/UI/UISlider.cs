using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private Slider slider;
    [SerializeField] Image sliderHandle;
    [SerializeField] List<TextMeshProUGUI> sliderTitleMain = new List<TextMeshProUGUI>();
    [SerializeField] List<TextMeshProUGUI> percentageTextList = new List<TextMeshProUGUI>();
    [SerializeField] string percentageSymbol;
    [SerializeField] private Color selectedColor;

    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { UpdatePercentageText(); });  //caralhou essa aq o copilot foi longe.
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    void UpdatePercentageText()
    {
        foreach (TextMeshProUGUI text in percentageTextList)
        {
            text.text = slider.value.ToString() + percentageSymbol;
        }
    }

    void HandleSelect()
    {
        foreach (TextMeshProUGUI text in sliderTitleMain)
        {
            text.color = selectedColor;
        }
    }

    void HandleDeselect()
    {
        foreach (TextMeshProUGUI text in sliderTitleMain)
        {
            text.color = Color.white;
        }
    }

    public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
    {
        HandleSelect();
    }

    public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        HandleDeselect();
    }

    public void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
    {
        HandleSelect();
    }

    public void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
    {
        HandleDeselect();
    }
}
