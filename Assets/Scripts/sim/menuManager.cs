using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuManager : MonoBehaviour
{

    int sceneBuildIndex;

    void Start()
    {
        sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(sceneBuildIndex + 1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
