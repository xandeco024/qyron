using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

class LobbyPlayer
{
    public PlayableCharacter character;
    public InputDevice device;

    public LobbyPlayer(PlayableCharacter character, InputDevice device)
    {
        this.character = character;
        this.device = device;
    }
}

public class LobbyManager : MonoBehaviour
{
    // Start is called before the first frame update

    private int playerCount = 0;
    [SerializeField] PlayableCharacter[] playableCharacterPrefabs;
    string[] selectedCharacters = new string[4];
    private PlayerInputManager playerInputManager;

    private LobbyPlayer[] lobbyPlayers = {null, null, null, null};

    void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void JoinPlayer()
    {
        if (playerCount < 4)
        {
            lobbyPlayers[playerCount] = new LobbyPlayer(playableCharacterPrefabs[playerCount], null);
        }
        else
        {
            Debug.Log("Lobby is full.");
        }
    }

    public void LeavePlayer(int playerIndex)
    {
        if (lobbyPlayers[playerIndex] != null)
        {
            lobbyPlayers[playerIndex] = null;
            playerCount--;
        }
    }
}
