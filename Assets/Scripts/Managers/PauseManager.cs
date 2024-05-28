using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    private InputMaster inputMaster;
    private GameManager gameManager;
    private bool paused = false;
    [SerializeField] private GameObject pauseCanvasObject;
    [SerializeField] private Button reusmeButton;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
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
            if (!paused)
            {
                Time.timeScale = 0;
                pauseCanvasObject.SetActive(true);
                paused = true;
                reusmeButton.Select();
            }
            else
            {
                Time.timeScale = 1;
                pauseCanvasObject.SetActive(false);
                paused = false;
            }
    }
}
