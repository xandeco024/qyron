using Unity.VisualScripting;
using UnityEngine;

public class Qyron : Character {

    private Vector3 movementInput;

    [Header("Jump Settings")]
    private bool isGrounded;
    private bool jumpTrigger;
    [SerializeField] private int maxJumps;
    private int jumps;
    [SerializeField] float raycastDistance;
    [SerializeField] private Vector3 raycastOffset;
    private RaycastHit groundRaycastHit;
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

        if (isGrounded)
        {
            jumps = maxJumps;
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
        LimitZ();
        Flip();
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

        //HANDLE JUMPING
        if (jumpTrigger && jumps > 0)
        {
            jumps--;
            jumpTrigger = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    #endregion
}