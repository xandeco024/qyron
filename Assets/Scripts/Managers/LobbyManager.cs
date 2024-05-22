using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    // Start is called before the first frame update

    private LobbyPlayer[] lobbyPlayers = new LobbyPlayer[4];
    private PlayerInput[] playerInputs = new PlayerInput[4];

    private PlayerInputManager playerInputManager;

    void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();

        for (int i = 0; i < 4; i++)
        {
            lobbyPlayers[i] = GameObject.Find("Player Frame 2 " + i).GetComponent<LobbyPlayer>();
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void JoinPlayer()
    {
        //search for gameobjects called playerInputOBJ

        for (int i = 0; i < playerInputs.Length; i++)
        {
            if(playerInputs[i] == null)
            {
                playerInputs[i] = GameObject.Find("teste(Clone)").GetComponent<PlayerInput>();
                break;
            }
        }

        /*for(int i = 0; i < foundPlayerInputs.Length; i++)
        {
            if (playerInputs[i] != foundPlayerInputs[i])
            {
                playerInputs[i] = foundPlayerInputs[i];
                break;
            }
        }

        Debug.Log("Join Player");
        for (int i = 0; i < 4; i++)
        {
            if (!lobbyPlayers[i].GetComponent<PlayerInput>())
            {
                lobbyPlayers[i].JoinPlayer(playerInputs[playerInputs.Count - 1]);
            }
        }*/

        lobbyPlayers[playerInputs.Length - 1].JoinPlayer(playerInputs[playerInputs.Length - 1]);
    }

    public void LeavePlayer()
    {

    }
}
