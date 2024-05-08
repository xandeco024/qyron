using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private InputMaster inputMaster;
    private GameManager gameManager;
    private bool paused = false;
    [SerializeField] private GameObject pausePanel;

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
        inputMaster.UI.PauseResume.performed += _ => Pause();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Pause()
    {
        if(paused)
        {
            paused = false;
            Time.timeScale = 1;
            pausePanel.SetActive(false);
        }
        else
        {
            paused = true;
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        }
    }
}
