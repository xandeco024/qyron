using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private GameManager gameManager;
    private List<PlayableCharacter> playerList = new List<PlayableCharacter>();

    [SerializeField] Vector3 offset;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerList = gameManager.PlayerList;
    }

    void Update()
    {
        // If there are no players in the scene, return.
        if (playerList.Count == 0)
        {
            return;
        }
        else
        {
            //middle position between all players
            Vector3 middlePosition = Vector3.zero;
            foreach (PlayableCharacter player in playerList)
            {
                middlePosition += player.transform.position;
            }
            middlePosition /= playerList.Count;

            //camera position
            transform.position = middlePosition + offset;
        }
    }
}
