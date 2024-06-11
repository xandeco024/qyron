using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    private MainInputManager mainInputManager;
    private GameManager gameManager;
    private GameOverManager gameOverManager;
    private bool paused = false;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button reusmeButton;

    void Start()
    {
        mainInputManager = FindObjectOfType<MainInputManager>();
        mainInputManager.InputMaster.UI.PauseResume.performed += ctx => TogglePauseAction(ctx);
        gameManager = FindObjectOfType<GameManager>();
        gameOverManager = FindObjectOfType<GameOverManager>();
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
            pausePanel.SetActive(true);
            paused = true;
            reusmeButton.Select();
        }
        else if(!value)
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
            paused = false;
        }
    }
}
