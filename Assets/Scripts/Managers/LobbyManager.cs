using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private MenuManager menuManager;

    private InputMaster inputMaster;

    void Awake()
    {
        menuManager = FindObjectOfType<MenuManager>();
        inputMaster = new InputMaster();
        //inputMaster.UI.Cancel.performed += ctx => BackToMainMenu();

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
        /*
        int notNullPlayersAmount = lobbyPlayersList.Where(player => player != null).ToList().Count;

        if (notNullPlayersAmount == 0)
        {
            menuManager.BackToMainMenu(menuManager.LobbyGameObject);
        }*/
        inputMaster.Enable();
    }

    void OnDisable()
    {
        inputMaster.Disable();
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
        int notNullPlayersAmount = lobbyPlayersList.Where(player => player != null).ToList().Count;
        readyPlayers = 0;

        if (notNullPlayersAmount > 0)
        {
            foreach (LobbyPlayer lobbyPlayer in lobbyPlayersList)
            {
                if (lobbyPlayer != null && lobbyPlayer.Ready)
                {
                    readyPlayers++;
                }
            }

            if (readyPlayers == notNullPlayersAmount && !countdownStarted)
            {
                readyPlayersAnimator.SetBool("starting", true);
                startGameCoroutine = StartCoroutine(StartGame());
            }

            else if (readyPlayers < notNullPlayersAmount && countdownStarted)
            {
                countdownStarted = false;
                readyPlayersAnimator.SetBool("starting", false);
                StopCoroutine(startGameCoroutine);
            }
        }

        else if (countdownStarted)
        {
            countdownStarted = false;
            readyPlayersAnimator.SetBool("starting", false);
            StopCoroutine(startGameCoroutine);
        }

        readyPlayersAnimator.SetFloat("readyPlayers", readyPlayers);
        readyPlayersAnimator.SetFloat("lobbyPlayers", notNullPlayersAmount);
    }

    IEnumerator StartGame()
    {
        countdownStarted = true;
        yield return new WaitForSeconds(1);
        yield return new WaitForSeconds(1);
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

        int emptyIndex = lobbyPlayersList.FindIndex(player => player == null);

        if (emptyIndex != -1)
        {
            // Adicione o novo jogador no índice vazio
            lobbyPlayersList[emptyIndex] = joinedPlayer;
            joinedPlayer.SetPlayerFrame(playerFrameObjects[emptyIndex]);
        }
        else
        {
            // Adicione o novo jogador no final da lista
            lobbyPlayersList.Add(joinedPlayer);
            joinedPlayer.SetPlayerFrame(playerFrameObjects[lobbyPlayersList.Count - 1]);
        }
    }

    public void LeavePlayer(PlayerInput lobbyPlayerInput)
    {
        LobbyPlayer leftPlayer = lobbyPlayerInput.GetComponent<LobbyPlayer>();

        if (leftPlayer.Ready)
        {
            lockedCharacterNamesList.Remove(leftPlayer.SelectedCharacterName);
        }

        foreach (LobbyPlayer player in lobbyPlayersList)
        {
            if (player != null)
            {
                if(player.Ready)
                {
                    player.UnsetReady();
                }
            }
        }

        if (countdownStarted)
        {
            countdownStarted = false;
            readyPlayersAnimator.SetBool("starting", false);
            StopCoroutine(startGameCoroutine);
        }

        int playerIndex = lobbyPlayersList.IndexOf(leftPlayer);

        if (playerIndex != -1)
        {
            lobbyPlayersList[playerIndex] = null;
        }
    }
}
