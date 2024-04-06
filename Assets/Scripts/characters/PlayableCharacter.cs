using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayableCharacter : Character {

    [Header("Character Stats")]

    [SerializeField] private bool debug;
    private int level = 1;
    public int Level { get { return level; } }
    private int exP = 0;
    public int ExP { get { return exP; } }
    private int nextLevelExP = 100;
    public int NextLevelExP { get { return nextLevelExP; } }
    private int coins;
    public int Coins { get { return coins; } }

    [Header("Movement")]
    [SerializeField] private bool limitZ;
    private Vector3 movementInput;


    [Header("Jump Settings")]
    private bool isGrounded;
    [SerializeField] private int maxJumps;
    private int jumps;
    [SerializeField] float raycastDistance;
    [SerializeField] private Vector3 raycastOffset;
    [SerializeField] private LayerMask groundLayer;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    private bool canDash = true;
    [SerializeField] private GameObject dashCloneTrailPrefab;
    [SerializeField] private int dashCloneAmount;

    [Header("Input")]
    private InputMaster inputMaster;

    [Header("Animation")]
    private int attackAnimationIndex = 1;
    private bool fighting;

    [Header("Combat")]
    [SerializeField] private bool friendlyFire;
    [SerializeField] private float lightAttackCD;
    private bool canLightAttack = true;
    [SerializeField] private float heavyAttackCD;
    private bool canHeavyAttack = true;

    [SerializeField] private float grabAttackCD;
    private bool canGrabAttack = true;
    [SerializeField] private Vector3 CombatBoxOffset;
    [SerializeField] private Vector3 CombatRaycastSize;
    [SerializeField] private Vector3 grabbedCharacterOffset;
    private bool isGrabbing;
    private Character grabbedCharacter;

    void Awake()
    {
        GetComponentsOnCharacter();
        Controls();
    }

    private void OnEnable()
    {
        inputMaster.Enable();
    }

    private void OnDisable()
    {
        inputMaster.Disable();
    }

    void Start()
    {
        currentHealth = maxHealth;
        moveSpeed = baseMoveSpeed;
    }   

    void Update()
    {
        if (currentHealth <= 0) 
        {
            Die();
        }
        DetectGround();
        DebugHandler();
    }

    void FixedUpdate()
    {
        ApplyMovement();
        FlipSprite();
    }

    void Controls()
    {
        inputMaster = new InputMaster();
        inputMaster.Player.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputMaster.Player.Movement.canceled += ctx => movementInput = Vector2.zero;

        inputMaster.Player.Jump.performed += Jump;
        inputMaster.Player.Dash.performed += Dash;

        inputMaster.Player.LightAttack.performed += LightAttack;
        inputMaster.Player.HeavyAttack.performed += HeavyAttack;
        inputMaster.Player.GrabAttack.performed += GrabAttack;
    }

    #region Combat

    void LightAttack(InputAction.CallbackContext ctx)
    {
        if (canLightAttack && !isAttacking)
        {                                                                               
            StartCoroutine(LightAttackCoroutine());
        }
    }

    IEnumerator LightAttackCoroutine()
    {
        canLightAttack = false;
        if (!isAttacking) isAttacking = true;
        if (!fighting) fighting = true;

        //logica para calcular dano que o hit vai dar.
        float damage = baseDamage;

        attackAnimationIndex = (attackAnimationIndex == 1) ? 2 : 1;

        animator.SetTrigger("lightAttackTrigger");
        animator.SetInteger("attackAnimationIndex", attackAnimationIndex);

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);
        DealDamage(hitColliders, damage);

        yield return new WaitForSeconds(0.2f);

        isAttacking = false;

        yield return new WaitForSeconds(lightAttackCD);

        canLightAttack = true;
    }

    void HeavyAttack(InputAction.CallbackContext ctx)
    {
        if (canHeavyAttack && !isAttacking)
        {
            StartCoroutine(HeavyAttackCoroutine());
        }
    }

    IEnumerator HeavyAttackCoroutine()
    {
        canHeavyAttack = false;
        if (!isAttacking) isAttacking = true;
        if (!fighting) fighting = true;

        //logica para calcular dano que o hit vai dar.
        float damage = baseDamage * 2;

        attackAnimationIndex = (attackAnimationIndex == 1) ? 2 : 1;

        animator.SetTrigger("heavyAttackTrigger");
        animator.SetInteger("attackAnimationIndex", attackAnimationIndex);

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);
        DealDamage(hitColliders, damage);

        yield return new WaitForSeconds(0.3f);

        isAttacking = false;

        yield return new WaitForSeconds(heavyAttackCD);

        canHeavyAttack = true;
    }

    void GrabAttack(InputAction.CallbackContext ctx)
    {
        if (canGrabAttack && !isAttacking && !isGrabbing)
        {
            StartCoroutine(GrabAttackCoroutine());
        }

        else if (isGrabbing)
        {
            CancelGrab();
            StopCoroutine(GrabAttackCoroutine());
        }
    }

    IEnumerator GrabAttackCoroutine()
    {
        if (canGrabAttack) canGrabAttack = false;
        if (!isGrabbing) isGrabbing = true;
        if (!isAttacking) isAttacking = true;
        if (!fighting) fighting = true;

        moveSpeed = baseMoveSpeed / 4;

        animator.SetTrigger("grabAttackTrigger");
        animator.SetBool("grabbing", true);

        Debug.Log("Tentou grabar");

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<Character>() && hitCollider.GetComponent<Character>() != this && hitCollider.GetComponent<Character>().grabbable)
            {
                grabbedCharacter = hitCollider.GetComponent<Character>();

                Debug.Log("Grabou " + grabbedCharacter.gameObject.name);

                grabbedCharacter.transform.position = transform.position + new Vector3(grabbedCharacterOffset.x * facingDirection, grabbedCharacterOffset.y, grabbedCharacterOffset.z);
                grabbedCharacter.transform.SetParent(transform);
                grabbedCharacter.SetGrabbed(true);

                yield return new WaitForSeconds(2);

                continue;
            }
        }

        CancelGrab();

        yield return new WaitForSeconds(grabAttackCD);
        Debug.Log("Recarregou grab");
        canGrabAttack = true;
    }

    private void CancelGrab()
    {
        isGrabbing = false;
        isAttacking = false;
        animator.SetBool("grabbing", false);
        if (grabbedCharacter != null)
        {
            grabbedCharacter.SetGrabbed(false);
            grabbedCharacter.transform.SetParent(null);
            grabbedCharacter = null;
        }
        moveSpeed = baseMoveSpeed;
    }

    public override void TakeDamage(float damage, Vector3 knockbackDir = default, float knockbackForce = 0)
    {
        base.TakeDamage(damage, knockbackDir, knockbackForce);

        animator.SetTrigger("damageTrigger");

        if (isGrabbing)
        {
            CancelGrab();
        }
    }

    #endregion

    #region Movement

