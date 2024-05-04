using System.Collections;
using System.Collections.Generic;
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



    [Header("Other")]
    [SerializeField] protected int deathTime;
    public int DeathTime { get => deathTime; }
    [SerializeField] protected int xpAmount;
    public int XpAmount { get => xpAmount; }
    [SerializeField] protected Vector3 exPBoxSize;
    public Vector3 XpBoxSize { get => exPBoxSize; }


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
    
        //draw xp box
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, exPBoxSize);
    }

    public IEnumerator Die(int deathTime = 0)
    {
        int timesToFlash = deathTime * 5;

        if (GetComponent<SpriteRenderer>() != null)
        {
            for (int i = 0; i < timesToFlash; i++)
            {
                sr.color = new Color(1, 1, 1, 0);
                yield return new WaitForSeconds(0.1f);
                sr.color = new Color(1, 1, 1, 1);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else if (GetComponents<Renderer>() != null)
        {
            for (int i = 0; i < timesToFlash; i++)
            {
                foreach (Renderer renderer in GetComponents<Renderer>())
                {
                    renderer.material.color = new Color(1, 1, 1, 0);
                }
                yield return new WaitForSeconds(0.1f);
                foreach (Renderer renderer in GetComponents<Renderer>())
                {
                    renderer.material.color = new Color(1, 1, 1, 1);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            Debug.Log("No sprite renderer or renderer found");
            Debug.Log("Destroying object in " + deathTime + " seconds");
            yield return new WaitForSeconds(deathTime);
        }

        //StopAllCoroutines();
        Destroy(gameObject);
    }

    public void GiveXP(int xpAmount)
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, exPBoxSize / 2, transform.rotation);
        List<PlayableCharacter> playersOnRange = new List<PlayableCharacter>();

        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<PlayableCharacter>() != null)
            {
                playersOnRange.Add(collider.GetComponent<PlayableCharacter>());
            }
        }

        foreach (PlayableCharacter player in playersOnRange)
        {
            player.AddExP(Mathf.RoundToInt(xpAmount / playersOnRange.Count));
        }

        if(debug) Debug.Log("Gave " + Mathf.RoundToInt(xpAmount / playersOnRange.Count) + " xp to " + playersOnRange.Count + " players");
    } 
}
