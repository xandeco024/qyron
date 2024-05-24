using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private GameObject lobbyGameObject;
    [SerializeField] private GameObject settingsGameObject;
    [SerializeField] private GameObject creditsGameObject;

    void Start()
    {
        
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
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
