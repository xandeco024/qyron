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
    GameManager gameManager;
    LevelManager levelManager;
    LoadSceneManager sceneLoader;
    [SerializeField] GameObject gameOverCanvasObject;
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
        gameOverCanvasObject.SetActive(false);
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
        gameOverCanvasObject.SetActive(true);
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
            if (levelManager.CurrentSegment.Index > 0)
            {
            }
            
            lifes--;
            levelManager.GoToSegment(levelManager.CurrentSegment.Index, true, true);
            levelManager.CurrentSegment.Reset();
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
}