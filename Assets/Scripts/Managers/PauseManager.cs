using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    private InputMaster inputMaster;
    private GameManager gameManager;
    private GameOverManager gameOverManager;
    private EventSystem eventSystem;
    private bool paused = false;
    [SerializeField] private GameObject pauseCanvasObject;
    [SerializeField] private Button reusmeButton;

    void Awake()
    {
        Controls();
    }

    void OnEnable()
    {
        inputMaster.Enable();
    }

    void OnDisable()
    {
        inputMaster.Disable();
    }

    void Controls()
    {
        inputMaster = new InputMaster();
        inputMaster.UI.PauseResume.performed += ctx => TogglePauseAction(ctx);
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameOverManager = FindObjectOfType<GameOverManager>();
        eventSystem = FindObjectOfType<EventSystem>();
        SetPause(false);
    }

    void Update()
    {
        
    }

    public void TogglePauseAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (!gameOverManager.GameOver)
        {
            SetPause(!paused);
        }
    }

    public void SetPause(bool value)
    {
        if (value)
        {
            Time.timeScale = 0;
            pauseCanvasObject.SetActive(true);
            paused = true;
            //reusmeButton.Select();
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(reusmeButton.gameObject);
        }
        else if(!value)
        {
            Time.timeScale = 1;
            pauseCanvasObject.SetActive(false);
            paused = false;
        }
    }
}
