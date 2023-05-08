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



    private bool isGrounded = true; 
    int groundLayer;

    private bool isDashing = false;

    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    private bool canDash = true;

    [Header("Qyron")]
    private Rigidbody2D qyronRB;
    private SpriteRenderer qyronSR;
    private BoxCollider2D qyronCol;
    private qyronCombat qyronCombat;

    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");

        qyronCombat = GetComponent<qyronCombat>();
        qyronCol = GetComponent<BoxCollider2D>();
        qyronSR = GetComponent<SpriteRenderer>();
        qyronRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Movimento horizontal
       if(!isDashing)
        {
            float x = Input.GetAxisRaw("Horizontal");
            qyronRB.velocity = new Vector2(x * speed, qyronRB.velocity.y);

            if (x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            // Pulo
            if (Input.GetButtonDown("Jump") && jumps < maxJumps)
            {
                qyronRB.velocity = new Vector2(qyronRB.velocity.x, jumpForce);
                jumps++;
            }

            // Dash
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine(Dash());
            }
        }

        // Verifica se o personagem está no chão
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, groundLayer).collider != null;
        Debug.DrawRay(transform.position, Vector2.down * 1.5f, Color.green);

        if(isGrounded)
        {
            jumps = 0; 
        }

    }

    void FixedUpdate()
    {
    }

    private IEnumerator Dash()
    {   
        canDash = false;
        isDashing = true;
        float originalGravity = qyronRB.gravityScale;
        qyronRB.gravityScale = 0;
        if(isGrounded) qyronRB.velocity = new Vector2(dashForce * transform.localScale.x, qyronRB.velocity.y);
        else qyronRB.velocity = new Vector2(dashForce * transform.localScale.x, dashForce/10 * transform.localScale.y);
        yield return new WaitForSeconds(dashDuration);
        qyronRB.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
