using UnityEngine;

public class Pigeon : Enemy
{
    //[Header("Livre arbitrio falso")]

    void Awake()
    {
        GetComponentsOnCharacter();
    }

    void Start()
    {
        players = FindObjectsOfType<PlayableCharacter>();

        SetStats();

    }

    void Update()
    {
        FlipSprite();
        
        if(currentHealth <= 0)
        {
            Die();
        }

        if(Searching)
        {
            SearchingForTarget();
        }

        if(Following)
        {
            FollowTarget();
        }
    }

    private void FixedUpdate()
    {
        LimitZ();
    }

    override protected void SearchingForTarget()
    {
        base.SearchingForTarget();

        animator.SetBool("Moving", moving);
    }

    private void OnDrawGizmos()
    {

    }
}
