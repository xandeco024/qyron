using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UINavButton : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] MenuManager manager;
    private bool editing = false;
    private bool selected = false;
    [SerializeField] TextMeshProUGUI mainText;
    [SerializeField] List<string> options = new List<string>();
    public List<string> Options { get { return options; } }
    private int currentOptionIndex = 0;
    public int CurrentOptionIndex { get { return currentOptionIndex; } }
    [SerializeField] List<TextMeshProUGUI> mainOptionTextList = new List<TextMeshProUGUI>();
    [SerializeField] List<TextMeshProUGUI> OptionTextDecorationList = new List<TextMeshProUGUI>();
    [SerializeField] private Color selectedColor;
    [SerializeField] List<GameObject> arrows = new List<GameObject>();

    void Start()
    {
        manager.InputMaster.UI.Navigate.performed += ctx => Navigate(ctx.ReadValue<Vector2>());
        manager.InputMaster.UI.Submit.performed += ctx => ToggleEditing(ctx);

        if(options != null)
        {
            SetOption(currentOptionIndex);
        }
    }

    void Update()
    {

    }

    public void SetEditing(bool editing)
    {
        this.editing = editing;

        if (editing)
        {
            foreach (TextMeshProUGUI text in mainOptionTextList)
            {
                text.color = selectedColor;
            }
        }
        else
        {
            foreach (TextMeshProUGUI text in mainOptionTextList)
            {
                text.color = Color.white;
            }
        }
    }

    void ToggleEditing(InputAction.CallbackContext context)
    {
        if (context.performed && selected)
        {
            editing = !editing;
            SetEditing(editing);
        }
    }

    void Navigate(Vector2 direction)
    {
        if (editing)
        {
            if (direction.x > 0)
            {
                PreviousOption();
            }
            else if (direction.x < 0)
            {
                NextOption();
            }
        }
    }

    public void ButtonNextOption(Button button)
    {
        //for it to reset the button after the next option is selected, so you can keep pressing the button to change the options
        button.gameObject.SetActive(false);
        Debug.Log("ButtonNext");
        NextOption();
        button.gameObject.SetActive(true);
    }

    public void ButtonPreviousOption(Button button)
    {
        //for it to reset the button after the previous option is selected, so you can keep pressing the button to change the options
        button.gameObject.SetActive(false);
        Debug.Log("ButtonPrevious");
        PreviousOption();
        button.gameObject.SetActive(true);
    }

    public void NextOption()
    {
        currentOptionIndex++;
        if (currentOptionIndex >= options.Count)
        {
            currentOptionIndex = 0;
        }
        SetOption(currentOptionIndex);
    }

    public void PreviousOption()
    {
        currentOptionIndex--;
        if (currentOptionIndex < 0)
        {
            currentOptionIndex = options.Count - 1;
        }
        SetOption(currentOptionIndex);
    }

    public void SetOption(int index)
    {
        if (index < options.Count)
        {
            foreach (TextMeshProUGUI text in mainOptionTextList)
            {
                text.text = options[index];
            }

            foreach (TextMeshProUGUI text in OptionTextDecorationList)
            {
                text.text = options[index];
            }
        }
        else
        {
            Debug.LogError("Option index out of range");
        }
    }

    public void SetOptionsList(List<string> options)
    {
        this.options = options;
    }

    void HandleSelect()
    {
        selected = true;
        mainText.color = selectedColor;

        foreach (GameObject arrow in arrows)
        {
            arrow.SetActive(true);
        }
    }

    void HandleDeselect()
    {
        selected = false;
        editing = false;
        mainText.color = Color.white;

        foreach (GameObject arrow in arrows)
        {
            arrow.SetActive(false);
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
