using System.Collections;
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
    protected bool canAttack = true;
    public bool CanAttack { get => canAttack; }
    [SerializeField] protected float attackDelay;
    public float AttackDelay { get => attackDelay; }
    [SerializeField] protected float attackCD;
    public float AttackCD { get => attackCD; }
    [SerializeField] protected Vector3 combatBoxSize;
    public Vector3 CombatBoxSize { get => combatBoxSize; }
    [SerializeField] protected Vector3 combatBoxOffset;
    public Vector3 CombatBoxOffset { get => combatBoxOffset; }


    
    [Header("Fake Liberty")]
    [SerializeField] protected bool freeMovement;
    public bool FreeMovement { get => freeMovement; }
    [SerializeField] protected Vector2 moveTimeRange;
    public Vector2 MoveTimeRange { get => moveTimeRange; }
    [SerializeField] protected Vector2 idleTimeRange;
    public Vector2 IdleTimeRange { get => idleTimeRange; }



    public IEnumerator BasicAttack()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackDelay);

        Collider[] colliders = Physics.OverlapBox(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.y), combatBoxSize / 2, transform.rotation);
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<PlayableCharacter>() != null)
            {
                collider.GetComponent<PlayableCharacter>().TakeDamage(attackDamage);
            }
        }

        yield return new WaitForSeconds(attackCD);
        canAttack = true;
    }

    public bool PlayerOnAttackRange()
    {
        bool range = false;
        Collider[] colliders = Physics.OverlapBox(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.y), combatBoxSize / 2, transform.rotation);
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<PlayableCharacter>() != null)
            {
                range = true;
                break;
            }
        }
        return range;
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

    protected virtual void OnDrawGizmos()
    {
        //draw combat box
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.z), combatBoxSize);
    }
}
