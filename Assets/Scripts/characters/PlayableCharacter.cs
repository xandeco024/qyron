using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

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
    [SerializeField] private int maxJumps;
    private int jumps;

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
    private List<string> combo = new List<string>();
    private List<string> validLightCombos = new List<string>() {"LLL"};
    private List<string> validHeavyCombos = new List<string>() {"HHH", "LLH", "LLL"};
    public List<string> Combo { get { return combo; } }
    private float lastAttackTime;
    [SerializeField] private bool friendlyFire;
    [SerializeField] private float lightAttackCD;
    private bool canLightAttack = true;
    [SerializeField] private float heavyAttackCD;
    private bool canHeavyAttack = true;

    [SerializeField] private float grabAttackCD;
    private bool canGrab = true;
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
        SetStats();
    }   

    void Update()
    {
        if (currentHealth <= 0) 
        {
            animator.SetBool("downed", true);
            isMovingAllowed = false;
        }

        DetectGround();
        DebugHandler(); 

        CombatHandler();
    }

    void FixedUpdate()
    {
        ApplyMovement();
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

    void CombatHandler()
    {
        lastAttackTime += Time.deltaTime;

        if (lastAttackTime > 1.5f)
        {
            fighting = false;
            combo.Clear();
        }

        if (combo.Count > 3)
        {
            combo.Clear();  
        }

        animator.SetBool("fighting", fighting);
    }

    void LightComboHandler()
    {
        switch (string.Join("", combo.GetRange(combo.Count - 3, 3)))
        {
            case "LLL":
                Debug.Log("Combo LLLL Realizado");
                combo.Clear();
                StartCoroutine(LLLLCombo());
                break;
            default:
                Debug.Log("Combo invalido" + string.Join("", combo.GetRange(combo.Count - 3, 3)));
                combo.Clear();
                break;
        }
    }

    void HeavyComboHandler()
    {
        switch (string.Join("", combo.GetRange(combo.Count - 3, 3)))
        {
            case "LLH":
                Debug.Log("Combo LLHH Realizado");
                combo.Clear();
                StartCoroutine(LLHHCombo());
                break;
            case "HHH":
                Debug.Log("Combo HHHH Realizado");
                combo.Clear();
                StartCoroutine(HHHHCombo());
                break;

            case "LLL":
                Debug.Log("Combo LLLH Realizado");
                combo.Clear();
                StartCoroutine(LLLHCombo());
                break;
            default:
                Debug.Log("Combo invalido" + string.Join("", combo.GetRange(combo.Count - 3, 3)));
                combo.Clear();
                break;
        }
    }

    void LightAttack(InputAction.CallbackContext ctx)
    {
        if (canLightAttack)
        {
            if(isGrabbing) // se estiver grebbando então faz o combo de grab
            { 
                CancelGrab();
                Debug.Log("Light Grab Attack " + combo);
                combo.Clear();
            }

            else if (combo.Count == 3 && !isAttacking && validLightCombos.Contains(string.Join("", combo.GetRange(combo.Count - 3, 3)))) // se o combo for valido e tiver 3 ataques
            {
                LightComboHandler();
            }

            else if (!isAttacking)
            {
                StartCoroutine(LightAttackCoroutine());
            }
        }
    }
    IEnumerator LightAttackCoroutine()
    {
        canLightAttack = false;
        isMovingAllowed = false;
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        if (!isAttacking) isAttacking = true;
        if (!fighting) fighting = true;

        lastAttackTime = 0;

        combo.Add("L");

        //logica para calcular dano que o hit vai dar.
        //calcula se vai dar critico ou não
        bool critical = Random.Range(0, 100) < criticalChance;
        Debug.Log(critical);
        float damage = baseAttackDamage * (critical? 2f : 1f);

        attackAnimationIndex = (attackAnimationIndex == 1) ? 2 : 1;

        animator.SetTrigger("lightAttackTrigger");
        animator.SetInteger("attackAnimationIndex", attackAnimationIndex);

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);
        DealDamage(hitColliders, damage, critical);

        yield return new WaitForSeconds(0.3f);

        isMovingAllowed = true;
        isAttacking = false;

        yield return new WaitForSeconds(lightAttackCD);

        canLightAttack = true;
    }

    void HeavyAttack(InputAction.CallbackContext ctx)
    {
        if (canHeavyAttack)
        {
            if(isGrabbing) // se estiver grebbando então faz o combo de grab
            { 
                CancelGrab();
                Debug.Log("Heavy Grab Attack " + combo);
                combo.Clear();
            }

            else if (combo.Count == 3 && !isAttacking && validHeavyCombos.Contains(string.Join("", combo.GetRange(combo.Count - 3, 3)))) // se o combo for valido e tiver 3 ataques
            {
                HeavyComboHandler();
            }

            else if (!isAttacking)
            {
                StartCoroutine(HeavyAttackCoroutine());
            }
        }
    }

    IEnumerator HeavyAttackCoroutine()
    {
        canHeavyAttack = false;
        isMovingAllowed = false;
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        if (!isAttacking) isAttacking = true;
        if (!fighting) fighting = true;

        lastAttackTime = 0;

        combo.Add("H");

        //logica para calcular dano que o hit vai dar.
        bool critical = Random.Range(0, 100) < criticalChance;
        float damage = baseAttackDamage * 1.25f * (critical? 2f : 1f);

        attackAnimationIndex = (attackAnimationIndex == 1) ? 2 : 1;

        animator.SetTrigger("heavyAttackTrigger");
        animator.SetInteger("attackAnimationIndex", attackAnimationIndex);

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);
        DealDamage(hitColliders, damage, critical);

        yield return new WaitForSeconds(0.3f);

        isMovingAllowed = true;
        isAttacking = false;

        yield return new WaitForSeconds(heavyAttackCD);

        canHeavyAttack = true;
    }

    void GrabAttack(InputAction.CallbackContext ctx)
    {
        if (canGrab && !isAttacking && !isGrabbing)
        {
            if (combo.Count > 0)
            {
                // só vai grabar se no combo não houver um outro grab
                if (combo.Contains("G")) return;
                else
                {
                    StartCoroutine(GrabAttackCoroutine());
                }
            }

            else
            {
                StartCoroutine(GrabAttackCoroutine());
            }
        }

        else if (isGrabbing)
        {
            CancelGrab();
            StopCoroutine(GrabAttackCoroutine());
        }
    }

    IEnumerator GrabAttackCoroutine()
    {
        if (canGrab) canGrab = false;
        if (!isGrabbing) isGrabbing = true;
        if (!isAttacking) isAttacking = true;
        if (!fighting) fighting = true;

        lastAttackTime = 0;

        moveSpeed = baseMoveSpeed / 4;

        animator.SetTrigger("grabAttackTrigger");
        animator.SetBool("grabbing", true);

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);

        foreach (Collider collider in hitColliders)
        {
            if (collider.GetComponent<Character>() && collider.GetComponent<Character>() != this && collider.GetComponent<Character>().IsGrabbable)
            {
                combo.Add("G");

                grabbedCharacter = collider.GetComponent<Character>();

                grabbedCharacter.transform.position = transform.position + new Vector3(grabbedCharacterOffset.x * facingDirection, grabbedCharacterOffset.y, grabbedCharacterOffset.z);
                grabbedCharacter.transform.SetParent(transform);
                grabbedCharacter.SetGrabbed(true, facingDirection);

                yield return new WaitForSeconds(3);

                continue;
            }
        }

        CancelGrab();

        yield return new WaitForSeconds(grabAttackCD);
        canGrab = true;
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

    IEnumerator LLLLCombo()
    {
        canLightAttack = false;
        isMovingAllowed = false;
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        if (!isAttacking) isAttacking = true;
        if (!fighting) fighting = true;

        lastAttackTime = 0;

        //logica para calcular dano que o hit vai dar.
        bool critical = Random.Range(0, 100) < criticalChance;
        float damage = baseAttackDamage * 2 * (critical? 2f : 1f);

        animator.SetTrigger("LLLTrigger");

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);
        DealDamage(hitColliders, damage, critical, new Vector3(1 * facingDirection,1,0), 1f);

        yield return new WaitForSeconds(0.3f);

        isMovingAllowed = true;
        isAttacking = false;

        yield return new WaitForSeconds(lightAttackCD);

        canLightAttack = true;
    }

    IEnumerator LLLHCombo()
    {
        canHeavyAttack = false;
        isMovingAllowed = false;
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        if (!isAttacking) isAttacking = true;
        if (!fighting) fighting = true;

        lastAttackTime = 0;

        //logica para calcular dano que o hit vai dar.
        bool critical = Random.Range(0, 100) < criticalChance;
        float damage = baseAttackDamage * 2.5f * (critical? 2f : 1f);

        Debug.Log("Deu o combo LLH");

        yield return new WaitForSeconds(0.27f);

        Debug.Log("Terminou de girar");

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);
        DealDamage(hitColliders, damage, critical, new Vector3(0,1,1), 2.5f);

        yield return new WaitForSeconds(0.3f);

        isMovingAllowed = true;
        isAttacking = false;

        yield return new WaitForSeconds(heavyAttackCD);

        canHeavyAttack = true;
    }

    IEnumerator LLHHCombo()
    {
        canHeavyAttack = false;
        isMovingAllowed = false;
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        if (!isAttacking) isAttacking = true;
        if (!fighting) fighting = true;

        lastAttackTime = 0;

        //logica para calcular dano que o hit vai dar.
        bool critical = Random.Range(0, 100) < criticalChance;
        float damage = baseAttackDamage * 2.5f * (critical? 2f : 1f);

        Debug.Log("Deu o combo LLH");

        yield return new WaitForSeconds(0.5f);

        Debug.Log("Terminou de girar");

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);
        DealDamage(hitColliders, damage, critical, new Vector3(0,1,-1), 4f);

        yield return new WaitForSeconds(0.3f);

        isMovingAllowed = true;
        isAttacking = false;

        yield return new WaitForSeconds(heavyAttackCD);

        canHeavyAttack = true;
    }

    IEnumerator HHHHCombo()
    {
        canHeavyAttack = false;
        isMovingAllowed = false;
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        if (!isAttacking) isAttacking = true;
        if (!fighting) fighting = true;

        lastAttackTime = 0;

        //logica para calcular dano que o hit vai dar.
        bool critical = Random.Range(0, 100) < criticalChance;
        float damage = baseAttackDamage * 2.5f * (critical? 2f : 1f);

        animator.SetTrigger("HHHTrigger");

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);
        DealDamage(hitColliders, damage, critical, new Vector3(1 * facingDirection,.5f,0), 4);

        yield return new WaitForSeconds(0.3f);

        isMovingAllowed = true;
        isAttacking = false;

        yield return new WaitForSeconds(heavyAttackCD);

        canHeavyAttack = true;
    }

    public override void TakeDamage(float damage, bool critical = false, Vector3 knockbackDir = default, float knockbackForce = 0, float knockbackDuration = .2f)
    {
        base.TakeDamage(damage, critical, knockbackDir, knockbackForce);

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
    bool grounded = isGrounded();
    animator.SetBool("grounded", grounded);

    if (grounded)
    {
        jumps = maxJumps;
    }
}

