using UnityEngine;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;

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
    [SerializeField] protected bool debug;
    [SerializeField] protected float baseMaxHealth;
    protected float maxHealth;
    public float MaxHealth { get { return maxHealth; } }
    protected float currentHealth;
    public float CurrentHealth { get { return currentHealth; } }
    [SerializeField] protected float baseResistance;
    protected float resistance;
    public float Resistance { get { return resistance; } }
    [SerializeField] protected float baseDamageReduction;
    protected float damageReduction;
    public float DamageReduction { get { return damageReduction; } }
    [SerializeField] protected float baseAttackDamage;
    protected float attackDamage;
    public float AttackDamage { get { return attackDamage; } }
    [SerializeField] protected float baseCriticalChance;
    protected float criticalChance;
    public float CriticalChance { get { return criticalChance; } }
    [SerializeField] protected float baseSpeed;
    protected float speed;
    public float Speed { get { return speed; } }
    [SerializeField] protected float baseDodgeChance;
    protected float dodgeChance;
    public float DodgeChance { get { return dodgeChance; } }
    [SerializeField] protected float baseMoveSpeed;
    protected float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }
    [SerializeField] protected float baseJumpForce;
    protected float jumpForce;
    public float JumpForce { get { return jumpForce; } }
    [SerializeField] protected float baseRespect;
    protected float respect;
    public float Respect { get { return respect; } }

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
    [SerializeField] protected Vector3 grabPoint;
    public Vector3 GrabPoint { get { return grabPoint; } }
    protected bool isReceivingCombo;
    public bool IsReceivingCombo { get { return isReceivingCombo; } }
    protected bool isDead;
    public bool IsDead { get { return isDead; } }
    [SerializeField] protected Vector2 damageTextOffset = new Vector2(0.5f, 0.5f);
    [SerializeField] protected bool damageTextFaceToDirection;
    protected bool stunned = false;
    public bool Stunned { get { return stunned; } }



    [Header("Animation")]
    protected bool isTakingDamage;
    public bool IsTakingDamage { get { return isTakingDamage; } }
    protected float damageTime;
    public float DamageTime { get { return damageTime; } }
    protected int attackAnimationIndex = 1;
    protected bool isAttacking;
    protected bool isLightAttacking;
    protected bool isHeavyAttacking;

    #region Movement
    public void LimitZ()
    {
        if (transform.position.z >= 20f) transform.position = new Vector3(transform.position.x, transform.position.y, 20f);
        if (transform.position.z <= -13f) transform.position = new Vector3(transform.position.x, transform.position.y, -13f);
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

    protected IEnumerator HandleStun(float stunDuration)
    {
        stunned = true;
        yield return new WaitForSeconds(stunDuration);
        stunned = false;
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
            TakeDamage(10, 0);
            fallDamage = false;
        }
    }

    protected void DealDamage(Collider[] hitColliders, float damage, float stunDuration, bool critical = false, Vector3 knockbackDir = default, float knockbackForce = 0, float knockbackDuration = .2f)
    {
        foreach(Collider hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<Character>() && hitCollider.GetComponent<Character>() != this)
            {
                hitCollider.GetComponent<Character>().TakeDamage(damage, stunDuration, critical, knockbackDir, knockbackForce, knockbackDuration);
            }
        }
    }

    public virtual void TakeDamage(float damage, float stunDuration, bool critical = false, Vector3 knockbackDir = default, float knockbackForce = 0, float knockbackDuration = .2f)
    {
        StartCoroutine(FlashRed(2));
        currentHealth -= damage;
        
        Vector3 damageTextPosition = transform.position + new Vector3(damageTextOffset.x * (damageTextFaceToDirection == true? facingDirection : 1), damageTextOffset.y, -1f);
        Instantiate(damageTextPrefab, damageTextPosition, Quaternion.identity).GetComponent<damageText>().SetText(damage.ToString(), critical);

        if (stunDuration > 0)
        {
            StartCoroutine(HandleStun(stunDuration));
        }

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
        this.isGrabbed = grabbed;
        isMovingAllowed = !grabbed;
        bc.enabled = !grabbed;
        rb.isKinematic = grabbed;
        if (animator != null) animator.SetBool("grabbed", grabbed);
    }

    protected void SetRecievingComboOnTargets(bool receivingCombo, Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<Character>())
            {
                collider.GetComponent<Character>().SetReceivingCombo(receivingCombo);
            }
        }
    }

    public void SetReceivingCombo(bool receivingCombo)
    {
        this.isReceivingCombo = receivingCombo;
    }
}