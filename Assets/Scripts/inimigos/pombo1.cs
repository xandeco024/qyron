using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pombo1 : MonoBehaviour
{
    private Vector3 playerDirection;

    [Header("Pigeon")]
    [SerializeField] private float pigeonDamage;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackCD;
 
    [Header("Components")]
    private GameObject qyron;
    private qyronCombat qyronCombat;
    private Rigidbody pigeonRB;
    private SpriteRenderer pigeonSR;
    private BoxCollider pigeonCOL;
    private enemyCombat pigeonCombat;
    private Animator pigeonAnimator;
    private LayerMask groundLayer;

    [Header("State")]
    private bool playerInAttackRange = false;
    private bool isFollowing;
    private bool isAttacking;
    private bool wasHit;
    private bool isGrounded;

    [Header("Combat Hitbox")]
    [SerializeField] Vector3 CombatRaycastSize;
    [SerializeField] Vector3 CombatBoxOffset;
    [SerializeField] bool pigeonHitCollision;
    private LayerMask playerLayer;
    private int pigeonSpriteDirection = 1;

    [Header("Livre arbitrio falso")]
    private Vector3 destination;
    private float idleTime;
    private float startIdleTime;
    private float moveTime;
    private float startMoveTime;
    private bool isIdle;
    private bool isMoving;

    void Start()
    {
        qyron = GameObject.FindWithTag("Player");
        qyronCombat = qyron.GetComponent<qyronCombat>();
            
        groundLayer = LayerMask.GetMask("Ground");
        playerLayer = LayerMask.GetMask("Player");
        pigeonRB = GetComponent<Rigidbody>();
        pigeonCOL = GetComponent<BoxCollider>();
        pigeonCombat = GetComponent<enemyCombat>();
        pigeonSR = GetComponent<SpriteRenderer>();
        pigeonAnimator = GetComponent<Animator>();

        Physics.IgnoreCollision(pigeonCOL, qyron.GetComponent<BoxCollider>());
    }

    void Update()
    {
        pigeonHitCollision = Physics.CheckBox(transform.position + CombatBoxOffset * pigeonSpriteDirection, CombatRaycastSize / 2, transform.rotation, playerLayer);

        playerDirection = (qyron.transform.position - transform.position).normalized;

        if(!pigeonCombat.isTakingDamage)
        {
            FlipSprite();

            if(pigeonHitCollision)
            {
                playerInAttackRange = true;
                
                if(!isAttacking)
                {
                    StartCoroutine (Attack());
                }
            }

            else

            {
                playerInAttackRange = false;
            }
        }

        Limit();
    }

    private void FixedUpdate()
    {
        if(!pigeonCombat.isTakingDamage && !playerInAttackRange)
        {
            if(Vector3.Distance(transform.position, qyron.transform.position) <= 5)
            {
                FollowPlayer();
            }
        }
    }

    private void FreeMove()
    {
        if(!playerInAttackRange)
        {
            if(transform.position == destination)
            {
                isIdle = true
            }
        }
    }

    private IEnumerator Attack()
    {
        Debug.Log("#carregando"); 
        isAttacking = true;
        pigeonAnimator.SetTrigger("Attack");

        if(playerInAttackRange)
        {
            StartCoroutine(qyronCombat.TakeDamage(pigeonDamage, true, new Vector3(1, 1, 0)));
        }

        yield return new WaitForSeconds(.1f);
        Debug.Log("#atacando");

        yield return new WaitForSeconds(attackCD);
        isAttacking = false;
    }

    private void FollowPlayer()
    {
        pigeonRB.velocity = new Vector3(playerDirection.x * moveSpeed, pigeonRB.velocity.y, playerDirection.z * moveSpeed);
    }

    private void FlipSprite()
    {
        if(pigeonRB.velocity.x > 0)
        {
            pigeonSR.flipX = false;
            pigeonSpriteDirection = 1;
        }

        else if(pigeonRB.velocity.x < 0)
        {
            pigeonSR.flipX = true;
            pigeonSpriteDirection = -1;
        }
    }

    private void Limit()
    {
        if (transform.position.z >= 2.5f) transform.position = new Vector3(transform.position.x, transform.position.y, 2.5f);
        if (transform.position.z <= -2.5f) transform.position = new Vector3(transform.position.x, transform.position.y, -2.5f);
    }

    private void OnDrawGizmos()
    {
        if (pigeonHitCollision)
        {
            Gizmos.color = Color.red;
        }

        else

        {
            Gizmos.color = Color.yellow;
        }

        Gizmos.DrawWireCube(transform.position + CombatBoxOffset * pigeonSpriteDirection, CombatRaycastSize);
    }
}
