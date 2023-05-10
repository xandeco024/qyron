using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class panels : MonoBehaviour
{
    [Header("Pause")]
    private GameObject pausePanel;
    private bool isPaused;
    private int currentSceneIndex;

    [Header("Game Over")]
    private GameObject gameOverPanel;
    private bool isDead;

    void Start()
    {
        //currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        pausePanel = GameObject.FindWithTag("PausePanel");
        gameOverPanel = GameObject.FindWithTag("GameOverPanel");
    }

    void Update()
    {
        isDead = GetComponent<gameManager>().GetIsDead();

        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            Pause();
        }

        if(!isDead)
        {
            PausePanel();
        }

        else

        {
            GameOverPanel();
        }
    }

    private void PausePanel()
    {
        if(isPaused)
        {
            Time.timeScale = 0.0f;
            pausePanel.GetComponent<Canvas>().enabled = true;
        }

        else if(!isPaused)
        {
            Time.timeScale = 1.0f;
            pausePanel.GetComponent<Canvas>().enabled = false;
        }
    }

    public void Pause()
    {
        if(isPaused)
        {
            isPaused = false;
        }

        else if(!isPaused)
        {
            isPaused = true;
        }
    }

    private void GameOverPanel()
    {
        if (!isDead)
        {
            return;
        }
        else
        {
            Time.timeScale = 0;
            gameOverPanel.GetComponent<Canvas>().enabled = true;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(currentSceneIndex);
    }
}
