using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private InputMaster inputMaster;
    private GameManager gameManager;
    private bool paused;
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
        inputMaster.UI.PauseResume.performed += _ => Pause(paused);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void Pause(bool pause)
    {
        paused = !paused;
        Time.timeScale = pause ? 0 : 1;
        pausePanel.SetActive(pause);
    }
}
