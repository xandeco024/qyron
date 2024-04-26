using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

    [Header("Components")]
    [SerializeField] protected GameObject damageTextPrefab;
    public Rigidbody rb;
    public Animator animator;
    public SpriteRenderer sr;
    public BoxCollider bc;

    protected virtual void GetComponentsOnCharacter()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider>();
    }

    [Header("Character Stats")]
    [SerializeField] protected float baseMaxHealth;
    protected float maxHealth;
    public float MaxHealth { get { return maxHealth; } }
    protected float currentHealth;
    public float CurrentHealth { get { return currentHealth; } }
    [SerializeField] protected float baseAttackDamage;
    protected float attackDamage;
    public float AttackDamage { get { return attackDamage; } }
    [SerializeField] protected float baseCriticalChance;
    protected float criticalChance;
    public float CriticalChance { get { return criticalChance; } }
    [SerializeField] protected float baseMoveSpeed;
    protected float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }
    [SerializeField] protected float baseJumpForce;
    protected float jumpForce;
    public float JumpForce { get { return jumpForce; } }

    protected void SetStats()
    {
        maxHealth = baseMaxHealth;
        currentHealth = maxHealth;
        attackDamage = baseAttackDamage;
        criticalChance = baseCriticalChance;
        moveSpeed = baseMoveSpeed;
        jumpForce = baseJumpForce;
    }


    [Header("Character Behaviour")]
    protected bool invincible;
    protected bool fallDamage;
    [SerializeField] protected bool fallDamageEnabled;
    [SerializeField] float raycastDistance;
    [SerializeField] private Vector3 raycastOffset;
    [SerializeField] private LayerMask groundLayer;
    protected bool isGrounded() {
        RaycastHit hit;
        bool hitGround = Physics.Raycast(transform.position + raycastOffset, Vector3.down, out hit, raycastDistance, groundLayer);
        return hitGround;
    }
    
    public int FacingDirection { get { return facingDirection; } }
    protected int facingDirection = 1;
    protected bool isMovingAllowed = true;
    public bool IsMovingAllowed { get { return isMovingAllowed; } }
    [SerializeField] protected bool Grabbable;
    public bool IsGrabbable { get { return Grabbable; } }
    protected bool isGrabbed;
    public bool IsGrabbed { get { return isGrabbed; } }



    [Header("Animation")]
    protected bool isTakingDamage;
    protected bool isAttacking;

    #region Movement

    public void LimitZ()
    {
        if (transform.position.z >= 2.5f) transform.position = new Vector3(transform.position.x, transform.position.y, 2.5f);
        if (transform.position.z <= -2.5f) transform.position = new Vector3(transform.position.x, transform.position.y, -2.5f);
    }

    public void Flip(bool right = false)
    {
        if (right)
        {
            facingDirection = 1;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            facingDirection = -1;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public void FlipHandler()
    {
        if (rb.velocity.x > 0.1f) 
        {
            Flip(true);
        }
        else if (rb.velocity.x < -0.1f) 
        {
            Flip(false);
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

    protected void DealDamage(Collider[] hitColliders, float damage, bool critical = false, Vector3 knockbackDir = default, float knockbackForce = 0)
    {
        foreach(Collider hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<Character>() && hitCollider.GetComponent<Character>() != this)
            {
                hitCollider.GetComponent<Character>().TakeDamage(damage, critical, knockbackDir, knockbackForce);
            }
        }
    }

    public virtual void TakeDamage(float damage, bool critical = false, Vector3 knockbackDir = default, float knockbackForce = 0, float knockbackDuration = .2f)
    {
        StartCoroutine(FlashRed(2));
        currentHealth -= damage;
        
        Vector3 damageTextPosition = transform.position + new Vector3(facingDirection, 0f, -1f);
        Instantiate(damageTextPrefab, damageTextPosition, Quaternion.identity).GetComponent<damageText>().SetText(damage.ToString(), critical);

        if (knockbackForce > 0)
        {
            StartCoroutine(TakeKnockback(knockbackDir, knockbackForce,knockbackDuration));
        }
    }


    protected IEnumerator TakeKnockback(Vector3 knockbackDir, float knockbackForce, float duration)
    {
        isMovingAllowed = false;
        rb.velocity = new Vector3(0, 0, 0);
        rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
        yield return new WaitForSeconds(duration);
        rb.velocity = new Vector3(0, 0, 0);
        isMovingAllowed = true;
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

    public void SetGrabbed(bool grabbed, int facingDirection = 1)
    {
        Flip(facingDirection == 1? false : true);
        this.isGrabbed = grabbed;
        isMovingAllowed = !grabbed;
        bc.enabled = !grabbed;
        rb.isKinematic = grabbed;
    }
}