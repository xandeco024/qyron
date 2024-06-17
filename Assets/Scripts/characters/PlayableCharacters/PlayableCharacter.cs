using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;
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
    [SerializeField] private float lightAttackStunDuration;
    private bool canLightAttack = true;

    [SerializeField] private float heavyAttackCD;
    [SerializeField] private float heavyAttackStunDuration;
    private bool canHeavyAttack = true;

    [SerializeField] private float grabAttackCD;
    [SerializeField] private float grabAttackStunDuration;
    private bool canGrab = true;

    [SerializeField] private float lightComboStunDuration;
    [SerializeField] private float heavyComboStunDuration;
    [SerializeField] private float grabComboStunDuration;
    [SerializeField] private Vector3 grabbedCharacterOffset;
    private bool isGrabbing;
    private Character grabbedCharacter;

    private List<string> movementRestrictions = new List<string>();
    public List<string> MovementRestrictions { get { return movementRestrictions; } }

    [Header("UI")]
    private bool isDowned;
    public bool IsDowned { get { return isDowned; } }
    [SerializeField] private Image downedFiller;
    [SerializeField] private GameObject downedUIObject;
    private bool beingCured;
    PlayableCharacter downedFriend = null;

    [Header("PlayableCharacterData")]
    private SpriteLibrary spriteLibrary;
    private SpriteLibraryAsset spriteLibraryAsset;
    private PlayableCharacterData playableCharacterData;
    private Color color;

    void Awake()
    {
        GetComponentsOnCharacter();
        ApplyCharacterData();
        DontDestroyOnLoad(gameObject);
    }

    protected override void GetComponentsOnCharacter()
    {
        base.GetComponentsOnCharacter();
        spriteLibrary = GetComponent<SpriteLibrary>();
    }

    protected override void ApplyCharacterData()
    {
        base.ApplyCharacterData();
        playableCharacterData = (PlayableCharacterData)characterData;
        spriteLibraryAsset = playableCharacterData.spriteLibraryAsset;
        color = playableCharacterData.color;

        spriteLibrary.spriteLibraryAsset = spriteLibraryAsset;
    }

    void Start()
    {

    }   

    void Update()
    {
        DetectGround();
        DebugHandler(); 
        CombatHandler();
        DownedHandler();
        StepAssist();
        StunHandler();
        animator.SetFloat("speed", speed);
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

    void Attack() //funcao para fazer o personagem atacar
    {
        // faz tal coisa
        // dps tal coisa
    }

    public void LightAttack(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            if (canLightAttack && !isDowned && isGrounded)
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

        Attack(damage, lightAttackStunDuration, critical);

        yield return new WaitForSeconds(0.3f / speed);

        if (!isDowned) isMovingAllowed = true;
        isAttacking = false;

        yield return new WaitForSeconds(lightAttackCD / speed);

        canLightAttack = true;
    }

    public void HeavyAttack(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            if (canHeavyAttack && !isDowned && isGrounded)
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

        Attack(damage, heavyAttackStunDuration, critical);

        yield return new WaitForSeconds(0.3f / speed);

        if (!isDowned) isMovingAllowed = true;
        isAttacking = false;

        yield return new WaitForSeconds(heavyAttackCD / speed);

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

        currentMoveSpeed = moveSpeed / 4;

        animator.SetTrigger("grabAttackTrigger");
        animator.SetBool("grabbing", true);

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.z), combatBoxSize / 2, transform.rotation);

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
        currentMoveSpeed = moveSpeed;
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

        CallScreenShake(0.15f, 0.25f, 0.25f);

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.z), combatBoxSize / 2, transform.rotation);
        Attack(damage, lightComboStunDuration, critical, new Vector3(0.8f * facingDirection,1,0), 3f, 0.2f);

        SetRecievingComboOnTargets(true ,hitColliders);

        yield return new WaitForSeconds(0.3f / speed);

        if (!isDowned) isMovingAllowed = true;
        isAttacking = false;

        combo.Clear();

        SetRecievingComboOnTargets(false ,hitColliders);

        yield return new WaitForSeconds(lightAttackCD / speed);

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

        CallScreenShake(0.3f, 0.5f, 0.5f);

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.z), combatBoxSize / 2, transform.rotation);
        SetRecievingComboOnTargets(true ,hitColliders);
        
        Debug.Log("Deu o combo LLH");

        yield return new WaitForSeconds(0.27f / speed);

        Debug.Log("Terminou de girar");

        Attack(damage, heavyComboStunDuration, critical, new Vector3(0,1,1), 2.5f, 0.3f);

        yield return new WaitForSeconds(0.3f / speed);

        if (!isDowned) isMovingAllowed = true;
        isAttacking = false;

        combo.Clear();

        SetRecievingComboOnTargets(false ,hitColliders);

        yield return new WaitForSeconds(heavyAttackCD / speed);

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

        CallScreenShake(0.3f, 0.5f, 0.5f);

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.z), combatBoxSize / 2, transform.rotation);
        SetRecievingComboOnTargets(true ,hitColliders);

        Debug.Log("Deu o combo LLH");

        yield return new WaitForSeconds(0.5f / speed);

        Debug.Log("Terminou de girar");

        Attack(damage, heavyComboStunDuration, critical, new Vector3(0,1,-1), 4f, 0.4f);

        yield return new WaitForSeconds(0.3f / speed);

        if (!isDowned) isMovingAllowed = true;
        isAttacking = false;

        combo.Clear();

        SetRecievingComboOnTargets(false ,hitColliders);

        yield return new WaitForSeconds(heavyAttackCD / speed);

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

        CallScreenShake(0.3f, 0.5f, 0.5f);

        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.z), combatBoxSize / 2, transform.rotation);

        Attack(damage, heavyComboStunDuration, critical, new Vector3(1 * facingDirection,.5f,0), 4, 0.3f);

        SetRecievingComboOnTargets(true ,hitColliders);

        yield return new WaitForSeconds(0.3f / speed);

        if (!isDowned) isMovingAllowed = true;
        isAttacking = false;

        combo.Clear();

        SetRecievingComboOnTargets(false ,hitColliders);

        yield return new WaitForSeconds(heavyAttackCD / speed);

        canHeavyAttack = true;
    }

    #endregion

    public override void TakeDamage(float damage, float stunDuration, bool critical = false, Vector3 knockbackDir = default, float knockbackForce = 0, float knockbackDuration = .2f)
    {
        float lastHealth = currentHealth;
        base.TakeDamage(damage, stunDuration, critical, knockbackDir, knockbackForce);

        //se cancelar o seu ataque ele nao reseta mais
        canLightAttack = true;
        canHeavyAttack = true;
        canGrab = true;

        if (currentHealth < lastHealth)
        {
            animator.SetTrigger("damageTrigger");

            if (isGrabbing)
            {
                CancelGrab();
            }
        }
        else
        {
            //dodjou!
        }
    }

    #endregion

    #region Movement

    void DetectGround()
    {   
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position + raycastOffset, Vector3.down, out hit, raycastDistance, groundLayer);

        animator.SetBool("grounded", isGrounded);

        if (isGrounded)
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

            rb.velocity = new Vector3(x * moveSpeed * speed, y, z * moveSpeed * speed);

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
        animator.SetBool("running", isGrounded && movementInput != Vector3.zero);

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
            dashTrailIstance.GetComponent<SpriteRenderer>().color = color;
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

    private int CalculateNextLevelExp(int level)
    {
        if (level <= 1) return 100; // Valores base para os primeiros níveis
        return Fibonacci(level) * 100; // Multiplique por 100 para ajustar a escala
    }

    private int Fibonacci(int n)
    {
        if (n <= 1) return n;
        int a = 0;
        int b = 1;
        for (int i = 2; i <= n; i++)
        {
            int temp = a + b;
            a = b;
            b = temp;
        }
        return b;
    }

    private void HandleLevel()
    {
        if (exP >= nextLevelExP)
        {
            if (debug) Debug.Log("Level Up");

            // efeitos de level up

            level++;
            exP = 0;
            nextLevelExP = CalculateNextLevelExp(level);
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
                Debug.Log("Tomou 2 de dano");
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

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                ApplyCharacterData();
                Reset();
                Debug.Log("CharacterData Reaplicado e Resetado");   
            }
        }
    }

    private void CallScreenShake(float duration, float magnitude, float frequency)
    {
        CameraManager cameraManager = FindObjectOfType<CameraManager>();
        cameraManager.ScreenShake(duration, magnitude, frequency);
    }

    protected override void OnDrawGizmos()
    {
        if(debug)
        {
            base.OnDrawGizmos();
            Gizmos.DrawSphere(transform.position + new Vector3(grabbedCharacterOffset.x * facingDirection, grabbedCharacterOffset.y, grabbedCharacterOffset.z), 0.1f);
        }
    }

    #endregion
}