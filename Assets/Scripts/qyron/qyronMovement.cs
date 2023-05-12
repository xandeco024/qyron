using System;
using System.Collections;
using System.Collections.Generic;
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

    private Ray GroundedRaycast;
    private RaycastHit GroundedRaycastHit;

    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");

        qyronCombat = GetComponent<qyronCombat>();
        qyronCol = GetComponent<BoxCollider>();
        qyronSR = GetComponent<SpriteRenderer>();
        qyronRB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        else if (_canMove)

        {
            if (!isDashing)
            {
                float x = Input.GetAxisRaw("Horizontal");
                float z = Input.GetAxisRaw("Vertical");
                qyronRB.velocity = new Vector3(x * speed, qyronRB.velocity.y ,z * speed);

                if (x < 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (x > 0)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }

                if (Input.GetButtonDown("Jump") && jumps < maxJumps)
                {
                    qyronRB.velocity = new Vector2(qyronRB.velocity.x, jumpForce);
                    jumps++;
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
                {
                    StartCoroutine(Dash());
                }
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

            //Debug.Log(isGrounded);
        }

        Limit();
    }

    void FixedUpdate()
    {
    }

    private IEnumerator Dash()
    {   
        canDash = false;
        isDashing = true;
        //float originalGravity = qyronRB.grav;
        qyronRB.useGravity = false;
        if(isGrounded) qyronRB.velocity = new Vector2(dashForce * transform.localScale.x, qyronRB.velocity.y);
        else qyronRB.velocity = new Vector2(dashForce * transform.localScale.x, dashForce/10 * transform.localScale.y);
        yield return new WaitForSeconds(dashDuration);
        //qyronRB.gravityScale = originalGravity;
        qyronRB.useGravity = true;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void Limit()
    {
        if (transform.position.z >= 2.5f) transform.position = new Vector3(transform.position.x, transform.position.y, 2.5f);
        if (transform.position.z <= -2.5f) transform.position = new Vector3(transform.position.x, transform.position.y, -2.5f);
    }

    
}
