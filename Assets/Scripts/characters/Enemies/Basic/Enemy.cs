using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;
public class Enemy : Character
{
    protected PlayableCharacter[] players;
    protected PlayableCharacter target;
    public PlayableCharacter Target { get => target; }
    [SerializeField] protected float targetRange;
    public float TargetRange { get => targetRange; }


    [Header("Attack")]
    [SerializeField] protected float attackRange;
    public float AttackRange { get => attackRange; }


    
    [Header("Fake Liberty")]
    [SerializeField] protected bool freeMovement;
    public bool FreeMovement { get => freeMovement; }
    [SerializeField] protected Vector2 moveTimeRange;
    public Vector2 MoveTimeRange { get => moveTimeRange; }
    [SerializeField] protected Vector2 idleTimeRange;
    public Vector2 IdleTimeRange { get => idleTimeRange; }


    
    void Start()
    {
        GetComponentsOnCharacter();
        SetStats();
        players = FindObjectsOfType<PlayableCharacter>();
    }

    void Update()
    {
        target = FindTargetOnRange(players, targetRange);
    }

    protected PlayableCharacter FindTargetOnRange(PlayableCharacter[] players, float range)
    {
        PlayableCharacter target = null;

        foreach (PlayableCharacter player in players)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= range)
            {
                if (target == null)
                {
                    target = player;
                }
                else
                {
                    if (Vector3.Distance(transform.position, player.transform.position) < Vector3.Distance(transform.position, target.transform.position))
                    {
                        target = player;
                    }
                }
            }
        }

        return target;
    }
}
