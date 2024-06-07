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
        playersOnSegment.ForEach(player => RestrictPlayerToSegment(player));
    }

    void RestrictPlayerToSegment(PlayableCharacter player)
    {
        // só vai limitar no segmento que não está completo
        if (!complete && player.transform.position.x > transform.position.x + size.x / 2 - 1)
        {
            player.transform.position = new Vector3(transform.position.x + size.x / 2, player.transform.position.y, player.transform.position.z);
        }
        
        //só limita para voltar no segmento 0 pra ele não ir pro segmento -1 (inexistente.)
        if (index == 0 && player.transform.position.x < transform.position.x - size.x / 2 + 1)
        {
            player.transform.position = new Vector3(transform.position.x - size.x / 2, player.transform.position.y, player.transform.position.z);
        }

        // a limitação Z sempre ocorre em todo player no segmento.
        if (player.transform.position.z > transform.position.z + size.z / 2 - 1)
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z + size.z / 2);
        }
        else if (player.transform.position.z < transform.position.z - size.z / 2 + 1)
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z - size.z / 2);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
