using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanelObject;
    [SerializeField] private Button mainMenuFirstButton;

    [Header("Lobby")]
    [SerializeField] private GameObject lobbyPanelObject;
    [SerializeField] private Button lobbyFirstButton;

    [Header("Settings")]
    [SerializeField] private GameObject settingsPanelObject;
    [SerializeField] private Selectable settingsFirstButton;

    [Header("Credits")]
    [SerializeField] private GameObject creditsPanelObject;
    [SerializeField] private Button creditsFirstButton;

    private GameObject lockedObject;
    public GameObject LockedObject { get => lockedObject; set => lockedObject = value; }
    private MainInputManager mainInputManager;

    void Awake()
    {

    }

    void Start()
    {
        mainInputManager = FindObjectOfType<MainInputManager>();

        mainInputManager.InputMaster.UI.Cancel.performed += ctx => BackToMainMenu(settingsPanelObject);
        mainInputManager.InputMaster.UI.Cancel.performed += ctx => BackToMainMenu(creditsPanelObject);
        mainInputManager.InputMaster.UI.Cancel.performed += ctx => BackToMainMenu(lobbyPanelObject);

        ResumeGameIfPaused();
    }

    void Update()
    {
        
    }

    public void Play()
    {
        mainMenuPanelObject.SetActive(false);
        lobbyPanelObject.SetActive(true);
        if (lobbyFirstButton != null) lobbyFirstButton.Select();
    }

    public void OpenCredits()
    {
        mainMenuPanelObject.SetActive(false);
        creditsPanelObject.SetActive(true);
        if (creditsFirstButton != null) creditsFirstButton.Select();
    }

    public void OpenSettings()
    {
        mainMenuPanelObject.SetActive(false);
        settingsPanelObject.SetActive(true);
        if (settingsFirstButton != null) settingsFirstButton.Select();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void BackToMainMenu(GameObject currentMenu)
    {
        if (mainInputManager.LockedObject == null)
        {
            currentMenu.SetActive(false);
            mainMenuPanelObject.SetActive(true);
            if (mainMenuFirstButton != null) mainMenuFirstButton.Select();
        }
    }

    private void ResumeGameIfPaused()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }
}
