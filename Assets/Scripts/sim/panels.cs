using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class panels : MonoBehaviour
{
    [Header("Pause")]
    [SerializeField] private GameObject pausePanel;
    //[SerializeField] private Button continueButotn;
    private bool isPaused;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            Pause();
        }

        PausePanel();
    }

    private void PausePanel()
    {
        if(isPaused)
        {
            Time.timeScale = 0.0f;
            pausePanel.SetActive(true);
        }

        else if(!isPaused)
        {
            Time.timeScale = 1.0f;
            pausePanel.SetActive(false);
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
}
