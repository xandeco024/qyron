using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject lobbyGameObject;
    public GameObject LobbyGameObject { get => lobbyGameObject; }
    [SerializeField] private GameObject settingsGameObject;
    public GameObject SettingsGameObject { get => settingsGameObject; }
    [SerializeField] private GameObject creditsGameObject;
    public GameObject CreditsGameObject { get => creditsGameObject; }

    void Start()
    {
        ResumeGameIfPaused();
    }

    void Update()
    {
        
    }

    public void Play()
    {
        if (mainMenuObject.activeSelf)
        {
            mainMenuObject.SetActive(false);
        }

        if (!lobbyGameObject.activeSelf)
        {
            lobbyGameObject.SetActive(true);
        }
    }

    public void BackToMainMenu(GameObject currentMenu)
    {
        if (currentMenu.activeSelf)
        {
            currentMenu.SetActive(false);
        }

        if (!mainMenuObject.activeSelf)
        {
            mainMenuObject.SetActive(true);
            playButton.Select();
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void ResumeGameIfPaused()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }
}
