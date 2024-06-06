using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<Segment> segments;
    private Segment currentSegment;
    public Segment CurrentSegment { get { return currentSegment; } }

    private CameraManager cameraManager;

    private GameManager gameManager;
    private List<PlayableCharacter> playerList;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerList = gameManager.PlayerList;

        cameraManager = FindObjectOfType<CameraManager>();

        segments = new List<Segment>(FindObjectsOfType<Segment>());
        //order the segments by their index
        segments.Sort((x, y) => x.SegmentIndex.CompareTo(y.SegmentIndex));

        currentSegment = segments[0];

        cameraManager.SetCameraLimits(transform.position.x - currentSegment.Size.x / 2, transform.position.x + currentSegment.Size.x / 2, transform.position.z - currentSegment.Size.z / 2, transform.position.z + currentSegment.Size.z / 2);
    }

    // Update is called once per frame
    void Update()
    {
        LimitPlayersToSegment();
        HandleSegmentCompletion();
    }

    void LimitPlayersToSegment()
    {
        foreach (PlayableCharacter player in playerList)
        {
            //limit x
            if (player.transform.position.x > currentSegment.transform.position.x + currentSegment.Size.x / 2)
            {
                player.transform.position = new Vector3(currentSegment.transform.position.x + currentSegment.Size.x / 2, player.transform.position.y, player.transform.position.z);
            }
            /*
            else if (player.transform.position.x < currentSegment.transform.position.x - currentSegment.Size.x / 2)
            {
                player.transform.position = new Vector3(currentSegment.transform.position.x - currentSegment.Size.x / 2, player.transform.position.y, player.transform.position.z);
            }*/

            //limit z
            if (player.transform.position.z > currentSegment.transform.position.z + currentSegment.Size.z / 2)
            {
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, currentSegment.transform.position.z + currentSegment.Size.z / 2);
            }
            /*
            else if (player.transform.position.z < currentSegment.transform.position.z - currentSegment.Size.z / 2)
            {
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, currentSegment.transform.position.z - currentSegment.Size.z / 2);
            }*/
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
        int nextIndex = currentSegment.SegmentIndex + 1;
        if (nextIndex < segments.Count)
        {
            currentSegment = segments[nextIndex];
        }
    }
}
