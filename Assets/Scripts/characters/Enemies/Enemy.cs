using UnityEngine;
public class Enemy : Character
{
    protected PlayableCharacter[] players;
    protected PlayableCharacter target;
    
    [SerializeField] protected float targetRange;
    protected bool targetOnRange { get { if (target != null) return Vector3.Distance(transform.position, target.transform.position) <= targetRange; else return false; }}


    
    [Header("Fake Liberty")]
    [SerializeField] protected bool freeMovement;
    protected Vector3 freeMovementDirection;
    [SerializeField] protected float[] idleTimeRange;
    protected float idleTime = 1;
    protected float currentIdleTime;
    [SerializeField] protected float[] moveTimeRange;
    protected float moveTime = 1;
    protected float currentMoveTime;
    protected bool moving;
    protected bool idle = true;



    [Header("STATES")]
    protected bool Patrol;
    protected bool Searching = true;
    protected bool Following;
    protected bool Attacking;



    protected virtual void SearchingForTarget()
    {
        target = FindClosestTarget();

        // if on attack range, follow the player
        if (Vector3.Distance(transform.position, target.transform.position) <= targetRange)
        {
            Following = true;
            Searching = false;
        }

        if (currentIdleTime >= idleTime)
        {
            freeMovementDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            moving = true;
            idle = false;

            currentIdleTime = 0;
            idleTime = Random.Range(idleTimeRange[0], idleTimeRange[1]);
        }

        if (currentMoveTime >= moveTime)
        {
            rb.velocity = Vector3.zero;
            moving = false;
            idle = true;

            currentMoveTime = 0;
            moveTime = Random.Range(moveTimeRange[0], moveTimeRange[1]);
        }

        if (idle)
        {
            currentIdleTime += Time.deltaTime;
            Debug.Log("ciscando o chão.");
        }

        if (moving)
        {
            currentMoveTime += Time.deltaTime;
            Debug.Log("zanzando.");
            FollowDirection(freeMovementDirection);
        }
    }

    protected void FollowTarget()
    {
        if (!targetOnRange)
        {
            Following = false;
            Searching = true;
            target = null;
        }
        else
        {
            Vector3 targetDirection = (target.transform.position - transform.position).normalized;
            FollowDirection(targetDirection);
        }
    }

    protected void FollowDirection(Vector3 direction)
    {
        rb.velocity = new Vector3(direction.x * moveSpeed, rb.velocity.y, direction.z * moveSpeed);
    }

    protected PlayableCharacter FindClosestTarget()
    {
        // Find the closest target
        PlayableCharacter closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (PlayableCharacter player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = player;
            }
        }

        return closestTarget;
    }
}
