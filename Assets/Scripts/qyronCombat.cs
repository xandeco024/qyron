using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class qyronCombat : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float health;
    [SerializeField] private Color damageColor, originalColor;

    [SerializeField] private float attackDamage;
    [SerializeField] private bool isAttacking;
    [SerializeField] private float attackRange = 2;

    private LayerMask enemyLayer;

    [Header("Qyron")]
    private Rigidbody2D qyronRB;
    private SpriteRenderer qyronSR;
    private BoxCollider2D qyronCol;
    private qyronMovement qyronMovement;
    private qyronSFX qyronSFX;


    void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");

        qyronMovement = GetComponent<qyronMovement>();
        qyronCol = GetComponent<BoxCollider2D>();
        qyronSR = GetComponent<SpriteRenderer>();
        qyronRB = GetComponent<Rigidbody2D>();
        qyronSFX = GetComponent<qyronSFX>();
    }

    void Update()
    {
        Vector2 attackDirection = new Vector2(transform.localScale.x, 0);

        Debug.DrawRay(transform.position, attackDirection * 1, Color.red);

        if (Input.GetButtonDown("Fire1") && !isAttacking) StartCoroutine(BasicAttack());

        
    }

    public void TakeDamage(float damage, bool takeKnockBack, float knockBackForce)
    {
        health -= damage;
        StartCoroutine(FlashRed());
        if(takeKnockBack)
        {
            qyronRB.AddForce(new Vector2(knockBackForce * transform.localScale.x, knockBackForce / 4), ForceMode2D.Impulse);
        }
    }

    IEnumerator FlashRed()
    {
        for (int i = 0; i < 3; i++)
        {
            qyronSR.color = damageColor;
            yield return new WaitForSeconds(0.1f);
            qyronSR.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator BasicAttack()
    {
        Debug.Log("bateu msm");

        isAttacking = true;

        Vector3 attackDirection = new Vector2(transform.localScale.x, 0);

        RaycastHit2D BasicAttackRaycast = Physics2D.Raycast(transform.position, attackDirection, 2, enemyLayer);


        if (BasicAttackRaycast.collider != null)
        {
            qyronSFX.PlayAttackSFX(Random.Range(0, 2));

            BasicAttackRaycast.collider.gameObject.GetComponent<enemyCombat>().TakeDamage(attackDamage);
        }

        else

        {
            qyronSFX.PlayMissSFX(Random.Range(0,2));
        }

        yield return new WaitForSeconds(.15f);

        isAttacking = false;
    }
}
