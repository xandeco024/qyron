using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<Segment> segments;
    private Segment currentSegment;
    public Segment CurrentSegment { get { return currentSegment; } }

    private CameraManager cameraManager;
    [SerializeField] float segmentBorderCamOffset;

    private GameManager gameManager;
    private List<PlayableCharacter> playerList;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerList = gameManager.PlayerList;

        cameraManager = FindObjectOfType<CameraManager>();

        segments = new List<Segment>(FindObjectsOfType<Segment>());
        //order the segments by their index
        segments.Sort((x, y) => x.Index.CompareTo(y.Index));

        currentSegment = segments[0];

        cameraManager.SetCameraLimits(segments[0].transform.position.x - currentSegment.Size.x / 2 + segmentBorderCamOffset, currentSegment.transform.position.x + currentSegment.Size.x / 2 - segmentBorderCamOffset, currentSegment.transform.position.z - currentSegment.Size.z / 2, currentSegment.transform.position.z + currentSegment.Size.z / 2);
    }

    // Update is called once per frame
    void Update()
    {
        LimitPlayersOnSegment();
        HandleSegmentCompletion();
    }

    void LimitPlayersOnSegment()
    {
        // O SEGMENTO ATUAL e o segmento MAXIMO DESBLOQUEADO são coisas diferentes.

        foreach (PlayableCharacter player in playerList)
        {
            //limit x ele não pode de maneira alguma avançar no X, e sempre vai ser assim.
            if (player.transform.position.x > currentSegment.transform.position.x + currentSegment.Size.x / 2)
            {
                player.transform.position = new Vector3(currentSegment.transform.position.x + currentSegment.Size.x / 2, player.transform.position.y, player.transform.position.z);
            }
            // para que ele possa voltar para o segmento anterior, menos no 0
            else if (currentSegment.Index == 0 && player.transform.position.x < currentSegment.transform.position.x - currentSegment.Size.x / 2)
            {
                player.transform.position = new Vector3(currentSegment.transform.position.x - currentSegment.Size.x / 2, player.transform.position.y, player.transform.position.z);
            }

            //limit z as vezes, o segmento atual é mais esguio que o anterior
            if (player.transform.position.z > currentSegment.transform.position.z + currentSegment.Size.z / 2)
            {
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, currentSegment.transform.position.z + currentSegment.Size.z / 2);
            }

            else if (player.transform.position.z < currentSegment.transform.position.z - currentSegment.Size.z / 2)
            {
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, currentSegment.transform.position.z - currentSegment.Size.z / 2);
            }
        }
    }

    void HandleSegmentCompletion()
    {
        if (currentSegment.Complete)
        {
            NextSegment();
        }
    }

    public void NextSegment()
    {
        int nextIndex = currentSegment.Index + 1;

        currentSegment = segments[nextIndex];
        cameraManager.SetCameraLimits(segments[0].transform.position.x - currentSegment.Size.x / 2 + segmentBorderCamOffset, currentSegment.transform.position.x + currentSegment.Size.x / 2 - segmentBorderCamOffset, currentSegment.transform.position.z - currentSegment.Size.z / 2, currentSegment.transform.position.z + currentSegment.Size.z / 2);
    }
}
