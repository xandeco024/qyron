using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class LobbyManager : MonoBehaviour
{
    // Start is called before the first frame update

    private LobbyPlayer[] lobbyPlayers = new LobbyPlayer[4];
    private PlayerInput[] playerInputs;
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
        playerInputs = FindObjectsOfType<PlayerInput>();

        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (lobbyPlayers[i].linkedCharacter == null)
            {
                lobbyPlayers[i].linkedCharacter = playerInputs[i].gameObject.GetComponent<PlayableCharacter>();
            }
        }
    }

    public void LeavePlayer()
    {

    }
}
