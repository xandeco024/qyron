using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }

    [SerializeField] bool cutscene;
    [SerializeField] GameObject cutscenePanel;
    [SerializeField] GameObject transitionPanel;
    [SerializeField] GameObject skipPrompt; // UI mostrando "Pressione X para pular"

    private MainInputManager mainInputManager;
    private bool cutsceneActive = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        mainInputManager = MainInputManager.Instance;

        if (cutscene)
        {
            StartCutscene();
        }
        else
        {
            cutscenePanel.SetActive(false);
            if (skipPrompt != null) skipPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (cutsceneActive)
        {
            // Detecta qualquer botão para pular (Espaço, Enter, ou botão do controle)
            if (Keyboard.current != null &&
                (Keyboard.current.spaceKey.wasPressedThisFrame ||
                 Keyboard.current.enterKey.wasPressedThisFrame ||
                 Keyboard.current.escapeKey.wasPressedThisFrame))
            {
                SkipCutscene();
            }

            if (Gamepad.current != null &&
                (Gamepad.current.buttonSouth.wasPressedThisFrame || // A/Cross
                 Gamepad.current.startButton.wasPressedThisFrame))
            {
                SkipCutscene();
            }
        }
    }

    private void StartCutscene()
    {
        cutsceneActive = true;
        GameManager.Instance.Pause();
        cutscenePanel.SetActive(true);
        if (skipPrompt != null) skipPrompt.SetActive(true);
    }

    public void SkipCutscene()
    {
        if (!cutsceneActive) return;

        EndCutscene();
    }

    public void EndCutscene()
    {
        cutsceneActive = false;
        cutscenePanel.SetActive(false);
        if (skipPrompt != null) skipPrompt.SetActive(false);
        transitionPanel.SetActive(true);
        GameManager.Instance.Resume();
    }
}
