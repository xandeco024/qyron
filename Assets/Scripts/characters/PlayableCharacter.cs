using System.Collections;
using System.Security.Cryptography.X509Certificates;
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
    private bool isMovingAllowed = true;


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
    }   

    void Update()
    {
        if (currentHealth <= 0) 
        {
            Die();
        }
        DetectGround();
        Animation();

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
    }

    void Animation()
    {
        if (movementInput.x != 0 || movementInput.y != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }


    #region Movement

    void DetectGround()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + raycastOffset, Vector3.down);
        Debug.DrawRay(transform.position + raycastOffset, Vector3.down * raycastDistance, Color.green);


        Physics.Raycast(ray, out hit, raycastDistance, groundLayer);

        if (hit.collider != null)
        {
            isGrounded = true;
            jumps = maxJumps;
        }
        else
        {
            isGrounded = false;
        }
    }

    void ApplyMovement() //HANDLE X,Z MOVEMENT, JUMPING AND DASHING
    {
        if (isMovingAllowed)
        {
            rb.velocity = new Vector3(movementInput.x * moveSpeed, rb.velocity.y, movementInput.y * moveSpeed);
        }

        if (limitZ)
        {
            LimitZ();
        }
    }


    void Jump(InputAction.CallbackContext ctx)
    {
        if (jumps > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumps--;
        }
    }

    private void Dash(InputAction.CallbackContext ctx)
    {
        if (canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    IEnumerator DashCoroutine()
    {
        canDash = false;
        isMovingAllowed = false;
        rb.useGravity = false;
        rb.velocity = new Vector3(dashForce * facingDirection, 0, rb.velocity.z);
        StartCoroutine(DashCloneTrail(dashDuration, dashCloneAmount, 0.5f));

        yield return new WaitForSeconds(dashDuration);
        
        rb.useGravity = true;
        rb.velocity = new Vector3(0, 0, 0);
        isMovingAllowed = true;

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
                TakeDamage(2, true, 3);
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

    #endregion
}