void DetectGround()
{
    RaycastHit hit;
    bool hitGround = Physics.Raycast(transform.position + raycastOffset, Vector3.down, out hit, raycastDistance, groundLayer);
    isGrounded = hitGround;
    animator.SetBool("grounded", hitGround);

    if (hitGround)
    {
        jumps = maxJumps;
    }
}

void ApplyMovement()
{
    if (isMovingAllowed)
    {
        rb.velocity = new Vector3(movementInput.x * moveSpeed, rb.velocity.y, movementInput.y * moveSpeed);
    }

    if (limitZ)
    {
        LimitZ();
    }

    // Define o estado "running" diretamente com base na condição, sem verificar o estado atual
    animator.SetBool("running", isGrounded && movementInput != Vector3.zero);

    // Atualiza a velocidade Y no animator
    animator.SetFloat("yVelocity", rb.velocity.y);
}


    void Jump(InputAction.CallbackContext ctx)
    {
        if (jumps > 0 && isMovingAllowed && !isGrabbing)
        {
            animator.SetTrigger("jumpTrigger");
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumps--;
        }
    }

    private void Dash(InputAction.CallbackContext ctx)
    {
        if (canDash && isMovingAllowed && !isGrabbing)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    IEnumerator DashCoroutine()
    {
        animator.SetBool("dashing", true);
        canDash = false;
        isMovingAllowed = false;
        rb.useGravity = false;
        rb.velocity = new Vector3(dashForce * facingDirection, 0, rb.velocity.z);
        StartCoroutine(DashCloneTrail(dashDuration, dashCloneAmount, 0.5f));

        yield return new WaitForSeconds(dashDuration);
        
        rb.useGravity = true;
        rb.velocity = new Vector3(0, 0, 0);
        isMovingAllowed = true;
        animator.SetBool("dashing", false);

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    IEnumerator DashCloneTrail(float dashDuration, int cloneAmount, float cloneDuration)
    {
        for (int i = 0; i < cloneAmount; i++)
        {
            yield return new WaitForSeconds(dashDuration / cloneAmount);
            GameObject dashTrailIstance = GameObject.Instantiate(dashCloneTrailPrefab, transform.position, Quaternion.Euler(0, facingDirection == 1 ? 0 : 180, 0));
            Destroy(dashTrailIstance, cloneDuration);
        }
    }

    #endregion

    #region ???

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    private void HandleLevel()
    {
        if (exP >= nextLevelExP)
        {
            if(debug) Debug.Log("Level Up");
            level++;
            exP = 0;
            nextLevelExP = 100 * level; //CHANGE THIS TO A FORMULA
        }
    }

    public void AddExP(int amount)
    {
        exP += amount;
        HandleLevel();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
    }

    public void RemoveCoins(int amount)
    {
        coins -= amount;
    }

    #endregion

    #region Debug

    private void DebugHandler()
    {
        if (debug)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Tomou 10 de dano");
                TakeDamage(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Curou 10 de vida");
                Heal(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("Adicionou 50 moedas");
                AddCoins(50);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Debug.Log("Gastou 50 moedas");
                RemoveCoins(50);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Debug.Log("Adicionou 50 de experiencia");
                AddExP(50);
            }
        }
    }

    void OnDrawGizmos()
    {
        if(debug)
        {
            Collider[] hitColliders = Physics.OverlapBox(transform.position + CombatBoxOffset * facingDirection, CombatRaycastSize / 2, transform.rotation);

            if (hitColliders.Length > 0)
            {
                Gizmos.color = Color.red;

            Gizmos.DrawWireCube(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize);
            }

            Gizmos.DrawSphere(transform.position + new Vector3(grabbedCharacterOffset.x * facingDirection, grabbedCharacterOffset.y, grabbedCharacterOffset.z), 0.1f);
        }
    }

    #endregion
}
