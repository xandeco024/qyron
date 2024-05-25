using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private LoadSceneManager loadSceneManager;
    private LobbyPlayer[] lobbyPlayers;
    private List<string> lockedCharacterNames = new List<string>();
    public List<string> LockedCharacterNames { get => lockedCharacterNames; }
    private GameObject[] playerFrameObjects = new GameObject[4];
    private GameObject[] characterObjects = new GameObject[4];
    [SerializeField] private TextMeshProUGUI readyPlayersText;
    private int readyPlayers;
    private bool countdownStarted = false;

    Coroutine startGameCoroutine;

    private PlayerInputManager playerInputManager;

    void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        for (int i = 0; i < 4; i++)
        {
            playerFrameObjects[i] = GameObject.Find("Player Frame 2 " + i);
            characterObjects[i] = playerFrameObjects[i].transform.GetChild(0).gameObject;
        }
    }

    void OnEnable()
    {
        ResetLobby();
    }


    void Start()
    {

    }

    void Update()
    {
        HandleLobby();
    }

    void ResetLobby()
    {
        PlayableCharacter[] players = FindObjectsOfType<PlayableCharacter>();

        if (players != null)
        {
            Debug.Log("Destruidos " + players.Length + " jogadores");

            for (int i = 0; i < players.Length; i++)
            {
                Destroy(players[i].gameObject);
            }
        }

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

        for (int i = 0; i < playerFrameObjects.Length; i++)
        {
            if (i == 0) playerFrameObjects[i].GetComponent<Button>().Select();

            playerFrameObjects[i].GetComponent<Animator>().SetBool("empty", true);
            characterObjects[i].SetActive(false);
        }

        lockedCharacterNames.Clear();

        readyPlayersText.text = "Aguardando Jogadores!";
    }

    void HandleLobby()
    {
        if (lobbyPlayers != null)
        {
            readyPlayers = 0;

            lockedCharacterNames.Clear();

            for (int i = 0; i < lobbyPlayers.Length; i++)
            {
                if (lobbyPlayers[i].Ready)
                {
                    lockedCharacterNames.Add(lobbyPlayers[i].SelectedCharacterName);
                }
            }

            for (int i = 0; i < lobbyPlayers.Length; i++)
            {
                if (lobbyPlayers[i].Ready)
                {
                    readyPlayers++;
                }
            }

            if (readyPlayers == lobbyPlayers.Length && !countdownStarted)
            {
                startGameCoroutine = StartCoroutine(StartGame());
            }

            else if (readyPlayers < lobbyPlayers.Length && countdownStarted)
            {
                Debug.Log("PAROU");
                countdownStarted = false;
                StopCoroutine(startGameCoroutine);
            }

            else if (readyPlayers < lobbyPlayers.Length)
            {
                readyPlayersText.text = readyPlayers + " / " + lobbyPlayers.Length + " Jogadores Prontos!";
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
            lobbyPlayers[i].SetPlayerFrame(playerFrameObjects[i]);
        }
    }

    public void LeavePlayer()
    {

    }
}