void ApplyMovement()
{
    if (isMovingAllowed)
    {
        rb.velocity = new Vector3(movementInput.x * moveSpeed, rb.velocity.y, movementInput.y * moveSpeed);

        if (movementInput.x != 0)
        {
            facingDirection = movementInput.x > 0 ? 1 : -1;
            transform.rotation = Quaternion.Euler(0, facingDirection == 1 ? 0 : 180, 0);
        }
    }

    if (limitZ)
    {
        LimitZ();
    }

    // Define o estado "running" diretamente com base na condição, sem verificar o estado atual
    animator.SetBool("running", isGrounded() && movementInput != Vector3.zero);

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

    public void Revive()
    {
        currentHealth = maxHealth/4;
        animator.SetBool("downed", false);
        isMovingAllowed = true;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public IEnumerator BuffMaxHealth(float amount, bool overrideStat = false, float time = 0)
    {
        if (overrideStat) maxHealth = amount;
        else maxHealth += amount;

        yield return new WaitForSeconds(time);

        if (overrideStat) maxHealth = baseMaxHealth;
        else maxHealth -= amount;
    }

    public IEnumerator BuffAttackDamage(float amount, bool overrideStat = false, float time = 0)
    {
        if (overrideStat) attackDamage = amount;
        else attackDamage += amount;

        yield return new WaitForSeconds(time);

        if (overrideStat) attackDamage = baseAttackDamage;
        else attackDamage -= amount;
    }

    public IEnumerator BuffCriticalChance(float amount, bool overrideStat = false, float time = 0)
    {
        if (overrideStat) criticalChance = amount;
        else criticalChance += amount;

        yield return new WaitForSeconds(time);

        if (overrideStat) criticalChance = baseCriticalChance;
        else criticalChance -= amount;
    }

    public IEnumerator BuffMoveSpeed(float amount, bool overrideStat = false, float time = 0)
    {
        if (overrideStat) moveSpeed = amount;
        else moveSpeed += amount;

        yield return new WaitForSeconds(time);

        if (overrideStat) moveSpeed = baseMoveSpeed;
        else moveSpeed -= amount;
    }

    public IEnumerator BuffJumpForce(float amount, bool overrideStat = false, float time = 0)
    {
        if (overrideStat) jumpForce = amount;
        else jumpForce += amount;

        yield return new WaitForSeconds(time);

        if (overrideStat) jumpForce = baseJumpForce;
        else jumpForce -= amount;
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

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Debug.Log("Reviveu");
                Revive();
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