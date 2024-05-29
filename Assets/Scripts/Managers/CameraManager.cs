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

    [SerializeField] Vector3 offset;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerList = gameManager.PlayerList;
        extremeLeftPlayer = playerList[0];
        extremeRightPlayer = playerList[0];
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
                if (player != null) middlePosition += player.transform.position;
            }
            middlePosition /= playerList.Count;

            //camera position
            transform.position = middlePosition + offset;
        }

        if (playerList.Count > 1)
        {
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
        }

        // Limitar o FOV
        virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(virtualCamera.m_Lens.FieldOfView, minFov, maxFov);

        if (virtualCamera.m_Lens.FieldOfView == maxFov)
        {
            extremeLeftPlayer.SetMovementRestrictions(new List<string> { "left" });
            extremeRightPlayer.SetMovementRestrictions(new List<string> { "right" });
        }

        else 
        {
            extremeLeftPlayer.RemoveMovementRestriction("left");
            extremeRightPlayer.RemoveMovementRestriction("right");
        }
    }
}