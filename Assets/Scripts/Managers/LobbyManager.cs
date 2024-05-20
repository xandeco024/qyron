using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void OnPlayerJoined()
    {
        playerCount++;
        Debug.Log("Player " + playerCount + "Joined");
    }

    public void OnPlayerLeft()
    {
        playerCount--;
        Debug.Log("Player " + playerCount + "Left");
    }
}
