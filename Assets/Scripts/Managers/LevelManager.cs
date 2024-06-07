using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

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
        //LimitPlayersOnSegments();
        RestrictPlayersToSegment();
        HandleSegmentCompletion();
    }

    /*void LimitPlayersOnSegments()
    {
        // O SEGMENTO ATUAL e o segmento MAXIMO DESBLOQUEADO são coisas diferentes.

        foreach (Segment segment in segments)
        {
            if (segment.PlayersOnSegment.Count > 0)
            {
                foreach (PlayableCharacter player in segment.PlayersOnSegment)
                {
                    if (segment == currentSegment)
                    {
                        if (player.transform.position.x > segment.transform.position.x + segment.Size.x / 2)
                        {
                            player.transform.position = new Vector3(segment.transform.position.x + segment.Size.x / 2, player.transform.position.y, player.transform.position.z);
                        }
                    }
                    
                    //bloqueia no segmento 0 pra esquerda.
                    else if (segment.Index == 0 && player.transform.position.x < segment.transform.position.x - segment.Size.x / 2)
                    {
                        player.transform.position = new Vector3(segment.transform.position.x - segment.Size.x / 2, player.transform.position.y, player.transform.position.z);
                    }

                    if (player.transform.position.z > segment.transform.position.z + segment.Size.z / 2)
                    {
                        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, segment.transform.position.z + segment.Size.z / 2);
                    }

                    else if (player.transform.position.z < segment.transform.position.z - segment.Size.z / 2)
                    {
                        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, segment.transform.position.z - segment.Size.z / 2);
                    }
                }
            }
        }
    }*/

    void RestrictPlayersToSegment()
    {
        foreach (PlayableCharacter player in playerList)
        {
            if (player.transform.position.x > currentSegment.transform.position.x + currentSegment.Size.x / 2)
            {
                player.transform.position = new Vector3(currentSegment.transform.position.x + currentSegment.Size.x / 2, player.transform.position.y, player.transform.position.z);
            }

            else if (player.transform.position.x < currentSegment.transform.position.x - currentSegment.Size.x / 2)
            {
                player.transform.position = new Vector3(currentSegment.transform.position.x - currentSegment.Size.x / 2, player.transform.position.y, player.transform.position.z);
            }

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
