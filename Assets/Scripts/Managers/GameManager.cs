using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState { Playing, Paused, GameOver }

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }

    #region Game State

    [Header("Game State")]
    [SerializeField] private GameState currentState = GameState.Playing;
    public GameState CurrentState => currentState;
    public bool IsPaused => currentState == GameState.Paused;
    public bool IsGameOver => currentState == GameState.GameOver;

    #endregion

    #region Players

    private List<PlayableCharacter> playerList = new List<PlayableCharacter>();
    public List<PlayableCharacter> PlayerList { get => playerList; }

    #endregion

    #region Time

    [Header("Time")]
    [SerializeField] private float secondDuration = 1f;
    [SerializeField] private int hours, minutes;
    public int Hours { get => hours; }
    public int Minutes { get => minutes; }

    #endregion

    #region Lives & Game Over

    [Header("Lives & Game Over")]
    [SerializeField] private int lifes = 3;
    public int Lifes { get => lifes; set => lifes = value; }
    private bool restarting = false;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private Button restartButton;

    #endregion

    #region Pause

    [Header("Pause UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;

    #endregion

    #region References

    private LevelManager levelManager;
    private LoadSceneManager sceneLoader;
    private MainInputManager mainInputManager;

    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Setup players from lobby
        SetupPlayersFromLobby();
    }

    void Start()
    {
        levelManager = LevelManager.Instance;
        sceneLoader = LoadSceneManager.Instance;
        mainInputManager = MainInputManager.Instance;

        // Setup input
        if (mainInputManager != null)
        {
            mainInputManager.InputMaster.UI.PauseResume.performed += OnPauseInput;
        }

        // Initialize level
        if (levelManager != null)
        {
            levelManager.GoToSegment(levelManager.FirstSegmentIndex, false, false);
        }

        // Initialize UI
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        // Start time
        InvokeRepeating(nameof(HandleTime), 0, secondDuration);
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            CheckGameOver();
        }
    }

    void OnDestroy()
    {
        // Cleanup input subscription
        if (mainInputManager != null && mainInputManager.InputMaster != null)
        {
            mainInputManager.InputMaster.UI.PauseResume.performed -= OnPauseInput;
        }
    }

    #endregion

    #region Player Setup

    private void SetupPlayersFromLobby()
    {
        // O nome do player que vem do lobby é Player(Clone), então se ele existir, se já tiver um player na cena, ele deve ser destruido.
        PlayableCharacter[] foundPlayers = FindObjectsOfType<PlayableCharacter>();

        bool destroyExistingPlayers = false;

        foreach (PlayableCharacter player in foundPlayers)
        {
            if (player.gameObject.name == "Player(Clone)")
            {
                destroyExistingPlayers = true;
                break;
            }
        }

        if (destroyExistingPlayers)
        {
            foreach (PlayableCharacter player in foundPlayers)
            {
                if (player.gameObject.name == "Player")
                {
                    Destroy(player.gameObject);
                }
                else
                {
                    playerList.Add(player);
                }
            }
        }
        else
        {
            playerList.AddRange(foundPlayers);
        }
    }

    public void DestroyAllPlayers()
    {
        playerList.Clear();
        PlayableCharacter[] unlucklyFoundPlayers = FindObjectsOfType<PlayableCharacter>();

        foreach (PlayableCharacter player in unlucklyFoundPlayers)
        {
            Destroy(player.gameObject);
        }
    }

    #endregion

    #region Time System

    private void HandleTime()
    {
        if (currentState != GameState.Playing) return;

        minutes++;
        if (minutes >= 60)
        {
            hours++;
            minutes = 0;
        }

        if (hours >= 24)
        {
            hours = 0;
        }
    }

    public void SetTime(int hours, int minutes)
    {
        this.hours = hours;
        this.minutes = minutes;
    }

    #endregion

    #region Pause System

    private void OnPauseInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (currentState == GameState.GameOver) return;

        if (currentState == GameState.Paused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (currentState == GameState.GameOver) return;

        currentState = GameState.Paused;
        Time.timeScale = 0;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            if (resumeButton != null) resumeButton.Select();
        }
    }

    public void Resume()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    #endregion

    #region Game Over System

    private void CheckGameOver()
    {
        int downedPlayers = 0;

        foreach (PlayableCharacter player in playerList)
        {
            if (player.IsDowned)
            {
                downedPlayers++;
            }
        }

        if (downedPlayers == playerList.Count && playerList.Count > 0)
        {
            if (lifes > 0)
            {
                if (!restarting) StartCoroutine(RestartSegment());
            }
            else
            {
                TriggerGameOver();
            }
        }
    }

    private void TriggerGameOver()
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (restartButton != null) restartButton.Select();
        }
    }

    public void Restart()
    {
        foreach (PlayableCharacter player in playerList)
        {
            player.Reset();
        }

        currentState = GameState.Playing;
        Time.timeScale = 1;

        if (sceneLoader != null)
        {
            sceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private IEnumerator RestartSegment()
    {
        restarting = true;

        if (transitionPanel != null) transitionPanel.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        lifes--;

        if (levelManager != null)
        {
            levelManager.GoToSegment(levelManager.CurrentSegment.Index, true, true);
            levelManager.CurrentSegment.Reset();
        }

        yield return new WaitForSeconds(0.5f);

        if (transitionPanel != null) transitionPanel.SetActive(false);

        restarting = false;
    }

    #endregion

    #region Backwards Compatibility (para não quebrar código existente)

    // Mantém compatibilidade com código que usava GameOverManager.Instance.GameOver
    public bool GameOver => IsGameOver;

    #endregion
}
