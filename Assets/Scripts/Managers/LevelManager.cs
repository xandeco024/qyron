using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.U2D;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<Segment> segments;
    private Segment currentSegment;
    public Segment CurrentSegment { get { return currentSegment; } }
    private int startMinutes; // controla o tempo que INICIOU o segmento, pra quando morrer, voltar um poquinho no tmepo
    private int startHours;

    [SerializeField] private int firstSegmentIndex;
    public int FirstSegmentIndex { get { return firstSegmentIndex; } }

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

        currentSegment = segments[firstSegmentIndex];
        cameraManager.SetCameraLimits(segments[firstSegmentIndex].transform.position.x - currentSegment.Size.x / 2 + segmentBorderCamOffset, currentSegment.transform.position.x + currentSegment.Size.x / 2 - segmentBorderCamOffset, currentSegment.transform.position.z - currentSegment.Size.z / 2, currentSegment.transform.position.z + currentSegment.Size.z / 2);
    
        startMinutes = gameManager.Minutes;
        startHours = gameManager.Hours;
    }

    // Update is called once per frame
    void Update()
    {
        HandleSegmentCompletion();
    }

    void HandleSegmentCompletion()
    {
        if (currentSegment.Complete && currentSegment.Index < segments.Count - 1)
        {
            NextSegment();
        }
        else if (currentSegment.Complete && currentSegment.Index == segments.Count - 1)
        {
            //end of the level
        }
    }

    public void NextSegment()
    {
        int nextIndex = currentSegment.Index + 1;

        startHours = gameManager.Hours;
        startMinutes = gameManager.Minutes;

        HUDManager hudManager = FindObjectOfType<HUDManager>();
        hudManager.SetPoitingRightPaw(true);
        currentSegment = segments[nextIndex];
        cameraManager.SetCameraLimits(segments[0].transform.position.x - currentSegment.Size.x / 2 + segmentBorderCamOffset, currentSegment.transform.position.x + currentSegment.Size.x / 2 - segmentBorderCamOffset, currentSegment.transform.position.z - currentSegment.Size.z / 2, currentSegment.transform.position.z + currentSegment.Size.z / 2);
        StartCoroutine(UnsetPoitingRightPaw(hudManager));
    }

    public void GoToSegment(int segmentIndex, bool resetPlayer, bool backInTime)
    {
        int x = 0, z = 0;

        //if there are only one player, place  him on x 0, 0
        //if there are 2 players, the first will be placed on x -1, z1, and the second on x 1, z-1
        //if there are 3 players, the first will be placed on x -2, z2, the second on x 0, z0 and the third on x 2, z -2
        //if there are 4 players, the first will be placed on x -3, z3, the second on x -1, z1, the third on x 1, z-1 and the fourth on x 3, z-3

        switch (playerList.Count)
        {
            case 1:
                x = 0;
                z = 0;
                break;
            case 2:
                x = -1;
                z = -1;
                break;
            case 3:
                x = -2;
                z = -2;
                break;
            case 4:
                x = -3;
                z = -3;
                break;
        }

        if (backInTime)
        {
            gameManager.SetTime(startHours, startMinutes);
        }


        foreach (PlayableCharacter player in playerList)
        {
            if (resetPlayer)
            {
                player.Reset();
            }
            player.rb.velocity = Vector3.zero; // pra ele nao voar pro chao
            Vector3 sp = segments[segmentIndex].TranslateSpawnPoint();
            player.transform.position = new Vector3(sp.x + x, sp.y, sp.z + z);
            x += 2;
            z += 2;
        }
    }

    IEnumerator UnsetPoitingRightPaw(HUDManager hudManager)
    {
        yield return new WaitForSeconds(1.5f);
        hudManager.SetPoitingRightPaw(false);
    }
}