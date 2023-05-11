using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pombo1 : MonoBehaviour
{
    private Vector3 direction;

 

    [Header("Pigeon")]
    [SerializeField] private float pigeonDamage;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackCD;
 
    [Header("Components")]
    private GameObject qyron;
    private Rigidbody pigeonRB;
    private BoxCollider pigeonCOL;
    private enemyCombat pigeonCombat;
    private LayerMask playerLayer;
    private LayerMask groundLayer;

    [Header("State")]
    private bool playerInAttackRange;
    private bool isFollowing;
    private bool isAttacking;
    private bool wasHit;
    private bool isGrounded;

    void Start()
    {
        qyron = GameObject.FindWithTag("Player");

        groundLayer = LayerMask.GetMask("Ground");
        playerLayer = LayerMask.GetMask("Player");
        pigeonRB = GetComponent<Rigidbody>();
        pigeonCOL = GetComponent<BoxCollider>();
        pigeonCombat = GetComponent<enemyCombat>();

        Physics.IgnoreCollision(pigeonCOL, qyron.GetComponent<BoxCollider>());
    }

    void Update()
    {
        direction = (qyron.transform.position - transform.position).normalized;

        if(!pigeonCombat.isTakingDamage)
        {
            Flip();
            FollowPlayer();

            Vector3 attackDirection = new Vector2(transform.localScale.x, 0);
            RaycastHit2D AttackRaycast = Physics2D.Raycast(transform.position, attackDirection, 2, playerLayer);
            Debug.DrawRay(transform.position, attackDirection * 1, Color.red);

            if (AttackRaycast.collider != null)
            {
                if (AttackRaycast.collider.gameObject.CompareTag("Player"))
                {
                    if (!wasHit)
                    {
                        //Debug.Log("entrou no range");
                        playerInAttackRange = true;
                    }

                    wasHit = true;
                }
            }

            else

            {
                if (wasHit)
                {
                    //Debug.Log("saiu do range");
                    playerInAttackRange = false;
                }

                wasHit = false;
            }

            if (!isAttacking && playerInAttackRange)
            {
                StartCoroutine(Attack());
            }
        }

        Limit();
    }

    private IEnumerator Attack()
    {
       // Debug.Log("#carregando"); 
        isAttacking = true;

        yield return new WaitForSeconds(.1f);
       //Debug.Log("#atacando");

        Vector3 attackDirection = new Vector2(transform.localScale.x, 0);
        RaycastHit2D AttackRaycast = Physics2D.Raycast(transform.position, attackDirection, 2, playerLayer);
        Debug.DrawRay(transform.position, attackDirection * 1, Color.blue);

        if (AttackRaycast.collider != null)
        {
            if (AttackRaycast.collider.gameObject.CompareTag("Player"))
            {
                //Debug.Log("bateu");
                StartCoroutine(AttackRaycast.collider.gameObject.GetComponent<qyronCombat>().TakeDamage(pigeonDamage, true, new Vector2(4,2)));
            }
        }

        yield return new WaitForSeconds(attackCD);
        isAttacking = false;
    }

    private void FollowPlayer()
    {
        if (Vector3.Distance(qyron.transform.position, transform.position) <= 10)
        {
            isFollowing = true;
        }

        if (!playerInAttackRange && isFollowing)
        {
            pigeonRB.velocity = new Vector2(direction.x * moveSpeed, pigeonRB.velocity.y);
        }
    }

    private void Flip()
    {
        if(pigeonRB.velocity.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        else if(pigeonRB.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void Limit()
    {
        if (transform.position.z >= 2.5f) transform.position = new Vector3(transform.position.x, transform.position.y, 2.5f);
        if (transform.position.z <= -2.5f) transform.position = new Vector3(transform.position.x, transform.position.y, -2.5f);
    }
}
