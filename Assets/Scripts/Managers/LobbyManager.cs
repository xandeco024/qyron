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

    private LobbyPlayer[] lobbyPlayers;
    private GameObject[] playerFrames = new GameObject[4];

    private PlayerInputManager playerInputManager;

    void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        for (int i = 0; i < 4; i++)
        {
            playerFrames[i] = GameObject.Find("Player Frame 2 " + i);
        }
    }


    // Update is called once per frame
    void Update()
    {
        HandlePlayerFrames();
    }

    void HandlePlayerFrames()
    {
        if (lobbyPlayers != null)
        {
            for (int i = 0; i < lobbyPlayers.Length; i++)
            {
                playerFrames[i].GetComponent<Animator>().SetBool("empty" , false);

                int charIndex;

                switch (lobbyPlayers[i].SelectedCharacterName)
                {
                    case "Qyron":
                        charIndex = 0;
                        break;
                    case "Qyana":
                        charIndex = 1;
                        break;
                    case "Meowcello":
                        charIndex = 2;
                        break;
                    case "Gark":
                        charIndex = 3;
                        break;
                    default:
                        charIndex = 0;
                        break;
                }

                playerFrames[i].GetComponent<Animator>().SetFloat("blend" , charIndex);

            }
        }
    
    }

    public void JoinPlayer()
    {
        lobbyPlayers = FindObjectsOfType<LobbyPlayer>();
        //invert the order of the players
        lobbyPlayers = lobbyPlayers.Reverse().ToArray();
    }

    public void LeavePlayer()
    {

    }
}
