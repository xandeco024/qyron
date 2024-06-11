using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    private bool gameOver = false;
    public bool GameOver { get => gameOver; }
    bool restarting = false;
    GameManager gameManager;
    LevelManager levelManager;
    LoadSceneManager sceneLoader;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject transitionPanel;
    [SerializeField] Button restartButton;
    private List<PlayableCharacter> playerList;
    [SerializeField] private int lifes;
    public int Lifes { get => lifes; set => lifes = value; }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        levelManager = FindObjectOfType<LevelManager>();
        sceneLoader = FindObjectOfType<LoadSceneManager>();
        playerList = gameManager.PlayerList;
        gameOverPanel.SetActive(false);
    }
    void Update()
    {
        //only game over if all players are downed

        if (!gameOver)
        {
            GameOverCheck();
        }
        else
        {
            HandleGameOver();
        }
    }

    void HandleGameOver()
    {
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
        restartButton.Select();
    }

    void GameOverCheck()
    {
        int downedPlayers = 0;

        foreach (PlayableCharacter player in playerList)
        {
            if (player.IsDowned)
            {
                downedPlayers++;
            }
        }

        if (downedPlayers == playerList.Count && lifes > 0)
        {            
            if (!restarting) StartCoroutine(RestartSegment());
        }
        else if (downedPlayers == playerList.Count && lifes <= 0)
        {
            gameOver = true;
        }
    }

    public void Restart()
    {
        foreach (PlayableCharacter player in playerList)
        {
            player.Reset();
        }
        Time.timeScale = 1;
        sceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator RestartSegment()
    {
        restarting = true;
        transitionPanel.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        lifes--;
        levelManager.GoToSegment(levelManager.CurrentSegment.Index, true, true);
        levelManager.CurrentSegment.Reset();
        yield return new WaitForSeconds(0.5f);
        
        transitionPanel.SetActive(false);
        restarting = false;
    }
}