using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

    [Header("Components")]
    protected Rigidbody rb;
    protected Animator animator;
    protected SpriteRenderer sr;
    protected BoxCollider bc;

    protected virtual void GetComponentsOnCharacter()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider>();
    }

    [Header("Character Stats")]
    [SerializeField] protected float maxHealth;
    public float MaxHealth { get { return maxHealth; } }
    protected float currentHealth;
    public float CurrentHealth {
        get { return currentHealth; } 
        set { if (value > maxHealth) currentHealth = maxHealth;
        else if (value < 0) currentHealth = 0; 
        else currentHealth = value; }
        }

    [SerializeField] protected bool invincible;
    protected bool fallDamage;
    [SerializeField] protected bool fallDamageEnabled;
    [SerializeField] protected float baseDamage;
    [SerializeField] protected float baseMoveSpeed;
    protected float moveSpeed;
    [SerializeField] protected float jumpForce;

    [SerializeField] float raycastDistance;
    [SerializeField] private Vector3 raycastOffset;
    [SerializeField] private LayerMask groundLayer;
    protected bool isGrounded() {
        RaycastHit hit;
        bool hitGround = Physics.Raycast(transform.position + raycastOffset, Vector3.down, out hit, raycastDistance, groundLayer);
        return hitGround;
    }
    protected int facingDirection = 1;

    public bool isMovingAllowed = true;
    public bool grabbable;

    [Header("Animation")]
    protected bool isTakingDamage;
    protected bool isAttacking;

    #region Movement

    protected void LimitZ()
    {
        if (transform.position.z >= 2.5f) transform.position = new Vector3(transform.position.x, transform.position.y, 2.5f);
        if (transform.position.z <= -2.5f) transform.position = new Vector3(transform.position.x, transform.position.y, -2.5f);
    }

    protected void FlipSprite()
    {
        if (rb.velocity.x > 0.1f) 
        {
            facingDirection = 1;
            //sr.flipX = false;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (rb.velocity.x < -0.1f) 
        {
            facingDirection = -1;
            //sr.flipX = true;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    #endregion

    protected void FallDetect()
    {
        if (rb.velocity.y < -10)
        {
            fallDamage = true;
        }

        if (isGrounded() && fallDamage)
        {
            TakeDamage(10);
            fallDamage = false;
        }
    }

    protected void DealDamage(Collider[] hitColliders, float damage, Vector3 knockbackDir = default, float knockbackForce = 0)
    {
        foreach(Collider hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<Character>() && hitCollider.GetComponent<Character>() != this)
            {
                hitCollider.GetComponent<Character>().TakeDamage(damage, knockbackDir, knockbackForce);
            }
        }
    }

    public virtual void TakeDamage(float damage, Vector3 knockbackDir = default, float knockbackForce = 0)
    {
        StartCoroutine(FlashRed(2));

        if (knockbackForce > 0)
        {
            TakeKnockBack(knockbackDir, knockbackForce);
        }

        currentHealth -= damage;
    }

    protected void TakeKnockBack(Vector3 knockbackDir, float knockbackForce)
    {
        rb.velocity = new Vector3(0, 0, 0);
        rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
    }

    protected void Die()
    {
        // Play death animation
        // Disable movement
        // Disable combat
        // Disable collision
        // Disable this script
        StopAllCoroutines(); // ver se acaba o bug dos pombo (nao acabou...)
        Destroy(gameObject);
    }

    protected IEnumerator FlashRed(int timesToFlash)
    {
        if (GetComponent<SpriteRenderer>() != null)
        {
            for (int i = 0; i < timesToFlash; i++)
            {
                sr.color = new Color(1,0.5f,0.5f,1);
                yield return new WaitForSeconds(0.1f);
                sr.color = new Color(1,1,1,1);
                yield return new WaitForSeconds(0.1f);
            }
        }

        // if isnt a sprite, change material color

        else if (GetComponent<Renderer>() != null)
        {
            for (int i = 0; i < timesToFlash; i++)
            {
                GetComponent<Renderer>().material.color = new Color(0.5f,0.2f,0.2f,1);
                yield return new WaitForSeconds(0.1f);
                GetComponent<Renderer>().material.color = new Color(1,1,1,1);
                yield return new WaitForSeconds(0.1f);
            }
        }

        else
        {
            Debug.LogError("No SpriteRenderer or Renderer found on this object");
        }
    }

    public void SetGrabbed(bool grabbed)
    {
        isMovingAllowed = !grabbed;
        bc.enabled = !grabbed;
        rb.isKinematic = grabbed;
    }
}