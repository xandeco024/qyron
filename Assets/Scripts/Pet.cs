using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    [Header("Components")]
    protected Rigidbody rb;
    protected Animator anim;
    protected SpriteRenderer sr;
    protected Qyron qyron;

    [Header("Pet Stats")]
    [SerializeField] protected bool debug;
    [SerializeField] protected float speed;
    [SerializeField] protected Vector3 offset;
    [SerializeField] protected float distance;

    [Header("Buffs")]

    [SerializeField] private float attackBuff;

    protected void GetComponentsOnPet()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        qyron = FindObjectOfType<Qyron>();
    }

    void Awake()
    {
        GetComponentsOnPet();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (debug)
        {
            DrawTargetPosition();
        }

        FlipSprite();
    }

    void FixedUpdate()
    {
        FollowQyron();
    }   

    //smoothly follow qyron
    void FollowQyron()
    {
        if (Vector3.Distance(transform.position, qyron.transform.position + offset) > distance)
        {
            Vector3 direction = (qyron.transform.position + offset - transform.position).normalized;
            rb.velocity = new Vector3(direction.x * speed, direction.y * speed, direction.z * speed);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    void DrawTargetPosition()
    {
        Debug.DrawLine(transform.position, qyron.transform.position + offset, Color.red);
    }

    void FlipSprite()
    {
        if (qyron.transform.position.x > transform.position.x)
        {
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
        }
    }


}
