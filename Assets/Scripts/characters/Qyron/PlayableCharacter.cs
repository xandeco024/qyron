using UnityEngine;

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
    private bool jumpTrigger;
    [SerializeField] private int maxJumps;
    private int jumps;
    [SerializeField] float raycastDistance;
    [SerializeField] private Vector3 raycastOffset;
    [SerializeField] private LayerMask groundLayer;

    void Awake()
    {
        GetComponentsOnCharacter();
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

        DetectMovementInput();

        RaycastHit groundRaycastHit;

        Ray groundRay = new Ray(transform.position + raycastOffset, Vector3.down);
        Debug.DrawRay(transform.position + raycastOffset, Vector3.down * raycastDistance, Color.green);

        Physics.Raycast(groundRay, out groundRaycastHit, raycastDistance, groundLayer);

        if(groundRaycastHit.collider != null)
        {
            isGrounded = true;
        }
            
        else
        {
            isGrounded = false;
        }

        Debug.Log(isGrounded);

        if (isGrounded)
        {
            jumps = maxJumps;
        }

        DebugHandler();
    }

    void FixedUpdate()
    {
        ApplyMovement();
        HandleJump();
        if (limitZ) LimitZ();
        FlipSprite();

    }

    #region Movement

    void DetectMovementInput()
    {
        movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space)) jumpTrigger = true;
        else jumpTrigger = false;
    }

    void ApplyMovement() //HANDLE X,Z MOVEMENT, JUMPING AND DASHING
    {
        rb.velocity = new Vector3(movementInput.x * moveSpeed, rb.velocity.y, movementInput.z * moveSpeed);
    }

    void HandleJump()
    {
        if (jumpTrigger && jumps > 0)
        {
            Debug.Log("Pulou");
            jumps--;
            jumpTrigger = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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