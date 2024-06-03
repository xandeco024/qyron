using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayableCharacter : Character {

    [Header("Character Stats")]
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

    private string characterName;
    public string CharacterName { get { return characterName; } }

    private List<string> movementRestrictions = new List<string>();
    public List<string> MovementRestrictions { get { return movementRestrictions; } }

    [Header("UI")]
    private bool isDowned;
    public bool IsDowned { get { return isDowned; } }
    [SerializeField] private Image downedFiller;
    [SerializeField] private GameObject downedUIObject;
    private bool beingCured;
    PlayableCharacter downedFriend = null;

    public void SetupCharacter(string name)
    {
        characterName = name;
    }

    void Start()
    {
        GetComponentsOnCharacter();
        SetStats();
        DontDestroyOnLoad(gameObject);
    }   

    void Update()
    {
        DetectGround();
        DebugHandler(); 
        CombatHandler();
        DownedHandler();
        StepAssist();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    #region Combat

    void DownedHandler()
    {
        if (currentHealth < 0) currentHealth = 0;

        if (currentHealth <= 0 && !isDowned)
        {
            SetDowned(true);
        }

        if (beingCured)
        {
            //fill downedFiller to 1 in 2 seconds
            downedFiller.fillAmount += 1 * Time.deltaTime / 2;
        }
        else
        {
            downedFiller.fillAmount = 0;
        }

        if (downedFiller.fillAmount >= 1)
        {
            SetDowned(false);
        }
    }

    void SetDowned(bool value)
    {
        isDowned = value;

        isMovingAllowed = !value;
        canLightAttack = !value;
        canHeavyAttack = !value;
        canGrab = !value;

        downedFiller.fillAmount = 0;
        currentHealth = value ? 0 : maxHealth/4;
        animator.SetBool("downed", value);
        downedUIObject.SetActive(value);

        if (value) 
        {
            downedUIObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            rb.velocity = Vector3.zero;
        }

    }

    public void RevivePlayer(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            //beingCured = true;

            Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(2,2,2), transform.rotation);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.GetComponent<PlayableCharacter>() && hitCollider.GetComponent<PlayableCharacter>().IsDowned && hitCollider.GetComponent<PlayableCharacter>() != this)
                {
                    downedFriend = hitCollider.GetComponent<PlayableCharacter>();
                    break;
                }
                else
                {
                    downedFriend = null;
                }
            }

            if (downedFriend != null && !isDowned)
            {
                downedFriend.SetReviving(true);
            }
        }

        if (ctx.canceled)
        {
            if (downedFriend != null && !isDowned)
            {
                downedFriend.SetReviving(false);
            }
        }
    }

    public void SetReviving(bool value)
    {
        beingCured = value;
    }
    

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
                StartCoroutine(LLHHCombo());
                break;
            case "HHH":
                Debug.Log("Combo HHHH Realizado");
                StartCoroutine(HHHHCombo());
                break;

            case "LLL":
                Debug.Log("Combo LLLH Realizado");
                StartCoroutine(LLLHCombo());
                break;
            default:
                Debug.Log("Combo invalido" + string.Join("", combo.GetRange(combo.Count - 3, 3)));
                combo.Clear();
                break;
        }
    }

    public void LightAttack(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            if (canLightAttack && !isDowned)
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
        //Debug.Log(critical);
        float damage = attackDamage * (critical? 2f : 1f);

        attackAnimationIndex = (attackAnimationIndex == 1) ? 2 : 1;

        animator.SetTrigger("lightAttackTrigger");
        animator.SetInteger("attackAnimationIndex", attackAnimationIndex);

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);
        DealDamage(hitColliders, damage, 1f,critical);

        yield return new WaitForSeconds(0.3f);

        if (!isDowned) isMovingAllowed = true;
        isAttacking = false;

        yield return new WaitForSeconds(lightAttackCD);

        canLightAttack = true;
    }

    public void HeavyAttack(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            if (canHeavyAttack && !isDowned)
            {
                if(isGrabbing) // se estiver grebbando então faz o combo de grab
                { 
                    CancelGrab();
                    //uDebug.Log("Heavy Grab Attack " + combo);
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
        float damage = attackDamage * 1.25f * (critical? 2f : 1f);

        attackAnimationIndex = (attackAnimationIndex == 1) ? 2 : 1;

        animator.SetTrigger("heavyAttackTrigger");
        animator.SetInteger("attackAnimationIndex", attackAnimationIndex);

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);
        DealDamage(hitColliders, damage, 0.4f, critical);

        yield return new WaitForSeconds(0.3f);

        if (!isDowned) isMovingAllowed = true;
        isAttacking = false;

        yield return new WaitForSeconds(heavyAttackCD);

        canHeavyAttack = true;
    }

    public void GrabAttack(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            if (canGrab && !isAttacking && !isGrabbing && !isDowned)
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

                //set grabbedCharacter grabbedPoint to to the grabbedCharacterOffset

                grabbedCharacter.transform.position = transform.position + new Vector3(grabbedCharacterOffset.x * facingDirection, grabbedCharacterOffset.y, grabbedCharacterOffset.z);
                
                grabbedCharacter.transform.SetParent(transform);
                grabbedCharacter.SetGrabbed(true);
                grabbedCharacter.Flip(facingDirection == 1 ? false : true);

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

    #region Combos

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
        float damage = attackDamage * 2 * (critical? 2f : 1f);

        animator.SetTrigger("LLLTrigger");  

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);
        DealDamage(hitColliders, damage, 0.5f, critical, new Vector3(0.8f * facingDirection,1,0), 3f, 0.2f);

        SetRecievingComboOnTargets(true ,hitColliders);

        yield return new WaitForSeconds(0.3f);

        if (!isDowned) isMovingAllowed = true;
        isAttacking = false;

        combo.Clear();

        SetRecievingComboOnTargets(false ,hitColliders);

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
        float damage = attackDamage * 2.5f * (critical? 2f : 1f);

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);
        SetRecievingComboOnTargets(true ,hitColliders);
        
        Debug.Log("Deu o combo LLH");

        yield return new WaitForSeconds(0.27f);

        Debug.Log("Terminou de girar");

        DealDamage(hitColliders, damage, 0.5f, critical, new Vector3(0,1,1), 2.5f, 0.3f);

        yield return new WaitForSeconds(0.3f);

        if (!isDowned) isMovingAllowed = true;
        isAttacking = false;

        combo.Clear();

        SetRecievingComboOnTargets(false ,hitColliders);

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
        float damage = attackDamage * 2.5f * (critical? 2f : 1f);

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);
        SetRecievingComboOnTargets(true ,hitColliders);

        Debug.Log("Deu o combo LLH");

        yield return new WaitForSeconds(0.5f);

        Debug.Log("Terminou de girar");

        DealDamage(hitColliders, damage, 0.5f, critical, new Vector3(0,1,-1), 4f, 0.4f);

        yield return new WaitForSeconds(0.3f);

        if (!isDowned) isMovingAllowed = true;
        isAttacking = false;

        combo.Clear();

        SetRecievingComboOnTargets(false ,hitColliders);

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
        float damage = attackDamage * 2.5f * (critical? 2f : 1f);

        animator.SetTrigger("HHHTrigger");

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(CombatBoxOffset.x * facingDirection, CombatBoxOffset.y, CombatBoxOffset.z), CombatRaycastSize / 2, transform.rotation);
        DealDamage(hitColliders, damage, 0.5f, critical, new Vector3(1 * facingDirection,.5f,0), 4, 0.3f);

        SetRecievingComboOnTargets(true ,hitColliders);

        yield return new WaitForSeconds(0.3f);

        if (!isDowned) isMovingAllowed = true;
        isAttacking = false;

        combo.Clear();

        SetRecievingComboOnTargets(false ,hitColliders);

        yield return new WaitForSeconds(heavyAttackCD);

        canHeavyAttack = true;
    }

    #endregion

    public override void TakeDamage(float damage, float stunDuration, bool critical = false, Vector3 knockbackDir = default, float knockbackForce = 0, float knockbackDuration = .2f)
    {
        base.TakeDamage(damage, stunDuration, critical, knockbackDir, knockbackForce);

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

public void OnMove(InputAction.CallbackContext ctx)
{
    movementInput = ctx.ReadValue<Vector2>();
}

public void SetMovementRestrictions(List<string> restrictions)
{
    movementRestrictions = restrictions;
}

public void ResetMovementRestrictions()
{
    movementRestrictions.Clear();
}

public void RemoveMovementRestriction(string restriction)
{
    movementRestrictions.Remove(restriction);
}

public void AddMovementRestriction(string restriction)
{
    movementRestrictions.Add(restriction);
}

void ApplyMovement()
{
    if (isMovingAllowed)
    {
        float x = movementInput.x;

        if (movementRestrictions.Contains("left")) if (x < 0) x = 0;
        if (movementRestrictions.Contains("right")) if (x > 0) x = 0;

        float y = rb.velocity.y;

        if (movementRestrictions.Contains("up")) if (y > 0) y = 0;
        if (movementRestrictions.Contains("down")) if (y < 0) y = 0;

        float z = movementInput.y;

        if (movementRestrictions.Contains("forward")) if (z > 0) z = 0;
        if (movementRestrictions.Contains("backward")) if (z < 0) z = 0;

        rb.velocity = new Vector3(x * moveSpeed, y, z * moveSpeed);

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


    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (jumps > 0 && isMovingAllowed && !isGrabbing)

            {
                animator.SetTrigger("jumpTrigger");
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jumps--;
            }
        }
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (canDash && isMovingAllowed && !isGrabbing)
            {
                StartCoroutine(DashCoroutine());
            }
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

    public void Reset()
    {
        if (currentHealth <= 0)
        {
            SetDowned(false);
        }



        currentHealth = maxHealth;
        coins = 0;
        exP = 0;
        level = 1;
        nextLevelExP = 100;
    
        canDash = true;
        canLightAttack = true;
        canHeavyAttack = true;
        canGrab = true;
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
                TakeDamage(2, 0);
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
                SetDowned(false);
            }
        }
    }

    protected override void OnDrawGizmos()
    {
        if(debug)
        {
            base.OnDrawGizmos();

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