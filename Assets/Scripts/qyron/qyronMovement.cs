using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class qyronMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private int jumps = 0;
    [SerializeField] private int maxJumps;
    private bool _canMove = true;
    public bool canMove {get { return _canMove;} set { _canMove = value; } }
    private bool isGrounded = true; 
    int groundLayer;

    private bool isDashing = false;

    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    private bool canDash = true;

    [Header("Qyron")]
    private Rigidbody qyronRB;
    private SpriteRenderer qyronSR;
    private BoxCollider qyronCol;
    private qyronCombat qyronCombat;
    private qyronSFX qyronSFX;
    private Animator qyronAnimator;

    private Ray GroundedRaycast;
    private RaycastHit GroundedRaycastHit;

    private int direction;

    private Vector3 movementInput;

    [SerializeField] private GameObject dashTrail;
    private float startTimeBetweenDashTrailInstance = 0.020f;
    private float timeBetweenDashTrailInstance = 0;

    [Header("Movement Triggers")]
    private bool jumpTrigger;
    private bool dashTrigger;

    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");

        qyronCombat = GetComponent<qyronCombat>();
        qyronCol = GetComponent<BoxCollider>();
        qyronSR = GetComponent<SpriteRenderer>();
        qyronRB = GetComponent<Rigidbody>();
        qyronSFX = GetComponent<qyronSFX>();
        qyronAnimator = GetComponent<Animator>();

        dashTrigger = false;
        jumpTrigger = false;
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        else

        {
            movementInput.x = Input.GetAxisRaw("Horizontal");
            movementInput.z = Input.GetAxisRaw("Vertical");

            if (!qyronCombat.GetAttacking())
            {
                if (movementInput.x != 0 || movementInput.z != 0)
                {
                    qyronAnimator.SetBool("isRunning", true);
                }

                else

                {
                    qyronAnimator.SetBool("isRunning", false);
                }
            }

            else

            {
                qyronAnimator.SetBool("isRunning", false);
            }

            GroundedRaycast = new Ray(transform.position, Vector3.down);
            bool isGrounded = false;

            Physics.Raycast(GroundedRaycast, out GroundedRaycastHit, 1.1f, groundLayer);
            Debug.DrawRay(transform.position, Vector3.down, Color.green);

            if(GroundedRaycastHit.collider != null)
            {
                isGrounded = true;
            }

            if (isGrounded)
            {
                jumps = 0;
            }

            if(_canMove && !isDashing) 
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !dashTrigger)
                {
                    dashTrigger = true;
                }

                if (Input.GetButtonDown("Jump") && jumps < maxJumps && !jumpTrigger)
                {
                    jumpTrigger = true;
                }
            }

        }
    }

    void FixedUpdate()
    {
        if(Time.timeScale == 0)
        {
            return;
        }

        else

        {
            DashTrail();
            Limit();
            if (jumpTrigger) Jump();
            if(dashTrigger) StartCoroutine(Dash());

            if (_canMove && !isDashing)
            {
                FlipSprite(movementInput.x);
                Movement(movementInput.x,movementInput.z);
            }
        }
    }

    private void Movement(float inputX, float inputZ)
    {
        qyronRB.velocity = new Vector3(inputX * speed, qyronRB.velocity.y, inputZ * speed);
    }

    private void Jump()
    {
        jumpTrigger = false;
        qyronSFX.PlayMovementSFX("pulando 1");
        qyronRB.velocity = new Vector2(qyronRB.velocity.x, jumpForce);
        jumps++;
    }

    private IEnumerator Dash()
    {
        dashTrigger = false;
        canDash = false;
        isDashing = true;
        qyronAnimator.SetBool("isDashing", true);
        qyronCombat.SetInvincible(true);
        qyronRB.useGravity = false;
        qyronSFX.PlayMovementSFX("dash 1");
        if (isGrounded) qyronRB.velocity = new Vector3(dashForce * direction, qyronRB.velocity.y, 0);
        else qyronRB.velocity = new Vector3(dashForce * direction, dashForce/5, 0);
        yield return new WaitForSeconds(dashDuration);
        qyronRB.useGravity = true;
        isDashing = false;
        qyronCombat.SetInvincible(false);
        qyronAnimator.SetBool("isDashing", false);
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void DashTrail()
    {
        if (isDashing)
        {
            if (timeBetweenDashTrailInstance <= 0)
            {
                if (direction == 1)
                {
                    GameObject dashTrailIstance = GameObject.Instantiate(dashTrail, transform.position, Quaternion.identity);
                    Destroy(dashTrailIstance, 0.4f);
                    timeBetweenDashTrailInstance = startTimeBetweenDashTrailInstance;
                }

                else if(direction == -1) 
                {
                    GameObject dashTrailIstance = GameObject.Instantiate(dashTrail, transform.position, new Quaternion(0, -180, 0, 0));
                    Destroy(dashTrailIstance, 0.4f);
                    timeBetweenDashTrailInstance = startTimeBetweenDashTrailInstance;
                }
            }
            else
            {
                timeBetweenDashTrailInstance -= Time.deltaTime;
            }
        }
    }

    private void Limit()
    {
        if (transform.position.z >= 2.5f) transform.position = new Vector3(transform.position.x, transform.position.y, 2.5f);
        if (transform.position.z <= -2.5f) transform.position = new Vector3(transform.position.x, transform.position.y, -2.5f);
    }

    private void FlipSprite(float x)
    {
        if (x < 0)
        {
            qyronSR.flipX = true;
            direction = -1;
        }
        else if (x > 0)
        {
            qyronSR.flipX = false;
            direction = 1;
        }
    }
}
