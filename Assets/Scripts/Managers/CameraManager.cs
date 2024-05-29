using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float minFov, maxFov;
    private GameManager gameManager;
    private List<PlayableCharacter> playerList = new List<PlayableCharacter>();
    private PlayableCharacter extremeLeftPlayer, extremeRightPlayer;

    [SerializeField] Vector3 singlePlayerOffset;
    [SerializeField] Vector3 multiplePlayersOffset;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerList = gameManager.PlayerList;

        virtualCamera.m_Lens.FieldOfView = minFov;

        if (playerList.Count > 1)
        {
            extremeLeftPlayer = playerList[0];
            extremeRightPlayer = playerList[0];
        }
    }

    void Update()
    {
        if (playerList.Count > 1)
        {
            Vector3 middlePosition = Vector3.zero;
            foreach (PlayableCharacter player in playerList)
            {
                if (player != null) middlePosition += player.transform.position;
            }
            middlePosition /= playerList.Count;

            //camera position
            transform.position = middlePosition + multiplePlayersOffset;

            // Calcular a média das distâncias
            float totalDistance = 0;
            foreach (PlayableCharacter player in playerList)
            {
                foreach (PlayableCharacter otherPlayer in playerList)
                {
                    totalDistance += Vector3.Distance(player.transform.position, otherPlayer.transform.position);
                }
            }
            float averageDistance = totalDistance / (playerList.Count * (playerList.Count - 1));

            foreach (PlayableCharacter player in playerList)
            {
                if (player.transform.position.x < extremeLeftPlayer.transform.position.x)
                {
                    extremeLeftPlayer = player;
                }
                if (player.transform.position.x > extremeRightPlayer.transform.position.x)
                {
                    extremeRightPlayer = player;
                }
            }

            // Ajustar o FOV proporcionalmente
            float scaleFactor = 0.1f; // Ajuste conforme necessário
            virtualCamera.m_Lens.FieldOfView = minFov + scaleFactor * averageDistance;
            Debug.Log(virtualCamera.m_Lens.FieldOfView);

            if (virtualCamera.m_Lens.FieldOfView >= maxFov)
            {
                extremeLeftPlayer.SetMovementRestrictions(new List<string> { "left" });
                extremeRightPlayer.SetMovementRestrictions(new List<string> { "right" });
            }

            else 
            {
                if (extremeLeftPlayer.MovementRestrictions.Contains("left"))
                {
                    extremeLeftPlayer.RemoveMovementRestriction("left");
                }

                if (extremeRightPlayer.MovementRestrictions.Contains("right"))
                {
                    extremeRightPlayer.RemoveMovementRestriction("right");
                }
            }

            virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(virtualCamera.m_Lens.FieldOfView, minFov, maxFov);   
        }

        else if (playerList.Count == 1) 
        {
            transform.position = playerList[0].transform.position + new Vector3(singlePlayerOffset.x * playerList[0].transform.rotation.y == 0 ? 1 : -1, singlePlayerOffset.y, singlePlayerOffset.z);
        }
    }
}