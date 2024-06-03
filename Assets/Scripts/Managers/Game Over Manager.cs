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
    LoadSceneManager sceneLoader;
    [SerializeField] GameObject gameOverCanvasObject;
    [SerializeField] Button restartButton;
    private List<PlayableCharacter> playerList;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
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

        if (downedPlayers == playerList.Count)
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