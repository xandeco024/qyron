using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float minFov, maxFov;

    private float currentFov;
    public float CurrentFov { get { return currentFov; } }

    float cameraHalfWidth;
    float cameraHalfHeight;

    float cameraRightBound;
    float cameraLeftBound;
    float cameraTopBound;
    float cameraBottomBound;

    float cameraTopLimit;
    float cameraBottomLimit;
    float cameraLeftLimit;
    float cameraRightLimit;

    private GameManager gameManager;
    private List<PlayableCharacter> playerList = new List<PlayableCharacter>();
    private PlayableCharacter extremeLeftPlayer, extremeRightPlayer;

    [SerializeField] Vector3 singlePlayerOffset;
    [SerializeField] Vector3 multiplePlayersOffset;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerList = gameManager.PlayerList;

        mainCamera = Camera.main;
        cameraHalfHeight = mainCamera.orthographicSize;
        cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;

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
            AdjustCameraForMultiplePlayers();
        }
        else if (playerList.Count == 1)
        {
            AdjustCameraForSinglePlayer();
        }

        LimitCamera();
    }

    private void AdjustCameraForMultiplePlayers()
    {
        Vector3 middlePosition = GetMiddlePosition();
        transform.position = middlePosition + multiplePlayersOffset;

        float averageDistance = CalculateAverageDistance();
        UpdateExtremePlayers();

        AdjustFovAndRestrictions(averageDistance);
    }

    private Vector3 GetMiddlePosition()
    {
        Vector3 middlePosition = Vector3.zero;
        foreach (PlayableCharacter player in playerList)
        {
            if (player != null) middlePosition += player.transform.position;
        }
        return middlePosition / playerList.Count;
    }

    private float CalculateAverageDistance()
    {
        float totalDistance = 0;
        int count = playerList.Count;

        for (int i = 0; i < count; i++)
        {
            for (int j = i + 1; j < count; j++)
            {
                totalDistance += Vector3.Distance(playerList[i].transform.position, playerList[j].transform.position);
            }
        }
        return totalDistance / (count * (count - 1) / 2);
    }

    private void UpdateExtremePlayers()
    {
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
    }

    private void AdjustFovAndRestrictions(float averageDistance)
    {
        float scaleFactor = 0.1f; // Ajuste conforme necessário

        currentFov = Mathf.Clamp(minFov + averageDistance * scaleFactor, minFov, maxFov);
        virtualCamera.m_Lens.FieldOfView = currentFov;

        if (currentFov >= maxFov)
        {
            extremeLeftPlayer.SetMovementRestrictions(new List<string> { "left" });
            extremeRightPlayer.SetMovementRestrictions(new List<string> { "right" });
        }
        else
        {
            RemoveMovementRestrictions(extremeLeftPlayer, "left");
            RemoveMovementRestrictions(extremeRightPlayer, "right");
        }
    }

    private void RemoveMovementRestrictions(PlayableCharacter player, string direction)
    {
        if (player.MovementRestrictions.Contains(direction))
        {
            player.RemoveMovementRestriction(direction);
        }
    }

    private void AdjustCameraForSinglePlayer()
    {
        float xOffset = singlePlayerOffset.x * playerList[0].FacingDirection;
        transform.position = playerList[0].transform.position + new Vector3(xOffset, singlePlayerOffset.y, singlePlayerOffset.z);
    }

    private void UpdateCameraSizeAndBounds()
    {
        cameraHalfHeight = mainCamera.orthographicSize;
        cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;

        cameraRightBound = transform.position.x + cameraHalfWidth;
        cameraLeftBound = transform.position.x - cameraHalfWidth;
        cameraTopBound = transform.position.y + cameraHalfHeight;
        cameraBottomBound = transform.position.y - cameraHalfHeight;
    }

    private void LimitCamera()
    {
        UpdateCameraSizeAndBounds();

        if (cameraRightBound > cameraRightLimit)
        {
            transform.position = new Vector3(cameraRightLimit - cameraHalfWidth, transform.position.y, transform.position.z);
            Debug.Log("Passou o limite direita");
        }
        else if (cameraLeftBound < cameraLeftLimit)
        {
            transform.position = new Vector3(cameraLeftLimit + cameraHalfWidth, transform.position.y, transform.position.z);
            Debug.Log("Passou o limite esquerda");
        }
    }

    public void SetCameraLimits(float left, float right, float top, float bottom)
    {
        cameraLeftLimit = left;
        cameraRightLimit = right;
        cameraTopLimit = top;
        cameraBottomLimit = bottom;
    }
}
