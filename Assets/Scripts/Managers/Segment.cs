using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    private GameManager gameManager;
    private LevelManager levelManager;

    [SerializeField] private int index;

    public int Index { get { return index; }}

    [SerializeField] private Vector3 size;
    public Vector3 Size { get { return size; }}

    private List<PlayableCharacter> playersOnSegment = new List<PlayableCharacter>();
    public List<PlayableCharacter> PlayersOnSegment { get { return playersOnSegment; }}

    private BoxCollider boxCollider;

    [SerializeField] private bool complete;
    public bool Complete { get { return complete; } set { complete = value; }}

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = size;
        levelManager = FindObjectOfType<LevelManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        playersOnSegment = gameManager.PlayerList.FindAll(player => boxCollider.bounds.Contains(player.transform.position));
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
