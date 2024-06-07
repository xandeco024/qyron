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
        HandleSegmentCompletion();
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
