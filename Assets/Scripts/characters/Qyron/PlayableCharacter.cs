using System.Collections;
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
    private bool canMove = true;


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
        DetectGround();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        DebugHandler();
    }

    void FixedUpdate()
    {
        if (canMove) ApplyMovement();
        if (jumpTrigger()) Jump();
        if (DashTrigger()) StartCoroutine(Dash());
        if (limitZ) LimitZ();
        FlipSprite();
    }

    #region Movement

    void DetectMovementInput()
    {
        movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    void DetectGround()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + raycastOffset, Vector3.down);
        Debug.DrawRay(transform.position + raycastOffset, Vector3.down * raycastDistance, Color.green);

        if (Physics.Raycast(ray, out hit, raycastDistance, groundLayer))
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
        rb.velocity = new Vector3(movementInput.x * moveSpeed, rb.velocity.y, movementInput.z * moveSpeed);
    }

    bool jumpTrigger() {
        return Input.GetKeyDown(KeyCode.Space);
    }

    void Jump()
    {
        if (jumps > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumps--;
        }
    }

    bool DashTrigger()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    IEnumerator Dash()
    {
        if (canDash)
        {
            canDash = false;
            canMove = false;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
            yield return new WaitForSeconds(dashDuration);
            rb.velocity = new Vector3(0, 0, 0);
            yield return new WaitForSeconds(dashCooldown);
            canMove = true;
            canDash = true;
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