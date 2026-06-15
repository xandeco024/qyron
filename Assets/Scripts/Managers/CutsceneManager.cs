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
        // Se a cutscene estiver ativa e o jogador apertar ENTER, pula a cutscene
        if (cutscene && Input.GetKeyDown(KeyCode.Return))
        {
            EndCutscene();
        }
    }

    public void EndCutscene()
    {
        cutscenePanel.SetActive(false);
        transitionPanel.SetActive(true);
        pauseManager.SetPause(false);

        // Opcional: define cutscene como false para evitar que entre no Start novamente
        cutscene = false;
    }
}