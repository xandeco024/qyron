using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    // Start is called before the first frame update

    private int playerCount = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JoinPlayer()
    {

    }

    public void OnPlayerJoined(PlayerInputManager.PlayerJoinedEvent playerJoinedEvent)
    {
        playerCount++;
        Debug.Log("Player " + playerCount + "Joined");
    }

    public void OnPlayerLeft(PlayerInputManager.PlayerLeftEvent playerLeftEvent)
    {
        playerCount--;
        Debug.Log("Player " + playerCount + "Left");
    }
}
