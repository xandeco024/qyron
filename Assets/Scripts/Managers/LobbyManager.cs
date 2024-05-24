using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;

public class LobbyManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private LoadSceneManager loadSceneManager;
    private LobbyPlayer[] lobbyPlayers;
    private GameObject[] playerFramesObject = new GameObject[4];
    private GameObject[] joinObject = new GameObject[4];
    [SerializeField] private TextMeshProUGUI readyPlayersText;
    private int readyPlayers;
    private bool countdownStarted = false;

    private PlayerInputManager playerInputManager;

    void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        for (int i = 0; i < 4; i++)
        {
            playerFramesObject[i] = GameObject.Find("Player Frame 2 " + i);
            //joinObject[i] = playerFramesObject[i].GetComponentInChildren<TextMeshProUGUI>().gameObject;
        }
    }


    // Update is called once per frame
    void Update()
    {
        //HandlePlayerFrames();
    }

    void OnEnable()
    {
        if (lobbyPlayers != null)
        {
            for (int i = 0; i < lobbyPlayers.Length; i++)
            {
                Destroy(lobbyPlayers[i].gameObject);
            }
        }

        if(playerInputManager != null)
        {
            playerInputManager.EnableJoining();
        }
    }

    void HandlePlayerFrames()
    {
        readyPlayers = 0;

        if (lobbyPlayers != null)
        {
            for (int i = 0; i < lobbyPlayers.Length; i++)
            {
                if (i == 0)
                {
                    InputSystemUIInputModule inputSysUI = GameObject.FindObjectOfType<InputSystemUIInputModule>();
                    if(inputSysUI != null)
                    {
                        lobbyPlayers[i].GetComponent<PlayerInput>().uiInputModule = inputSysUI;
                    }
                }

                playerFramesObject[i].GetComponent<Animator>().SetBool("empty" , false);
                joinObject[i].SetActive(false);



                if (lobbyPlayers[i].Ready)
                {
                    readyPlayers++;
                }

            }

            if (readyPlayers == lobbyPlayers.Length && !countdownStarted)
            {
                StartCoroutine(StartGame());
            }
            else if (readyPlayers != lobbyPlayers.Length)
            {
                if (countdownStarted)
                {
                    StopCoroutine(StartGame());
                    countdownStarted = false;
                }
                readyPlayersText.text = readyPlayers + " / " + lobbyPlayers.Length + " Players Prontos!";
            }
        }
    }

    IEnumerator StartGame()
    {
        countdownStarted = true;
        readyPlayersText.text = "Iniciando em 3";
        yield return new WaitForSeconds(1);
        readyPlayersText.text = "Iniciando em 2";
        yield return new WaitForSeconds(1);
        readyPlayersText.text = "Iniciando em 1";
        yield return new WaitForSeconds(1);
        playerInputManager.DisableJoining();

        for (int i = 0; i < lobbyPlayers.Length; i++)
        {
            lobbyPlayers[i].EnablePlayerActionMap();
        }
        
        loadSceneManager.LoadScene(1);
    }

    public void JoinPlayer()
    {
        lobbyPlayers = FindObjectsOfType<LobbyPlayer>();
        //invert the order of the players pq senao ele troca o primeiro index pelo segundo e afins.
        lobbyPlayers = lobbyPlayers.Reverse().ToArray();

        for (int i = 0; i < lobbyPlayers.Length; i++)
        {
            lobbyPlayers[i].SetPlayerFrame(playerFramesObject[i]);
        }

        /*LobbyPlayer[] foundLobbyPlayers = FindObjectsOfType<LobbyPlayer>();

        for (int i = 0; i < foundLobbyPlayers.Length; i++)
        {
            if (!lobbyPlayers.Contains(foundLobbyPlayers[i]))
            {
                for (int j = 0; j < lobbyPlayers.Length; j++)
                {
                    if (lobbyPlayers[j] == null)
                    {
                        lobbyPlayers[j] = foundLobbyPlayers[i];
                        break;
                    }
                }
            }
        }*/


    }

    public void LeavePlayer()
    {

    }
}
