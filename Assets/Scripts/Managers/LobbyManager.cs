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
    private List<LobbyPlayer> lobbyPlayersList = new List<LobbyPlayer>();
    int readyPlayers;
    private List<string> lockedCharacterNamesList = new List<string>();
    public List<string> LockedCharacterNamesList { get => lockedCharacterNamesList; }
    private GameObject[] playerFrameObjects = new GameObject[4];
    private GameObject[] characterObjects = new GameObject[4];
    [SerializeField] private Animator readyPlayersAnimator;
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
        //destroy all players to create new ones.

        PlayableCharacter[] players = FindObjectsOfType<PlayableCharacter>();

        if (players != null)
        {
            Debug.Log("Destruidos " + players.Length + " jogadores");

            for (int i = 0; i < players.Length; i++)
            {
                Destroy(players[i].gameObject);
            }
        }

        if(playerInputManager != null)
        {
            playerInputManager.EnableJoining();
        }

        playerFrameObjects[0].GetComponent<Button>().Select();

        for (int i = 0; i < playerFrameObjects.Length; i++)
        {
            playerFrameObjects[i].GetComponent<Animator>().SetBool("empty", true);
            characterObjects[i].SetActive(false);
        }

        lockedCharacterNamesList.Clear();
        lobbyPlayersList.Clear();

        readyPlayersAnimator.SetFloat("readyPlayers", 0);
        readyPlayersAnimator.SetFloat("lobbyPlayers", 0);
        readyPlayersAnimator.SetBool("starting", false);
    }

    public void ToggleLockedCharacter(string characterName, bool locked)
    {
        if (locked)
        {
            lockedCharacterNamesList.Add(characterName);
        }

        else
        {
            lockedCharacterNamesList.Remove(characterName);
        }
    }

    void HandleLobby() // handle ready and locked players
    {
        if (lobbyPlayersList.Count > 0)
        {
            readyPlayers = 0;

            foreach (LobbyPlayer lobbyPlayer in lobbyPlayersList)
            {
                if (lobbyPlayer.Ready)
                {
                    readyPlayers++;
                }
            }

            if (readyPlayers == lobbyPlayersList.Count && !countdownStarted)
            {
                readyPlayersAnimator.SetBool("starting", true);
                startGameCoroutine = StartCoroutine(StartGame());
            }

            else if (readyPlayers < lobbyPlayersList.Count && countdownStarted)
            {
                countdownStarted = false;
                readyPlayersAnimator.SetBool("starting", false);
                StopCoroutine(startGameCoroutine);
            }

            else if (readyPlayers < lobbyPlayersList.Count)
            {
                readyPlayersAnimator.SetFloat("readyPlayers", readyPlayers);
                readyPlayersAnimator.SetFloat("lobbyPlayers", lobbyPlayersList.Count);
            }
        }

        else if (countdownStarted)
        {
            countdownStarted = false;
            readyPlayersAnimator.SetBool("starting", false);
            StopCoroutine(startGameCoroutine);
        }
    }

    IEnumerator StartGame()
    {
        countdownStarted = true;
        yield return new WaitForSeconds(1);
        yield return new WaitForSeconds(1);
        yield return new WaitForSeconds(1);
        playerInputManager.DisableJoining();

        foreach (LobbyPlayer lobbyPlayer in lobbyPlayersList)
        {
            lobbyPlayer.EnablePlayerActionMap();
            lobbyPlayer.GetComponent<PlayableCharacter>().SetupCharacter(lobbyPlayer.SelectedCharacterName);
        }
        
        loadSceneManager.LoadScene(1);
    }

    public void JoinPlayer(PlayerInput lobbyPlayerInput)
    {
        LobbyPlayer joinedPlayer = lobbyPlayerInput.GetComponent<LobbyPlayer>();
        lobbyPlayersList.Add(joinedPlayer);
        joinedPlayer.SetPlayerFrame(playerFrameObjects[lobbyPlayersList.IndexOf(joinedPlayer)]);
    }

    public void LeavePlayer(PlayerInput lobbyPlayerInput)
    {
        LobbyPlayer leftPlayer = lobbyPlayerInput.GetComponent<LobbyPlayer>();

        if (leftPlayer.Ready)
        {
            readyPlayers--;
            lockedCharacterNamesList.Remove(leftPlayer.SelectedCharacterName);
        }

        lobbyPlayersList.Remove(leftPlayer);
    }
}
