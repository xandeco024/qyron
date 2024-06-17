using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] bool cutscene;
    [SerializeField] PauseManager pauseManager;
    [SerializeField] GameObject cutscenePanel;
    [SerializeField] GameObject transitionPanel;
    void Start()
    {
        if (cutscene)
        {
            pauseManager.SetPause(true);
            cutscenePanel.SetActive(true);
        }
        else 
        {
            cutscenePanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndCutscene()
    {
        cutscenePanel.SetActive(false);
        transitionPanel.SetActive(true);
        pauseManager.SetPause(false);
    }
}
