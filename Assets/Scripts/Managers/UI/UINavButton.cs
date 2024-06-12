using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UINavButton : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] MainInputManager mainInputManager;
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
        mainInputManager = FindObjectOfType<MainInputManager>();

        mainInputManager.InputMaster.UI.Navigate.performed += ctx => Navigate(ctx.ReadValue<Vector2>());

        mainInputManager.InputMaster.UI.Submit.performed += ctx => SetEditing(ctx);
        mainInputManager.InputMaster.UI.Cancel.performed += ctx => UnsetEditing(ctx);

        if(options != null && options.Count > 0)
        {
            SetOption(currentOptionIndex, Color.white);
        }
    }

    void Update()
    {

    }

    public void ChangeEditing(bool editing)
    {
        this.editing = editing;

        if (editing)
        {
            mainInputManager.LockedObject = gameObject;
            foreach (TextMeshProUGUI text in mainOptionTextList)
            {
                text.color = selectedColor;
            }
        }
        else
        {
            mainInputManager.LockedObject = null;
            foreach (TextMeshProUGUI text in mainOptionTextList)
            {
                text.color = Color.white;
            }
        }
    }

    void SetEditing(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && selected)
        {
            ChangeEditing(true);
        }
    }

    void UnsetEditing(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && selected)
        {
            ChangeEditing(false);
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
        //for it to reset the button after the next option is selected, so you can keep pressing the button to change the options GAMBIARRA
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
        SetOption(currentOptionIndex , selectedColor);
    }

    public void PreviousOption()
    {
        currentOptionIndex--;
        if (currentOptionIndex < 0)
        {
            currentOptionIndex = options.Count - 1;
        }
        SetOption(currentOptionIndex, selectedColor);
    }

    public void SetOption(int index, Color color)
    {
        if (index < options.Count)
        {
            currentOptionIndex = index;
            UpdateText(options[index], color);
        }
        else
        {
            Debug.LogError("Option index out of range on object" + gameObject.name);
        }
    }

    public void SetOptionsList(List<string> options)
    {
        this.options = options;
    }

    void HandleSelect()
    {
        selected = true;
        if (editing)
        {
            ChangeEditing(false);
        }
        mainText.color = selectedColor;

        foreach (GameObject arrow in arrows)
        {
            arrow.SetActive(true);
        }
    }

    void HandleDeselect()
    {
        selected = false;
        if (editing)
        {
            ChangeEditing(false);
        }
        mainText.color = Color.white;

        foreach (GameObject arrow in arrows)
        {
            arrow.SetActive(false);
        }
    }

    void UpdateText(string text, Color color)
    {
        foreach (TextMeshProUGUI textElement in mainOptionTextList)
        {
            textElement.text = text;
            textElement.color = color;
        }

        foreach (TextMeshProUGUI textElement in OptionTextDecorationList)
        {
            textElement.text = text;
            //textElement.color = color; // Uncomment if decorations should also change color
        }
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

    public void Penis()
    {
        Debug.Log("Penis");
    }
}
